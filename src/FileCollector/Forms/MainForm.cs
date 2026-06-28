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

        // Per-folder progress card controls: folderId -> (nameLabel, progressBar, fileLabel)
        private readonly Dictionary<int, Tuple<Label, ProgressBar, Label>> _folderCards =
            new Dictionary<int, Tuple<Label, ProgressBar, Label>>();

        // Minimal button style — applied once in constructor
        private void ApplyButtonStyle(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            btn.BackColor = Color.White;
            btn.ForeColor = Color.FromArgb(51, 51, 51);
            btn.Font = new Font("Tahoma", 9.75F);
            btn.Cursor = Cursors.Hand;
        }

        public MainForm()
        {
            InitializeComponent();

            // Apply minimal style to all toolbar buttons
            ApplyButtonStyle(btnStartAll);
            ApplyButtonStyle(btnStopAll);
            ApplyButtonStyle(btnAddFolder);
            ApplyButtonStyle(btnEditFolder);
            ApplyButtonStyle(btnRemoveFolder);
            ApplyButtonStyle(btnExportConfig);
            ApplyButtonStyle(btnImportConfig);
            ApplyButtonStyle(btnViewHistory);
            ApplyButtonStyle(btnClearHistory);

            SetupLocalization();
            LoadConfig();
            SetupEngine();
            SetupEventHandlers();
            LayoutToolbar();
            SetupTimer();
            RefreshFolderList();
            UpdateStatus("آماده");
        }

        // Colors used for dynamically-created folder progress cards
        private static readonly Color CardBg       = Color.White;
        private static readonly Color CardBorder    = Color.FromArgb(220, 220, 215);
        private static readonly Color CardTextDark  = Color.FromArgb(51, 51, 51);
        private static readonly Color CardTextMed   = Color.FromArgb(90, 90, 90);
        private static readonly Color CardProgress  = Color.FromArgb(160, 160, 155);
        private static readonly Color CardProgressBg = Color.FromArgb(245, 245, 242);

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

            this.Resize += (s, e) => { LayoutToolbar(); LayoutFolderProgressCards(); };
            toolbarPanel.Resize += (s, e) => LayoutToolbar();
            pnlFolderProgress.Resize += (s, e) => LayoutFolderProgressCards();
        }

        private void LayoutToolbar()
        {
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

        private void UpdateStatus(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke((Action)(() => lblStatus.Text = text));
            }
            else
            {
                lblStatus.Text = text;
            }
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

                UpdateStatus("در حال شروع نظارت...");
                Application.DoEvents();

                _engine.StartAll();
                LogToUi("▶ شروع مشاهده همه پوشه‌ها");
                UpdateStatus("در حال نظارت");
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
            UpdateStatus("در حال توقف...");
            Application.DoEvents();

            _engine.StopAll();
            LogToUi("⏹ توقف همه پوشه‌ها");
            UpdateStatus("متوقف‌شده");

            // Reset folder statuses in grid
            foreach (DataGridViewRow row in dgvFolders.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells["colStatus"].Value = "متوقف‌شده";
                row.Cells["colProgress"].Value = 0;
            }
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
            RebuildFolderProgressCards();
        }

        /// <summary>
        /// Rebuilds the per-folder progress bar cards in the bottom panel.
        /// Should be called whenever the folder list changes.
        /// </summary>
        private void RebuildFolderProgressCards()
        {
            if (pnlFolderProgress == null) return;

            pnlFolderProgress.SuspendLayout();
            pnlFolderProgress.Controls.Clear();
            _folderCards.Clear();

            foreach (var folder in _config.Folders)
            {
                var card = CreateFolderProgressCard(folder.Id, folder.Name);
                pnlFolderProgress.Controls.Add(card);
            }

            pnlFolderProgress.ResumeLayout(true);
            LayoutFolderProgressCards();
        }

        private Panel CreateFolderProgressCard(int folderId, string folderName)
        {
            var card = new Panel();
            card.Width = Math.Max(200, pnlFolderProgress.ClientSize.Width - 25);
            card.Height = 48;
            card.Padding = new Padding(4, 2, 4, 2);
            card.BackColor = CardBg;
            card.Margin = new Padding(0, 0, 0, 4);

            var lblName = new Label();
            lblName.Text = folderName + "  —  آماده";
            lblName.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lblName.ForeColor = CardTextDark;
            lblName.Dock = DockStyle.Top;
            lblName.Height = 20;
            lblName.TextAlign = ContentAlignment.MiddleRight;
            lblName.Padding = new Padding(4, 0, 4, 0);

            var pb = new ProgressBar();
            pb.Dock = DockStyle.Fill;
            pb.Minimum = 0;
            pb.Maximum = 100;
            pb.Value = 0;
            pb.ForeColor = CardProgress;
            pb.BackColor = CardProgressBg;
            pb.Height = 18;

            var lblFile = new Label();
            lblFile.Text = "";
            lblFile.Font = new Font("Tahoma", 8F);
            lblFile.ForeColor = CardTextMed;
            lblFile.Dock = DockStyle.Bottom;
            lblFile.Height = 16;
            lblFile.TextAlign = ContentAlignment.MiddleRight;
            lblFile.Padding = new Padding(4, 0, 4, 0);

            card.Controls.Add(pb);       // Fill
            card.Controls.Add(lblFile);  // Bottom
            card.Controls.Add(lblName);  // Top

            _folderCards[folderId] = Tuple.Create(lblName, pb, lblFile);

            return card;
        }

        /// <summary>
        /// Adjusts the width of all folder progress cards to fit the panel.
        /// </summary>
        private void LayoutFolderProgressCards()
        {
            if (pnlFolderProgress == null) return;
            int w = Math.Max(200, pnlFolderProgress.ClientSize.Width - 25);
            foreach (Control c in pnlFolderProgress.Controls)
            {
                c.Width = w;
            }
        }

        private void OnFolderCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvFolders.Rows[e.RowIndex];
            int folderId = (int)row.Cells["colId"].Value;
            var folder = _config.Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null) return;

            if (e.ColumnIndex == colStart.Index)
            {
                // StartFolder auto-starts the engine if needed — no need to click "Start All" first.
                // If the folder is already running, StartFolder is a no-op.
                UpdateStatus($"در حال شروع پوشه: {folder.Name}...");
                _engine.StartFolder(folder);
                row.Cells["colStatus"].Value = "در حال اجرا";
                LogToUi($"▶ شروع پوشه: {folder.Name}");
                UpdateStatus("در حال نظارت");
            }
            else if (e.ColumnIndex == colStop.Index)
            {
                _engine.StopFolder(folderId);
                row.Cells["colStatus"].Value = "متوقف‌شده";
                LogToUi($"⏹ توقف پوشه: {folder.Name}");
            }
            else if (e.ColumnIndex == colPause.Index)
            {
                _engine.PauseFolder(folderId);
                row.Cells["colStatus"].Value = "مکث";
                LogToUi($"⏸ مکث پوشه: {folder.Name}");
            }
            else if (e.ColumnIndex == colResume.Index)
            {
                _engine.ResumeFolder(folderId);
                row.Cells["colStatus"].Value = "در حال اجرا";
                LogToUi($"▶ ادامه پوشه: {folder.Name}");
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
                BackColor = Color.FromArgb(252, 251, 248)
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
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    RowHeadersWidth = 4
                };
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 242);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 51, 51);
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
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

            // Per-folder — update both grid rows AND progress cards
            lock (_folderProgress)
            {
                foreach (var info in _folderProgress)
                {
                    // Update grid row
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

                    // Update progress card
                    if (_folderCards.TryGetValue(info.FolderId, out var card))
                    {
                        var lblName = card.Item1;
                        var pb = card.Item2;
                        var lblFile = card.Item3;

                        if (lblName.InvokeRequired || pb.InvokeRequired || lblFile.InvokeRequired)
                        {
                            // Should not happen since this runs on UI timer, but be safe
                            continue;
                        }

                        string statusText = TranslateStatus(info.Status);
                        lblName.Text = $"{info.FolderName}  —  {info.Percent}%  ({info.ProcessedFiles}/{info.TotalFiles})  {statusText}";
                        pb.Value = Math.Min(100, Math.Max(0, info.Percent));
                        lblFile.Text = info.CurrentFile ?? "";
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
                case "paused": return "مکث";
                case "stopped": return "متوقف‌شده";
                case "idle": return "بیکار";
                default: return status;
            }
        }
    }
}
