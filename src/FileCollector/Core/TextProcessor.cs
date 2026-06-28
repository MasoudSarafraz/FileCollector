using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Performs text processing operations on text files: Find/Replace, Header/Footer,
    /// Template rendering, Append, Prepend. Only processes files identified as text.
    /// </summary>
    public static class TextProcessor
    {
        public class ProcessResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string OutputPath { get; set; }
            public long OriginalSize { get; set; }
            public long NewSize { get; set; }
        }

        /// <summary>
        /// Determines if a file is likely text by scanning first 8KB for null bytes.
        /// </summary>
        public static bool IsTextFile(string filePath, int scanBytes = 8192)
        {
            try
            {
                var info = new FileInfo(filePath);
                if (info.Length == 0) return true;

                int readSize = (int)Math.Min(scanBytes, info.Length);
                byte[] buffer = new byte[readSize];

                using (var fs = File.OpenRead(filePath))
                {
                    int read = fs.Read(buffer, 0, readSize);
                    if (read == 0) return true;

                    for (int i = 0; i < read; i++)
                    {
                        // Allow tab, newline, CR, and any printable char
                        if (buffer[i] == 0) return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the file extension matches the configured extensions list.
        /// </summary>
        public static bool ExtensionMatches(string filePath, string extensionsCsv)
        {
            if (string.IsNullOrWhiteSpace(extensionsCsv)) return true;
            string ext = Path.GetExtension(filePath).TrimStart('.').ToLowerInvariant();
            var list = (extensionsCsv ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in list)
            {
                if (string.Equals(item.Trim().TrimStart('.'), ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Main entry: applies the text processing config to a file.
        /// </summary>
        public static ProcessResult Process(string filePath, TextProcessingConfig config,
            VariableResolver.Context ctx, string outputPath = null)
        {
            var result = new ProcessResult { Success = false, OutputPath = outputPath ?? filePath };

            if (config == null || !config.Enabled)
            {
                result.Success = true;
                result.OutputPath = filePath;
                return result;
            }

            try
            {
                // Validate file
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File not found: " + filePath;
                    return result;
                }

                var fi = new FileInfo(filePath);
                if (fi.Length > config.MaxSizeBytes)
                {
                    result.ErrorMessage = $"File size {fi.Length} exceeds max {config.MaxSizeBytes}";
                    return result;
                }

                if (!ExtensionMatches(filePath, config.Extensions))
                {
                    result.Success = true;
                    result.OutputPath = filePath;
                    return result;
                }

                if (!IsTextFile(filePath))
                {
                    result.ErrorMessage = "File appears to be binary, not text.";
                    return result;
                }

                Encoding encoding = GetEncoding(config.Encoding);
                bool hasBom = HasBom(filePath);

                // Read content
                string content;
                using (var reader = new StreamReader(filePath, encoding, true))
                {
                    content = reader.ReadToEnd();
                }

                string originalContent = content;
                long originalSize = fi.Length;

                // Create backup if needed
                if (config.CreateBackup && outputPath == null)
                {
                    string backup = filePath + ".bak";
                    File.Copy(filePath, backup, true);
                }

                // Apply Find/Replace
                if (config.EnableFindReplace && config.FindReplaceRules != null)
                {
                    foreach (var rule in config.FindReplaceRules)
                    {
                        if (string.IsNullOrEmpty(rule.Find)) continue;
                        content = ApplyFindReplace(content, rule, ctx);
                    }
                }

                // Apply Header
                if (config.EnableHeader && !string.IsNullOrEmpty(config.HeaderTemplate))
                {
                    string header = VariableResolver.Resolve(config.HeaderTemplate, ctx);
                    content = header + Environment.NewLine + Environment.NewLine + content;
                }

                // Apply Footer
                if (config.EnableFooter && !string.IsNullOrEmpty(config.FooterTemplate))
                {
                    string footer = VariableResolver.Resolve(config.FooterTemplate, ctx);
                    content = content + Environment.NewLine + Environment.NewLine + footer;
                }

                // Apply Prepend
                if (config.EnablePrepend && !string.IsNullOrEmpty(config.PrependText))
                {
                    string prepend = VariableResolver.Resolve(config.PrependText, ctx);
                    content = prepend + content;
                }

                // Apply Append
                if (config.EnableAppend && !string.IsNullOrEmpty(config.AppendText))
                {
                    string append = VariableResolver.Resolve(config.AppendText, ctx);
                    content = content + append;
                }

                // Write output
                string targetPath = outputPath ?? filePath;
                string tempPath = targetPath + ".tmp";

                using (var writer = new StreamWriter(tempPath, false, encoding))
                {
                    writer.Write(content);
                }

                // Restore BOM if original had one and using UTF-8
                if (hasBom && (encoding is UTF8Encoding))
                {
                    RestoreBom(tempPath);
                }

                if (File.Exists(targetPath))
                    File.Delete(targetPath);
                File.Move(tempPath, targetPath);

                result.Success = true;
                result.OutputPath = targetPath;
                result.OriginalSize = originalSize;
                result.NewSize = new FileInfo(targetPath).Length;
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                LogManager.Error("TextProcessor.Process failed", ex);
                return result;
            }
        }

        private static string ApplyFindReplace(string content, FindReplaceRule rule, VariableResolver.Context ctx)
        {
            string find = VariableResolver.Resolve(rule.Find, ctx);
            string replace = VariableResolver.Resolve(rule.Replace, ctx);

            if (rule.UseRegex)
            {
                try
                {
                    RegexOptions opts = RegexOptions.None;
                    if (!rule.CaseSensitive) opts |= RegexOptions.IgnoreCase;
                    opts |= RegexOptions.Multiline;
                    return Regex.Replace(content, find, replace, opts);
                }
                catch (Exception ex)
                {
                    LogManager.Warn($"Invalid regex '{find}': {ex.Message}");
                    return content;
                }
            }
            else
            {
                StringComparison cmp = rule.CaseSensitive
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                return ReplaceString(content, find, replace, cmp);
            }
        }

        private static string ReplaceString(string source, string find, string replace, StringComparison cmp)
        {
            if (string.IsNullOrEmpty(find)) return source;
            var sb = new StringBuilder(source.Length);
            int idx = 0;
            int found;
            while ((found = source.IndexOf(find, idx, cmp)) >= 0)
            {
                sb.Append(source, idx, found - idx);
                sb.Append(replace);
                idx = found + find.Length;
            }
            sb.Append(source, idx, source.Length - idx);
            return sb.ToString();
        }

        private static Encoding GetEncoding(string name)
        {
            if (string.IsNullOrEmpty(name)) return new UTF8Encoding(false);
            switch (name.ToLowerInvariant())
            {
                case "utf-8":
                case "utf8":
                    return new UTF8Encoding(false);
                case "utf-8-bom":
                    return new UTF8Encoding(true);
                case "utf-16":
                case "unicode":
                    return Encoding.Unicode;
                case "utf-16-be":
                    return Encoding.BigEndianUnicode;
                case "ascii":
                    return Encoding.ASCII;
                case "windows-1256":
                case "cp1256":
                    try { return Encoding.GetEncoding("windows-1256"); }
                    catch { return new UTF8Encoding(false); }
                default:
                    try { return Encoding.GetEncoding(name); }
                    catch { return new UTF8Encoding(false); }
            }
        }

        private static bool HasBom(string filePath)
        {
            try
            {
                using (var fs = File.OpenRead(filePath))
                {
                    if (fs.Length >= 3)
                    {
                        byte[] b = new byte[3];
                        fs.Read(b, 0, 3);
                        return b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF;
                    }
                }
            }
            catch { }
            return false;
        }

        private static void RestoreBom(string filePath)
        {
            try
            {
                byte[] bom = { 0xEF, 0xBB, 0xBF };
                byte[] content = File.ReadAllBytes(filePath);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bom, 0, 3);
                    fs.Write(content, 0, content.Length);
                }
            }
            catch (Exception ex)
            {
                LogManager.Warn("RestoreBom failed: " + ex.Message);
            }
        }
    }
}
