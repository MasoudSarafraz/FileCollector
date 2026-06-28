using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FileCollector.Core;
using FileCollector.Models;

namespace FileCollector.Forms
{
    public partial class MainForm : Form
    {
        private CollectorEngine _engine;
        private AppConfig _config;
        private System.Windows.Forms.Timer _uiTimer;
        private readonly List<FolderProgressInfo> _folderProgress = new List<FolderProgressInfo>();
        private FileProgressInfo _currentFileProgress;
        private OverallProgressInfo _overallProgress;

        public MainForm()
        {
            InitializeComponent();
            SetupLocalization();
            LoadConfig();
            SetupEngine();
            SetupEventHandlers();
            LayoutToolbar();
            SetupTimer();
            RefreshFolderList();
        }

        private void SetupLocalization()
        {
            this.Text = "File Collector — جمع‌آوری‌کننده فایل";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
        }

        private void LoadConfig()
        {
            _config = ConfigManager.Load();
            ConfigManager.ConfigChanged += OnConfigChanged;
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            try
            {
                BeginInvoke((Action)(() =>
                {
                    _config = ConfigManager.Load();
                    RefreshFolderList();
                }));
            }
            catch { }
        }

        private void SetupEngine()
        {
            _engine = new CollectorEngine(_config);
            _engine.OverallProgressChanged += OnOverallProgress;
            _engine.FolderProgressChanged += OnFolderProgress;
            _engine.FileProgressChanged += OnFileProgress;
            _engine.LogMessage += OnLogMessage;
        }

        private void SetupEventHandlers()
        {
            btnStartAll.Click += (s, e) => StartAll();
            btnStopAll.Click += (s, e) => StopAll();
            btnAddFolder.Click += (s, e) => AddFolder();
            btnEditFolder.Click += (s, e) => EditFolder();
            btnRemoveFolder.Click += (s, e) => RemoveFolder();
            btnExportConfig.Click += (s, e) => ExportConfig();
            btnImportConfig.Click += (s, e) => ImportConfig();
            btnViewHistory.Click += (s, e) => ViewHistory();
            btnClearHistory.Click += (s, e) => ClearHistory();

            dgvFolders.CellClick += OnFolderCellClick;

            this.FormClosing += OnFormClosing;
            notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
            exitToolStripMenuItem.Click += OnExitClick;

            // Re-layout toolbar when form resizes
            this.Resize += (s, e) => LayoutToolbar();
            toolbarPanel.Resize += (s, e) => LayoutToolbar();
        }

        /// <summary>
        /// Lays out toolbar buttons right-to-left across the toolbar panel.
        /// </summary>
        private void LayoutToolbar()
        {
            // Order in RTL: rightmost first
            var buttons = new Button[]
            {
                btnStartAll, btnStopAll,
                btnAddFolder, btnEditFolder, btnRemoveFolder,
                btnExportConfig, btnImportConfig,
                btnViewHistory, btnClearHistory
            };

            int btnWidth = 110;
            int gap = 6;
            int top = (toolbarPanel.Height - 32) / 2;
            int x = toolbarPanel.Width - 8 - btnWidth;

            foreach (var btn in buttons)
            {
                btn.Location = new Point(x, top);
                btn.Size = new Size(btnWidth, 32);
                x -= btnWidth + gap;
            }
        }

        private void SetupTimer()
        {
            _uiTimer = new System.Windows.Forms.Timer
            {
                Interval = 100
            };
            _uiTimer.Tick += (s, e) => UpdateProgressBars();
            _uiTimer.Start();
        }

        // ===========================================================
        // START / STOP
        // ===========================================================

        private void StartAll()
        {
            try
            {
                if (_config.Folders.Count == 0)
                {
                    MessageBox.Show(this, "ابتدا یک پوشه اضافه کنید.", "اطلاع",
                        MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                _engine.StartAll();
                btnStartAll.Enabled = false;
                btnStopAll.Enabled = true;
                LogToUi("▶ شروع مشاهده همه پوشه‌ها");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "خطا در شروع: " + ex.Message, "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private void StopAll()
        {
            _engine.StopAll();
            btnStartAll.Enabled = true;
            btnStopAll.Enabled = false;
            LogToUi("⏹ توقف همه پوشه‌ها");
        }

        // ===========================================================
        // FOLDER MANAGEMENT
        // ===========================================================

        private void AddFolder()
        {
            var newFolder = new FolderConfig
            {
                Id = _config.Folders.Count > 0 ? _config.Folders.Max(f => f.Id) + 1 : 1,
                Name = "پوشه جدید",
                ConflictStrategy = "rename"
            };

            using (var form = new FolderConfigForm(newFolder))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _config.Folders.Add(newFolder);
                    ConfigManager.Save(_config);
                    RefreshFolderList();
                    LogToUi("+ پوشه اضافه شد: " + newFolder.Name);
                }
            }
        }

        private void EditFolder()
        {
            if (dgvFolders.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "یک پوشه را انتخاب کنید.", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }
            int folderId = (int)dgvFolders.SelectedRows[0].Cells["colId"].Value;
            var folder = _config.Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null) return;

            using (var form = new FolderConfigForm(folder))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    folder.UpdatedAt = DateTime.Now;
                    ConfigManager.Save(_config);
                    RefreshFolderList();
                    LogToUi("✎ پوشه ویرایش شد: " + folder.Name);
                }
            }
        }

        private void RemoveFolder()
        {
            if (dgvFolders.SelectedRows.Count == 0) return;
            int folderId = (int)dgvFolders.SelectedRows[0].Cells["colId"].Value;
            if (MessageBox.Show(this, "آیا از حذف این پوشه مطمئن هستید؟", "تأیید حذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) != DialogResult.Yes) return;

            var folder = _config.Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                _engine.StopFolder(folderId);
                _config.Folders.Remove(folder);
                ConfigManager.Save(_config);
                RefreshFolderList();
                LogToUi("- پوشه حذف شد: " + folder.Name);
            }
        }

        private void RefreshFolderList()
        {
            dgvFolders.Rows.Clear();
            foreach (var folder in _config.Folders)
            {
                int rowIdx = dgvFolders.Rows.Add();
                var row = dgvFolders.Rows[rowIdx];
                row.Cells["colId"].Value = folder.Id;
                row.Cells["colName"].Value = folder.Name;
                row.Cells["colPath"].Value = folder.SourcePath;
                row.Cells["colEnabled"].Value = folder.Enabled ? "بله" : "خیر";
                row.Cells["colMode"].Value = folder.WatchMode;
                row.Cells["colActions"].Value = folder.Actions?.Count ?? 0;
                row.Cells["colStatus"].Value = "بیکار";
                row.Cells["colProgress"].Value = 0;
            }
        }

        private void OnFolderCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvFolders.Rows[e.RowIndex];
            int folderId = (int)row.Cells["colId"].Value;

            if (e.ColumnIndex == colStart.Index)
            {
                var folder = _config.Folders.FirstOrDefault(f => f.Id == folderId);
                if (folder != null)
                {
                    if (!_engine.IsRunning)
                    {
                        MessageBox.Show(this, "ابتدا روی «شروع همه» کلیک کنید تا موتور فعال شود.",
                            "اطلاع", MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        return;
                    }
                    _engine.StartFolder(folder);
                    LogToUi($"▶ شروع پوشه: {folder.Name}");
                }
            }
            else if (e.ColumnIndex == colStop.Index)
            {
                _engine.StopFolder(folderId);
                LogToUi($"⏹ توقف پوشه با ID: {folderId}");
            }
            else if (e.ColumnIndex == colPause.Index)
            {
                _engine.PauseFolder(folderId);
            }
            else if (e.ColumnIndex == colResume.Index)
            {
                _engine.ResumeFolder(folderId);
            }
        }

        // ===========================================================
        // IMPORT / EXPORT
        // ===========================================================

        private void ExportConfig()
        {
            using (var dlg = new SaveFileDialog
            {
                Filter = "JSON|*.json|All Files|*.*",
                FileName = "filecollector-config.json"
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ConfigManager.ExportConfig(dlg.FileName);
                        MessageBox.Show(this, "تنظیمات با موفقیت ذخیره شد.", "موفق",
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "خطا در ذخیره: " + ex.Message, "خطا",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                }
            }
        }

        private void ImportConfig()
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "JSON|*.json|All Files|*.*"
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ConfigManager.ImportConfig(dlg.FileName);
                        _config = ConfigManager.Load();
                        RefreshFolderList();
                        MessageBox.Show(this, "تنظیمات با موفقیت بارگذاری شد.", "موفق",
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "خطا در بارگذاری: " + ex.Message, "خطا",
                            MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                }
            }
        }

        // ===========================================================
        // HISTORY
        // ===========================================================

        private void ViewHistory()
        {
            var dt = DatabaseManager.GetHistory(500);
            using (var form = new Form
            {
                Text = "تاریخچه عملیات",
                Size = new Size(900, 500),
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(245, 247, 250)
            })
            {
                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = dt,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false
                };
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                form.Controls.Add(grid);
                form.ShowDialog(this);
            }
        }

        private void ClearHistory()
        {
            if (MessageBox.Show(this, "آیا تاریخچه پاک شود؟", "تأیید",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) != DialogResult.Yes) return;

            DatabaseManager.ClearHistory();
            LogToUi("تاریخچه پاک شد.");
        }

        // ===========================================================
        // FORM CLOSE / TRAY
        // ===========================================================

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_config.MinimizeToTray && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(2000, "File Collector",
                    "برنامه در سیستم‌تری فعال است.", ToolTipIcon.Info);
            }
            else
            {
                _engine?.StopAll();
            }
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            _engine?.StopAll();
            this.FormClosing -= OnFormClosing;
            this.Close();
        }

        // ===========================================================
        // PROGRESS HANDLERS
        // ===========================================================

        private void OnOverallProgress(OverallProgressInfo info)
        {
            if (IsDisposed) return;
            _overallProgress = info;
        }

        private void OnFolderProgress(FolderProgressInfo info)
        {
            if (IsDisposed) return;
            lock (_folderProgress)
            {
                var existing = _folderProgress.FirstOrDefault(p => p.FolderId == info.FolderId);
                if (existing != null)
                {
                    var idx = _folderProgress.IndexOf(existing);
                    _folderProgress[idx] = info;
                }
                else
                {
                    _folderProgress.Add(info);
                }
            }
        }

        private void OnFileProgress(FileProgressInfo info)
        {
            if (IsDisposed) return;
            _currentFileProgress = info;
        }

        private void OnLogMessage(string message)
        {
            if (IsDisposed) return;
            try
            {
                BeginInvoke((Action)(() => LogToUi(message)));
            }
            catch { }
        }

        private void LogToUi(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((Action)(() => LogToUi(message)));
                return;
            }

            string line = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
            txtLog.AppendText(line);

            if (txtLog.TextLength > 100_000)
            {
                txtLog.Text = txtLog.Text.Substring(txtLog.TextLength - 50_000);
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void UpdateProgressBars()
        {
            // Overall
            if (_overallProgress != null)
            {
                pbOverall.Value = Math.Min(100, Math.Max(0, _overallProgress.Percent));
                lblOverallStats.Text =
                    $"پردازش‌شده: {_overallProgress.ProcessedFiles} | " +
                    $"رد‌شده: {_overallProgress.SkippedFiles} | " +
                    $"خطا: {_overallProgress.FailedFiles} | " +
                    $"صف: {_overallProgress.QueuedFiles} | " +
                    $"سرعت: {_overallProgress.FilesPerSecond} فایل/ث | " +
                    $"سپری‌شده: {_overallProgress.ElapsedTime} | " +
                    $"باقی‌مانده: {_overallProgress.EstimatedRemaining} | " +
                    $"نرخ موفقیت: {_overallProgress.SuccessRate}% | " +
                    $"ورکر فعال: {_overallProgress.ActiveWorkers}";
            }

            // Per-folder
            lock (_folderProgress)
            {
                foreach (var info in _folderProgress)
                {
                    foreach (DataGridViewRow row in dgvFolders.Rows)
                    {
                        if (row.IsNewRow) continue;
                        int fid = (int)row.Cells["colId"].Value;
                        if (fid == info.FolderId)
                        {
                            row.Cells["colStatus"].Value = TranslateStatus(info.Status);
                            row.Cells["colProgress"].Value = info.Percent;
                            if (!string.IsNullOrEmpty(info.CurrentFile))
                                row.Cells["colCurrentFile"].Value = info.CurrentFile;
                            break;
                        }
                    }
                }
            }

            // Current file
            if (_currentFileProgress != null)
            {
                pbCurrentFile.Value = Math.Min(100, Math.Max(0, _currentFileProgress.Percent));
                lblCurrentFile.Text = _currentFileProgress.Status == "done"
                    ? $"✓ {_currentFileProgress.FileName}"
                    : $"[{_currentFileProgress.CurrentStepIndex}/{_currentFileProgress.TotalSteps}] {_currentFileProgress.CurrentStep} — {_currentFileProgress.FileName}";
            }
        }

        private string TranslateStatus(string status)
        {
            switch (status)
            {
                case "running": return "در حال اجرا";
                case "paused": return "متوقف‌شده";
                case "stopped": return "خاموش";
                case "idle": return "بیکار";
                default: return status;
            }
        }
    }
}
