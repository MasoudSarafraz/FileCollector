using System.Collections.Generic;
using Newtonsoft.Json;

namespace FileCollector.Models
{
    /// <summary>
    /// Type of text processing operation.
    /// </summary>
    public enum TextOperationType
    {
        FindReplace,
        InsertHeader,
        InsertFooter,
        TemplateRender,
        Append,
        Prepend
    }

    /// <summary>
    /// A single find/replace rule.
    /// </summary>
    public class FindReplaceRule
    {
        public string Find { get; set; } = string.Empty;
        public string Replace { get; set; } = string.Empty;
        public bool UseRegex { get; set; } = false;
        public bool CaseSensitive { get; set; } = false;
    }

    /// <summary>
    /// Configuration for text processing on text files only.
    /// </summary>
    public class TextProcessingConfig
    {
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Comma-separated list of extensions to process (without dot). e.g. "txt,json,xml"
        /// </summary>
        public string Extensions { get; set; } = "txt,json,xml,csv,ini,log,md,html,css,js,py";

        /// <summary>
        /// Encoding name: "utf-8", "utf-16", "ascii", "windows-1256"
        /// </summary>
        public string Encoding { get; set; } = "utf-8";

        /// <summary>
        /// Create a .bak backup before modifying.
        /// </summary>
        public bool CreateBackup { get; set; } = true;

        /// <summary>
        /// Max file size to process in bytes (default 10MB).
        /// </summary>
        public long MaxSizeBytes { get; set; } = 10 * 1024 * 1024;

        public List<FindReplaceRule> FindReplaceRules { get; set; } = new List<FindReplaceRule>();

        public string HeaderTemplate { get; set; } = string.Empty;
        public string FooterTemplate { get; set; } = string.Empty;
        public string AppendText { get; set; } = string.Empty;
        public string PrependText { get; set; } = string.Empty;

        /// <summary>
        /// Apply find/replace rules.
        /// </summary>
        public bool EnableFindReplace { get; set; } = false;

        /// <summary>
        /// Apply header template.
        /// </summary>
        public bool EnableHeader { get; set; } = false;

        /// <summary>
        /// Apply footer template.
        /// </summary>
        public bool EnableFooter { get; set; } = false;

        /// <summary>
        /// Apply append text.
        /// </summary>
        public bool EnableAppend { get; set; } = false;

        /// <summary>
        /// Apply prepend text.
        /// </summary>
        public bool EnablePrepend { get; set; } = false;
    }
}
