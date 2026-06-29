using System;
using System.IO;
using Newtonsoft.Json;
using FileCollector.Models;

namespace FileCollector.Core
{
    /// <summary>
    /// Loads and saves the application configuration as JSON.
    /// Supports hot-reload via file watcher.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly object _lock = new object();
        private static string _configPath = "config.json";
        private static AppConfig _current;
        private static FileSystemWatcher _watcher;
        private static DateTime _lastWrite = DateTime.MinValue;

        public static event EventHandler ConfigChanged;

        public static string ConfigPath => _configPath;

        static ConfigManager()
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                _configPath = Path.Combine(exeDir, "config.json");
            }
            catch
            {
                _configPath = "config.json";
            }
        }

        public static AppConfig Load()
        {
            lock (_lock)
            {
                if (_current != null) return _current;

                try
                {
                    if (File.Exists(_configPath))
                    {
                        string json = File.ReadAllText(_configPath);
                        _current = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                    }
                    else
                    {
                        _current = new AppConfig();
                        Save(_current);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Config load failed: " + ex.Message);
                    _current = new AppConfig();
                }

                return _current;
            }
        }

        public static void Save(AppConfig config)
        {
            lock (_lock)
            {
                try
                {
                    _current = config;
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Include
                    };
                    string json = JsonConvert.SerializeObject(config, settings);
                    File.WriteAllText(_configPath, json);
                }
                catch (Exception ex)
                {
                    LogManager.Error("Failed to save config", ex);
                }
            }
        }

        public static AppConfig Current => Load();

        public static void StartWatching()
        {
            if (_watcher != null) return;

            try
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(_configPath)) ?? ".";
                string filename = Path.GetFileName(_configPath);

                _watcher = new FileSystemWatcher(dir, filename)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };
                _watcher.Changed += OnConfigFileChanged;
                _watcher.Created += OnConfigFileChanged;
            }
            catch (Exception ex)
            {
                LogManager.Warn("Could not start config watcher", ex);
            }
        }

        public static void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Changed -= OnConfigFileChanged;
                _watcher.Created -= OnConfigFileChanged;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            // Debounce rapid file changes
            try
            {
                var writeTime = File.GetLastWriteTime(e.FullPath);
                if ((writeTime - _lastWrite).TotalMilliseconds < 500) return;
                _lastWrite = writeTime;
            }
            catch { return; }

            lock (_lock)
            {
                _current = null; // Force reload on next access
            }

            ConfigChanged?.Invoke(null, EventArgs.Empty);
            LogManager.Info("Config file changed externally — reloaded.");
        }

        public static string ExportConfig(string targetPath)
        {
            try
            {
                var config = Load();
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(targetPath, json);
                return targetPath;
            }
            catch (Exception ex)
            {
                LogManager.Error("Export config failed", ex);
                throw;
            }
        }

        public static void ImportConfig(string sourcePath)
        {
            try
            {
                string json = File.ReadAllText(sourcePath);
                var config = JsonConvert.DeserializeObject<AppConfig>(json);
                Save(config);
            }
            catch (Exception ex)
            {
                LogManager.Error("Import config failed", ex);
                throw;
            }
        }
    }
}
