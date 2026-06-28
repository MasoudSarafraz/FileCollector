using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FileCollector.Models
{
    /// <summary>
    /// Represents a single watched folder configuration.
    /// </summary>
    public class FolderConfig
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Watch subdirectories recursively.
        /// </summary>
        public bool IncludeSubfolders { get; set; } = true;

        /// <summary>
        /// File pattern filter, e.g. "*.pdf;*.docx".
        /// </summary>
        public string FileFilter { get; set; } = "*.*";

        /// <summary>
        /// Minimum file size in bytes (0 = no limit).
        /// </summary>
        public long MinSizeBytes { get; set; } = 0;

        /// <summary>
        /// Maximum file size in bytes (0 = no limit).
        /// </summary>
        public long MaxSizeBytes { get; set; } = 0;

        /// <summary>
        /// Watch mode: "realtime", "interval", "scheduled"
        /// </summary>
        public string WatchMode { get; set; } = "realtime";

        /// <summary>
        /// For interval mode, interval in seconds.
        /// </summary>
        public int IntervalSeconds { get; set; } = 300;

        /// <summary>
        /// Whether this folder is currently enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Conflict resolution strategy: overwrite, skip, rename, keepboth
        /// </summary>
        public string ConflictStrategy { get; set; } = "rename";

        /// <summary>
        /// The destination folder for copy/move actions.
        /// </summary>
        public string DestinationPath { get; set; } = string.Empty;

        /// <summary>
        /// Subfolder structure pattern at destination using variables.
        /// e.g. "{year}\{month}" or "{ext}\{date}"
        /// </summary>
        public string DestinationSubfolderPattern { get; set; } = string.Empty;

        /// <summary>
        /// Output filename pattern using variables. Leave empty to keep original.
        /// </summary>
        public string DestinationFilenamePattern { get; set; } = string.Empty;

        /// <summary>
        /// List of actions in this folder's processing chain (max 5).
        /// </summary>
        public List<ActionConfig> Actions { get; set; } = new List<ActionConfig>();

        /// <summary>
        /// Optional text processing configuration.
        /// </summary>
        public TextProcessingConfig TextProcessing { get; set; }

        /// <summary>
        /// Optional remote database storage configuration.
        /// </summary>
        public DatabaseConfig DatabaseStorage { get; set; }

        /// <summary>
        /// Whether to write a successful file's MD5 into the local dedup table.
        /// </summary>
        public bool EnableDeduplication { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
