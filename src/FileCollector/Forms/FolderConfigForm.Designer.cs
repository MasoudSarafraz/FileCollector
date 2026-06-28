using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;

namespace FileCollector.Forms
{
    partial class FolderConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        // Tab control
        private TabControl tabMain;
        private TabPage tabGeneral;
        private TabPage tabActions;
        private TabPage tabTextProcessing;
        private TabPage tabDatabase;

        // General tab controls
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
        private Panel pnlGeneral;

        // Actions tab controls
        private ListBox lstActions;
        private Button btnAddAction;
        private Button btnEditAction;
        private Button btnRemoveAction;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Label lblActionsHint;
        private Panel pnlActions;
        private Panel pnlActionsButtons;

        // Text processing tab controls
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
        private Panel pnlTextProcessing;

        // Database tab controls
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
        private Panel pnlDatabase;

        // Bottom buttons
        private Button btnSave;
        private Button btnCancel;
        private Panel pnlBottom;

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
            this.pnlGeneral = new Panel();

            this.lstActions = new ListBox();
            this.btnAddAction = new Button();
            this.btnEditAction = new Button();
            this.btnRemoveAction = new Button();
            this.btnMoveUp = new Button();
            this.btnMoveDown = new Button();
            this.lblActionsHint = new Label();
            this.pnlActions = new Panel();
            this.pnlActionsButtons = new Panel();

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
            this.pnlTextProcessing = new Panel();

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
            this.pnlDatabase = new Panel();

            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.pnlBottom = new Panel();

            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindReplace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFileSizeMb)).BeginInit();

            // ---------- Form ----------
            this.Text = "تنظیمات پوشه";
            this.Size = new Size(850, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9.75F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ---------- Tab control ----------
            this.tabMain.Dock = DockStyle.Fill;
            this.tabMain.Font = new Font("Tahoma", 10F, FontStyle.Bold);

            this.tabGeneral.Text = "عمومی";
            this.tabActions.Text = "اکشن‌ها";
            this.tabTextProcessing.Text = "پردازش متن";
            this.tabDatabase.Text = "پایگاه‌داده";
            this.tabMain.TabPages.Add(this.tabGeneral);
            this.tabMain.TabPages.Add(this.tabActions);
            this.tabMain.TabPages.Add(this.tabTextProcessing);
            this.tabMain.TabPages.Add(this.tabDatabase);

            BuildGeneralTab();
            BuildActionsTab();
            BuildTextProcessingTab();
            BuildDatabaseTab();

            // ---------- Bottom panel ----------
            this.pnlBottom.Dock = DockStyle.Bottom;
            this.pnlBottom.Height = 50;
            this.pnlBottom.BackColor = Color.White;

            this.btnSave.Text = "ذخیره";
            this.btnSave.Size = new Size(100, 32);
            this.btnSave.BackColor = Color.White;
            this.btnSave.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 1;
            this.btnSave.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.DialogResult = DialogResult.Cancel;

            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Resize += PnlBottom_Resize;

            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlBottom);

            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindReplace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFileSizeMb)).EndInit();
        }

        // The following Build* methods are NOT designer-parsed (they run at runtime)
        // so they can use any logic. Designer only parses InitializeComponent.

        private void PnlBottom_Resize(object sender, EventArgs e)
        {
            int y = (this.pnlBottom.Height - 32) / 2;
            this.btnSave.Location = new Point(this.pnlBottom.Width - 230, y);
            this.btnCancel.Location = new Point(this.pnlBottom.Width - 120, y);
        }

        private void BuildGeneralTab()
        {
            this.pnlGeneral.Dock = DockStyle.Fill;
            this.pnlGeneral.BackColor = Color.White;
            this.pnlGeneral.AutoScroll = true;

            int x1 = 480;
            int w = 320;
            int h = 26;
            int gap = 10;
            int y = 15;

            this.lblName.Text = "نام:";
            this.lblName.Location = new Point(x1, y);
            this.lblName.Size = new Size(140, h);
            this.lblName.TextAlign = ContentAlignment.MiddleRight;

            this.txtName.Location = new Point(x1 - w - 5, y);
            this.txtName.Size = new Size(w, h);
            y += h + gap;

            this.lblSourcePath.Text = "مسیر منبع:";
            this.lblSourcePath.Location = new Point(x1, y);
            this.lblSourcePath.Size = new Size(140, h);
            this.lblSourcePath.TextAlign = ContentAlignment.MiddleRight;

            this.txtSourcePath.Location = new Point(x1 - w - 5, y);
            this.txtSourcePath.Size = new Size(w - 35, h);

            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.Location = new Point(x1 - w - 5 - 30, y);
            this.btnBrowseSource.Size = new Size(30, h);
            y += h + gap;

            this.chkIncludeSubfolders.Text = "شامل زیرپوشه‌ها";
            this.chkIncludeSubfolders.Location = new Point(x1 - w - 5, y);
            this.chkIncludeSubfolders.Size = new Size(w, h);
            y += h + gap;

            this.lblFileFilter.Text = "فیلتر فایل:";
            this.lblFileFilter.Location = new Point(x1, y);
            this.lblFileFilter.Size = new Size(140, h);
            this.lblFileFilter.TextAlign = ContentAlignment.MiddleRight;

            this.txtFileFilter.Location = new Point(x1 - w - 5, y);
            this.txtFileFilter.Size = new Size(w, h);
            y += h + gap;

            this.lblMinSize.Text = "حداقل حجم (بایت):";
            this.lblMinSize.Location = new Point(x1, y);
            this.lblMinSize.Size = new Size(140, h);
            this.lblMinSize.TextAlign = ContentAlignment.MiddleRight;

            this.numMinSize.Location = new Point(x1 - w - 5, y);
            this.numMinSize.Size = new Size(150, h);
            this.numMinSize.Maximum = long.MaxValue;
            y += h + gap;

            this.lblMaxSize.Text = "حداکثر حجم (بایت):";
            this.lblMaxSize.Location = new Point(x1, y);
            this.lblMaxSize.Size = new Size(140, h);
            this.lblMaxSize.TextAlign = ContentAlignment.MiddleRight;

            this.numMaxSize.Location = new Point(x1 - w - 5, y);
            this.numMaxSize.Size = new Size(150, h);
            this.numMaxSize.Maximum = long.MaxValue;
            y += h + gap;

            this.lblWatchMode.Text = "حالت نظارت:";
            this.lblWatchMode.Location = new Point(x1, y);
            this.lblWatchMode.Size = new Size(140, h);
            this.lblWatchMode.TextAlign = ContentAlignment.MiddleRight;

            this.cmbWatchMode.Location = new Point(x1 - w - 5, y);
            this.cmbWatchMode.Size = new Size(150, h);
            this.cmbWatchMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbWatchMode.Items.AddRange(new object[] { "realtime", "interval", "scheduled" });
            y += h + gap;

            this.lblInterval.Text = "فاصله (ثانیه):";
            this.lblInterval.Location = new Point(x1, y);
            this.lblInterval.Size = new Size(140, h);
            this.lblInterval.TextAlign = ContentAlignment.MiddleRight;

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
            this.lblConflict.Size = new Size(140, h);
            this.lblConflict.TextAlign = ContentAlignment.MiddleRight;

            this.cmbConflict.Location = new Point(x1 - w - 5, y);
            this.cmbConflict.Size = new Size(150, h);
            this.cmbConflict.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbConflict.Items.AddRange(new object[] { "overwrite", "skip", "rename", "keepboth" });
            y += h + gap;

            this.lblDestination.Text = "مقصد:";
            this.lblDestination.Location = new Point(x1, y);
            this.lblDestination.Size = new Size(140, h);
            this.lblDestination.TextAlign = ContentAlignment.MiddleRight;

            this.txtDestination.Location = new Point(x1 - w - 5, y);
            this.txtDestination.Size = new Size(w - 35, h);

            this.btnBrowseDest.Text = "...";
            this.btnBrowseDest.Location = new Point(x1 - w - 5 - 30, y);
            this.btnBrowseDest.Size = new Size(30, h);
            y += h + gap;

            this.lblSubfolder.Text = "الگوی زیرپوشه:";
            this.lblSubfolder.Location = new Point(x1, y);
            this.lblSubfolder.Size = new Size(140, h);
            this.lblSubfolder.TextAlign = ContentAlignment.MiddleRight;

            this.txtSubfolderPattern.Location = new Point(x1 - w - 5, y);
            this.txtSubfolderPattern.Size = new Size(w, h);
            y += h + gap;

            this.lblFilenamePattern.Text = "الگوی نام فایل:";
            this.lblFilenamePattern.Location = new Point(x1, y);
            this.lblFilenamePattern.Size = new Size(140, h);
            this.lblFilenamePattern.TextAlign = ContentAlignment.MiddleRight;

            this.txtFilenamePattern.Location = new Point(x1 - w - 5, y);
            this.txtFilenamePattern.Size = new Size(w, h);
            y += h + gap;

            this.chkEnableDedup.Text = "جلوگیری از پردازش فایل تکراری (MD5)";
            this.chkEnableDedup.Location = new Point(x1 - w - 5, y);
            this.chkEnableDedup.Size = new Size(w + 140, h);
            y += h + gap + 5;

            this.btnVariables.Text = "راهنمای متغیرها";
            this.btnVariables.Location = new Point(x1 - w - 5, y);
            this.btnVariables.Size = new Size(150, h);

            this.pnlGeneral.Controls.Add(this.lblName);
            this.pnlGeneral.Controls.Add(this.txtName);
            this.pnlGeneral.Controls.Add(this.lblSourcePath);
            this.pnlGeneral.Controls.Add(this.txtSourcePath);
            this.pnlGeneral.Controls.Add(this.btnBrowseSource);
            this.pnlGeneral.Controls.Add(this.chkIncludeSubfolders);
            this.pnlGeneral.Controls.Add(this.lblFileFilter);
            this.pnlGeneral.Controls.Add(this.txtFileFilter);
            this.pnlGeneral.Controls.Add(this.lblMinSize);
            this.pnlGeneral.Controls.Add(this.numMinSize);
            this.pnlGeneral.Controls.Add(this.lblMaxSize);
            this.pnlGeneral.Controls.Add(this.numMaxSize);
            this.pnlGeneral.Controls.Add(this.lblWatchMode);
            this.pnlGeneral.Controls.Add(this.cmbWatchMode);
            this.pnlGeneral.Controls.Add(this.lblInterval);
            this.pnlGeneral.Controls.Add(this.numIntervalSeconds);
            this.pnlGeneral.Controls.Add(this.chkEnabled);
            this.pnlGeneral.Controls.Add(this.lblConflict);
            this.pnlGeneral.Controls.Add(this.cmbConflict);
            this.pnlGeneral.Controls.Add(this.lblDestination);
            this.pnlGeneral.Controls.Add(this.txtDestination);
            this.pnlGeneral.Controls.Add(this.btnBrowseDest);
            this.pnlGeneral.Controls.Add(this.lblSubfolder);
            this.pnlGeneral.Controls.Add(this.txtSubfolderPattern);
            this.pnlGeneral.Controls.Add(this.lblFilenamePattern);
            this.pnlGeneral.Controls.Add(this.txtFilenamePattern);
            this.pnlGeneral.Controls.Add(this.chkEnableDedup);
            this.pnlGeneral.Controls.Add(this.btnVariables);

            this.tabGeneral.Controls.Add(this.pnlGeneral);
        }

        private void BuildActionsTab()
        {
            this.pnlActions.Dock = DockStyle.Fill;
            this.pnlActions.Padding = new Padding(15);
            this.pnlActions.BackColor = Color.White;

            this.lblActionsHint.Text = "اکشن‌ها به ترتیب اجرا می‌شوند (حداکثر ۵ اکشن). زنجیره اکشن‌ها به شما اجازه می‌دهد Copy → ZIP → Store و ... را پشت سر هم اجرا کنید.";
            this.lblActionsHint.Dock = DockStyle.Top;
            this.lblActionsHint.Height = 50;
            this.lblActionsHint.Font = new Font("Tahoma", 9F);
            this.lblActionsHint.ForeColor = Color.FromArgb(80, 80, 80);

            this.lstActions.Dock = DockStyle.Fill;
            this.lstActions.Font = new Font("Tahoma", 10F);
            this.lstActions.BorderStyle = BorderStyle.FixedSingle;

            this.pnlActionsButtons.Dock = DockStyle.Right;
            this.pnlActionsButtons.Width = 130;
            this.pnlActionsButtons.Padding = new Padding(5);

            int y = 10;
            int btnW = 115;

            this.btnAddAction.Text = "افزودن";
            this.btnAddAction.Size = new Size(btnW, 32);
            this.btnAddAction.Location = new Point(5, y);
            this.btnAddAction.BackColor = Color.White;
            this.btnAddAction.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnAddAction.FlatStyle = FlatStyle.Flat;
            this.btnAddAction.FlatAppearance.BorderSize = 1;
            this.btnAddAction.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            y += 38;

            this.btnEditAction.Text = "ویرایش";
            this.btnEditAction.Size = new Size(btnW, 32);
            this.btnEditAction.Location = new Point(5, y);
            this.btnEditAction.BackColor = Color.White;
            this.btnEditAction.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnEditAction.FlatStyle = FlatStyle.Flat;
            this.btnEditAction.FlatAppearance.BorderSize = 1;
            this.btnEditAction.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            y += 38;

            this.btnRemoveAction.Text = "حذف";
            this.btnRemoveAction.Size = new Size(btnW, 32);
            this.btnRemoveAction.Location = new Point(5, y);
            this.btnRemoveAction.BackColor = Color.White;
            this.btnRemoveAction.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnRemoveAction.FlatStyle = FlatStyle.Flat;
            this.btnRemoveAction.FlatAppearance.BorderSize = 1;
            this.btnRemoveAction.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            y += 50;

            this.btnMoveUp.Text = "↑ بالا";
            this.btnMoveUp.Size = new Size(btnW, 32);
            this.btnMoveUp.Location = new Point(5, y);
            this.btnMoveUp.BackColor = Color.White;
            this.btnMoveUp.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnMoveUp.FlatStyle = FlatStyle.Flat;
            this.btnMoveUp.FlatAppearance.BorderSize = 1;
            this.btnMoveUp.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            y += 38;

            this.btnMoveDown.Text = "↓ پایین";
            this.btnMoveDown.Size = new Size(btnW, 32);
            this.btnMoveDown.Location = new Point(5, y);
            this.btnMoveDown.BackColor = Color.White;
            this.btnMoveDown.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnMoveDown.FlatStyle = FlatStyle.Flat;
            this.btnMoveDown.FlatAppearance.BorderSize = 1;
            this.btnMoveDown.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);

            this.pnlActionsButtons.Controls.Add(this.btnAddAction);
            this.pnlActionsButtons.Controls.Add(this.btnEditAction);
            this.pnlActionsButtons.Controls.Add(this.btnRemoveAction);
            this.pnlActionsButtons.Controls.Add(this.btnMoveUp);
            this.pnlActionsButtons.Controls.Add(this.btnMoveDown);

            this.pnlActions.Controls.Add(this.lstActions);
            this.pnlActions.Controls.Add(this.pnlActionsButtons);
            this.pnlActions.Controls.Add(this.lblActionsHint);

            this.tabActions.Controls.Add(this.pnlActions);
        }

        private void BuildTextProcessingTab()
        {
            this.pnlTextProcessing.Dock = DockStyle.Fill;
            this.pnlTextProcessing.BackColor = Color.White;
            this.pnlTextProcessing.AutoScroll = true;

            this.chkEnableTextProcessing.Text = "فعال‌سازی پردازش متن";
            this.chkEnableTextProcessing.Location = new Point(20, 10);
            this.chkEnableTextProcessing.Size = new Size(300, 26);
            this.chkEnableTextProcessing.Font = new Font("Tahoma", 10F, FontStyle.Bold);

            this.lblExtensions.Text = "پسوندها:";
            this.lblExtensions.Location = new Point(20, 45);
            this.lblExtensions.Size = new Size(80, 24);
            this.lblExtensions.TextAlign = ContentAlignment.MiddleRight;

            this.txtExtensions.Location = new Point(110, 45);
            this.txtExtensions.Size = new Size(300, 24);

            this.lblEncoding.Text = "Encoding:";
            this.lblEncoding.Location = new Point(20, 75);
            this.lblEncoding.Size = new Size(80, 24);
            this.lblEncoding.TextAlign = ContentAlignment.MiddleRight;

            this.cmbEncoding.Location = new Point(110, 75);
            this.cmbEncoding.Size = new Size(150, 24);
            this.cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbEncoding.Items.AddRange(new object[] { "utf-8", "utf-8-bom", "utf-16", "utf-16-be", "ascii", "windows-1256" });

            this.chkBackup.Text = "ساخت فایل پشتیبان (.bak) قبل از تغییر";
            this.chkBackup.Location = new Point(20, 105);
            this.chkBackup.Size = new Size(400, 24);

            // Find & Replace
            this.grpFindReplace.Text = "Find & Replace";
            this.grpFindReplace.Location = new Point(20, 140);
            this.grpFindReplace.Size = new Size(760, 220);
            this.grpFindReplace.Font = new Font("Tahoma", 10F, FontStyle.Bold);

            this.chkFR.Text = "فعال";
            this.chkFR.Location = new Point(10, 25);
            this.chkFR.Size = new Size(80, 24);
            this.chkFR.Font = new Font("Tahoma", 9.75F);

            this.dgvFindReplace.Location = new Point(10, 55);
            this.dgvFindReplace.Size = new Size(740, 150);
            this.dgvFindReplace.AllowUserToAddRows = true;
            this.dgvFindReplace.AllowUserToDeleteRows = true;
            this.dgvFindReplace.RowHeadersVisible = false;
            this.dgvFindReplace.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFindReplace.BackgroundColor = Color.White;
            this.dgvFindReplace.BorderStyle = BorderStyle.FixedSingle;
            this.dgvFindReplace.EnableHeadersVisualStyles = false;
            this.dgvFindReplace.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 242);
            this.dgvFindReplace.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 51, 51);
            this.dgvFindReplace.Columns.Add("find", "Find");
            this.dgvFindReplace.Columns.Add("replace", "Replace");
            this.dgvFindReplace.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Regex", Name = "regex" });
            this.dgvFindReplace.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "حساس به حروف", Name = "case" });

            this.grpFindReplace.Controls.Add(this.chkFR);
            this.grpFindReplace.Controls.Add(this.dgvFindReplace);

            // Header / Footer
            this.grpHeaderFooter.Text = "Header / Footer";
            this.grpHeaderFooter.Location = new Point(20, 370);
            this.grpHeaderFooter.Size = new Size(760, 110);
            this.grpHeaderFooter.Font = new Font("Tahoma", 10F, FontStyle.Bold);

            this.chkHeader.Text = "Header";
            this.chkHeader.Location = new Point(10, 25);
            this.chkHeader.Size = new Size(80, 24);
            this.chkHeader.Font = new Font("Tahoma", 9.75F);

            this.txtHeader.Location = new Point(100, 25);
            this.txtHeader.Size = new Size(640, 24);
            this.txtHeader.Font = new Font("Tahoma", 9.75F);

            this.chkFooter.Text = "Footer";
            this.chkFooter.Location = new Point(10, 60);
            this.chkFooter.Size = new Size(80, 24);
            this.chkFooter.Font = new Font("Tahoma", 9.75F);

            this.txtFooter.Location = new Point(100, 60);
            this.txtFooter.Size = new Size(640, 24);
            this.txtFooter.Font = new Font("Tahoma", 9.75F);

            this.grpHeaderFooter.Controls.Add(this.chkHeader);
            this.grpHeaderFooter.Controls.Add(this.txtHeader);
            this.grpHeaderFooter.Controls.Add(this.chkFooter);
            this.grpHeaderFooter.Controls.Add(this.txtFooter);

            // Append / Prepend
            this.grpAppendPrepend.Text = "Append / Prepend";
            this.grpAppendPrepend.Location = new Point(20, 490);
            this.grpAppendPrepend.Size = new Size(760, 110);
            this.grpAppendPrepend.Font = new Font("Tahoma", 10F, FontStyle.Bold);

            this.chkAppend.Text = "Append";
            this.chkAppend.Location = new Point(10, 25);
            this.chkAppend.Size = new Size(80, 24);
            this.chkAppend.Font = new Font("Tahoma", 9.75F);

            this.txtAppend.Location = new Point(100, 25);
            this.txtAppend.Size = new Size(640, 24);
            this.txtAppend.Font = new Font("Tahoma", 9.75F);

            this.chkPrepend.Text = "Prepend";
            this.chkPrepend.Location = new Point(10, 60);
            this.chkPrepend.Size = new Size(80, 24);
            this.chkPrepend.Font = new Font("Tahoma", 9.75F);

            this.txtPrepend.Location = new Point(100, 60);
            this.txtPrepend.Size = new Size(640, 24);
            this.txtPrepend.Font = new Font("Tahoma", 9.75F);

            this.grpAppendPrepend.Controls.Add(this.chkAppend);
            this.grpAppendPrepend.Controls.Add(this.txtAppend);
            this.grpAppendPrepend.Controls.Add(this.chkPrepend);
            this.grpAppendPrepend.Controls.Add(this.txtPrepend);

            this.pnlTextProcessing.Controls.Add(this.chkEnableTextProcessing);
            this.pnlTextProcessing.Controls.Add(this.lblExtensions);
            this.pnlTextProcessing.Controls.Add(this.txtExtensions);
            this.pnlTextProcessing.Controls.Add(this.lblEncoding);
            this.pnlTextProcessing.Controls.Add(this.cmbEncoding);
            this.pnlTextProcessing.Controls.Add(this.chkBackup);
            this.pnlTextProcessing.Controls.Add(this.grpFindReplace);
            this.pnlTextProcessing.Controls.Add(this.grpHeaderFooter);
            this.pnlTextProcessing.Controls.Add(this.grpAppendPrepend);

            this.tabTextProcessing.Controls.Add(this.pnlTextProcessing);
        }

        private void BuildDatabaseTab()
        {
            this.pnlDatabase.Dock = DockStyle.Fill;
            this.pnlDatabase.BackColor = Color.White;
            this.pnlDatabase.AutoScroll = true;

            int w = 400;
            int h = 26;
            int gap = 10;
            int y = 15;

            this.chkEnableDb.Text = "فعال‌سازی ذخیره در دیتابیس ریموت";
            this.chkEnableDb.Location = new Point(20, y);
            this.chkEnableDb.Size = new Size(300, h);
            this.chkEnableDb.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            y += h + gap + 5;

            this.lblConnString.Text = "Connection String:";
            this.lblConnString.Location = new Point(20, y);
            this.lblConnString.Size = new Size(140, h);
            this.lblConnString.TextAlign = ContentAlignment.MiddleRight;

            this.txtConnString.Location = new Point(170, y);
            this.txtConnString.Size = new Size(400, h);
            y += h + gap;

            this.lblTableName.Text = "نام جدول:";
            this.lblTableName.Location = new Point(20, y);
            this.lblTableName.Size = new Size(140, h);
            this.lblTableName.TextAlign = ContentAlignment.MiddleRight;

            this.txtTableName.Location = new Point(170, y);
            this.txtTableName.Size = new Size(300, h);
            y += h + gap;

            this.lblDbMode.Text = "روش ذخیره:";
            this.lblDbMode.Location = new Point(20, y);
            this.lblDbMode.Size = new Size(140, h);
            this.lblDbMode.TextAlign = ContentAlignment.MiddleRight;

            this.cmbDbMode.Location = new Point(170, y);
            this.cmbDbMode.Size = new Size(300, h);
            this.cmbDbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDbMode.Items.AddRange(new object[] { DatabaseStorageMode.BlobDirect, DatabaseStorageMode.Hybrid, DatabaseStorageMode.FileStream });
            y += h + gap;

            this.lblFileShare.Text = "مسیر اشتراک فایل:";
            this.lblFileShare.Location = new Point(20, y);
            this.lblFileShare.Size = new Size(140, h);
            this.lblFileShare.TextAlign = ContentAlignment.MiddleRight;

            this.txtFileShare.Location = new Point(170, y);
            this.txtFileShare.Size = new Size(365, h);

            this.btnBrowseShare.Text = "...";
            this.btnBrowseShare.Location = new Point(540, y);
            this.btnBrowseShare.Size = new Size(30, h);
            y += h + gap;

            this.lblMaxFileSizeMb.Text = "حداکثر حجم (MB):";
            this.lblMaxFileSizeMb.Location = new Point(20, y);
            this.lblMaxFileSizeMb.Size = new Size(140, h);
            this.lblMaxFileSizeMb.TextAlign = ContentAlignment.MiddleRight;

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
            this.lblDbSubfolder.TextAlign = ContentAlignment.MiddleRight;

            this.txtDbSubfolder.Location = new Point(170, y);
            this.txtDbSubfolder.Size = new Size(300, h);
            y += h + gap + 10;

            this.btnTestConnection.Text = "تست اتصال و ساخت جدول";
            this.btnTestConnection.Location = new Point(170, y);
            this.btnTestConnection.Size = new Size(220, 34);
            this.btnTestConnection.BackColor = Color.White;
            this.btnTestConnection.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnTestConnection.FlatStyle = FlatStyle.Flat;
            this.btnTestConnection.FlatAppearance.BorderSize = 1;
            this.btnTestConnection.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);

            this.pnlDatabase.Controls.Add(this.chkEnableDb);
            this.pnlDatabase.Controls.Add(this.lblConnString);
            this.pnlDatabase.Controls.Add(this.txtConnString);
            this.pnlDatabase.Controls.Add(this.lblTableName);
            this.pnlDatabase.Controls.Add(this.txtTableName);
            this.pnlDatabase.Controls.Add(this.lblDbMode);
            this.pnlDatabase.Controls.Add(this.cmbDbMode);
            this.pnlDatabase.Controls.Add(this.lblFileShare);
            this.pnlDatabase.Controls.Add(this.txtFileShare);
            this.pnlDatabase.Controls.Add(this.btnBrowseShare);
            this.pnlDatabase.Controls.Add(this.lblMaxFileSizeMb);
            this.pnlDatabase.Controls.Add(this.numMaxFileSizeMb);
            this.pnlDatabase.Controls.Add(this.chkSkipLarger);
            this.pnlDatabase.Controls.Add(this.chkCompress);
            this.pnlDatabase.Controls.Add(this.lblDbSubfolder);
            this.pnlDatabase.Controls.Add(this.txtDbSubfolder);
            this.pnlDatabase.Controls.Add(this.btnTestConnection);

            this.tabDatabase.Controls.Add(this.pnlDatabase);
        }
    }
}
