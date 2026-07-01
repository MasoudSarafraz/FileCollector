using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Text;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Manages both local SQLite (history/log) and remote SQL Server (file storage).
    /// </summary>
    public static class DatabaseManager
    {
        // ===========================================================
        // LOCAL SQLITE (for log/history/dedup)
        // ===========================================================

        private static string _localConnectionString;

        public static void InitializeLocalDatabase()
        {
            try
            {
                var config = ConfigManager.Load();
                string dbPath = Path.IsPathRooted(config.LocalDatabasePath)
                    ? config.LocalDatabasePath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.LocalDatabasePath);

                string dir = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _localConnectionString = $"Data Source={dbPath};Version=3;Pooling=True;Max Pool Size=10;";

                CreateLocalSchema();
                LogManager.Info("Local SQLite database initialized at: " + dbPath);
            }
            catch (Exception ex)
            {
                LogManager.Error("Failed to initialize local database", ex);
                throw;
            }
        }

        private static SQLiteConnection OpenLocal()
        {
            if (string.IsNullOrEmpty(_localConnectionString))
                InitializeLocalDatabase();
            var conn = new SQLiteConnection(_localConnectionString);
            conn.Open();
            return conn;
        }

        private static void CreateLocalSchema()
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS file_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    folder_id INTEGER NOT NULL,
    folder_name TEXT,
    filename TEXT NOT NULL,
    source_path TEXT NOT NULL,
    destination_path TEXT,
    size_bytes INTEGER,
    md5_hash TEXT,
    action_type TEXT,
    status TEXT,
    error_message TEXT,
    processed_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_history_md5 ON file_history(md5_hash);
CREATE INDEX IF NOT EXISTS idx_history_processed_at ON file_history(processed_at);
CREATE INDEX IF NOT EXISTS idx_history_folder ON file_history(folder_id);

CREATE TABLE IF NOT EXISTS dedup (
    md5_hash TEXT PRIMARY KEY,
    filename TEXT NOT NULL,
    source_path TEXT NOT NULL,
    size_bytes INTEGER,
    folder_id INTEGER,
    first_seen_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS app_log (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    level TEXT,
    message TEXT,
    exception TEXT,
    log_time DATETIME DEFAULT CURRENT_TIMESTAMP
);
";
            using (var conn = OpenLocal())
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static long LogFileHistory(int folderId, string folderName, string filename,
            string sourcePath, string destinationPath, long size, string md5,
            string actionType, string status, string errorMessage)
        {
            try
            {
                const string sql = @"INSERT INTO file_history
(folder_id, folder_name, filename, source_path, destination_path, size_bytes, md5_hash, action_type, status, error_message, processed_at)
VALUES (@fid, @fname, @filename, @src, @dst, @size, @md5, @action, @status, @err, @now);
SELECT last_insert_rowid();";

                using (var conn = OpenLocal())
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fid", folderId);
                    cmd.Parameters.AddWithValue("@fname", (object)folderName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@filename", filename ?? "");
                    cmd.Parameters.AddWithValue("@src", sourcePath ?? "");
                    cmd.Parameters.AddWithValue("@dst", (object)destinationPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@size", size);
                    cmd.Parameters.AddWithValue("@md5", (object)md5 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@action", actionType ?? "");
                    cmd.Parameters.AddWithValue("@status", status ?? "");
                    cmd.Parameters.AddWithValue("@err", (object)errorMessage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Convert.ToInt64(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("LogFileHistory failed", ex);
                return -1;
            }
        }

        public static bool IsMd5Seen(string md5)
        {
            if (string.IsNullOrEmpty(md5)) return false;
            try
            {
                using (var conn = OpenLocal())
                using (var cmd = new SQLiteCommand("SELECT COUNT(1) FROM dedup WHERE md5_hash = @md5", conn))
                {
                    cmd.Parameters.AddWithValue("@md5", md5);
                    return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                LogManager.Warn("IsMd5Seen failed", ex);
                return false;
            }
        }

        public static void RecordDedup(string md5, string filename, string sourcePath, long size, int folderId)
        {
            if (string.IsNullOrEmpty(md5)) return;
            try
            {
                using (var conn = OpenLocal())
                using (var cmd = new SQLiteCommand(
                    "INSERT OR IGNORE INTO dedup (md5_hash, filename, source_path, size_bytes, folder_id, first_seen_at) VALUES (@md5, @fn, @src, @size, @fid, @now)", conn))
                {
                    cmd.Parameters.AddWithValue("@md5", md5);
                    cmd.Parameters.AddWithValue("@fn", filename ?? "");
                    cmd.Parameters.AddWithValue("@src", sourcePath ?? "");
                    cmd.Parameters.AddWithValue("@size", size);
                    cmd.Parameters.AddWithValue("@fid", folderId);
                    cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Warn("RecordDedup failed", ex);
            }
        }

        public static DataTable GetHistory(int limit = 500)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = OpenLocal())
                using (var cmd = new SQLiteCommand(
                    "SELECT id, processed_at, folder_name, filename, action_type, status, size_bytes, error_message FROM file_history ORDER BY id DESC LIMIT @lim", conn))
                {
                    cmd.Parameters.AddWithValue("@lim", limit);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("GetHistory failed", ex);
            }
            return dt;
        }

        public static void ClearHistory()
        {
            try
            {
                using (var conn = OpenLocal())
                using (var cmd = new SQLiteCommand("DELETE FROM file_history; VACUUM;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("ClearHistory failed", ex);
            }
        }

        // ===========================================================
        // REMOTE SQL SERVER (for file storage)
        // ===========================================================

        /// <summary>
        /// Tests connection to remote SQL Server.
        /// </summary>
        public static bool TestRemoteConnection(string connectionString, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Ensures the target table exists in the remote database.
        /// </summary>
        public static bool EnsureRemoteTable(DatabaseConfig config, out string error)
        {
            error = null;
            try
            {
                string table = ParseTableName(config.TableName, out string schema);

                using (var conn = new SqlConnection(config.ConnectionString))
                {
                    conn.Open();

                    // Ensure schema exists
                    if (!string.IsNullOrEmpty(schema) && !string.Equals(schema, "dbo", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var cmd = new SqlCommand($"IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @s) EXEC('CREATE SCHEMA [' + @s + ']')", conn))
                        {
                            cmd.Parameters.AddWithValue("@s", schema);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string sql;
                    switch (config.Mode)
                    {
                        case DatabaseStorageMode.BlobDirect:
                            sql = $@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @s AND TABLE_NAME = @t)
BEGIN
    CREATE TABLE [{schema}].[{table}] (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        filename NVARCHAR(500) NOT NULL,
        mime_type NVARCHAR(100),
        size_bytes BIGINT NOT NULL,
        md5_hash CHAR(32),
        source_folder NVARCHAR(500),
        file_content VARBINARY(MAX),
        collected_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT UQ_{table}_md5 UNIQUE (md5_hash)
    );
END";
                            break;

                        case DatabaseStorageMode.Hybrid:
                            sql = $@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @s AND TABLE_NAME = @t)
BEGIN
    CREATE TABLE [{schema}].[{table}] (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        filename NVARCHAR(500) NOT NULL,
        mime_type NVARCHAR(100),
        size_bytes BIGINT NOT NULL,
        md5_hash CHAR(32),
        source_folder NVARCHAR(500),
        storage_path NVARCHAR(1000),
        collected_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT UQ_{table}_md5 UNIQUE (md5_hash)
    );
END";
                            break;

                        case DatabaseStorageMode.FileStream:
                            sql = $@"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @s AND TABLE_NAME = @t)
BEGIN
    CREATE TABLE [{schema}].[{table}] (
        id UNIQUEIDENTIFIER ROWGUIDCOL PRIMARY KEY DEFAULT NEWID(),
        filename NVARCHAR(500) NOT NULL,
        mime_type NVARCHAR(100),
        size_bytes BIGINT NOT NULL,
        md5_hash CHAR(32),
        source_folder NVARCHAR(500),
        file_content VARBINARY(MAX) FILESTREAM NULL,
        collected_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT UQ_{table}_md5 UNIQUE (md5_hash)
    );
END";
                            break;

                        default:
                            error = "Unknown storage mode";
                            return false;
                    }

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@s", schema);
                        cmd.Parameters.AddWithValue("@t", table);
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                LogManager.Error("EnsureRemoteTable failed", ex);
                return false;
            }
        }

        /// <summary>
        /// Parses a full table name like "dbo.collected_files" or "[my schema].[my table]"
        /// into schema and table parts. Also validates that both parts contain only
        /// safe identifier characters (letters, digits, underscore) to prevent
        /// SQL injection through table/schema names.
        /// </summary>
        private static string ParseTableName(string full, out string schema)
        {
            schema = "dbo";
            if (string.IsNullOrWhiteSpace(full))
            {
                return "collected_files";
            }

            // Strip outer brackets
            string s = full.Trim().TrimStart('[').TrimEnd(']');
            int idx = s.IndexOf('.');
            string table;
            if (idx > 0)
            {
                schema = s.Substring(0, idx).Trim('[', ']');
                table = s.Substring(idx + 1).Trim('[', ']');
            }
            else
            {
                table = s;
            }

            // Validate: only allow letters, digits, and underscores.
            // This prevents SQL injection through table/schema names.
            // If invalid, fall back to safe defaults.
            if (!IsValidIdentifier(schema))
            {
                LogManager.Warn($"Invalid schema name '{schema}', using 'dbo' instead.");
                schema = "dbo";
            }
            if (!IsValidIdentifier(table))
            {
                LogManager.Warn($"Invalid table name '{table}', using 'collected_files' instead.");
                table = "collected_files";
            }

            return table;
        }

        /// <summary>
        /// Returns true if the identifier contains only safe characters:
        /// letters, digits, and underscores. Must start with a letter or underscore.
        /// </summary>
        private static bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (!char.IsLetter(name[0]) && name[0] != '_') return false;
            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
            }
            return true;
        }

        /// <summary>
        /// Stores a file in the remote database using the configured mode.
        /// Returns the storage path used (for Hybrid mode) or empty string.
        /// </summary>
        public static bool StoreFile(DatabaseConfig config, string filePath, string filename,
            long size, string md5, string sourceFolder, string subfolderResolved,
            out string storagePath, out string errorMessage)
        {
            storagePath = string.Empty;
            errorMessage = null;

            try
            {
                string table = ParseTableName(config.TableName, out string schema);
                string fullTable = $"[{schema}].[{table}]";

                // Check if file too large
                long maxSizeBytes = (long)config.MaxFileSizeMb * 1024 * 1024;
                if (size > maxSizeBytes)
                {
                    if (config.SkipLargerThanMax)
                    {
                        errorMessage = $"File size {size} bytes exceeds max {maxSizeBytes} bytes — skipped.";
                        return false;
                    }
                }

                byte[] fileContent = null;
                byte[] storedContent = null;

                if (config.Mode == DatabaseStorageMode.BlobDirect || config.Mode == DatabaseStorageMode.FileStream)
                {
                    fileContent = File.ReadAllBytes(filePath);
                    storedContent = fileContent;

                    if (config.CompressBeforeStoring)
                    {
                        storedContent = GzipCompress(fileContent);
                    }
                }

                using (var conn = new SqlConnection(config.ConnectionString))
                {
                    conn.Open();

                    // Check dedup
                    using (var checkCmd = new SqlCommand($"SELECT COUNT(1) FROM {fullTable} WHERE md5_hash = @md5", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@md5", (object)md5 ?? DBNull.Value);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            errorMessage = "File with same MD5 already exists in remote database — skipped.";
                            return false;
                        }
                    }

                    if (config.Mode == DatabaseStorageMode.BlobDirect)
                    {
                        string sql = $@"INSERT INTO {fullTable}
(filename, mime_type, size_bytes, md5_hash, source_folder, file_content, collected_at)
VALUES (@fn, @mt, @sz, @md5, @sf, @fc, GETDATE())";
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@fn", filename);
                            cmd.Parameters.AddWithValue("@mt", (object)GuessMimeType(filename) ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sz", size);
                            cmd.Parameters.AddWithValue("@md5", (object)md5 ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sf", (object)sourceFolder ?? DBNull.Value);
                            cmd.Parameters.Add("@fc", SqlDbType.VarBinary, -1).Value = storedContent;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (config.Mode == DatabaseStorageMode.FileStream)
                    {
                        string sql = $@"INSERT INTO {fullTable}
(filename, mime_type, size_bytes, md5_hash, source_folder, file_content, collected_at)
VALUES (@fn, @mt, @sz, @md5, @sf, @fc, GETDATE())";
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@fn", filename);
                            cmd.Parameters.AddWithValue("@mt", (object)GuessMimeType(filename) ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sz", size);
                            cmd.Parameters.AddWithValue("@md5", (object)md5 ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sf", (object)sourceFolder ?? DBNull.Value);
                            cmd.Parameters.Add("@fc", SqlDbType.VarBinary, -1).Value = storedContent;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else // Hybrid
                    {
                        string sharePath = config.FileSharePath;
                        if (string.IsNullOrEmpty(sharePath))
                        {
                            errorMessage = "Hybrid mode requires FileSharePath.";
                            return false;
                        }

                        string fullPath = Path.Combine(sharePath, subfolderResolved ?? "");
                        if (!Directory.Exists(fullPath))
                            Directory.CreateDirectory(fullPath);

                        string destFile = Path.Combine(fullPath, filename);
                        if (File.Exists(destFile))
                            destFile = Path.Combine(fullPath,
                                Path.GetFileNameWithoutExtension(filename) + "_" +
                                DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + Path.GetExtension(filename));

                        // If compression is enabled, GZIP the file before copying.
                        // Otherwise just copy as-is.
                        if (config.CompressBeforeStoring)
                        {
                            string gzFile = destFile + ".gz";
                            using (var src = File.OpenRead(filePath))
                            using (var dst = new FileStream(gzFile, FileMode.Create))
                            using (var gzip = new GZipStream(dst, CompressionLevel.Optimal))
                            {
                                src.CopyTo(gzip);
                            }
                            // Rename .gz to destFile (so the DB record points to a single path)
                            File.Move(gzFile, destFile);
                        }
                        else
                        {
                            File.Copy(filePath, destFile, true);
                        }
                        storagePath = destFile;

                        string sql = $@"INSERT INTO {fullTable}
(filename, mime_type, size_bytes, md5_hash, source_folder, storage_path, collected_at)
VALUES (@fn, @mt, @sz, @md5, @sf, @sp, GETDATE())";
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@fn", filename);
                            cmd.Parameters.AddWithValue("@mt", (object)GuessMimeType(filename) ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sz", size);
                            cmd.Parameters.AddWithValue("@md5", (object)md5 ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sf", (object)sourceFolder ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@sp", destFile);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                LogManager.Error("StoreFile failed", ex);
                return false;
            }
        }

        /// <summary>
        /// Retrieve a file from the remote database by MD5 hash.
        /// </summary>
        public static bool RetrieveFile(DatabaseConfig config, string md5, string outputPath, out string error)
        {
            error = null;
            try
            {
                string table = ParseTableName(config.TableName, out string schema);
                string fullTable = $"[{schema}].[{table}]";

                using (var conn = new SqlConnection(config.ConnectionString))
                {
                    conn.Open();

                    if (config.Mode == DatabaseStorageMode.Hybrid)
                    {
                        using (var cmd = new SqlCommand($"SELECT storage_path FROM {fullTable} WHERE md5_hash = @md5", conn))
                        {
                            cmd.Parameters.AddWithValue("@md5", md5);
                            object path = cmd.ExecuteScalar();
                            if (path == null || path == DBNull.Value)
                            {
                                error = "File not found.";
                                return false;
                            }
                            File.Copy((string)path, outputPath, true);
                            return true;
                        }
                    }
                    else
                    {
                        using (var cmd = new SqlCommand($"SELECT file_content FROM {fullTable} WHERE md5_hash = @md5", conn))
                        {
                            cmd.Parameters.AddWithValue("@md5", md5);
                            using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                            {
                                if (!reader.Read())
                                {
                                    error = "File not found.";
                                    return false;
                                }

                                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                                {
                                    byte[] buffer = new byte[4096];
                                    long offset = 0;
                                    while (true)
                                    {
                                        int read = (int)reader.GetBytes(0, offset, buffer, 0, buffer.Length);
                                        if (read == 0) break;
                                        fs.Write(buffer, 0, read);
                                        offset += read;
                                    }
                                }
                            }
                        }

                        if (config.CompressBeforeStoring)
                        {
                            // Decompress
                            string temp = outputPath + ".gz";
                            File.Move(outputPath, temp);
                            using (var src = new FileStream(temp, FileMode.Open))
                            using (var gzip = new GZipStream(src, CompressionMode.Decompress))
                            using (var dst = new FileStream(outputPath, FileMode.Create))
                            {
                                gzip.CopyTo(dst);
                            }
                            File.Delete(temp);
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                LogManager.Error("RetrieveFile failed", ex);
                return false;
            }
        }

        // ===========================================================
        // HELPERS
        // ===========================================================

        private static byte[] GzipCompress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        private static string GuessMimeType(string filename)
        {
            string ext = Path.GetExtension(filename).ToLowerInvariant();
            switch (ext)
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".bmp": return "image/bmp";
                case ".txt": return "text/plain";
                case ".json": return "application/json";
                case ".xml": return "application/xml";
                case ".csv": return "text/csv";
                case ".html":
                case ".htm": return "text/html";
                case ".zip": return "application/zip";
                case ".rar": return "application/x-rar-compressed";
                case ".7z": return "application/x-7z-compressed";
                case ".mp3": return "audio/mpeg";
                case ".mp4": return "video/mp4";
                case ".avi": return "video/x-msvideo";
                case ".exe": return "application/x-msdownload";
                default: return "application/octet-stream";
            }
        }
    }
}
