using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly Dictionary<int, ManualResetEventSlim> _pauseEvents = new Dictionary<int, ManualResetEventSlim>();

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
        /// Ensures the engine is running (global CTS + reporter).
        /// Called automatically by StartFolder and StartAll.
        /// </summary>
        private void EnsureEngineRunning()
        {
            if (_globalRunning) return;
            _globalRunning = true;
            _globalCts = new CancellationTokenSource();
            _startTime = DateTime.Now;
            StartOverallReporter();
        }

        /// <summary>
        /// Starts watching all enabled folders.
        /// </summary>
        public void StartAll()
        {
            EnsureEngineRunning();

            int startedCount = 0;
            int skippedCount = 0;
            int failedCount = 0;

            foreach (var folder in _config.Folders)
            {
                if (!folder.Enabled)
                {
                    skippedCount++;
                    LogManager.Info($"StartAll: skipping folder '{folder.Name}' (Id={folder.Id}) — Enabled=false");
                    continue;
                }

                if (_watchers.ContainsKey(folder.Id))
                {
                    skippedCount++;
                    LogManager.Info($"StartAll: folder '{folder.Name}' (Id={folder.Id}) already running, skipping");
                    continue;
                }

                bool ok = StartFolder(folder);
                if (ok) startedCount++;
                else failedCount++;
            }

            LogMessage?.Invoke($"StartAll complete: started={startedCount}, already-running={skippedCount}, failed={failedCount}");
            LogManager.Info($"StartAll complete: started={startedCount}, already-running={skippedCount}, failed={failedCount}");
        }

        /// <summary>
        /// Stops all watchers and workers. Does NOT block the UI — workers
        /// are cancelled and allowed to exit on their own.
        /// </summary>
        public void StopAll()
        {
            if (!_globalRunning) return;
            _globalRunning = false;

            try { _globalCts?.Cancel(); } catch { }

            // Unblock all paused workers so they can see the cancellation
            lock (_statsLock)
            {
                foreach (var pe in _pauseEvents.Values)
                {
                    try { pe.Set(); } catch { }
                }
            }

            foreach (var f in _watchers.Values.ToList()) f.Stop();
            foreach (var q in _queues.Values.ToList())
            {
                try { q.CompleteAdding(); } catch { }
            }

            // Do NOT call Task.WaitAll here — it blocks the UI thread.
            // Workers will exit on their own when their cancellation token fires.

            _watchers.Clear();
            _queues.Clear();
            _workers.Clear();
            _cts.Clear();
            _pauseEvents.Clear();

            lock (_statsLock)
            {
                _stats.Clear();
            }

            LogMessage?.Invoke("All watchers stopped.");
        }

        /// <summary>
        /// Starts a single folder watcher. Auto-starts the engine if needed,
        /// so the user can click "Start" on a single folder without having to
        /// click "Start All" first.
        /// If the folder is already running, this triggers a re-scan of the
        /// source folder so any new or modified files get picked up.
        /// </summary>
        public bool StartFolder(FolderConfig folder)
        {
            EnsureEngineRunning();

            // If already watching, trigger a re-scan instead of doing nothing.
            // This lets the user click "Start" again after the initial scan
            // completes to pick up files that were added later, or files that
            // failed the first time.
            if (_watchers.ContainsKey(folder.Id))
            {
                TriggerRescan(folder);
                return true;
            }

            try
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(_globalCts?.Token ?? CancellationToken.None);
                var queue = new BlockingCollection<string>(1000);
                var watcher = new FolderWatcher(folder, queue);
                watcher.ScanCompleted += OnScanCompleted;

                _watchers[folder.Id] = watcher;
                _queues[folder.Id] = queue;
                _cts[folder.Id] = cts;
                _pauseEvents[folder.Id] = new ManualResetEventSlim(true); // initially not paused

                lock (_statsLock)
                {
                    _stats[folder.Id] = new FolderStats
                    {
                        FolderId = folder.Id,
                        FolderName = folder.Name,
                        Status = "running",
                        StartTime = DateTime.Now
                    };
                }

                // Start worker
                _workers[folder.Id] = Task.Run(() => WorkerLoop(folder, queue, cts.Token));

                // Start watcher (runs initial scan on background thread)
                watcher.Start();

                // Warn if no actions configured AND no database storage AND no text processing.
                // Files will be scanned but nothing will happen to them.
                if ((folder.Actions == null || folder.Actions.Count == 0)
                    && (folder.TextProcessing == null || !folder.TextProcessing.Enabled)
                    && (folder.DatabaseStorage == null || !folder.DatabaseStorage.Enabled))
                {
                    LogMessage?.Invoke($"⚠ هشدار: پوشه '{folder.Name}' هیچ اکشنی پیکربندی نشده. فایل‌ها اسکن می‌شوند ولی هیچ کاری روی آن‌ها انجام نمی‌شود. لطفاً در تنظیمات پوشه، اکشن (مثل Copy یا Move) اضافه کنید.");
                    LogManager.Warn($"Folder '{folder.Name}' has no actions configured. Files will be scanned but not processed.");
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
        /// Triggers a fresh re-scan of a folder that is already running.
        /// Does NOT restart the watcher — just calls ScanOnce again on a
        /// background thread so any new/modified files get enqueued.
        /// </summary>
        public void TriggerRescan(FolderConfig folder)
        {
            if (!_watchers.TryGetValue(folder.Id, out var watcher))
            {
                return;
            }

            LogMessage?.Invoke($"🔄 در حال اسکن مجدد پوشه: {folder.Name}");
            LogManager.Info($"TriggerRescan: re-scanning folder '{folder.Name}' (Id={folder.Id})");

            // Reset folder stats so progress bar starts fresh
            lock (_statsLock)
            {
                if (_stats.TryGetValue(folder.Id, out var s))
                {
                    s.TotalFiles = 0;
                    s.ProcessedFiles = 0;
                    s.SuccessFiles = 0;
                    s.SkippedFiles = 0;
                    s.FailedFiles = 0;
                    s.TotalBytes = 0;
                    s.StartTime = DateTime.Now;
                    s.Status = "running";
                }
            }

            // Trigger the re-scan on a background thread
            System.Threading.Tasks.Task.Run(() => watcher.Rescan());
        }

        /// <summary>
        /// Returns true if the given folder is currently being watched.
        /// </summary>
        public bool IsFolderRunning(int folderId)
        {
            return _watchers.ContainsKey(folderId);
        }

        /// <summary>
        /// Restarts a single folder watcher (stop + start fresh).
        /// Used when the user clicks Start on a folder that was previously stopped.
        /// </summary>
        public void RestartFolder(FolderConfig folder)
        {
            StopFolder(folder.Id);

            // Clean up entries so StartFolder can re-create them
            // (StopFolder already does this, but we double-check here)
            _watchers.Remove(folder.Id);
            _queues.Remove(folder.Id);
            _workers.Remove(folder.Id);
            _cts.Remove(folder.Id);
            _pauseEvents.Remove(folder.Id);

            lock (_statsLock)
            {
                _stats.Remove(folder.Id);
            }

            // Small delay to let old worker exit
            Thread.Sleep(100);

            StartFolder(folder);
        }

        private void OnScanCompleted(int folderId, int queuedCount)
        {
            // Set TotalFiles to the number of files found in this scan.
            // Use Math.Max with (processed + queued) because the worker may
            // have already processed some files by the time ScanCompleted fires,
            // and we don't want TotalFiles < ProcessedFiles (which would make
            // percent > 100%).
            lock (_statsLock)
            {
                if (_stats.TryGetValue(folderId, out var s))
                {
                    int minNeeded = s.ProcessedFiles + queuedCount;
                    if (minNeeded > s.TotalFiles)
                        s.TotalFiles = minNeeded;
                }
            }
        }

        /// <summary>
        /// Stops a single folder watcher. Fully removes it from internal
        /// dictionaries so StartFolder() can re-start it later.
        /// Resets all progress stats to 0 so the UI shows initial state.
        /// </summary>
        public void StopFolder(int folderId)
        {
            if (!_watchers.ContainsKey(folderId)) return;

            try
            {
                // Unblock the worker if it's paused (so it can see the cancellation)
                lock (_statsLock)
                {
                    if (_pauseEvents.TryGetValue(folderId, out var pe))
                        pe.Set();
                }

                _watchers[folderId]?.Stop();
                _queues[folderId]?.CompleteAdding();
                _cts[folderId]?.Cancel();

                // Remove from all dictionaries so StartFolder can re-create them
                _watchers.Remove(folderId);
                _queues.Remove(folderId);
                _cts.Remove(folderId);
                _workers.Remove(folderId);
                _pauseEvents.Remove(folderId);

                // Reset stats to 0 so progress bars go back to initial state
                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folderId, out var s))
                    {
                        s.Status = "stopped";
                        s.TotalFiles = 0;
                        s.ProcessedFiles = 0;
                        s.SuccessFiles = 0;
                        s.SkippedFiles = 0;
                        s.FailedFiles = 0;
                        s.TotalBytes = 0;
                        s.FilesPerSecond = 0;
                        s.LastFileName = null;
                        s.StartTime = default;
                        s.LastFileTime = default;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Pauses a single folder. Blocks the worker thread (freezes processing
        /// of queued files) AND disables watcher events (no new files enqueued).
        /// Progress bars stay frozen at their current position.
        /// Use ResumeFolder() to continue from where it was paused.
        /// </summary>
        public void PauseFolder(int folderId)
        {
            // Pause the worker thread (freeze processing) AND the watcher
            // (no new files enqueued). Progress bars stay frozen at current position.
            lock (_statsLock)
            {
                if (_pauseEvents.TryGetValue(folderId, out var pe))
                    pe.Reset(); // block the worker
            }

            if (_watchers.TryGetValue(folderId, out var w))
            {
                LogManager.Info($"PauseFolder: folderId={folderId}, watcher running={w.IsRunning}, paused={w.IsPaused}");
                w.Pause();
            }

            lock (_statsLock)
            {
                if (_stats.TryGetValue(folderId, out var s)) s.Status = "paused";
            }
        }

        /// <summary>
        /// Resumes a paused folder. Unblocks the worker thread and re-enables
        /// watcher events. Progress continues from where it was frozen.
        /// </summary>
        public void ResumeFolder(int folderId)
        {
            // Unblock the worker thread
            lock (_statsLock)
            {
                if (_pauseEvents.TryGetValue(folderId, out var pe))
                    pe.Set(); // unblock the worker
            }

            if (_watchers.TryGetValue(folderId, out var w))
            {
                LogManager.Info($"ResumeFolder: folderId={folderId}, watcher running={w.IsRunning}, paused={w.IsPaused}");
                w.Resume();
            }

            lock (_statsLock)
            {
                if (_stats.TryGetValue(folderId, out var s)) s.Status = "running";
            }
        }

        // ===========================================================
        // WORKER LOOP
        // ===========================================================

        private void WorkerLoop(FolderConfig folder, BlockingCollection<string> queue, CancellationToken token)
        {
            var executor = new ActionExecutor();
            // Single worker per folder. If parallelism is needed later, spawn N tasks here
            // each pulling from the same BlockingCollection<string>.
            try
            {
                foreach (var filePath in queue.GetConsumingEnumerable(token))
                {
                    if (token.IsCancellationRequested) break;

                    // Wait here if the folder is paused. This blocks the worker
                    // from processing the next file, effectively freezing progress.
                    // The file stays in the queue (already dequeued, but not processed).
                    // When resumed, processing continues from this point.
                    ManualResetEventSlim pauseEvent = null;
                    lock (_statsLock)
                    {
                        _pauseEvents.TryGetValue(folder.Id, out pauseEvent);
                    }
                    if (pauseEvent != null)
                    {
                        try
                        {
                            pauseEvent.Wait(token);
                        }
                        catch (OperationCanceledException)
                        {
                            break; // stopped while paused
                        }
                    }

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
            int currentCounter = Interlocked.Increment(ref _globalCounter);

            // Mark folder as actively processing
            lock (_statsLock)
            {
                if (_stats.TryGetValue(folder.Id, out var sActive))
                {
                    sActive.Status = "processing";
                    sActive.LastFileName = Path.GetFileName(filePath);
                }
            }

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

            // Cache file info once at the start so we don't re-stat the file
            // (which may have been moved/deleted by the time we log success).
            long originalSize = 0;
            try { originalSize = new FileInfo(filePath).Length; } catch { }
            string originalMd5 = VariableResolver.ComputeMd5(filePath);

            try
            {
                if (!File.Exists(filePath))
                {
                    LogSkipped(folder.Id, filePath, "File disappeared");
                    return;
                }

                // Deduplication check
                if (folder.EnableDeduplication && !string.IsNullOrEmpty(originalMd5))
                {
                    if (DatabaseManager.IsMd5Seen(originalMd5))
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
                bool chainAborted = false;

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
                            filePath, currentPath, originalSize, originalMd5,
                            action.Type.ToString(), "failed", result.ErrorMessage);

                        lock (_statsLock)
                        {
                            if (_stats.TryGetValue(folder.Id, out var s))
                            {
                                s.FailedFiles++;
                                s.ProcessedFiles++;
                            }
                        }

                        if (!action.ContinueOnFailure)
                        {
                            chainAborted = true;
                            break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(result.OutputPath) && result.OutputPath != currentPath)
                    {
                        // Action produced a new file path (Copy, Move, Zip, etc.)
                        currentPath = result.OutputPath;
                    }
                }

                if (chainAborted) return;

                // Optional: standalone text processing if no actions in chain at all.
                // This runs BEFORE database storage so the processed file gets stored,
                // not the original.
                if ((folder.Actions == null || folder.Actions.Count == 0)
                    && folder.TextProcessing != null && folder.TextProcessing.Enabled)
                {
                    var tpResult = TextProcessor.Process(currentPath, folder.TextProcessing, ctx);
                    if (!tpResult.Success)
                    {
                        LogManager.Warn($"Text processing failed: {tpResult.ErrorMessage}");
                    }
                    else
                    {
                        currentPath = tpResult.OutputPath;
                    }
                }

                // Track whether database storage failed (so we log correctly later)
                bool dbStoreFailed = false;
                string dbStoreError = null;

                // Auto-store to database if folder.DatabaseStorage is enabled.
                // (DatabaseStore was removed as an action type — it's now automatic
                // when the Database tab is configured and enabled.)
                if (folder.DatabaseStorage != null && folder.DatabaseStorage.Enabled)
                {
                    try
                    {
                        // Recompute size and MD5 from the CURRENT path (post-action-chain),
                        // not the original file. This ensures the stored metadata matches
                        // the actual stored bytes.
                        long dbSize = 0;
                        try { dbSize = new FileInfo(currentPath).Length; } catch { }
                        string dbMd5 = VariableResolver.ComputeMd5(currentPath);

                        string subfolder = string.IsNullOrEmpty(folder.DatabaseStorage.SubfolderPattern)
                            ? ""
                            : VariableResolver.Resolve(folder.DatabaseStorage.SubfolderPattern, ctx);

                        bool dbOk = DatabaseManager.StoreFile(
                            folder.DatabaseStorage, currentPath, Path.GetFileName(currentPath),
                            dbSize, dbMd5, folder.SourcePath, subfolder,
                            out string dbStoragePath, out string dbError);

                        if (!dbOk)
                        {
                            dbStoreFailed = true;
                            dbStoreError = dbError;
                            LogManager.Warn($"Auto database store failed: {dbError}");
                        }
                        else
                        {
                            LogManager.Info($"Auto-stored to database: {Path.GetFileName(currentPath)} -> {dbStoragePath}");
                        }
                    }
                    catch (Exception dbEx)
                    {
                        dbStoreFailed = true;
                        dbStoreError = dbEx.Message;
                        LogManager.Error("Auto database store failed", dbEx);
                    }
                }

                // Record dedup
                if (folder.EnableDeduplication && !string.IsNullOrEmpty(originalMd5))
                {
                    DatabaseManager.RecordDedup(originalMd5, Path.GetFileName(filePath), filePath, originalSize, folder.Id);
                }

                // Log result: "success" if everything OK, "db_failed" if DB store failed
                string finalStatus = dbStoreFailed ? "db_failed" : "success";
                string finalError = dbStoreFailed ? dbStoreError : null;
                DatabaseManager.LogFileHistory(folder.Id, folder.Name, Path.GetFileName(filePath),
                    filePath, currentPath, originalSize, originalMd5,
                    "Chain", finalStatus, finalError);

                // Log to file log (not just UI) so user can see activity in filecollector.log
                string dbLogSuffix = dbStoreFailed ? " [DB STORE FAILED]" : "";
                LogManager.Info($"✓ Processed: {Path.GetFileName(filePath)} (folder='{folder.Name}', size={originalSize}B, actions={folder.Actions?.Count ?? 0}, dest='{currentPath}'){dbLogSuffix}");

                lock (_statsLock)
                {
                    if (_stats.TryGetValue(folder.Id, out var s))
                    {
                        s.ProcessedFiles++;
                        s.SuccessFiles++;
                        s.TotalBytes += originalSize;
                        s.LastFileTime = DateTime.Now;
                        s.LastFileName = Path.GetFileName(filePath);

                        // Folder finished processing this file — go back to "running"
                        // (watcher is still active, waiting for next file).
                        s.Status = "running";

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
                    // Get current queue count
                    int inQueue = 0;
                    BlockingCollection<string> q;
                    if (_queues.TryGetValue(s.FolderId, out q))
                    {
                        try { inQueue = q.Count; } catch { }
                    }

                    // Update TotalFiles to be at least (processed + queued) so the
                    // progress bar never goes backwards. But don't shrink it either,
                    // because the user might re-scan and we don't want the bar to
                    // jump back to 0%.
                    int minTotal = s.ProcessedFiles + inQueue;
                    if (minTotal > s.TotalFiles)
                        s.TotalFiles = minTotal;

                    overall.TotalFiles += s.TotalFiles;
                    overall.ProcessedFiles += s.ProcessedFiles;
                    overall.SkippedFiles += s.SkippedFiles;
                    overall.FailedFiles += s.FailedFiles;
                    overall.QueuedFiles += inQueue;
                    overall.TotalBytes += s.TotalBytes;
                    overall.ProcessedBytes += s.TotalBytes;
                    if (s.Status == "running" || s.Status == "processing")
                    {
                        overall.ActiveWorkers++;
                    }
                }
            }

            if (overall.ProcessedFiles > 0)
                overall.SuccessRate = Math.Round((overall.ProcessedFiles - overall.FailedFiles) * 100.0 / overall.ProcessedFiles, 1);

            TimeSpan elapsed = DateTime.Now - _startTime;
            overall.ElapsedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            if (overall.ProcessedFiles > 0 && elapsed.TotalSeconds > 0)
            {
                overall.FilesPerSecond = Math.Round(overall.ProcessedFiles / elapsed.TotalSeconds, 2);
                int remainingFiles = overall.TotalFiles - overall.ProcessedFiles;
                if (remainingFiles > 0 && overall.FilesPerSecond > 0)
                {
                    int remaining = (int)(remainingFiles / overall.FilesPerSecond);
                    overall.EstimatedRemaining = $"{remaining / 3600:D2}:{(remaining % 3600) / 60:D2}:{remaining % 60:D2}";
                }
            }

            // Percent: 0 if no files, 100 if all processed, otherwise processed/total
            if (overall.TotalFiles > 0)
            {
                int pct = (int)(overall.ProcessedFiles * 100 / overall.TotalFiles);
                // Clamp to 0-100
                overall.Percent = Math.Min(100, Math.Max(0, pct));
            }
            else
            {
                overall.Percent = 0;
            }

            OverallProgressChanged?.Invoke(overall);

            // Report each folder
            lock (_statsLock)
            {
                foreach (var s in _stats.Values)
                {
                    int inQueue = 0;
                    BlockingCollection<string> q;
                    if (_queues.TryGetValue(s.FolderId, out q))
                    {
                        try { inQueue = q.Count; } catch { }
                    }

                    var info = new FolderProgressInfo
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
                    };

                    if (s.TotalFiles > 0)
                        info.Percent = Math.Min(100, Math.Max(0, (int)(s.ProcessedFiles * 100 / s.TotalFiles)));
                    else
                        info.Percent = 0;

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
