namespace FileCollector.Forms
{
    partial class FolderConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabGeneral;
        private TabPage tabActions;
        private TabPage tabTextProcessing;
        private TabPage tabDatabase;

        // General tab
        private Label lblName;
        private TextBox txtName;
        private Label lblSourcePath;
        private TextBox txtSourcePath;
        private Button btnBrowseSource;
        private CheckBox chkIncludeSubfolders;
        private Label lblFileFilter;
        private TextBox txtFileFilter;
        private Label lblMinSize;
        private NumericUpDown numMinSize;
        private Label lblMaxSize;
        private NumericUpDown numMaxSize;
        private Label lblWatchMode;
        private ComboBox cmbWatchMode;
        private Label lblInterval;
        private NumericUpDown numIntervalSeconds;
        private CheckBox chkEnabled;
        private Label lblConflict;
        private ComboBox cmbConflict;
        private Label lblDestination;
        private TextBox txtDestination;
        private Button btnBrowseDest;
        private Label lblSubfolder;
        private TextBox txtSubfolderPattern;
        private Label lblFilenamePattern;
        private TextBox txtFilenamePattern;
        private CheckBox chkEnableDedup;
        private Button btnVariables;

        // Actions tab
        private ListBox lstActions;
        private Button btnAddAction;
        private Button btnEditAction;
        private Button btnRemoveAction;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Label lblActionsHint;

        // Text processing tab
        private CheckBox chkEnableTextProcessing;
        private Label lblExtensions;
        private TextBox txtExtensions;
        private Label lblEncoding;
        private ComboBox cmbEncoding;
        private CheckBox chkBackup;
        private GroupBox grpFindReplace;
        private CheckBox chkFR;
        private DataGridView dgvFindReplace;
        private GroupBox grpHeaderFooter;
        private CheckBox chkHeader;
        private CheckBox chkFooter;
        private TextBox txtHeader;
        private TextBox txtFooter;
        private GroupBox grpAppendPrepend;
        private CheckBox chkAppend;
        private CheckBox chkPrepend;
        private TextBox txtAppend;
        private TextBox txtPrepend;

        // Database tab
        private CheckBox chkEnableDb;
        private Label lblConnString;
        private TextBox txtConnString;
        private Label lblTableName;
        private TextBox txtTableName;
        private Label lblDbMode;
        private ComboBox cmbDbMode;
        private Label lblFileShare;
        private TextBox txtFileShare;
        private Button btnBrowseShare;
        private Label lblMaxFileSizeMb;
        private NumericUpDown numMaxFileSizeMb;
        private CheckBox chkSkipLarger;
        private CheckBox chkCompress;
        private Label lblDbSubfolder;
        private TextBox txtDbSubfolder;
        private Button btnTestConnection;

        // Bottom buttons
        private Button btnSave;
        private Button btnCancel;

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
            this.tabMain = new TabControl();
            this.tabGeneral = new TabPage();
            this.tabActions = new TabPage();
            this.tabTextProcessing = new TabPage();
            this.tabDatabase = new TabPage();

            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblSourcePath = new Label();
            this.txtSourcePath = new TextBox();
            this.btnBrowseSource = new Button();
            this.chkIncludeSubfolders = new CheckBox();
            this.lblFileFilter = new Label();
            this.txtFileFilter = new TextBox();
            this.lblMinSize = new Label();
            this.numMinSize = new NumericUpDown();
            this.lblMaxSize = new Label();
            this.numMaxSize = new NumericUpDown();
            this.lblWatchMode = new Label();
            this.cmbWatchMode = new ComboBox();
            this.lblInterval = new Label();
            this.numIntervalSeconds = new NumericUpDown();
            this.chkEnabled = new CheckBox();
            this.lblConflict = new Label();
            this.cmbConflict = new ComboBox();
            this.lblDestination = new Label();
            this.txtDestination = new TextBox();
            this.btnBrowseDest = new Button();
            this.lblSubfolder = new Label();
            this.txtSubfolderPattern = new TextBox();
            this.lblFilenamePattern = new Label();
            this.txtFilenamePattern = new TextBox();
            this.chkEnableDedup = new CheckBox();
            this.btnVariables = new Button();

            this.lstActions = new ListBox();
            this.btnAddAction = new Button();
            this.btnEditAction = new Button();
            this.btnRemoveAction = new Button();
            this.btnMoveUp = new Button();
            this.btnMoveDown = new Button();
            this.lblActionsHint = new Label();

            this.chkEnableTextProcessing = new CheckBox();
            this.lblExtensions = new Label();
            this.txtExtensions = new TextBox();
            this.lblEncoding = new Label();
            this.cmbEncoding = new ComboBox();
            this.chkBackup = new CheckBox();
            this.grpFindReplace = new GroupBox();
            this.chkFR = new CheckBox();
            this.dgvFindReplace = new DataGridView();
            this.grpHeaderFooter = new GroupBox();
            this.chkHeader = new CheckBox();
            this.chkFooter = new CheckBox();
            this.txtHeader = new TextBox();
            this.txtFooter = new TextBox();
            this.grpAppendPrepend = new GroupBox();
            this.chkAppend = new CheckBox();
            this.chkPrepend = new CheckBox();
            this.txtAppend = new TextBox();
            this.txtPrepend = new TextBox();

            this.chkEnableDb = new CheckBox();
            this.lblConnString = new Label();
            this.txtConnString = new TextBox();
            this.lblTableName = new Label();
            this.txtTableName = new TextBox();
            this.lblDbMode = new Label();
            this.cmbDbMode = new ComboBox();
            this.lblFileShare = new Label();
            this.txtFileShare = new TextBox();
            this.btnBrowseShare = new Button();
            this.lblMaxFileSizeMb = new Label();
            this.numMaxFileSizeMb = new NumericUpDown();
            this.chkSkipLarger = new CheckBox();
            this.chkCompress = new CheckBox();
            this.lblDbSubfolder = new Label();
            this.txtDbSubfolder = new TextBox();
            this.btnTestConnection = new Button();

            this.btnSave = new Button();
            this.btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindReplace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFileSizeMb)).BeginInit();

            // ----- Form -----
            this.Text = "تنظیمات پوشه";
            this.Size = new System.Drawing.Size(850, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // ----- Tabs -----
            this.tabMain.Dock = DockStyle.Fill;
            this.tabMain.TabPages.AddRange(new TabPage[] { this.tabGeneral, this.tabActions, this.tabTextProcessing, this.tabDatabase });
            this.tabGeneral.Text = "عمومی";
            this.tabActions.Text = "اکشن‌ها";
            this.tabTextProcessing.Text = "پردازش متن";
            this.tabDatabase.Text = "پایگاه‌داده";

            // ----- General Tab -----
            BuildGeneralTab();

            // ----- Actions Tab -----
            BuildActionsTab();

            // ----- Text Processing Tab -----
            BuildTextProcessingTab();

            // ----- Database Tab -----
            BuildDatabaseTab();

            // ----- Bottom buttons -----
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            this.btnSave.Text = "ذخیره";
            this.btnSave.Size = new Size(100, 32);
            this.btnSave.DialogResult = DialogResult.None;
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            bottomPanel.Controls.Add(this.btnSave);
            bottomPanel.Controls.Add(this.btnCancel);
            this.btnSave.Click += btnSave_Click;
            this.btnCancel.Click += btnCancel_Click;

            bottomPanel.Layout += (s, e) =>
            {
                int y = (bottomPanel.Height - 32) / 2;
                this.btnSave.Location = new Point(bottomPanel.Width - 110, y);
                this.btnCancel.Location = new Point(bottomPanel.Width - 220, y);
            };

            this.Controls.Add(this.tabMain);
            this.Controls.Add(bottomPanel);

            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindReplace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFileSizeMb)).EndInit();
        }

        private void BuildGeneralTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            int x1 = 400, x2 = 30, w = 350, h = 24, gap = 10;
            int y = 20;

            this.lblName.Text = "نام:";
            this.lblName.Location = new Point(x1, y);
            this.lblName.Size = new Size(100, h);
            this.txtName.Location = new Point(x1 - w - 5, y);
            this.txtName.Size = new Size(w, h);
            y += h + gap;

            this.lblSourcePath.Text = "مسیر منبع:";
            this.lblSourcePath.Location = new Point(x1, y);
            this.lblSourcePath.Size = new Size(100, h);
            this.txtSourcePath.Location = new Point(x1 - w - 5, y);
            this.txtSourcePath.Size = new Size(w - 35, h);
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.Location = new Point(x1 - w - 5 - 30, y);
            this.btnBrowseSource.Size = new Size(30, h);
            this.btnBrowseSource.Click += btnBrowseSource_Click;
            y += h + gap;

            this.chkIncludeSubfolders.Text = "شامل زیرپوشه‌ها";
            this.chkIncludeSubfolders.Location = new Point(x1 - w - 5, y);
            this.chkIncludeSubfolders.Size = new Size(w, h);
            y += h + gap;

            this.lblFileFilter.Text = "فیلتر فایل:";
            this.lblFileFilter.Location = new Point(x1, y);
            this.lblFileFilter.Size = new Size(100, h);
            this.txtFileFilter.Location = new Point(x1 - w - 5, y);
            this.txtFileFilter.Size = new Size(w, h);
            y += h + gap;

            this.lblMinSize.Text = "حداقل حجم (بایت):";
            this.lblMinSize.Location = new Point(x1, y);
            this.lblMinSize.Size = new Size(100, h);
            this.numMinSize.Location = new Point(x1 - w - 5, y);
            this.numMinSize.Size = new Size(150, h);
            this.numMinSize.Maximum = long.MaxValue;
            y += h + gap;

            this.lblMaxSize.Text = "حداکثر حجم (بایت):";
            this.lblMaxSize.Location = new Point(x1, y);
            this.lblMaxSize.Size = new Size(100, h);
            this.numMaxSize.Location = new Point(x1 - w - 5, y);
            this.numMaxSize.Size = new Size(150, h);
            this.numMaxSize.Maximum = long.MaxValue;
            y += h + gap;

            this.lblWatchMode.Text = "حالت نظارت:";
            this.lblWatchMode.Location = new Point(x1, y);
            this.lblWatchMode.Size = new Size(100, h);
            this.cmbWatchMode.Location = new Point(x1 - w - 5, y);
            this.cmbWatchMode.Size = new Size(150, h);
            this.cmbWatchMode.DropDownStyle = ComboBoxStyle.DropDownList;
            y += h + gap;

            this.lblInterval.Text = "فاصله (ثانیه):";
            this.lblInterval.Location = new Point(x1, y);
            this.lblInterval.Size = new Size(100, h);
            this.numIntervalSeconds.Location = new Point(x1 - w - 5, y);
            this.numIntervalSeconds.Size = new Size(150, h);
            this.numIntervalSeconds.Minimum = 1;
            this.numIntervalSeconds.Maximum = 86400;
            y += h + gap;

            this.chkEnabled.Text = "فعال";
            this.chkEnabled.Location = new Point(x1 - w - 5, y);
            this.chkEnabled.Size = new Size(120, h);
            y += h + gap;

            this.lblConflict.Text = "استراتژی تعارض:";
            this.lblConflict.Location = new Point(x1, y);
            this.lblConflict.Size = new Size(100, h);
            this.cmbConflict.Location = new Point(x1 - w - 5, y);
            this.cmbConflict.Size = new Size(150, h);
            this.cmbConflict.DropDownStyle = ComboBoxStyle.DropDownList;
            y += h + gap;

            this.lblDestination.Text = "مقصد:";
            this.lblDestination.Location = new Point(x1, y);
            this.lblDestination.Size = new Size(100, h);
            this.txtDestination.Location = new Point(x1 - w - 5, y);
            this.txtDestination.Size = new Size(w - 35, h);
            this.btnBrowseDest.Text = "...";
            this.btnBrowseDest.Location = new Point(x1 - w - 5 - 30, y);
            this.btnBrowseDest.Size = new Size(30, h);
            this.btnBrowseDest.Click += btnBrowseDest_Click;
            y += h + gap;

            this.lblSubfolder.Text = "الگوی زیرپوشه:";
            this.lblSubfolder.Location = new Point(x1, y);
            this.lblSubfolder.Size = new Size(100, h);
            this.txtSubfolderPattern.Location = new Point(x1 - w - 5, y);
            this.txtSubfolderPattern.Size = new Size(w, h);
            y += h + gap;

            this.lblFilenamePattern.Text = "الگوی نام فایل:";
            this.lblFilenamePattern.Location = new Point(x1, y);
            this.lblFilenamePattern.Size = new Size(100, h);
            this.txtFilenamePattern.Location = new Point(x1 - w - 5, y);
            this.txtFilenamePattern.Size = new Size(w, h);
            y += h + gap;

            this.chkEnableDedup.Text = "جلوگیری از پردازش فایل تکراری (MD5)";
            this.chkEnableDedup.Location = new Point(x1 - w - 5, y);
            this.chkEnableDedup.Size = new Size(w + 100, h);
            y += h + gap;

            this.btnVariables.Text = "راهنمای متغیرها";
            this.btnVariables.Location = new Point(x1 - w - 5, y);
            this.btnVariables.Size = new Size(150, h);
            this.btnVariables.Click += btnVariables_Click;

            panel.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblSourcePath, txtSourcePath, btnBrowseSource,
                chkIncludeSubfolders,
                lblFileFilter, txtFileFilter,
                lblMinSize, numMinSize,
                lblMaxSize, numMaxSize,
                lblWatchMode, cmbWatchMode,
                lblInterval, numIntervalSeconds,
                chkEnabled,
                lblConflict, cmbConflict,
                lblDestination, txtDestination, btnBrowseDest,
                lblSubfolder, txtSubfolderPattern,
                lblFilenamePattern, txtFilenamePattern,
                chkEnableDedup,
                btnVariables
            });

            this.tabGeneral.Controls.Add(panel);
        }

        private void BuildActionsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            this.lblActionsHint.Text = "اکشن‌ها به ترتیب اجرا می‌شوند (حداکثر ۵ اکنش). زنجیره اکشن‌ها به شما اجازه می‌دهد Copy → ZIP → Store و ... را پشت سر هم اجرا کنید.";
            this.lblActionsHint.Dock = DockStyle.Top;
            this.lblActionsHint.Height = 50;

            this.lstActions.Dock = DockStyle.Fill;

            var btnPanel = new Panel { Dock = DockStyle.Right, Width = 120, Padding = new Padding(5) };
            int y = 10;
            this.btnAddAction.Text = "افزودن";
            this.btnAddAction.Size = new Size(110, 32);
            this.btnAddAction.Location = new Point(5, y);
            this.btnAddAction.Click += btnAddAction_Click;
            y += 38;

            this.btnEditAction.Text = "ویرایش";
            this.btnEditAction.Size = new Size(110, 32);
            this.btnEditAction.Location = new Point(5, y);
            this.btnEditAction.Click += btnEditAction_Click;
            y += 38;

            this.btnRemoveAction.Text = "حذف";
            this.btnRemoveAction.Size = new Size(110, 32);
            this.btnRemoveAction.Location = new Point(5, y);
            this.btnRemoveAction.Click += btnRemoveAction_Click;
            y += 50;

            this.btnMoveUp.Text = "↑ بالا";
            this.btnMoveUp.Size = new Size(110, 32);
            this.btnMoveUp.Location = new Point(5, y);
            this.btnMoveUp.Click += btnMoveUp_Click;
            y += 38;

            this.btnMoveDown.Text = "↓ پایین";
            this.btnMoveDown.Size = new Size(110, 32);
            this.btnMoveDown.Location = new Point(5, y);
            this.btnMoveDown.Click += btnMoveDown_Click;

            btnPanel.Controls.AddRange(new Control[]
            {
                btnAddAction, btnEditAction, btnRemoveAction, btnMoveUp, btnMoveDown
            });

            panel.Controls.Add(this.lstActions);
            panel.Controls.Add(btnPanel);
            panel.Controls.Add(this.lblActionsHint);

            this.tabActions.Controls.Add(panel);
        }

        private void BuildTextProcessingTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            this.chkEnableTextProcessing.Text = "فعال‌سازی پردازش متن";
            this.chkEnableTextProcessing.Location = new Point(20, 10);
            this.chkEnableTextProcessing.Size = new Size(300, 24);

            this.lblExtensions.Text = "پسوندها:";
            this.lblExtensions.Location = new Point(20, 40);
            this.lblExtensions.Size = new Size(80, 24);
            this.txtExtensions.Location = new Point(110, 40);
            this.txtExtensions.Size = new Size(300, 24);

            this.lblEncoding.Text = "Encoding:";
            this.lblEncoding.Location = new Point(20, 70);
            this.lblEncoding.Size = new Size(80, 24);
            this.cmbEncoding.Location = new Point(110, 70);
            this.cmbEncoding.Size = new Size(150, 24);
            this.cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;

            this.chkBackup.Text = "ساخت فایل پشتیبان (.bak) قبل از تغییر";
            this.chkBackup.Location = new Point(20, 100);
            this.chkBackup.Size = new Size(400, 24);

            // Find & Replace
            this.grpFindReplace.Text = "Find & Replace";
            this.grpFindReplace.Location = new Point(20, 130);
            this.grpFindReplace.Size = new Size(760, 220);

            this.chkFR.Text = "فعال";
            this.chkFR.Location = new Point(10, 22);
            this.chkFR.Size = new Size(80, 24);

            this.dgvFindReplace.Location = new Point(10, 50);
            this.dgvFindReplace.Size = new Size(740, 155);
            this.dgvFindReplace.AllowUserToAddRows = true;
            this.dgvFindReplace.AllowUserToDeleteRows = true;
            this.dgvFindReplace.RowHeadersVisible = false;
            this.dgvFindReplace.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFindReplace.Columns.Add("find", "Find");
            this.dgvFindReplace.Columns.Add("replace", "Replace");
            this.dgvFindReplace.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Regex", Name = "regex" });
            this.dgvFindReplace.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "حساس به حروف", Name = "case" });

            this.grpFindReplace.Controls.Add(this.chkFR);
            this.grpFindReplace.Controls.Add(this.dgvFindReplace);

            // Header / Footer
            this.grpHeaderFooter.Text = "Header / Footer";
            this.grpHeaderFooter.Location = new Point(20, 360);
            this.grpHeaderFooter.Size = new Size(760, 130);

            this.chkHeader.Text = "Header";
            this.chkHeader.Location = new Point(10, 22);
            this.chkHeader.Size = new Size(80, 24);
            this.txtHeader.Location = new Point(100, 22);
            this.txtHeader.Size = new Size(640, 24);

            this.chkFooter.Text = "Footer";
            this.chkFooter.Location = new Point(10, 52);
            this.chkFooter.Size = new Size(80, 24);
            this.txtFooter.Location = new Point(100, 52);
            this.txtFooter.Size = new Size(640, 24);

            this.grpHeaderFooter.Controls.Add(this.chkHeader);
            this.grpHeaderFooter.Controls.Add(this.txtHeader);
            this.grpHeaderFooter.Controls.Add(this.chkFooter);
            this.grpHeaderFooter.Controls.Add(this.txtFooter);

            // Append / Prepend
            this.grpAppendPrepend.Text = "Append / Prepend";
            this.grpAppendPrepend.Location = new Point(20, 500);
            this.grpAppendPrepend.Size = new Size(760, 130);

            this.chkAppend.Text = "Append";
            this.chkAppend.Location = new Point(10, 22);
            this.chkAppend.Size = new Size(80, 24);
            this.txtAppend.Location = new Point(100, 22);
            this.txtAppend.Size = new Size(640, 24);

            this.chkPrepend.Text = "Prepend";
            this.chkPrepend.Location = new Point(10, 52);
            this.chkPrepend.Size = new Size(80, 24);
            this.txtPrepend.Location = new Point(100, 52);
            this.txtPrepend.Size = new Size(640, 24);

            this.grpAppendPrepend.Controls.Add(this.chkAppend);
            this.grpAppendPrepend.Controls.Add(this.txtAppend);
            this.grpAppendPrepend.Controls.Add(this.chkPrepend);
            this.grpAppendPrepend.Controls.Add(this.txtPrepend);

            panel.Controls.AddRange(new Control[]
            {
                chkEnableTextProcessing,
                lblExtensions, txtExtensions,
                lblEncoding, cmbEncoding,
                chkBackup,
                grpFindReplace,
                grpHeaderFooter,
                grpAppendPrepend
            });

            var scroll = new VScrollBar { Dock = DockStyle.Right };
            panel.Controls.Add(scroll);

            this.tabTextProcessing.Controls.Add(panel);
        }

        private void BuildDatabaseTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            int x1 = 200, w = 550, h = 24, gap = 10;
            int y = 15;

            this.chkEnableDb.Text = "فعال‌سازی ذخیره در دیتابیس ریموت";
            this.chkEnableDb.Location = new Point(20, y);
            this.chkEnableDb.Size = new Size(300, h);
            y += h + gap + 5;

            this.lblConnString.Text = "Connection String:";
            this.lblConnString.Location = new Point(20, y);
            this.lblConnString.Size = new Size(140, h);
            this.txtConnString.Location = new Point(170, y);
            this.txtConnString.Size = new Size(400, h);
            y += h + gap;

            this.lblTableName.Text = "نام جدول:";
            this.lblTableName.Location = new Point(20, y);
            this.lblTableName.Size = new Size(140, h);
            this.txtTableName.Location = new Point(170, y);
            this.txtTableName.Size = new Size(300, h);
            y += h + gap;

            this.lblDbMode.Text = "روش ذخیره:";
            this.lblDbMode.Location = new Point(20, y);
            this.lblDbMode.Size = new Size(140, h);
            this.cmbDbMode.Location = new Point(170, y);
            this.cmbDbMode.Size = new Size(300, h);
            this.cmbDbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            y += h + gap;

            this.lblFileShare.Text = "مسیر اشتراک فایل:";
            this.lblFileShare.Location = new Point(20, y);
            this.lblFileShare.Size = new Size(140, h);
            this.txtFileShare.Location = new Point(170, y);
            this.txtFileShare.Size = new Size(365, h);
            this.btnBrowseShare.Text = "...";
            this.btnBrowseShare.Location = new Point(540, y);
            this.btnBrowseShare.Size = new Size(30, h);
            this.btnBrowseShare.Click += btnBrowseShare_Click;
            y += h + gap;

            this.lblMaxFileSizeMb.Text = "حداکثر حجم (MB):";
            this.lblMaxFileSizeMb.Location = new Point(20, y);
            this.lblMaxFileSizeMb.Size = new Size(140, h);
            this.numMaxFileSizeMb.Location = new Point(170, y);
            this.numMaxFileSizeMb.Size = new Size(100, h);
            this.numMaxFileSizeMb.Minimum = 1;
            this.numMaxFileSizeMb.Maximum = 10240;
            y += h + gap;

            this.chkSkipLarger.Text = "رد کردن فایل‌های بزرگ‌تر از حداکثر";
            this.chkSkipLarger.Location = new Point(20, y);
            this.chkSkipLarger.Size = new Size(400, h);
            y += h + gap;

            this.chkCompress.Text = "فشرده‌سازی قبل از ذخیره (GZIP)";
            this.chkCompress.Location = new Point(20, y);
            this.chkCompress.Size = new Size(400, h);
            y += h + gap;

            this.lblDbSubfolder.Text = "الگوی زیرپوشه:";
            this.lblDbSubfolder.Location = new Point(20, y);
            this.lblDbSubfolder.Size = new Size(140, h);
            this.txtDbSubfolder.Location = new Point(170, y);
            this.txtDbSubfolder.Size = new Size(300, h);
            y += h + gap + 10;

            this.btnTestConnection.Text = "تست اتصال و ساخت جدول";
            this.btnTestConnection.Location = new Point(170, y);
            this.btnTestConnection.Size = new Size(200, 32);
            this.btnTestConnection.Click += btnTestConnection_Click;

            panel.Controls.AddRange(new Control[]
            {
                chkEnableDb,
                lblConnString, txtConnString,
                lblTableName, txtTableName,
                lblDbMode, cmbDbMode,
                lblFileShare, txtFileShare, btnBrowseShare,
                lblMaxFileSizeMb, numMaxFileSizeMb,
                chkSkipLarger,
                chkCompress,
                lblDbSubfolder, txtDbSubfolder,
                btnTestConnection
            });

            this.tabDatabase.Controls.Add(panel);
        }
    }

    // Helper class for the actions editor dialog
    public class ActionEditorForm : Form
    {
        private readonly ActionConfig _action;

        public ActionEditorForm(ActionConfig action)
        {
            _action = action;
            InitializeComponent();
            LoadData();
        }

        private ComboBox cmbType;
        private TextBox txtName;
        private TextBox txtDestPath;
        private TextBox txtFilename;
        private TextBox txtCommandExe;
        private TextBox txtCommandArgs;
        private TextBox txtWorkDir;
        private CheckBox chkWaitForExit;
        private NumericUpDown numTimeout;
        private NumericUpDown numRetry;
        private NumericUpDown numRetryDelay;
        private CheckBox chkContinueOnFail;
        private CheckBox chkEnabled;
        private Button btnOK;
        private Button btnCancel;
        private GroupBox grpCommon;
        private GroupBox grpCommand;
        private GroupBox grpAdvanced;

        private void InitializeComponent()
        {
            this.Text = "تنظیمات اکشن";
            this.Size = new System.Drawing.Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            cmbType = new ComboBox();
            txtName = new TextBox();
            txtDestPath = new TextBox();
            txtFilename = new TextBox();
            txtCommandExe = new TextBox();
            txtCommandArgs = new TextBox();
            txtWorkDir = new TextBox();
            chkWaitForExit = new CheckBox();
            numTimeout = new NumericUpDown();
            numRetry = new NumericUpDown();
            numRetryDelay = new NumericUpDown();
            chkContinueOnFail = new CheckBox();
            chkEnabled = new CheckBox();
            btnOK = new Button();
            btnCancel = new Button();
            grpCommon = new GroupBox();
            grpCommand = new GroupBox();
            grpAdvanced = new GroupBox();

            int y = 15;
            var lblType = new Label { Text = "نوع اکشن:", Location = new Point(20, y), Size = new Size(100, 24) };
            cmbType.Location = new Point(130, y);
            cmbType.Size = new Size(200, 24);
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.AddRange(System.Enum.GetNames(typeof(ActionType)));
            y += 35;

            var lblName = new Label { Text = "نام:", Location = new Point(20, y), Size = new Size(100, 24) };
            txtName.Location = new Point(130, y);
            txtName.Size = new Size(300, 24);
            y += 35;

            grpCommon.Text = "پارامترهای عمومی";
            grpCommon.Location = new Point(20, y);
            grpCommon.Size = new Size(640, 110);
            grpCommon.Controls.Add(new Label { Text = "مسیر مقصد:", Location = new Point(10, 25), Size = new Size(100, 24) });
            grpCommon.Controls.Add(new Label { Text = "الگوی نام فایل:", Location = new Point(10, 55), Size = new Size(100, 24) });
            txtDestPath.Location = new Point(120, 25);
            txtDestPath.Size = new Size(500, 24);
            txtDestPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilename.Location = new Point(120, 55);
            txtFilename.Size = new Size(500, 24);
            txtFilename.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpCommon.Controls.Add(txtDestPath);
            grpCommon.Controls.Add(txtFilename);
            y += 120;

            grpCommand.Text = "پارامترهای Command سفارشی";
            grpCommand.Location = new Point(20, y);
            grpCommand.Size = new Size(640, 160);
            grpCommand.Controls.Add(new Label { Text = "Executable:", Location = new Point(10, 25), Size = new Size(100, 24) });
            grpCommand.Controls.Add(new Label { Text = "Arguments:", Location = new Point(10, 55), Size = new Size(100, 24) });
            grpCommand.Controls.Add(new Label { Text = "Working Dir:", Location = new Point(10, 85), Size = new Size(100, 24) });
            grpCommand.Controls.Add(new Label { Text = "Timeout (s):", Location = new Point(10, 115), Size = new Size(100, 24) });
            txtCommandExe.Location = new Point(120, 25);
            txtCommandExe.Size = new Size(500, 24);
            txtCommandArgs.Location = new Point(120, 55);
            txtCommandArgs.Size = new Size(500, 24);
            txtWorkDir.Location = new Point(120, 85);
            txtWorkDir.Size = new Size(500, 24);
            numTimeout.Location = new Point(120, 115);
            numTimeout.Size = new Size(80, 24);
            chkWaitForExit.Text = "Wait for exit";
            chkWaitForExit.Location = new Point(220, 115);
            chkWaitForExit.Size = new Size(120, 24);
            grpCommand.Controls.Add(txtCommandExe);
            grpCommand.Controls.Add(txtCommandArgs);
            grpCommand.Controls.Add(txtWorkDir);
            grpCommand.Controls.Add(numTimeout);
            grpCommand.Controls.Add(chkWaitForExit);
            y += 170;

            grpAdvanced.Text = "تنظیمات پیشرفته";
            grpAdvanced.Location = new Point(20, y);
            grpAdvanced.Size = new Size(640, 110);
            grpAdvanced.Controls.Add(new Label { Text = "Retry:", Location = new Point(10, 25), Size = new Size(80, 24) });
            grpAdvanced.Controls.Add(new Label { Text = "Retry Delay (ms):", Location = new Point(220, 25), Size = new Size(120, 24) });
            numRetry.Location = new Point(100, 25);
            numRetry.Size = new Size(80, 24);
            numRetryDelay.Location = new Point(350, 25);
            numRetryDelay.Size = new Size(80, 24);
            chkContinueOnFail.Text = "ادامه زنجیره در صورت خطا";
            chkContinueOnFail.Location = new Point(10, 60);
            chkContinueOnFail.Size = new Size(200, 24);
            chkEnabled.Text = "فعال";
            chkEnabled.Location = new Point(220, 60);
            chkEnabled.Size = new Size(100, 24);
            grpAdvanced.Controls.Add(numRetry);
            grpAdvanced.Controls.Add(numRetryDelay);
            grpAdvanced.Controls.Add(chkContinueOnFail);
            grpAdvanced.Controls.Add(chkEnabled);
            y += 120;

            btnOK.Text = "تأیید";
            btnOK.Size = new Size(100, 32);
            btnOK.Location = new Point(440, y);
            btnOK.Click += (s, e) => { SaveData(); this.DialogResult = DialogResult.OK; this.Close(); };

            btnCancel.Text = "انصراف";
            btnCancel.Size = new Size(100, 32);
            btnCancel.Location = new Point(550, y);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblType, cmbType,
                lblName, txtName,
                grpCommon, grpCommand, grpAdvanced,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadData()
        {
            cmbType.SelectedItem = _action.Type.ToString();
            txtName.Text = _action.Name;
            txtDestPath.Text = _action.DestinationPath;
            txtFilename.Text = _action.FilenamePattern;
            txtCommandExe.Text = _action.CommandExecutable;
            txtCommandArgs.Text = _action.CommandArguments;
            txtWorkDir.Text = _action.WorkingDirectory;
            chkWaitForExit.Checked = _action.WaitForExit;
            numTimeout.Value = Math.Max(0, _action.TimeoutSeconds);
            numRetry.Value = Math.Max(0, _action.RetryCount);
            numRetryDelay.Value = Math.Max(0, _action.RetryDelayMs);
            chkContinueOnFail.Checked = _action.ContinueOnFailure;
            chkEnabled.Checked = _action.Enabled;
        }

        private void SaveData()
        {
            if (Enum.TryParse<ActionType>(cmbType.SelectedItem?.ToString(), out var t))
                _action.Type = t;
            _action.Name = txtName.Text;
            _action.DestinationPath = txtDestPath.Text;
            _action.FilenamePattern = txtFilename.Text;
            _action.CommandExecutable = txtCommandExe.Text;
            _action.CommandArguments = txtCommandArgs.Text;
            _action.WorkingDirectory = txtWorkDir.Text;
            _action.WaitForExit = chkWaitForExit.Checked;
            _action.TimeoutSeconds = (int)numTimeout.Value;
            _action.RetryCount = (int)numRetry.Value;
            _action.RetryDelayMs = (int)numRetryDelay.Value;
            _action.ContinueOnFailure = chkContinueOnFail.Checked;
            _action.Enabled = chkEnabled.Checked;
        }
    }
}
