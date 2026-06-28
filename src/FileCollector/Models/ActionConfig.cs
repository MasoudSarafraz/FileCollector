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
        DatabaseStore,
        Webhook
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

        // ----- Webhook parameters -----

        [JsonIgnore]
        public string WebhookUrl
        {
            get => Parameters.TryGetValue("WebhookUrl", out var v) ? v : string.Empty;
            set => Parameters["WebhookUrl"] = value;
        }

        [JsonIgnore]
        public string WebhookMethod
        {
            get => Parameters.TryGetValue("WebhookMethod", out var v) ? v : "POST";
            set => Parameters["WebhookMethod"] = value;
        }

        [JsonIgnore]
        public string WebhookHeaders
        {
            get => Parameters.TryGetValue("WebhookHeaders", out var v) ? v : string.Empty;
            set => Parameters["WebhookHeaders"] = value;
        }

        /// <summary>
        /// "notification" = send JSON metadata only,
        /// "upload" = send file as multipart/form-data,
        /// "both" = send notification then upload file
        /// </summary>
        [JsonIgnore]
        public string WebhookMode
        {
            get => Parameters.TryGetValue("WebhookMode", out var v) ? v : "notification";
            set => Parameters["WebhookMode"] = value;
        }

        [JsonIgnore]
        public string WebhookJsonTemplate
        {
            get => Parameters.TryGetValue("WebhookJsonTemplate", out var v) ? v : string.Empty;
            set => Parameters["WebhookJsonTemplate"] = value;
        }

        [JsonIgnore]
        public int WebhookTimeoutSeconds
        {
            get => Parameters.TryGetValue("WebhookTimeoutSeconds", out var v) && int.TryParse(v, out var i) ? i : 30;
            set => Parameters["WebhookTimeoutSeconds"] = value.ToString();
        }
    }
}
