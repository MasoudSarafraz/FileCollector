using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Watches a single folder and pushes detected file paths into a queue.
    /// Supports realtime FileSystemWatcher and interval scanning modes.
    /// </summary>
    public class FolderWatcher : IDisposable
    {
        private readonly FolderConfig _config;
        private readonly BlockingCollection<string> _queue;
        private FileSystemWatcher _fsw;
        private System.Threading.Timer _intervalTimer;
        private readonly HashSet<string> _enqueuedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object _enqueuedLock = new object();
        private bool _disposed;
        private bool _running;

        public int FolderId => _config.Id;
        public string FolderName => _config.Name;
        public bool IsRunning => _running;

        /// <summary>
        /// Raised after a scan completes with the total number of files queued.
        /// </summary>
        public event Action<int, int> ScanCompleted;

        public FolderWatcher(FolderConfig config, BlockingCollection<string> queue)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public void Start()
        {
            if (_running) return;
            _running = true;

            try
            {
                if (!Directory.Exists(_config.SourcePath))
                {
                    LogManager.Warn($"Source path does not exist: {_config.SourcePath}");
                    return;
                }

                LogManager.Info($"FolderWatcher.Start: folder='{_config.Name}', path='{_config.SourcePath}', includeSubfolders={_config.IncludeSubfolders}, watchMode={_config.WatchMode}, filter='{_config.FileFilter}'");

                // Run initial scan on a background thread to avoid blocking the UI.
                Task.Run(() =>
                {
                    try
                    {
                        ScanOnce();
                        // After the initial scan, start the realtime watcher (if configured).
                        // We do this AFTER the scan so we don't miss files created during the scan.
                        if (_running && (_config.WatchMode ?? "realtime").ToLowerInvariant() == "realtime")
                        {
                            StartRealtime();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error($"Initial scan failed for '{_config.Name}'", ex);
                    }
                });

                switch ((_config.WatchMode ?? "realtime").ToLowerInvariant())
                {
                    case "interval":
                        StartInterval();
                        break;
                    case "scheduled":
                        StartInterval();
                        break;
                        // realtime: watcher is started AFTER the initial scan above
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"FolderWatcher.Start failed for '{_config.Name}'", ex);
                _running = false;
            }
        }

        public void Stop()
        {
            if (!_running) return;
            _running = false;

            try
            {
                if (_fsw != null)
                {
                    _fsw.EnableRaisingEvents = false;
                    _fsw.Created -= OnFileCreated;
                    _fsw.Changed -= OnFileChanged;
                    _fsw.Renamed -= OnFileRenamed;
                    _fsw.Error -= OnWatcherError;
                    _fsw.Dispose();
                    _fsw = null;
                }
                if (_intervalTimer != null)
                {
                    _intervalTimer.Dispose();
                    _intervalTimer = null;
                }
                lock (_enqueuedLock)
                {
                    _enqueuedFiles.Clear();
                }
            }
            catch { }
        }

        /// <summary>
        /// Pauses the watcher WITHOUT disposing it. The FileSystemWatcher's
        /// EnableRaisingEvents is set to false, but the watcher object itself
        /// is kept alive so Resume() can re-enable it quickly.
        /// The queue and worker are NOT affected — files already in the queue
        /// will still be processed.
        /// </summary>
        public void Pause()
        {
            if (!_running) return;
            try
            {
                if (_fsw != null)
                {
                    _fsw.EnableRaisingEvents = false;
                }
                if (_intervalTimer != null)
                {
                    _intervalTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                LogManager.Info($"FolderWatcher paused for '{_config.Name}'");
            }
            catch (Exception ex)
            {
                LogManager.Warn($"FolderWatcher.Pause failed for '{_config.Name}': {ex.Message}");
            }
        }

        /// <summary>
        /// Resumes a paused watcher. Re-enables FileSystemWatcher events
        /// without re-scanning.
        /// </summary>
        public void Resume()
        {
            if (!_running) return;
            try
            {
                if (_fsw != null)
                {
                    _fsw.EnableRaisingEvents = true;
                }
                if (_intervalTimer != null)
                {
                    int intervalMs = Math.Max(5, _config.IntervalSeconds) * 1000;
                    _intervalTimer.Change(intervalMs, intervalMs);
                }
                LogManager.Info($"FolderWatcher resumed for '{_config.Name}'");
            }
            catch (Exception ex)
            {
                LogManager.Warn($"FolderWatcher.Resume failed for '{_config.Name}': {ex.Message}");
            }
        }

        /// <summary>
        /// Triggers a fresh ScanOnce on a background thread without
        /// restarting the watcher. Use this to pick up files that were
        /// added since the last scan, or files that failed processing
        /// the first time.
        /// </summary>
        public void Rescan()
        {
            if (!_running) return;
            LogManager.Info($"Rescan triggered for '{_config.Name}'");

            // Clear the dedup set so previously-queued files can be re-queued
            // if they still exist. (The user explicitly asked for a re-scan.)
            lock (_enqueuedLock)
            {
                _enqueuedFiles.Clear();
            }

            try
            {
                ScanOnce();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Rescan failed for '{_config.Name}'", ex);
            }
        }

        private void StartRealtime()
        {
            if (_fsw != null) return; // already started

            _fsw = new FileSystemWatcher(_config.SourcePath)
            {
                IncludeSubdirectories = _config.IncludeSubfolders,
                // Watch for as many events as possible — Created alone misses files
                // that are written to existing empty files, or modified after creation.
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                              | NotifyFilters.Size | NotifyFilters.CreationTime
                              | NotifyFilters.DirectoryName,
                InternalBufferSize = 64 * 1024
            };

            _fsw.Created += OnFileCreated;
            _fsw.Changed += OnFileChanged;
            _fsw.Renamed += OnFileRenamed;
            _fsw.Error += OnWatcherError;
            _fsw.EnableRaisingEvents = true;

            LogManager.Info($"Realtime watch started for '{_config.Name}' on '{_config.SourcePath}' (includeSubfolders={_config.IncludeSubfolders}, filter={_config.FileFilter})");
        }

        private void StartInterval()
        {
            int intervalMs = Math.Max(5, _config.IntervalSeconds) * 1000;
            _intervalTimer = new System.Threading.Timer(_ => ScanOnce(), null, 0, intervalMs);
            LogManager.Info($"Interval watch started for '{_config.Name}' (every {intervalMs}ms, includeSubfolders={_config.IncludeSubfolders})");
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            HandleFileEvent(e.FullPath, e.ChangeType);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            HandleFileEvent(e.FullPath, e.ChangeType);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            HandleFileEvent(e.FullPath, e.ChangeType);
        }

        /// <summary>
        /// Common handler for Created/Changed/Renamed events.
        /// Logs the event with subfolder info and enqueues the file.
        /// </summary>
        private void HandleFileEvent(string fullPath, WatcherChangeTypes changeType)
        {
            // Ignore directories
            try
            {
                if (Directory.Exists(fullPath)) return;
            }
            catch { }

            string sourcePathNormalized = _config.SourcePath.TrimEnd('\\', '/');
            string fileDir = Path.GetDirectoryName(fullPath)?.TrimEnd('\\', '/');

            if (_config.IncludeSubfolders
                && !string.IsNullOrEmpty(fileDir)
                && !string.Equals(fileDir, sourcePathNormalized, StringComparison.OrdinalIgnoreCase))
            {
                string relSubfolder = fileDir.Substring(sourcePathNormalized.Length).TrimStart('\\', '/');
                LogManager.Info($"[WATCHER] {changeType} in SUBFOLDER '{relSubfolder}': {Path.GetFileName(fullPath)}");
            }
            else
            {
                LogManager.Debug($"[WATCHER] {changeType}: {fullPath}");
            }
            EnqueueFile(fullPath);
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            LogManager.Warn($"FileSystemWatcher error on '{_config.Name}': {e.GetException().Message}");

            // Try to restart after short delay
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                if (_running)
                {
                    Stop();
                    Start();
                }
            });
        }

        private void ScanOnce()
        {
            int queuedCount = 0;
            int subfolderFileCount = 0;
            int skippedCount = 0;
            try
            {
                if (!Directory.Exists(_config.SourcePath))
                {
                    LogManager.Warn($"ScanOnce: source path does not exist: {_config.SourcePath}");
                    ScanCompleted?.Invoke(_config.Id, 0);
                    return;
                }

                var option = _config.IncludeSubfolders
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                var filters = (_config.FileFilter ?? "*.*").Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

                LogManager.Info($"ScanOnce: scanning '{_config.SourcePath}' (includeSubfolders={_config.IncludeSubfolders}, option={option}, filters=[{string.Join(",", filters)}])");

                string sourcePathNormalized = _config.SourcePath.TrimEnd('\\', '/');

                // Count total directories visited for diagnostics
                int dirCount = 0;
                if (_config.IncludeSubfolders)
                {
                    try
                    {
                        foreach (var d in Directory.EnumerateDirectories(_config.SourcePath, "*", SearchOption.AllDirectories))
                        {
                            dirCount++;
                            LogManager.Info($"  [SCAN] subfolder found: {d}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warn($"  [SCAN] could not enumerate subfolders: {ex.Message}");
                    }
                    LogManager.Info($"  [SCAN] total subdirectories: {dirCount}");
                }

                foreach (var filter in filters)
                {
                    IEnumerable<string> files;
                    try
                    {
                        files = Directory.EnumerateFiles(_config.SourcePath, filter.Trim(), option);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warn($"  [SCAN] EnumerateFiles failed for filter '{filter}': {ex.Message}");
                        continue;
                    }

                    foreach (var file in files)
                    {
                        bool enqueued = EnqueueFile(file);
                        if (enqueued)
                        {
                            queuedCount++;

                            string fileDir = Path.GetDirectoryName(file)?.TrimEnd('\\', '/');
                            if (_config.IncludeSubfolders
                                && !string.IsNullOrEmpty(fileDir)
                                && !string.Equals(fileDir, sourcePathNormalized, StringComparison.OrdinalIgnoreCase))
                            {
                                subfolderFileCount++;
                                string relSubfolder = fileDir.Substring(sourcePathNormalized.Length).TrimStart('\\', '/');
                                LogManager.Info($"  [SUBFOLDER] '{relSubfolder}' -> {Path.GetFileName(file)}");
                            }
                        }
                        else
                        {
                            skippedCount++;
                        }
                    }
                }

                LogManager.Info($"ScanOnce: '{_config.Name}' found {queuedCount + skippedCount} files, queued {queuedCount} ({subfolderFileCount} from subfolders), skipped {skippedCount}");
            }
            catch (Exception ex)
            {
                LogManager.Warn($"ScanOnce failed for '{_config.Name}': {ex.Message}");
            }
            finally
            {
                ScanCompleted?.Invoke(_config.Id, queuedCount);
            }
        }

        /// <summary>
        /// Returns true if the file was queued (or scheduled for retry), false if it was filtered out.
        /// Uses a deduplication set so the same file isn't queued twice (e.g. Created + Changed events).
        /// </summary>
        private bool EnqueueFile(string path)
        {
            try
            {
                if (!MatchesFilter(path)) return false;

                var fi = new FileInfo(path);
                if (_config.MinSizeBytes > 0 && fi.Length < _config.MinSizeBytes) return false;
                if (_config.MaxSizeBytes > 0 && fi.Length > _config.MaxSizeBytes) return false;

                // Deduplicate — same file may trigger Created + Changed + Renamed
                lock (_enqueuedLock)
                {
                    if (_enqueuedFiles.Contains(path))
                    {
                        // Already queued; don't queue again unless it's been processed.
                        // For simplicity, we skip re-queuing. If the user wants re-processing
                        // of modified files, they should use 'interval' mode.
                        return false;
                    }
                    _enqueuedFiles.Add(path);
                }

                // Try to enqueue right away if file is ready; otherwise retry on a background thread
                if (IsFileReady(path))
                {
                    _queue.Add(path);
                    return true;
                }
                else
                {
                    // File is still being written or locked. Retry on a background task.
                    Task.Run(async () =>
                    {
                        for (int i = 0; i < 30 && _running; i++)
                        {
                            await Task.Delay(500);
                            if (IsFileReady(path))
                            {
                                try { _queue.Add(path); } catch { }
                                return;
                            }
                        }
                        LogManager.Warn($"File never became ready, skipping: {path}");
                        lock (_enqueuedLock)
                        {
                            _enqueuedFiles.Remove(path);
                        }
                    });
                    return true; // file was eligible, just deferred
                }
            }
            catch (Exception ex)
            {
                LogManager.Debug($"EnqueueFile failed for '{path}': {ex.Message}");
                return false;
            }
        }

        private bool MatchesFilter(string path)
        {
            if (string.IsNullOrEmpty(_config.FileFilter) || _config.FileFilter == "*.*") return true;

            string filename = Path.GetFileName(path);
            var filters = _config.FileFilter.Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var f in filters)
            {
                string pattern = f.Trim();
                if (string.IsNullOrEmpty(pattern)) continue;
                if (MatchesWildcard(filename, pattern)) return true;
            }
            return false;
        }

        private bool MatchesWildcard(string input, string pattern)
        {
            // Convert wildcard to regex
            string regex = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(input, regex,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Checks if a file can be opened for reading. Uses FileShare.ReadWrite
        /// (NOT FileShare.None) so files that are still being written by another
        /// process can still be read. Many native libraries (especially on
        /// runtimes/linux-arm64/native) keep files locked with ReadWrite share.
        /// </summary>
        private bool IsFileReady(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                // Common for system files; treat as not-ready so we can retry
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception ex)
            {
                LogManager.Debug($"IsFileReady unexpected error for '{path}': {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
        }
    }
}
