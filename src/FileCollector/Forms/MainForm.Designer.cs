using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileCollector.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // ===== Color palette (minimal: white + very light gray + very light cream) =====
        private static readonly Color BgForm       = Color.FromArgb(252, 251, 248); // very light cream
        private static readonly Color BgPanel      = Color.White;
        private static readonly Color BgButton     = Color.White;
        private static readonly Color BgButtonAlt  = Color.FromArgb(245, 245, 242); // very light gray-cream
        private static readonly Color BorderLight  = Color.FromArgb(220, 220, 215); // subtle border
        private static readonly Color TextDark     = Color.FromArgb(51, 51, 51);
        private static readonly Color TextMedium   = Color.FromArgb(90, 90, 90);
        private static readonly Color BgGridHeader = Color.FromArgb(245, 245, 242);
        private static readonly Color BgGridAltRow = Color.FromArgb(250, 249, 246);
        private static readonly Color ProgressBar  = Color.FromArgb(160, 160, 155); // muted gray
        private static readonly Color BgLog        = Color.FromArgb(250, 249, 246); // very light cream
        private static readonly Color TextLog      = Color.FromArgb(51, 51, 51);

        // Top toolbar buttons
        private Button btnStartAll;
        private Button btnStopAll;
        private Button btnAddFolder;
        private Button btnEditFolder;
        private Button btnRemoveFolder;
        private Button btnExportConfig;
        private Button btnImportConfig;
        private Button btnViewHistory;
        private Button btnClearHistory;
        private Panel toolbarPanel;

        // Folder list
        private DataGridView dgvFolders;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colPath;
        private DataGridViewTextBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colMode;
        private DataGridViewTextBoxColumn colActions;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colCurrentFile;
        private DataGridViewTextBoxColumn colProgress;
        private DataGridViewButtonColumn colStart;
        private DataGridViewButtonColumn colStop;
        private DataGridViewButtonColumn colPause;
        private DataGridViewButtonColumn colResume;

        // Progress bars
        private ProgressBar pbOverall;
        private ProgressBar pbCurrentFile;
        private Label lblOverall;
        private Label lblOverallStats;
        private Label lblCurrentFile;

        // Log
        private TextBox txtLog;

        // Status label (shows what the app is currently doing)
        private Label lblStatus;

        // Group boxes (containers)
        private GroupBox grpFolders;
        private GroupBox grpOverall;
        private GroupBox grpCurrentFile;
        private GroupBox grpLog;

        // Tray
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem exitToolStripMenuItem;

        // Bottom panel
        private Panel bottomPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.toolbarPanel = new Panel();
            this.btnStartAll = new Button();
            this.btnStopAll = new Button();
            this.btnAddFolder = new Button();
            this.btnEditFolder = new Button();
            this.btnRemoveFolder = new Button();
            this.btnExportConfig = new Button();
            this.btnImportConfig = new Button();
            this.btnViewHistory = new Button();
            this.btnClearHistory = new Button();
            this.lblStatus = new Label();

            this.dgvFolders = new DataGridView();
            this.colId = new DataGridViewTextBoxColumn();
            this.colName = new DataGridViewTextBoxColumn();
            this.colPath = new DataGridViewTextBoxColumn();
            this.colEnabled = new DataGridViewTextBoxColumn();
            this.colMode = new DataGridViewTextBoxColumn();
            this.colActions = new DataGridViewTextBoxColumn();
            this.colStatus = new DataGridViewTextBoxColumn();
            this.colCurrentFile = new DataGridViewTextBoxColumn();
            this.colProgress = new DataGridViewTextBoxColumn();
            this.colStart = new DataGridViewButtonColumn();
            this.colStop = new DataGridViewButtonColumn();
            this.colPause = new DataGridViewButtonColumn();
            this.colResume = new DataGridViewButtonColumn();

            this.pbOverall = new ProgressBar();
            this.pbCurrentFile = new ProgressBar();
            this.lblOverall = new Label();
            this.lblOverallStats = new Label();
            this.lblCurrentFile = new Label();

            this.txtLog = new TextBox();

            this.grpFolders = new GroupBox();
            this.grpOverall = new GroupBox();
            this.grpCurrentFile = new GroupBox();
            this.grpLog = new GroupBox();

            this.bottomPanel = new Panel();

            this.notifyIcon = new NotifyIcon(this.components);
            this.trayMenu = new ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new ToolStripMenuItem();

            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).BeginInit();

            // ---------- Form ----------
            this.Text = "File Collector";
            this.Size = new Size(1300, 850);
            this.MinimumSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Tahoma", 9.75F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = BgForm;

            // ---------- Toolbar ----------
            this.toolbarPanel.Dock = DockStyle.Top;
            this.toolbarPanel.Height = 50;
            this.toolbarPanel.Padding = new Padding(8);
            this.toolbarPanel.BackColor = BgPanel;

            // All buttons use the same minimal style — only text distinguishes them
            this.btnStartAll.Text = "شروع همه";
            this.btnStopAll.Text = "توقف همه";
            this.btnAddFolder.Text = "+ افزودن پوشه";
            this.btnEditFolder.Text = "ویرایش";
            this.btnRemoveFolder.Text = "حذف";
            this.btnExportConfig.Text = "ذخیره تنظیمات";
            this.btnImportConfig.Text = "بارگذاری تنظیمات";
            this.btnViewHistory.Text = "تاریخچه";
            this.btnClearHistory.Text = "پاک‌سازی";

            this.btnStartAll.Size = new Size(110, 32);
            this.btnStopAll.Size = new Size(110, 32);
            this.btnAddFolder.Size = new Size(110, 32);
            this.btnEditFolder.Size = new Size(110, 32);
            this.btnRemoveFolder.Size = new Size(110, 32);
            this.btnExportConfig.Size = new Size(110, 32);
            this.btnImportConfig.Size = new Size(110, 32);
            this.btnViewHistory.Size = new Size(110, 32);
            this.btnClearHistory.Size = new Size(110, 32);

            this.btnStopAll.Enabled = false;

            this.toolbarPanel.Controls.Add(this.btnStartAll);
            this.toolbarPanel.Controls.Add(this.btnStopAll);
            this.toolbarPanel.Controls.Add(this.btnAddFolder);
            this.toolbarPanel.Controls.Add(this.btnEditFolder);
            this.toolbarPanel.Controls.Add(this.btnRemoveFolder);
            this.toolbarPanel.Controls.Add(this.btnExportConfig);
            this.toolbarPanel.Controls.Add(this.btnImportConfig);
            this.toolbarPanel.Controls.Add(this.btnViewHistory);
            this.toolbarPanel.Controls.Add(this.btnClearHistory);

            // ---------- Status label (below toolbar) ----------
            this.lblStatus.Dock = DockStyle.Top;
            this.lblStatus.Height = 24;
            this.lblStatus.Text = "آماده";
            this.lblStatus.TextAlign = ContentAlignment.MiddleRight;
            this.lblStatus.Font = new Font("Tahoma", 9F);
            this.lblStatus.BackColor = BgForm;
            this.lblStatus.ForeColor = TextMedium;
            this.lblStatus.Padding = new Padding(8, 0, 8, 0);

            // ---------- Folder list ----------
            this.dgvFolders.AllowUserToAddRows = false;
            this.dgvFolders.AllowUserToDeleteRows = false;
            this.dgvFolders.ReadOnly = true;
            this.dgvFolders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFolders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvFolders.RowHeadersVisible = false;
            this.dgvFolders.BackgroundColor = BgPanel;
            this.dgvFolders.BorderStyle = BorderStyle.None;
            this.dgvFolders.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFolders.GridColor = BorderLight;
            this.dgvFolders.EnableHeadersVisualStyles = false;
            this.dgvFolders.ColumnHeadersDefaultCellStyle.BackColor = BgGridHeader;
            this.dgvFolders.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            this.dgvFolders.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.dgvFolders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvFolders.ColumnHeadersHeight = 32;
            this.dgvFolders.RowHeadersWidth = 4;
            this.dgvFolders.RowTemplate.Height = 28;
            this.dgvFolders.AlternatingRowsDefaultCellStyle.BackColor = BgGridAltRow;
            this.dgvFolders.DefaultCellStyle.SelectionBackColor = BgButtonAlt;
            this.dgvFolders.DefaultCellStyle.SelectionForeColor = TextDark;
            this.dgvFolders.RightToLeft = RightToLeft.Yes;

            this.colId.HeaderText = "ID";
            this.colId.Name = "colId";
            this.colId.Width = 40;
            this.colId.Visible = false;

            this.colName.HeaderText = "نام";
            this.colName.Name = "colName";
            this.colName.Width = 120;

            this.colPath.HeaderText = "مسیر";
            this.colPath.Name = "colPath";
            this.colPath.Width = 250;

            this.colEnabled.HeaderText = "فعال";
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.Width = 60;

            this.colMode.HeaderText = "حالت";
            this.colMode.Name = "colMode";
            this.colMode.Width = 80;

            this.colActions.HeaderText = "اکشن‌ها";
            this.colActions.Name = "colActions";
            this.colActions.Width = 60;

            this.colStatus.HeaderText = "وضعیت";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;

            this.colCurrentFile.HeaderText = "فایل جاری";
            this.colCurrentFile.Name = "colCurrentFile";
            this.colCurrentFile.Width = 150;

            this.colProgress.HeaderText = "پیشرفت";
            this.colProgress.Name = "colProgress";
            this.colProgress.Width = 60;

            this.colStart.HeaderText = "";
            this.colStart.Name = "colStart";
            this.colStart.Text = "شروع";
            this.colStart.UseColumnTextForButtonValue = true;
            this.colStart.Width = 50;

            this.colStop.HeaderText = "";
            this.colStop.Name = "colStop";
            this.colStop.Text = "توقف";
            this.colStop.UseColumnTextForButtonValue = true;
            this.colStop.Width = 50;

            this.colPause.HeaderText = "";
            this.colPause.Name = "colPause";
            this.colPause.Text = "مکث";
            this.colPause.UseColumnTextForButtonValue = true;
            this.colPause.Width = 45;

            this.colResume.HeaderText = "";
            this.colResume.Name = "colResume";
            this.colResume.Text = "ادامه";
            this.colResume.UseColumnTextForButtonValue = true;
            this.colResume.Width = 50;

            this.dgvFolders.Columns.AddRange(new DataGridViewColumn[] {
                this.colId, this.colName, this.colPath, this.colEnabled,
                this.colMode, this.colActions, this.colStatus, this.colCurrentFile,
                this.colProgress, this.colStart, this.colStop, this.colPause, this.colResume
            });

            // ---------- Overall progress group ----------
            this.grpOverall.Text = "پیشرفت کلی";
            this.grpOverall.Dock = DockStyle.Top;
            this.grpOverall.Height = 95;
            this.grpOverall.Padding = new Padding(8, 22, 8, 8);
            this.grpOverall.BackColor = BgPanel;
            this.grpOverall.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.grpOverall.ForeColor = TextDark;

            this.lblOverall.Dock = DockStyle.Top;
            this.lblOverall.Text = "پیشرفت کلی";
            this.lblOverall.Font = new Font("Tahoma", 9.5F, FontStyle.Bold);
            this.lblOverall.Height = 22;
            this.lblOverall.TextAlign = ContentAlignment.MiddleRight;
            this.lblOverall.BackColor = BgPanel;
            this.lblOverall.ForeColor = TextDark;

            this.pbOverall.Dock = DockStyle.Top;
            this.pbOverall.Height = 28;
            this.pbOverall.Minimum = 0;
            this.pbOverall.Maximum = 100;
            this.pbOverall.Value = 0;
            this.pbOverall.ForeColor = ProgressBar;
            this.pbOverall.BackColor = BgGridAltRow;

            this.lblOverallStats.Dock = DockStyle.Top;
            this.lblOverallStats.Text = "آمار: ...";
            this.lblOverallStats.Height = 24;
            this.lblOverallStats.TextAlign = ContentAlignment.MiddleRight;
            this.lblOverallStats.Font = new Font("Tahoma", 9F);
            this.lblOverallStats.BackColor = BgPanel;
            this.lblOverallStats.ForeColor = TextMedium;

            this.grpOverall.Controls.Add(this.lblOverallStats);
            this.grpOverall.Controls.Add(this.pbOverall);
            this.grpOverall.Controls.Add(this.lblOverall);

            // ---------- Current file group ----------
            this.grpCurrentFile.Text = "فایل جاری";
            this.grpCurrentFile.Dock = DockStyle.Top;
            this.grpCurrentFile.Height = 65;
            this.grpCurrentFile.Padding = new Padding(8, 22, 8, 8);
            this.grpCurrentFile.BackColor = BgPanel;
            this.grpCurrentFile.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.grpCurrentFile.ForeColor = TextDark;

            this.lblCurrentFile.Dock = DockStyle.Top;
            this.lblCurrentFile.Text = "فایل جاری: --";
            this.lblCurrentFile.Height = 22;
            this.lblCurrentFile.TextAlign = ContentAlignment.MiddleRight;
            this.lblCurrentFile.Font = new Font("Tahoma", 9F);
            this.lblCurrentFile.BackColor = BgPanel;
            this.lblCurrentFile.ForeColor = TextMedium;

            this.pbCurrentFile.Dock = DockStyle.Top;
            this.pbCurrentFile.Height = 22;
            this.pbCurrentFile.Minimum = 0;
            this.pbCurrentFile.Maximum = 100;
            this.pbCurrentFile.Value = 0;
            this.pbCurrentFile.ForeColor = ProgressBar;
            this.pbCurrentFile.BackColor = BgGridAltRow;

            this.grpCurrentFile.Controls.Add(this.pbCurrentFile);
            this.grpCurrentFile.Controls.Add(this.lblCurrentFile);

            // ---------- Folder list group ----------
            this.grpFolders.Text = "پوشه‌های تحت نظر";
            this.grpFolders.Dock = DockStyle.Fill;
            this.grpFolders.Padding = new Padding(8, 22, 8, 8);
            this.grpFolders.BackColor = BgPanel;
            this.grpFolders.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.grpFolders.ForeColor = TextDark;

            this.dgvFolders.Dock = DockStyle.Fill;
            this.grpFolders.Controls.Add(this.dgvFolders);

            // ---------- Log group ----------
            this.grpLog.Text = "لاگ";
            this.grpLog.Dock = DockStyle.Fill;
            this.grpLog.Padding = new Padding(8, 22, 8, 8);
            this.grpLog.BackColor = BgPanel;
            this.grpLog.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.grpLog.ForeColor = TextDark;

            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = BgLog;
            this.txtLog.ForeColor = TextLog;
            this.txtLog.Font = new Font("Consolas", 9F);
            this.txtLog.Dock = DockStyle.Fill;
            this.txtLog.BorderStyle = BorderStyle.FixedSingle;

            this.grpLog.Controls.Add(this.txtLog);

            // ---------- Bottom panel layout ----------
            this.bottomPanel.Dock = DockStyle.Bottom;
            this.bottomPanel.Height = 380;
            this.bottomPanel.BackColor = BgForm;

            this.bottomPanel.Controls.Add(this.grpLog);
            this.bottomPanel.Controls.Add(this.grpCurrentFile);
            this.bottomPanel.Controls.Add(this.grpOverall);

            // ---------- Tray ----------
            this.trayMenu.Items.AddRange(new ToolStripItem[] { this.exitToolStripMenuItem });
            this.exitToolStripMenuItem.Text = "خروج";
            this.notifyIcon.Icon = SystemIcons.Application;
            this.notifyIcon.Text = "File Collector";
            this.notifyIcon.ContextMenuStrip = this.trayMenu;
            this.notifyIcon.Visible = false;

            // ---------- Main form controls ----------
            this.Controls.Add(this.grpFolders);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.toolbarPanel);

            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).EndInit();
        }
    }
}
