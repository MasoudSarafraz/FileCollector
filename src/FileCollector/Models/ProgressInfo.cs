namespace FileCollector.Models
{
    /// <summary>
    /// Progress information for a single file processing.
    /// </summary>
    public class FileProgressInfo
    {
        public string FolderName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string CurrentStep { get; set; } = string.Empty;
        public int CurrentStepIndex { get; set; }
        public int TotalSteps { get; set; }
        public int Percent { get; set; }
        public string Status { get; set; } = "idle";
    }

    /// <summary>
    /// Progress information for a single folder.
    /// </summary>
    public class FolderProgressInfo
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; } = string.Empty;
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int SkippedFiles { get; set; }
        public int FailedFiles { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public double FilesPerSecond { get; set; }
        public string EstimatedRemaining { get; set; } = "--:--:--";
        public int Percent { get; set; }
        public string Status { get; set; } = "idle";
    }

    /// <summary>
    /// Overall progress across all folders.
    /// </summary>
    public class OverallProgressInfo
    {
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int SkippedFiles { get; set; }
        public int FailedFiles { get; set; }
        public int QueuedFiles { get; set; }
        public long TotalBytes { get; set; }
        public long ProcessedBytes { get; set; }
        public int Percent { get; set; }
        public double FilesPerSecond { get; set; }
        public string ElapsedTime { get; set; } = "00:00:00";
        public string EstimatedRemaining { get; set; } = "--:--:--";
        public double SuccessRate { get; set; }
        public int ActiveWorkers { get; set; }
    }
}
