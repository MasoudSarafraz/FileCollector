using Newtonsoft.Json;

namespace FileCollector.Models
{
    /// <summary>
    /// Storage mode for files in remote database.
    /// </summary>
    public enum DatabaseStorageMode
    {
        /// <summary>
        /// Files stored directly as VARBINARY(MAX) BLOB.
        /// Best for small files (< 5 MB).
        /// </summary>
        BlobDirect,

        /// <summary>
        /// Metadata stored in DB, files on file share (UNC path).
        /// Best for large files.
        /// </summary>
        Hybrid,

        /// <summary>
        /// Use SQL Server FILESTREAM feature.
        /// Requires server-side configuration.
        /// </summary>
        FileStream
    }

    /// <summary>
    /// Remote database storage configuration for a folder.
    /// </summary>
    public class DatabaseConfig
    {
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Connection string for the remote SQL Server database.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Schema + table name, e.g. "dbo.collected_files".
        /// </summary>
        public string TableName { get; set; } = "dbo.collected_files";

        /// <summary>
        /// Storage strategy.
        /// </summary>
        public DatabaseStorageMode Mode { get; set; } = DatabaseStorageMode.Hybrid;

        /// <summary>
        /// UNC path of the file share, used only in Hybrid mode.
        /// </summary>
        public string FileSharePath { get; set; } = string.Empty;

        /// <summary>
        /// Max file size (MB) to store. Larger files are skipped or compressed.
        /// </summary>
        public int MaxFileSizeMb { get; set; } = 50;

        /// <summary>
        /// Skip files larger than max instead of compressing.
        /// </summary>
        public bool SkipLargerThanMax { get; set; } = false;

        /// <summary>
        /// GZIP files before storing.
        /// </summary>
        public bool CompressBeforeStoring { get; set; } = false;

        /// <summary>
        /// Subfolder pattern under the file share, e.g. "{year}\{month}".
        /// </summary>
        public string SubfolderPattern { get; set; } = "{year}\\{month}";
    }
}
