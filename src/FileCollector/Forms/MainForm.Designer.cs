namespace FileCollector.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Top toolbar
        private Button btnStartAll;
        private Button btnStopAll;
        private Button btnAddFolder;
        private Button btnEditFolder;
        private Button btnRemoveFolder;
        private Button btnExportConfig;
        private Button btnImportConfig;
        private Button btnViewHistory;
        private Button btnClearHistory;

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

        // Splitters
        private SplitContainer mainSplit;
        private SplitContainer rightSplit;

        // Tray
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem exitToolStripMenuItem;

        // Group boxes
        private GroupBox grpFolders;
        private GroupBox grpOverall;
        private GroupBox grpCurrentFile;
        private GroupBox grpLog;

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

            this.btnStartAll = new Button();
            this.btnStopAll = new Button();
            this.btnAddFolder = new Button();
            this.btnEditFolder = new Button();
            this.btnRemoveFolder = new Button();
            this.btnExportConfig = new Button();
            this.btnImportConfig = new Button();
            this.btnViewHistory = new Button();
            this.btnClearHistory = new Button();

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

            this.notifyIcon = new NotifyIcon(this.components);
            this.trayMenu = new ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new ToolStripMenuItem();

            // ----- Form -----
            this.Text = "File Collector";
            this.Size = new System.Drawing.Size(1300, 850);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // ----- Top toolbar (FlowLayoutPanel-like using manual layout) -----
            var toolbarPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8, 8, 8, 8) };
            int btnX = 8;
            var btnsTop = new[]
            {
                new { Btn = this.btnStartAll,    Text = "شروع همه" },
                new { Btn = this.btnStopAll,     Text = "توقف همه" },
                new { Btn = this.btnAddFolder,   Text = "+ افزودن پوشه" },
                new { Btn = this.btnEditFolder,  Text = "ویرایش" },
                new { Btn = this.btnRemoveFolder,Text = "حذف" },
                new { Btn = this.btnExportConfig,Text = "ذخیره تنظیمات" },
                new { Btn = this.btnImportConfig,Text = "بارگذاری تنظیمات" },
                new { Btn = this.btnViewHistory, Text = "تاریخچه" },
                new { Btn = this.btnClearHistory,Text = "پاک‌سازی تاریخچه" }
            };
            // In RTL, lay buttons from right to left
            btnX = toolbarPanel.Width - 8 - 110;
            foreach (var b in btnsTop)
            {
                b.Btn.Text = b.Text;
                b.Btn.Size = new Size(110, 32);
                b.Btn.Location = new Point(btnX, 9);
                b.Btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                toolbarPanel.Controls.Add(b.Btn);
                btnX -= 116;
            }
            this.btnStopAll.Enabled = false;
            toolbarPanel.Resize += (s, e) =>
            {
                int x = toolbarPanel.Width - 8 - 110;
                foreach (Control c in toolbarPanel.Controls)
                {
                    c.Location = new Point(x, 9);
                    x -= 116;
                }
            };

            // ----- Folder list -----
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).BeginInit();
            this.dgvFolders.AllowUserToAddRows = false;
            this.dgvFolders.AllowUserToDeleteRows = false;
            this.dgvFolders.ReadOnly = true;
            this.dgvFolders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFolders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvFolders.RowHeadersVisible = false;
            this.dgvFolders.BackgroundColor = SystemColors.Window;

            this.colId.HeaderText = "ID";
            this.colId.Name = "colId";
            this.colId.Width = 40;
            this.colId.ReadOnly = true;
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

            this.dgvFolders.Columns.AddRange(new DataGridViewColumn[]
            {
                this.colId, this.colName, this.colPath, this.colEnabled,
                this.colMode, this.colActions, this.colStatus, this.colCurrentFile,
                this.colProgress, this.colStart, this.colStop, this.colPause, this.colResume
            });

            // ----- Progress bars -----
            this.pbOverall.Dock = DockStyle.Top;
            this.pbOverall.Height = 28;
            this.pbOverall.Minimum = 0;
            this.pbOverall.Maximum = 100;
            this.pbOverall.Value = 0;

            this.lblOverall.Dock = DockStyle.Top;
            this.lblOverall.Text = "پیشرفت کلی";
            this.lblOverall.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblOverall.Height = 22;
            this.lblOverall.TextAlign = ContentAlignment.MiddleRight;

            this.lblOverallStats.Dock = DockStyle.Top;
            this.lblOverallStats.Text = "آمار: ...";
            this.lblOverallStats.Height = 22;
            this.lblOverallStats.TextAlign = ContentAlignment.MiddleRight;
            this.lblOverallStats.Font = new System.Drawing.Font("Tahoma", 8.5F);

            this.pbCurrentFile.Dock = DockStyle.Top;
            this.pbCurrentFile.Height = 22;
            this.pbCurrentFile.Minimum = 0;
            this.pbCurrentFile.Maximum = 100;
            this.pbCurrentFile.Value = 0;

            this.lblCurrentFile.Dock = DockStyle.Top;
            this.lblCurrentFile.Text = "فایل جاری: --";
            this.lblCurrentFile.Height = 22;
            this.lblCurrentFile.TextAlign = ContentAlignment.MiddleRight;
            this.lblCurrentFile.Font = new System.Drawing.Font("Tahoma", 8.5F);

            // ----- Log textbox -----
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = Color.FromArgb(245, 245, 245);
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Dock = DockStyle.Fill;

            // ----- Group boxes -----
            this.grpFolders.Text = "پوشه‌های تحت نظر";
            this.grpFolders.Dock = DockStyle.Fill;
            this.grpFolders.Padding = new Padding(8, 22, 8, 8);

            this.grpOverall.Text = "پیشرفت کلی";
            this.grpOverall.Dock = DockStyle.Top;
            this.grpOverall.Height = 90;
            this.grpOverall.Padding = new Padding(8, 22, 8, 8);

            this.grpCurrentFile.Text = "فایل جاری";
            this.grpCurrentFile.Dock = DockStyle.Top;
            this.grpCurrentFile.Height = 65;
            this.grpCurrentFile.Padding = new Padding(8, 22, 8, 8);

            this.grpLog.Text = "لاگ";
            this.grpLog.Dock = DockStyle.Bottom;
            this.grpLog.Height = 200;
            this.grpLog.Padding = new Padding(8, 22, 8, 8);

            // ----- Layout -----
            // Folder list fills grpFolders
            this.dgvFolders.Dock = DockStyle.Fill;
            this.grpFolders.Controls.Add(this.dgvFolders);

            // Overall group: stats label > overall bar > header
            this.grpOverall.Controls.Add(this.lblOverallStats);
            this.grpOverall.Controls.Add(this.pbOverall);
            this.grpOverall.Controls.Add(this.lblOverall);

            // Current file group: file bar > file label
            this.grpCurrentFile.Controls.Add(this.pbCurrentFile);
            this.grpCurrentFile.Controls.Add(this.lblCurrentFile);

            // Log group: just textbox
            this.grpLog.Controls.Add(this.txtLog);

            // Bottom split container for log + current file + overall
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 380 };
            bottomPanel.Controls.Add(this.grpLog);
            bottomPanel.Controls.Add(this.grpCurrentFile);
            bottomPanel.Controls.Add(this.grpOverall);

            // Main layout
            this.Controls.Add(this.grpFolders);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(toolbarPanel);

            // ----- Tray -----
            this.trayMenu.Items.AddRange(new ToolStripItem[] { this.exitToolStripMenuItem });
            this.exitToolStripMenuItem.Text = "خروج";
            this.notifyIcon.Icon = SystemIcons.Application;
            this.notifyIcon.Text = "File Collector";
            this.notifyIcon.ContextMenuStrip = this.trayMenu;
            this.notifyIcon.Visible = false;

            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).EndInit();
        }
    }
}
