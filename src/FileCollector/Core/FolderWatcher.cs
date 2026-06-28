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
        private bool _disposed;
        private bool _running;

        public int FolderId => _config.Id;
        public string FolderName => _config.Name;
        public bool IsRunning => _running;

        /// <summary>
        /// Raised after a scan completes with the total number of files queued.
        /// Used by CollectorEngine to set the folder's TotalFiles counter so the
        /// progress bar and ETA can be computed.
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

                // Run initial scan on a background thread to avoid blocking the UI.
                // FileSystemWatcher only fires for NEW files, so without this scan
                // existing files would never be processed.
                Task.Run(() => ScanOnce());

                switch ((_config.WatchMode ?? "realtime").ToLowerInvariant())
                {
                    case "realtime":
                        StartRealtime();
                        break;
                    case "interval":
                        StartInterval();
                        break;
                    case "scheduled":
                        StartInterval();
                        break;
                    default:
                        StartRealtime();
                        break;
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
                    _fsw.Dispose();
                    _fsw = null;
                }
                if (_intervalTimer != null)
                {
                    _intervalTimer.Dispose();
                    _intervalTimer = null;
                }
            }
            catch { }
        }

        private void StartRealtime()
        {
            _fsw = new FileSystemWatcher(_config.SourcePath)
            {
                IncludeSubdirectories = _config.IncludeSubfolders,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                InternalBufferSize = 64 * 1024
            };

            _fsw.Created += OnFileCreated;
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
            string sourcePathNormalized = _config.SourcePath.TrimEnd('\\', '/');
            string fileDir = Path.GetDirectoryName(e.FullPath)?.TrimEnd('\\', '/');

            if (_config.IncludeSubfolders
                && !string.IsNullOrEmpty(fileDir)
                && !string.Equals(fileDir, sourcePathNormalized, StringComparison.OrdinalIgnoreCase))
            {
                string relSubfolder = fileDir.Substring(sourcePathNormalized.Length).TrimStart('\\', '/');
                LogManager.Info($"File detected in SUBFOLDER '{relSubfolder}': {Path.GetFileName(e.FullPath)}");
            }
            else
            {
                LogManager.Debug($"File detected: {e.FullPath} (changeType={e.ChangeType})");
            }
            EnqueueFile(e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            LogManager.Debug($"File renamed: {e.FullPath} (was: {e.OldFullPath})");
            EnqueueFile(e.FullPath);
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

                LogManager.Info($"ScanOnce: scanning '{_config.SourcePath}' (includeSubfolders={_config.IncludeSubfolders}, option={option}, filters={filters.Length})");

                // Normalize source path for subfolder comparison
                string sourcePathNormalized = _config.SourcePath.TrimEnd('\\', '/');

                foreach (var filter in filters)
                {
                    foreach (var file in Directory.EnumerateFiles(_config.SourcePath, filter.Trim(), option))
                    {
                        if (EnqueueFile(file))
                        {
                            queuedCount++;

                            // Log explicitly when a file comes from a subfolder
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
                    }
                }

                LogManager.Info($"ScanOnce: '{_config.Name}' found and queued {queuedCount} files ({subfolderFileCount} from subfolders)");
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
        /// </summary>
        private bool EnqueueFile(string path)
        {
            try
            {
                if (!MatchesFilter(path)) return false;

                var fi = new FileInfo(path);
                if (_config.MinSizeBytes > 0 && fi.Length < _config.MinSizeBytes) return false;
                if (_config.MaxSizeBytes > 0 && fi.Length > _config.MaxSizeBytes) return false;

                // Try to enqueue right away if file is ready; otherwise retry on a background thread
                if (IsFileReady(path))
                {
                    _queue.Add(path);
                    return true;
                }
                else
                {
                    // File is still being written. Retry on a background task with small delay.
                    Task.Run(async () =>
                    {
                        for (int i = 0; i < 30 && _running; i++)
                        {
                            await Task.Delay(500);
                            if (IsFileReady(path))
                            {
                                _queue.Add(path);
                                return;
                            }
                        }
                        // File never became ready; skip silently.
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

        private bool IsFileReady(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
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
