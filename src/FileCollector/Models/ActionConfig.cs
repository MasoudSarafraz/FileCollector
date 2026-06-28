using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FileCollector.Models
{
    /// <summary>
    /// Action types supported by the collector.
    /// </summary>
    public enum ActionType
    {
        Copy,
        Move,
        Rename,
        Delete,
        Recycle,
        Zip,
        ZipAndMove,
        Extract,
        CustomCommand,
        TextProcessing,
        ApiUpload
    }

    /// <summary>
    /// Authentication type for ApiUpload action.
    /// </summary>
    public enum ApiAuthType
    {
        None,
        Basic,
        Bearer,
        ApiKeyHeader,
        ApiKeyQuery
    }

    /// <summary>
    /// Configuration of a single action in the chain.
    /// </summary>
    public class ActionConfig
    {
        public ActionType Type { get; set; } = ActionType.Copy;

        /// <summary>
        /// Display name in UI.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Generic parameters dictionary, used differently per action type.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether to continue the chain on this action's failure.
        /// </summary>
        public bool ContinueOnFailure { get; set; } = false;

        /// <summary>
        /// Number of retries on failure (0 = no retry).
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Delay between retries in milliseconds.
        /// </summary>
        public int RetryDelayMs { get; set; } = 1000;

        // ----- Convenience properties for common parameters -----

        [JsonIgnore]
        public string DestinationPath
        {
            get => Parameters.TryGetValue("DestinationPath", out var v) ? v : string.Empty;
            set => Parameters["DestinationPath"] = value;
        }

        [JsonIgnore]
        public string FilenamePattern
        {
            get => Parameters.TryGetValue("FilenamePattern", out var v) ? v : string.Empty;
            set => Parameters["FilenamePattern"] = value;
        }

        [JsonIgnore]
        public string CommandExecutable
        {
            get => Parameters.TryGetValue("CommandExecutable", out var v) ? v : string.Empty;
            set => Parameters["CommandExecutable"] = value;
        }

        [JsonIgnore]
        public string CommandArguments
        {
            get => Parameters.TryGetValue("CommandArguments", out var v) ? v : string.Empty;
            set => Parameters["CommandArguments"] = value;
        }

        [JsonIgnore]
        public string WorkingDirectory
        {
            get => Parameters.TryGetValue("WorkingDirectory", out var v) ? v : string.Empty;
            set => Parameters["WorkingDirectory"] = value;
        }

        [JsonIgnore]
        public bool WaitForExit
        {
            get => Parameters.TryGetValue("WaitForExit", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["WaitForExit"] = value.ToString();
        }

        [JsonIgnore]
        public int TimeoutSeconds
        {
            get => Parameters.TryGetValue("TimeoutSeconds", out var v) && int.TryParse(v, out var i) ? i : 0;
            set => Parameters["TimeoutSeconds"] = value.ToString();
        }

        [JsonIgnore]
        public string ZipPassword
        {
            get => Parameters.TryGetValue("ZipPassword", out var v) ? v : string.Empty;
            set => Parameters["ZipPassword"] = value;
        }

        [JsonIgnore]
        public int CompressionLevel
        {
            get => Parameters.TryGetValue("CompressionLevel", out var v) && int.TryParse(v, out var i) ? i : 6;
            set => Parameters["CompressionLevel"] = value.ToString();
        }

        // ----- ApiUpload parameters -----

        [JsonIgnore]
        public string ApiUrl
        {
            get => Parameters.TryGetValue("ApiUrl", out var v) ? v : string.Empty;
            set => Parameters["ApiUrl"] = value;
        }

        [JsonIgnore]
        public string ApiMethod
        {
            get => Parameters.TryGetValue("ApiMethod", out var v) ? v : "POST";
            set => Parameters["ApiMethod"] = value;
        }

        /// <summary>
        /// "multipart" = send file as multipart/form-data (field name "file")
        /// "base64" = send JSON with file content as base64 string
        /// </summary>
        [JsonIgnore]
        public string ApiUploadMode
        {
            get => Parameters.TryGetValue("ApiUploadMode", out var v) ? v : "multipart";
            set => Parameters["ApiUploadMode"] = value;
        }

        [JsonIgnore]
        public string ApiHeaders
        {
            get => Parameters.TryGetValue("ApiHeaders", out var v) ? v : "{}";
            set => Parameters["ApiHeaders"] = value;
        }

        /// <summary>
        /// JSON template for base64 mode. Use {filename}, {base64}, {size}, {md5}.
        /// If empty, a default template is used.
        /// </summary>
        [JsonIgnore]
        public string ApiJsonTemplate
        {
            get => Parameters.TryGetValue("ApiJsonTemplate", out var v) ? v : string.Empty;
            set => Parameters["ApiJsonTemplate"] = value;
        }

        [JsonIgnore]
        public int ApiTimeoutSeconds
        {
            get => Parameters.TryGetValue("ApiTimeoutSeconds", out var v) && int.TryParse(v, out var i) ? i : 60;
            set => Parameters["ApiTimeoutSeconds"] = value.ToString();
        }

        // ----- ApiUpload authentication -----

        /// <summary>
        /// Auth type: None, Basic, Bearer, ApiKeyHeader, ApiKeyQuery
        /// </summary>
        [JsonIgnore]
        public ApiAuthType AuthType
        {
            get => Parameters.TryGetValue("AuthType", out var v) && System.Enum.TryParse<ApiAuthType>(v, out var t) ? t : ApiAuthType.None;
            set => Parameters["AuthType"] = value.ToString();
        }

        [JsonIgnore]
        public string AuthUsername
        {
            get => Parameters.TryGetValue("AuthUsername", out var v) ? v : string.Empty;
            set => Parameters["AuthUsername"] = value;
        }

        [JsonIgnore]
        public string AuthPassword
        {
            get => Parameters.TryGetValue("AuthPassword", out var v) ? v : string.Empty;
            set => Parameters["AuthPassword"] = value;
        }

        [JsonIgnore]
        public string AuthToken
        {
            get => Parameters.TryGetValue("AuthToken", out var v) ? v : string.Empty;
            set => Parameters["AuthToken"] = value;
        }

        [JsonIgnore]
        public string AuthKeyName
        {
            get => Parameters.TryGetValue("AuthKeyName", out var v) ? v : string.Empty;
            set => Parameters["AuthKeyName"] = value;
        }

        [JsonIgnore]
        public string AuthKeyValue
        {
            get => Parameters.TryGetValue("AuthKeyValue", out var v) ? v : string.Empty;
            set => Parameters["AuthKeyValue"] = value;
        }

        // ----- TextProcessing parameters (used when Type == TextProcessing) -----

        /// <summary>
        /// Comma-separated list of extensions to process (without dot). e.g. "txt,json,xml"
        /// </summary>
        [JsonIgnore]
        public string TextExtensions
        {
            get => Parameters.TryGetValue("TextExtensions", out var v) ? v : "txt,json,xml,csv,ini,log,md,html,css,js,py";
            set => Parameters["TextExtensions"] = value;
        }

        [JsonIgnore]
        public string TextEncoding
        {
            get => Parameters.TryGetValue("TextEncoding", out var v) ? v : "utf-8";
            set => Parameters["TextEncoding"] = value;
        }

        [JsonIgnore]
        public bool TextCreateBackup
        {
            get => Parameters.TryGetValue("TextCreateBackup", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextCreateBackup"] = value.ToString();
        }

        /// <summary>
        /// Find/Replace rules serialized as JSON.
        /// </summary>
        [JsonIgnore]
        public string TextFindReplaceRulesJson
        {
            get => Parameters.TryGetValue("TextFindReplaceRulesJson", out var v) ? v : "[]";
            set => Parameters["TextFindReplaceRulesJson"] = value;
        }

        [JsonIgnore]
        public string TextHeaderTemplate
        {
            get => Parameters.TryGetValue("TextHeaderTemplate", out var v) ? v : string.Empty;
            set => Parameters["TextHeaderTemplate"] = value;
        }

        [JsonIgnore]
        public string TextFooterTemplate
        {
            get => Parameters.TryGetValue("TextFooterTemplate", out var v) ? v : string.Empty;
            set => Parameters["TextFooterTemplate"] = value;
        }

        [JsonIgnore]
        public string TextAppendContent
        {
            get => Parameters.TryGetValue("TextAppendContent", out var v) ? v : string.Empty;
            set => Parameters["TextAppendContent"] = value;
        }

        [JsonIgnore]
        public string TextPrependContent
        {
            get => Parameters.TryGetValue("TextPrependContent", out var v) ? v : string.Empty;
            set => Parameters["TextPrependContent"] = value;
        }

        [JsonIgnore]
        public bool TextEnableFindReplace
        {
            get => Parameters.TryGetValue("TextEnableFindReplace", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextEnableFindReplace"] = value.ToString();
        }

        [JsonIgnore]
        public bool TextEnableHeader
        {
            get => Parameters.TryGetValue("TextEnableHeader", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextEnableHeader"] = value.ToString();
        }

        [JsonIgnore]
        public bool TextEnableFooter
        {
            get => Parameters.TryGetValue("TextEnableFooter", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextEnableFooter"] = value.ToString();
        }

        [JsonIgnore]
        public bool TextEnableAppend
        {
            get => Parameters.TryGetValue("TextEnableAppend", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextEnableAppend"] = value.ToString();
        }

        [JsonIgnore]
        public bool TextEnablePrepend
        {
            get => Parameters.TryGetValue("TextEnablePrepend", out var v) && bool.TryParse(v, out var b) && b;
            set => Parameters["TextEnablePrepend"] = value.ToString();
        }

        /// <summary>
        /// Builds a TextProcessingConfig from this action's parameters.
        /// Used by ActionExecutor.DoTextProcessing.
        /// </summary>
        [JsonIgnore]
        public TextProcessingConfig TextProcessingConfig
        {
            get
            {
                var cfg = new TextProcessingConfig
                {
                    Enabled = true,
                    Extensions = TextExtensions,
                    Encoding = TextEncoding,
                    CreateBackup = TextCreateBackup,
                    EnableFindReplace = TextEnableFindReplace,
                    EnableHeader = TextEnableHeader,
                    EnableFooter = TextEnableFooter,
                    EnableAppend = TextEnableAppend,
                    EnablePrepend = TextEnablePrepend,
                    HeaderTemplate = TextHeaderTemplate,
                    FooterTemplate = TextFooterTemplate,
                    AppendText = TextAppendContent,
                    PrependText = TextPrependContent
                };

                // Deserialize find/replace rules
                try
                {
                    if (!string.IsNullOrEmpty(TextFindReplaceRulesJson))
                    {
                        cfg.FindReplaceRules = JsonConvert.DeserializeObject<List<FindReplaceRule>>(TextFindReplaceRulesJson)
                            ?? new List<FindReplaceRule>();
                    }
                }
                catch { }

                return cfg;
            }
        }
    }
}
