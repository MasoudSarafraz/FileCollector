using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Central engine that orchestrates all folder watchers and worker threads.
    /// Reports progress to UI via events.
    /// </summary>
    public class CollectorEngine : IDisposable
    {
        private readonly AppConfig _config;
        private readonly Dictionary<int, FolderWatcher> _watchers = new Dictionary<int, FolderWatcher>();
        private readonly Dictionary<int, BlockingCollection<string>> _queues = new Dictionary<int, BlockingCollection<string>>();
        private readonly Dictionary<int, Task> _workers = new Dictionary<int, Task>();
        private readonly Dictionary<int, CancellationTokenSource> _cts = new Dictionary<int, CancellationTokenSource>();
        private readonly Dictionary<int, FolderStats> _stats = new Dictionary<int, FolderStats>();

        private CancellationTokenSource _globalCts;
        private bool _disposed;
        private bool _globalRunning;
        private DateTime _startTime;

        private readonly object _statsLock = new object();
        private int _globalCounter = 0;

        public bool IsRunning => _globalRunning;

        // ===== Events =====
        public event Action<OverallProgressInfo> OverallProgressChanged;
        public event Action<FolderProgressInfo> FolderProgressChanged;
        public event Action<FileProgressInfo> FileProgressChanged;
        public event Action<string> LogMessage;

        public CollectorEngine(AppConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Starts watching all enabled folders.
        /// </summary>
        public void StartAll()
        {
            if (_globalRunning) return;
            _globalRunning = true;
            _globalCts = new CancellationTokenSource();
            _startTime = DateTime.Now;

            foreach (var folder in _config.Folders.Where(f => f.Enabled))
            {
                StartFolder(folder);
            }

            LogMessage?.Invoke("All watchers started.");
            StartOverallReporter();
        }

        /// <summary>
        /// Stops all watchers and workers.
        /// </summary>
        public void StopAll()
        {
            if (!_globalRunning) return;
            _globalRunning = false;

            try { _globalCts?.Cancel(); } catch { }

            foreach (var f in _watchers.Values.ToList()) f.Stop();
            foreach (var q in _queues.Values.ToList()) q.CompleteAdding();

            Task.WaitAll(_workers.Values.ToArray(), 3000);

            _watchers.Clear();
            _queues.Clear();
            _workers.Clear();
            _cts.Clear();

            LogMessage?.Invoke("All watchers stopped.");
        }

        /// <summary>
        /// Starts a single folder watcher.
        /// </summary>
        public bool StartFolder(FolderConfig folder)
        {
            if (_watchers.ContainsKey(folder.Id)) return false;

            try
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts?.Token ?? CancellationToken.None);
                var queue = new BlockingCollection<string>(1000);
                var watcher = new FolderWatcher(folder, queue);

                _watchers[folder.Id] = watcher;
                _queues[folder.Id] = queue;
                _cts[folder.Id] = cts;

                lock (_statsLock)
                {
                    _stats[folder.Id] = new FolderStats
                    {
                        FolderId = folder.Id,
                        FolderName = folder.Name,
                        Status = "running"
                    };
                }

                // Start worker
                _workers[folder.Id] = Task.Run(() => WorkerLoop(folder, queue, cts.Token));

                // Start watcher (only if engine is running)
                if (_globalRunning)
                {
                    watcher.Start();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to start folder '{folder.Name}'", ex);
                return false;
            }
        }

        /// <summary>
        /// Stops a single folder watcher.
        /// </summary>
        public void StopFolder(int folderId)
        {
            if (!_watchers.ContainsKey(folderId)) return;

            try
            {
                _watchers[folderId]?.Stop();
                _queues[folderId]?.CompleteAdding();
                _cts[folderId]?.Cancel();

                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folderId, out var s)) s.Status = "stopped";
                }
            }
            catch { }
        }

        /// <summary>
        /// Pauses a single folder (stops watcher but keeps queue).
        /// </summary>
        public void PauseFolder(int folderId)
        {
            if (_watchers.TryGetValue(folderId, out var w))
            {
                w.Stop();
                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folderId, out var s)) s.Status = "paused";
                }
            }
        }

        /// <summary>
        /// Resumes a paused folder.
        /// </summary>
        public void ResumeFolder(int folderId)
        {
            if (_watchers.TryGetValue(folderId, out var w) && !w.IsRunning)
            {
                w.Start();
                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folderId, out var s)) s.Status = "running";
                }
            }
        }

        // ===========================================================
        // WORKER LOOP
        // ===========================================================

        private void WorkerLoop(FolderConfig folder, BlockingCollection<string> queue, CancellationToken token)
        {
            var executor = new ActionExecutor();
            int workersPerFolder = Math.Max(1, _config.WorkersPerFolder);

            // For simplicity, single worker per folder; can be extended
            try
            {
                foreach (var filePath in queue.GetConsumingEnumerable(token))
                {
                    if (token.IsCancellationRequested) break;
                    ProcessFile(folder, filePath, executor, token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogManager.Error($"Worker crashed for folder '{folder.Name}'", ex);
            }
        }

        private void ProcessFile(FolderConfig folder, string filePath, ActionExecutor executor, CancellationToken token)
        {
            var fileTimer = Stopwatch.StartNew();
            int currentCounter = Interlocked.Increment(ref _globalCounter);

            var fileProgressInfo = new FileProgressInfo
            {
                FolderName = folder.Name,
                FileName = Path.GetFileName(filePath),
                CurrentStep = "Starting",
                CurrentStepIndex = 0,
                TotalSteps = folder.Actions?.Count ?? 0,
                Percent = 0,
                Status = "processing"
            };
            FileProgressChanged?.Invoke(fileProgressInfo);

            try
            {
                if (!File.Exists(filePath))
                {
                    LogSkipped(folder.Id, filePath, "File disappeared");
                    return;
                }

                // Deduplication check
                if (folder.EnableDeduplication)
                {
                    string md5 = VariableResolver.ComputeMd5(filePath);
                    if (!string.IsNullOrEmpty(md5) && DatabaseManager.IsMd5Seen(md5))
                    {
                        LogSkipped(folder.Id, filePath, "Duplicate (MD5 already processed)");
                        return;
                    }
                }

                // Setup variable context
                var ctx = new VariableResolver.Context
                {
                    FilePath = filePath,
                    SourceFolder = folder.SourcePath,
                    Now = DateTime.Now,
                    Counter = currentCounter
                };

                var execCtx = new ActionExecutor.ExecutionContext
                {
                    Folder = folder,
                    VariableContext = ctx,
                    CancellationToken = token,
                    FileProgress = new Progress<int>(p =>
                    {
                        fileProgressInfo.Percent = p;
                        FileProgressChanged?.Invoke(fileProgressInfo);
                    })
                };

                // Execute chain
                string currentPath = filePath;
                int totalSteps = folder.Actions?.Count ?? 0;

                for (int i = 0; i < (folder.Actions?.Count ?? 0); i++)
                {
                    var action = folder.Actions[i];
                    if (!action.Enabled) continue;

                    fileProgressInfo.CurrentStepIndex = i + 1;
                    fileProgressInfo.CurrentStep = $"Step {i + 1}/{totalSteps}: {action.Type}";
                    fileProgressInfo.Percent = 0;
                    FileProgressChanged?.Invoke(fileProgressInfo);

                    var result = executor.Execute(action, currentPath, execCtx);

                    if (!result.Success)
                    {
                        LogManager.Warn($"Action {action.Type} failed on {Path.GetFileName(filePath)}: {result.ErrorMessage}");

                        DatabaseManager.LogFileHistory(folder.Id, folder.Name, Path.GetFileName(filePath),
                            filePath, currentPath, new FileInfo(filePath).Length,
                            VariableResolver.ComputeMd5(filePath), action.Type.ToString(), "failed",
                            result.ErrorMessage);

                        lock (_statsLock)
                        {
                            if (_stats.TryGetValue(folder.Id, out var s))
                            {
                                s.FailedFiles++;
                                s.ProcessedFiles++;
                            }
                        }

                        if (!action.ContinueOnFailure) return;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result.OutputPath) && result.OutputPath != currentPath)
                        {
                            currentPath = result.OutputPath;
                        }

                        // Apply text processing if enabled and this is a text file
                        // (already handled as an action above; this is a no-op here)
                    }
                }

                // Optional: standalone text processing if no TextProcessing action in chain
                if ((folder.Actions == null || folder.Actions.Count == 0) && folder.TextProcessing != null && folder.TextProcessing.Enabled)
                {
                    var result = TextProcessor.Process(currentPath, folder.TextProcessing, ctx);
                    if (!result.Success)
                    {
                        LogManager.Warn($"Text processing failed: {result.ErrorMessage}");
                    }
                    else
                    {
                        currentPath = result.OutputPath;
                    }
                }

                // Record dedup
                if (folder.EnableDeduplication)
                {
                    string md5 = VariableResolver.ComputeMd5(filePath);
                    DatabaseManager.RecordDedup(md5, Path.GetFileName(filePath), filePath, new FileInfo(filePath).Length, folder.Id);
                }

                // Log success
                long size = 0;
                try { size = new FileInfo(filePath).Length; } catch { }
                DatabaseManager.LogFileHistory(folder.Id, folder.Name, Path.GetFileName(filePath),
                    filePath, currentPath, size, VariableResolver.ComputeMd5(filePath),
                    "Chain", "success", null);

                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folder.Id, out var s))
                    {
                        s.ProcessedFiles++;
                        s.SuccessFiles++;
                        s.TotalBytes += size;
                        s.LastFileTime = DateTime.Now;

                        // Compute speed
                        if (s.StartTime == default) s.StartTime = DateTime.Now;
                        var elapsed = (DateTime.Now - s.StartTime).TotalSeconds;
                        if (elapsed > 0)
                            s.FilesPerSecond = s.ProcessedFiles / elapsed;
                    }
                }

                fileProgressInfo.Percent = 100;
                fileProgressInfo.Status = "done";
                FileProgressChanged?.Invoke(fileProgressInfo);

                LogMessage?.Invoke($"✓ {folder.Name}: {Path.GetFileName(filePath)} processed");
            }
            catch (Exception ex)
            {
                LogManager.Error($"ProcessFile failed for {filePath}", ex);
                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folder.Id, out var s))
                    {
                        s.FailedFiles++;
                        s.ProcessedFiles++;
                    }
                }
            }
        }

        private void LogSkipped(int folderId, string filePath, string reason)
        {
            lock (_statsLock)
            {
                if (_stats.TryGetValue(folderId, out var s))
                {
                    s.SkippedFiles++;
                    s.ProcessedFiles++;
                }
            }
            LogMessage?.Invoke($"⚠ Skipped: {Path.GetFileName(filePath)} ({reason})");
        }

        // ===========================================================
        // PROGRESS REPORTER
        // ===========================================================

        private void StartOverallReporter()
        {
            Task.Run(async () =>
            {
                while (_globalRunning)
                {
                    try
                    {
                        await Task.Delay(_config.UiRefreshIntervalMs);
                        ReportOverallProgress();
                    }
                    catch { }
                }
            });
        }

        private void ReportOverallProgress()
        {
            var overall = new OverallProgressInfo();
            lock (_statsLock)
            {
                foreach (var s in _stats.Values)
                {
                    overall.TotalFiles += s.TotalFiles;
                    overall.ProcessedFiles += s.ProcessedFiles;
                    overall.SkippedFiles += s.SkippedFiles;
                    overall.FailedFiles += s.FailedFiles;
                    overall.QueuedFiles += s.QueuedFiles;
                    overall.TotalBytes += s.TotalBytes;
                    overall.ProcessedBytes += s.TotalBytes;
                    if (s.Status == "running") overall.ActiveWorkers++;
                }
            }

            if (overall.ProcessedFiles > 0)
                overall.SuccessRate = Math.Round((overall.ProcessedFiles - overall.FailedFiles) * 100.0 / overall.ProcessedFiles, 1);

            TimeSpan elapsed = DateTime.Now - _startTime;
            overall.ElapsedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            if (overall.ProcessedFiles > 0 && elapsed.TotalSeconds > 0)
            {
                overall.FilesPerSecond = Math.Round(overall.ProcessedFiles / elapsed.TotalSeconds, 2);
                if (overall.TotalFiles > overall.ProcessedFiles && overall.FilesPerSecond > 0)
                {
                    int remaining = (int)((overall.TotalFiles - overall.ProcessedFiles) / overall.FilesPerSecond);
                    overall.EstimatedRemaining = $"{remaining / 3600:D2}:{(remaining % 3600) / 60:D2}:{remaining % 60:D2}";
                }
            }
            if (overall.TotalFiles > 0)
                overall.Percent = (int)(overall.ProcessedFiles * 100 / overall.TotalFiles);

            OverallProgressChanged?.Invoke(overall);

            // Report each folder
            lock (_statsLock)
            {
                foreach (var s in _stats.Values)
                {
                    var info = new FolderProgressInfo
                    {
                        FolderId = s.FolderId,
                        FolderName = s.FolderName,
                        TotalFiles = s.TotalFiles,
                        ProcessedFiles = s.ProcessedFiles,
                        SkippedFiles = s.SkippedFiles,
                        FailedFiles = s.FailedFiles,
                        FilesPerSecond = s.FilesPerSecond,
                        Status = s.Status
                    };

                    if (s.TotalFiles > 0)
                        info.Percent = (int)(s.ProcessedFiles * 100 / s.TotalFiles);
                    if (s.LastFileTime != default)
                        info.CurrentFile = s.LastFileName;

                    int remaining = s.TotalFiles - s.ProcessedFiles;
                    if (s.FilesPerSecond > 0 && remaining > 0)
                    {
                        int r = (int)(remaining / s.FilesPerSecond);
                        info.EstimatedRemaining = $"{r / 3600:D2}:{(r % 3600) / 60:D2}:{r % 60:D2}";
                    }

                    FolderProgressChanged?.Invoke(info);
                }
            }
        }

        /// <summary>
        /// Updates the expected total files count for a folder (used when scanning).
        /// </summary>
        public void SetFolderTotalFiles(int folderId, int total)
        {
            lock (_statsLock)
            {
                if (_stats.TryGetValue(folderId, out var s))
                {
                    s.TotalFiles = total;
                }
            }
        }

        public List<FolderProgressInfo> GetFolderProgressSnapshot()
        {
            var list = new List<FolderProgressInfo>();
            lock (_statsLock)
            {
                foreach (var s in _stats.Values)
                {
                    list.Add(new FolderProgressInfo
                    {
                        FolderId = s.FolderId,
                        FolderName = s.FolderName,
                        TotalFiles = s.TotalFiles,
                        ProcessedFiles = s.ProcessedFiles,
                        SkippedFiles = s.SkippedFiles,
                        FailedFiles = s.FailedFiles,
                        FilesPerSecond = s.FilesPerSecond,
                        Status = s.Status,
                        CurrentFile = s.LastFileName
                    });
                }
            }
            return list;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            StopAll();
        }

        private class FolderStats
        {
            public int FolderId { get; set; }
            public string FolderName { get; set; }
            public int TotalFiles { get; set; }
            public int ProcessedFiles { get; set; }
            public int SuccessFiles { get; set; }
            public int SkippedFiles { get; set; }
            public int FailedFiles { get; set; }
            public int QueuedFiles { get; set; }
            public long TotalBytes { get; set; }
            public double FilesPerSecond { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime LastFileTime { get; set; }
            public string LastFileName { get; set; }
            public string Status { get; set; } = "idle";
        }
    }
}
