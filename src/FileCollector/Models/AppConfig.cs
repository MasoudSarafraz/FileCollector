using System.Collections.Generic;
using Newtonsoft.Json;

namespace FileCollector.Models
{
    /// <summary>
    /// Global application configuration.
    /// </summary>
    public class AppConfig
    {
        public List<FolderConfig> Folders { get; set; } = new List<FolderConfig>();

        /// <summary>
        /// Whether the engine should start watching on application launch.
        /// </summary>
        public bool AutoStartWatching { get; set; } = false;

        /// <summary>
        /// Number of parallel workers per folder (1 = serial).
        /// </summary>
        public int WorkersPerFolder { get; set; } = 1;

        /// <summary>
        /// UI refresh interval in milliseconds.
        /// </summary>
        public int UiRefreshIntervalMs { get; set; } = 100;

        /// <summary>
        /// Path to local SQLite history database.
        /// </summary>
        public string LocalDatabasePath { get; set; } = "history.db";

        /// <summary>
        /// Path to log file.
        /// </summary>
        public string LogFilePath { get; set; } = "filecollector.log";

        /// <summary>
        /// Log retention in days.
        /// </summary>
        public int LogRetentionDays { get; set; } = 30;

        /// <summary>
        /// Minimize to system tray on close.
        /// </summary>
        public bool MinimizeToTray { get; set; } = true;

        /// <summary>
        /// Auto-start with Windows.
        /// </summary>
        public bool AutoStartWithWindows { get; set; } = false;

        public string Language { get; set; } = "fa-IR";
    }
}
