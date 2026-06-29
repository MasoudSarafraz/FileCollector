using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Lightweight file logger with daily rotation and thread-safety.
    /// </summary>
    public static class LogManager
    {
        private static readonly object _lock = new object();
        private static string _logFilePath = "filecollector.log";
        private static int _retentionDays = 30;
        private static bool _initialized = false;
        private static PersianCalendar _persianCalendar = new PersianCalendar();

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                var config = ConfigManager.Load();
                _logFilePath = config.LogFilePath;
                _retentionDays = config.LogRetentionDays;

                string dir = Path.GetDirectoryName(Path.GetFullPath(_logFilePath));
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                CleanupOldLogs();
            }
            catch
            {
                _logFilePath = "filecollector.log";
            }
        }

        public static void Shutdown()
        {
            // Flush any pending writes
            lock (_lock) { }
        }

        private static void CleanupOldLogs()
        {
            try
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(_logFilePath)) ?? ".";
                string baseName = Path.GetFileNameWithoutExtension(_logFilePath);
                string ext = Path.GetExtension(_logFilePath);

                foreach (var file in Directory.GetFiles(dir, baseName + "_*" + ext))
                {
                    if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-_retentionDays))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }
            catch { }
        }

        private static string GetDailyLogPath()
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(_logFilePath)) ?? ".";
            string baseName = Path.GetFileNameWithoutExtension(_logFilePath);
            string ext = Path.GetExtension(_logFilePath);
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(dir, baseName + "_" + date + ext);
        }

        private static void Write(string level, string message, Exception ex = null)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string threadId = Thread.CurrentThread.ManagedThreadId.ToString("D3");
                string line = $"[{timestamp}] [T{threadId}] [{level,-5}] {message}";
                if (ex != null)
                {
                    line += Environment.NewLine + "  Exception: " + ex.GetType().Name + ": " + ex.Message;
                    line += Environment.NewLine + "  Stack: " + (ex.StackTrace ?? "").Replace(Environment.NewLine, Environment.NewLine + "    ");
                }

                lock (_lock)
                {
                    string path = GetDailyLogPath();
                    File.AppendAllText(path, line + Environment.NewLine);
                }

                System.Diagnostics.Debug.WriteLine(line);
            }
            catch
            {
                // Suppress logging errors
            }
        }

        public static void Debug(string message) => Write("DEBUG", message);
        public static void Info(string message) => Write("INFO", message);
        public static void Warn(string message) => Write("WARN", message);
        public static void Warn(string message, Exception ex) => Write("WARN", message, ex);
        public static void Error(string message) => Write("ERROR", message);
        public static void Error(string message, Exception ex) => Write("ERROR", message, ex);
        public static void Fatal(string message, Exception ex) => Write("FATAL", message, ex);
    }
}
