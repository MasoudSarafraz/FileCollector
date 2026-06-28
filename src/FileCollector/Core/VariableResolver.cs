using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCollector.Core
{
    /// <summary>
    /// Resolves {variable} placeholders in patterns using file metadata and context.
    /// </summary>
    public class VariableResolver
    {
        public class Context
        {
            public string FilePath { get; set; } = string.Empty;
            public string SourceFolder { get; set; } = string.Empty;
            public DateTime Now { get; set; } = DateTime.Now;
            public int Counter { get; set; } = 0;
            public string Username { get; set; } = Environment.UserName;
            public string MachineName { get; set; } = Environment.MachineName;
        }

        /// <summary>
        /// Resolves {var} placeholders in a pattern string.
        /// Supports: date, time, datetime, year, month, day, hour, minute, second,
        /// filename, filename_noext, ext, size, size_kb, size_mb, md5,
        /// source_folder, source_folder_name, username, machine_name, guid, counter.
        /// </summary>
        public static string Resolve(string pattern, Context ctx)
        {
            if (string.IsNullOrEmpty(pattern)) return string.Empty;

            FileInfo fi = null;
            try
            {
                if (File.Exists(ctx.FilePath))
                    fi = new FileInfo(ctx.FilePath);
            }
            catch { }

            string filename = fi?.Name ?? Path.GetFileName(ctx.FilePath);
            string filenameNoExt = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            long size = fi?.Length ?? 0;
            string md5 = ComputeMd5(ctx.FilePath);

            string sourceFolderName = string.Empty;
            try
            {
                sourceFolderName = new DirectoryInfo(ctx.SourceFolder).Name;
            }
            catch { }

            var replacements = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"date", ctx.Now.ToString("yyyy-MM-dd")},
                {"time", ctx.Now.ToString("HH:mm:ss")},
                {"datetime", ctx.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                {"year", ctx.Now.ToString("yyyy")},
                {"month", ctx.Now.ToString("MM")},
                {"day", ctx.Now.ToString("dd")},
                {"hour", ctx.Now.ToString("HH")},
                {"minute", ctx.Now.ToString("mm")},
                {"second", ctx.Now.ToString("ss")},
                {"filename", filename},
                {"filename_noext", filenameNoExt},
                {"ext", ext},
                {"size", size.ToString()},
                {"size_kb", (size / 1024.0).ToString("0.00")},
                {"size_mb", (size / (1024.0 * 1024.0)).ToString("0.00")},
                {"md5", md5},
                {"source_folder", ctx.SourceFolder},
                {"source_folder_name", sourceFolderName},
                {"username", ctx.Username},
                {"machine_name", ctx.MachineName},
                {"guid", Guid.NewGuid().ToString("N")},
                {"counter", ctx.Counter.ToString("D6")}
            };

            string result = pattern;
            // Match {varname} or {varname:format}
            var regex = new Regex(@"\{(\w+)(?::([^}]+))?\}", RegexOptions.Compiled);
            result = regex.Replace(result, m =>
            {
                string key = m.Groups[1].Value;
                string fmt = m.Groups[2].Success ? m.Groups[2].Value : null;

                if (string.Equals(key, "counter", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(fmt))
                {
                    if (int.TryParse(fmt, out int digits))
                        return ctx.Counter.ToString("D" + digits);
                    return ctx.Counter.ToString("D6");
                }

                if (replacements.TryGetValue(key, out string val))
                    return val;
                return m.Value; // keep original if unknown
            });

            return result;
        }

        /// <summary>
        /// Compute MD5 hash of a file. Returns empty string if file is unreadable.
        /// </summary>
        public static string ComputeMd5(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    var sb = new StringBuilder(hash.Length * 2);
                    foreach (byte b in hash)
                        sb.Append(b.ToString("x2"));
                    return sb.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Quickly validates a pattern: returns true if no broken placeholders remain.
        /// </summary>
        public static bool IsValidPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            var regex = new Regex(@"\{(\w+)(?::([^}]+))?\}", RegexOptions.Compiled);
            foreach (Match m in regex.Matches(pattern))
            {
                string key = m.Groups[1].Value;
                if (string.Equals(key, "counter", StringComparison.OrdinalIgnoreCase)) continue;
                switch (key.ToLowerInvariant())
                {
                    case "date":
                    case "time":
                    case "datetime":
                    case "year":
                    case "month":
                    case "day":
                    case "hour":
                    case "minute":
                    case "second":
                    case "filename":
                    case "filename_noext":
                    case "ext":
                    case "size":
                    case "size_kb":
                    case "size_mb":
                    case "md5":
                    case "source_folder":
                    case "source_folder_name":
                    case "username":
                    case "machine_name":
                    case "guid":
                        continue;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a list of all known variable names.
        /// </summary>
        public static string[] KnownVariables => new[]
        {
            "date", "time", "datetime", "year", "month", "day", "hour", "minute", "second",
            "filename", "filename_noext", "ext", "size", "size_kb", "size_mb", "md5",
            "source_folder", "source_folder_name", "username", "machine_name", "guid", "counter"
        };
    }
}
