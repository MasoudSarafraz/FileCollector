using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Executes a single action on a file. Each action returns the resulting
    /// file path (which may differ from input for Copy/Move/Zip) or null on failure.
    /// </summary>
    public class ActionExecutor
    {
        public class ActionResult
        {
            public bool Success { get; set; }
            public string OutputPath { get; set; }
            public string ErrorMessage { get; set; }
            public long BytesProcessed { get; set; }
        }

        public class ExecutionContext
        {
            public FolderConfig Folder { get; set; }
            public VariableResolver.Context VariableContext { get; set; }
            public IProgress<int> FileProgress { get; set; }
            public CancellationToken CancellationToken { get; set; }
        }

        /// <summary>
        /// Executes a single action.
        /// </summary>
        public ActionResult Execute(ActionConfig action, string inputPath, ExecutionContext ctx)
        {
            if (!action.Enabled)
                return new ActionResult { Success = true, OutputPath = inputPath };

            ActionResult result = null;
            Exception lastError = null;
            int attempts = action.RetryCount + 1;

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (ctx.CancellationToken.IsCancellationRequested)
                        return new ActionResult { Success = false, ErrorMessage = "Cancelled" };

                    result = ExecuteOnce(action, inputPath, ctx);
                    if (result.Success) return result;
                    lastError = new Exception(result.ErrorMessage);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    LogManager.Warn($"Action {action.Type} attempt {i + 1} failed: {ex.Message}");
                }

                if (i < attempts - 1)
                {
                    int delay = action.RetryDelayMs * (int)Math.Pow(2, i); // exponential backoff
                    Thread.Sleep(delay);
                }
            }

            return new ActionResult
            {
                Success = false,
                ErrorMessage = lastError?.Message ?? "Unknown error",
                OutputPath = inputPath
            };
        }

        private ActionResult ExecuteOnce(ActionConfig action, string inputPath, ExecutionContext ctx)
        {
            switch (action.Type)
            {
                case ActionType.Copy:
                    return DoCopy(inputPath, action, ctx);

                case ActionType.Move:
                    return DoMove(inputPath, action, ctx);

                case ActionType.Rename:
                    return DoRename(inputPath, action, ctx);

                case ActionType.Delete:
                    return DoDelete(inputPath);

                case ActionType.Recycle:
                    return DoRecycle(inputPath);

                case ActionType.Zip:
                    return DoZip(inputPath, action, ctx, false);

                case ActionType.ZipAndMove:
                    return DoZip(inputPath, action, ctx, true);

                case ActionType.Extract:
                    return DoExtract(inputPath, action, ctx);

                case ActionType.CustomCommand:
                    return DoCustomCommand(inputPath, action, ctx);

                case ActionType.TextProcessing:
                    return DoTextProcessing(inputPath, ctx);

                case ActionType.DatabaseStore:
                    return DoDatabaseStore(inputPath, ctx);

                default:
                    return new ActionResult
                    {
                        Success = false,
                        ErrorMessage = "Unknown action type: " + action.Type
                    };
            }
        }

        // ===========================================================
        // FILE OPERATIONS
        // ===========================================================

        private ActionResult DoCopy(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            try
            {
                string destPath = ResolveDestinationPath(inputPath, action, ctx);
                EnsureDirectory(destPath);
                destPath = ResolveConflict(destPath, ctx.Folder);

                // ResolveConflict returns null when strategy is "skip"
                if (destPath == null)
                {
                    return new ActionResult { Success = true, OutputPath = inputPath, ErrorMessage = "Skipped (conflict)" };
                }

                long size = new FileInfo(inputPath).Length;
                CopyWithProgress(inputPath, destPath, size, ctx);

                return new ActionResult { Success = true, OutputPath = destPath, BytesProcessed = size };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoMove(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            try
            {
                string destPath = ResolveDestinationPath(inputPath, action, ctx);
                EnsureDirectory(destPath);
                destPath = ResolveConflict(destPath, ctx.Folder);

                if (destPath == null)
                {
                    return new ActionResult { Success = true, OutputPath = inputPath, ErrorMessage = "Skipped (conflict)" };
                }

                long size = new FileInfo(inputPath).Length;
                MoveWithProgress(inputPath, destPath, size, ctx);

                return new ActionResult { Success = true, OutputPath = destPath, BytesProcessed = size };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoRename(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            try
            {
                string dir = Path.GetDirectoryName(inputPath);
                string newFilename = VariableResolver.Resolve(action.FilenamePattern, ctx.VariableContext);
                if (string.IsNullOrEmpty(newFilename)) newFilename = Path.GetFileName(inputPath);
                string destPath = Path.Combine(dir, newFilename);
                destPath = ResolveConflict(destPath, ctx.Folder);

                if (destPath == null)
                {
                    return new ActionResult { Success = true, OutputPath = inputPath, ErrorMessage = "Skipped (conflict)" };
                }

                File.Move(inputPath, destPath);
                return new ActionResult { Success = true, OutputPath = destPath };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoDelete(string inputPath)
        {
            try
            {
                File.Delete(inputPath);
                return new ActionResult { Success = true, OutputPath = null };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoRecycle(string inputPath)
        {
            try
            {
                // Use Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile for recycle bin
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                    inputPath,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                return new ActionResult { Success = true, OutputPath = null };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoZip(string inputPath, ActionConfig action, ExecutionContext ctx, bool moveAfter)
        {
            string zipPath = null;
            try
            {
                string dir = Path.GetDirectoryName(inputPath);

                if (!string.IsNullOrEmpty(action.DestinationPath))
                {
                    string destDir = VariableResolver.Resolve(action.DestinationPath, ctx.VariableContext);
                    EnsureDirectoryExists(destDir);
                    zipPath = Path.Combine(destDir, Path.GetFileNameWithoutExtension(inputPath) + ".zip");
                }
                else
                {
                    zipPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(inputPath) + ".zip");
                }

                zipPath = ResolveConflict(zipPath, ctx.Folder);
                if (zipPath == null)
                {
                    return new ActionResult { Success = true, OutputPath = inputPath, ErrorMessage = "Skipped (conflict)" };
                }

                int level = action.CompressionLevel;
                if (level < 0) level = 0;
                if (level > 9) level = 9;

                // Convert 0-9 numeric level to System.IO.Compression.CompressionLevel enum.
                // The .NET ZipArchive API only supports NoCompression / Fastest / Optimal.
                CompressionLevel compLevel = level == 0
                    ? CompressionLevel.NoCompression
                    : (level >= 6 ? CompressionLevel.Optimal : CompressionLevel.Fastest);

                ctx.FileProgress?.Report(10);

                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(inputPath, Path.GetFileName(inputPath), compLevel);
                }

                ctx.FileProgress?.Report(100);

                if (moveAfter)
                {
                    File.Delete(inputPath);
                }

                return new ActionResult { Success = true, OutputPath = zipPath };
            }
            catch (Exception ex)
            {
                // Clean up partial zip file on failure to avoid leaving corrupt archives behind
                if (!string.IsNullOrEmpty(zipPath) && File.Exists(zipPath))
                {
                    try { File.Delete(zipPath); } catch { }
                }
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoExtract(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            try
            {
                string destDir = !string.IsNullOrEmpty(action.DestinationPath)
                    ? VariableResolver.Resolve(action.DestinationPath, ctx.VariableContext)
                    : Path.GetDirectoryName(inputPath);

                EnsureDirectoryExists(destDir);

                ZipFile.ExtractToDirectory(inputPath, destDir);
                return new ActionResult { Success = true, OutputPath = destDir };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoCustomCommand(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            try
            {
                string exe = VariableResolver.Resolve(action.CommandExecutable, ctx.VariableContext);
                string args = VariableResolver.Resolve(action.CommandArguments, ctx.VariableContext);
                string workDir = VariableResolver.Resolve(action.WorkingDirectory, ctx.VariableContext);
                if (string.IsNullOrEmpty(workDir)) workDir = Path.GetDirectoryName(inputPath);

                var psi = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    WorkingDirectory = workDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var proc = Process.Start(psi))
                {
                    if (action.WaitForExit)
                    {
                        int timeoutMs = action.TimeoutSeconds > 0
                            ? action.TimeoutSeconds * 1000
                            : int.MaxValue;

                        if (!proc.WaitForExit(timeoutMs))
                        {
                            try { proc.Kill(); } catch { }
                            return new ActionResult { Success = false, ErrorMessage = "Process timeout", OutputPath = inputPath };
                        }

                        string output = proc.StandardOutput.ReadToEnd();
                        string err = proc.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(output))
                            LogManager.Debug($"Command output: {output}");
                        if (!string.IsNullOrEmpty(err))
                            LogManager.Warn($"Command stderr: {err}");

                        if (proc.ExitCode != 0)
                        {
                            return new ActionResult
                            {
                                Success = false,
                                ErrorMessage = $"Process exited with code {proc.ExitCode}",
                                OutputPath = inputPath
                            };
                        }
                    }
                }

                return new ActionResult { Success = true, OutputPath = inputPath };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoTextProcessing(string inputPath, ExecutionContext ctx)
        {
            try
            {
                var result = TextProcessor.Process(inputPath, ctx.Folder.TextProcessing, ctx.VariableContext);
                return new ActionResult
                {
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage,
                    OutputPath = result.OutputPath
                };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        private ActionResult DoDatabaseStore(string inputPath, ExecutionContext ctx)
        {
            try
            {
                var dbConfig = ctx.Folder.DatabaseStorage;
                if (dbConfig == null || !dbConfig.Enabled)
                {
                    return new ActionResult { Success = true, OutputPath = inputPath };
                }

                long size = new FileInfo(inputPath).Length;
                string md5 = VariableResolver.ComputeMd5(inputPath);

                string subfolder = string.IsNullOrEmpty(dbConfig.SubfolderPattern)
                    ? ""
                    : VariableResolver.Resolve(dbConfig.SubfolderPattern, ctx.VariableContext);

                bool ok = DatabaseManager.StoreFile(
                    dbConfig, inputPath, Path.GetFileName(inputPath), size, md5,
                    ctx.Folder.SourcePath, subfolder,
                    out string storagePath, out string error);

                return new ActionResult
                {
                    Success = ok,
                    ErrorMessage = error,
                    OutputPath = ok ? (storagePath ?? inputPath) : inputPath
                };
            }
            catch (Exception ex)
            {
                return new ActionResult { Success = false, ErrorMessage = ex.Message, OutputPath = inputPath };
            }
        }

        // ===========================================================
        // HELPERS
        // ===========================================================

        private string ResolveDestinationPath(string inputPath, ActionConfig action, ExecutionContext ctx)
        {
            // Cache MD5 once so we don't recompute the hash for each sub-pattern.
            string md5 = VariableResolver.ComputeMd5(inputPath);

            // Determine base destination path:
            // 1. Action-level DestinationPath (highest priority)
            // 2. Folder-level DestinationPath
            // 3. Same directory as source file (for Rename)
            string basePath = !string.IsNullOrEmpty(action.DestinationPath)
                ? VariableResolver.Resolve(action.DestinationPath, ctx.VariableContext, md5)
                : (!string.IsNullOrEmpty(ctx.Folder.DestinationPath)
                    ? ctx.Folder.DestinationPath
                    : Path.GetDirectoryName(inputPath));

            // Determine subfolder:
            // - If user provided an explicit DestinationSubfolderPattern, use it (resolved).
            // - Otherwise, when IncludeSubfolders is true, PRESERVE the source subfolder
            //   structure at destination. E.g. 'G:\Publish\runtimes\linux-arm64\native\foo.so'
            //   with destination 'E:\Backup' becomes 'E:\Backup\runtimes\linux-arm64\native\foo.so'.
            string subfolder;
            if (!string.IsNullOrEmpty(ctx.Folder.DestinationSubfolderPattern))
            {
                subfolder = VariableResolver.Resolve(ctx.Folder.DestinationSubfolderPattern, ctx.VariableContext, md5);
            }
            else if (ctx.Folder.IncludeSubfolders)
            {
                // Compute the relative subfolder of the file inside the source root.
                string sourceRoot = ctx.Folder.SourcePath.TrimEnd('\\', '/');
                string fileDir = Path.GetDirectoryName(inputPath)?.TrimEnd('\\', '/') ?? "";

                if (!string.IsNullOrEmpty(fileDir)
                    && fileDir.StartsWith(sourceRoot, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(fileDir, sourceRoot, StringComparison.OrdinalIgnoreCase))
                {
                    // Relative path from source root to file's directory, WITHOUT leading separator.
                    subfolder = fileDir.Substring(sourceRoot.Length).TrimStart('\\', '/');
                }
                else
                {
                    subfolder = "";
                }
            }
            else
            {
                subfolder = "";
            }

            string filename = !string.IsNullOrEmpty(action.FilenamePattern)
                ? VariableResolver.Resolve(action.FilenamePattern, ctx.VariableContext, md5)
                : Path.GetFileName(inputPath);

            if (string.IsNullOrEmpty(filename)) filename = Path.GetFileName(inputPath);

            string full = string.IsNullOrEmpty(subfolder)
                ? Path.Combine(basePath, filename)
                : Path.Combine(basePath, subfolder, filename);

            return full;
        }

        private void EnsureDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            EnsureDirectoryExists(dir);
        }

        private void EnsureDirectoryExists(string dir)
        {
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private string ResolveConflict(string destPath, FolderConfig folder)
        {
            if (!File.Exists(destPath)) return destPath;

            switch ((folder.ConflictStrategy ?? "rename").ToLowerInvariant())
            {
                case "overwrite":
                    File.Delete(destPath);
                    return destPath;

                case "skip":
                    return null;

                case "keepboth":
                    string dir = Path.GetDirectoryName(destPath);
                    string name = Path.GetFileNameWithoutExtension(destPath);
                    string ext = Path.GetExtension(destPath);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    return Path.Combine(dir, $"{name}_{timestamp}{ext}");

                case "rename":
                default:
                    dir = Path.GetDirectoryName(destPath);
                    name = Path.GetFileNameWithoutExtension(destPath);
                    ext = Path.GetExtension(destPath);
                    int i = 1;
                    while (true)
                    {
                        string candidate = Path.Combine(dir, $"{name}_{i}{ext}");
                        if (!File.Exists(candidate)) return candidate;
                        i++;
                    }
            }
        }

        private void CopyWithProgress(string source, string dest, long totalSize, ExecutionContext ctx)
        {
            const int bufferSize = 81920;
            byte[] buffer = new byte[bufferSize];
            long totalCopied = 0;

            using (var src = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true))
            using (var dst = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                while (true)
                {
                    if (ctx.CancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException(ctx.CancellationToken);

                    int read = src.Read(buffer, 0, bufferSize);
                    if (read == 0) break;
                    dst.Write(buffer, 0, read);
                    totalCopied += read;

                    if (totalSize > 0)
                    {
                        int percent = (int)(totalCopied * 100 / totalSize);
                        ctx.FileProgress?.Report(percent);
                    }
                }
            }
        }

        private void MoveWithProgress(string source, string dest, long totalSize, ExecutionContext ctx)
        {
            // Try fast move first (same volume), fallback to copy+delete
            try
            {
                File.Move(source, dest);
                ctx.FileProgress?.Report(100);
            }
            catch
            {
                CopyWithProgress(source, dest, totalSize, ctx);
                File.Delete(source);
            }
        }
    }
}
