using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;

namespace FileCollector.Forms
{
    partial class FolderConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        // Color palette (must match MainForm for consistency)
        private static readonly Color BgForm       = Color.FromArgb(252, 251, 248);
        private static readonly Color BgPanel      = Color.White;
        private static readonly Color BorderLight  = Color.FromArgb(220, 220, 215);
        private static readonly Color TextDark     = Color.FromArgb(51, 51, 51);
        private static readonly Color TextMedium   = Color.FromArgb(90, 90, 90);
        private static readonly Color BgGridHeader = Color.FromArgb(245, 245, 242);
        private static readonly Color BgGridAltRow = Color.FromArgb(250, 249, 246);

        // Tab control — now only 3 tabs (Text Processing removed; it's an action type now)
        private TabControl tabMain;
        private TabPage tabGeneral;
        private TabPage tabActions;
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

        // Actions tab controls — DataGridView instead of ListBox
        private DataGridView dgvActions;
        private DataGridViewTextBoxColumn colActionEnabled;
        private DataGridViewTextBoxColumn colActionType;
        private DataGridViewTextBoxColumn colActionName;
        private DataGridViewTextBoxColumn colActionDest;
        private Button btnAddAction;
        private Button btnEditAction;
        private Button btnRemoveAction;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Label lblActionsHint;
        private Panel pnlActions;
        private Panel pnlActionsButtons;

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

            this.dgvActions = new DataGridView();
            this.colActionEnabled = new DataGridViewTextBoxColumn();
            this.colActionType = new DataGridViewTextBoxColumn();
            this.colActionName = new DataGridViewTextBoxColumn();
            this.colActionDest = new DataGridViewTextBoxColumn();
            this.btnAddAction = new Button();
            this.btnEditAction = new Button();
            this.btnRemoveAction = new Button();
            this.btnMoveUp = new Button();
            this.btnMoveDown = new Button();
            this.lblActionsHint = new Label();
            this.pnlActions = new Panel();
            this.pnlActionsButtons = new Panel();

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
            ((System.ComponentModel.ISupportInitialize)(this.dgvActions)).BeginInit();
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
            this.BackColor = BgForm;

            // ---------- Tab control ----------
            this.tabMain.Dock = DockStyle.Fill;
            this.tabMain.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            this.tabMain.RightToLeft = RightToLeft.Yes;

            this.tabGeneral.Text = "عمومی";
            this.tabGeneral.RightToLeft = RightToLeft.Yes;
            this.tabActions.Text = "اکشن‌ها";
            this.tabActions.RightToLeft = RightToLeft.Yes;
            this.tabDatabase.Text = "پایگاه‌داده";
            this.tabDatabase.RightToLeft = RightToLeft.Yes;
            this.tabMain.TabPages.Add(this.tabGeneral);
            this.tabMain.TabPages.Add(this.tabActions);
            this.tabMain.TabPages.Add(this.tabDatabase);

            BuildGeneralTab();
            BuildActionsTab();
            BuildDatabaseTab();

            // ---------- Bottom panel ----------
            this.pnlBottom.Dock = DockStyle.Bottom;
            this.pnlBottom.Height = 50;
            this.pnlBottom.BackColor = BgPanel;

            this.btnSave.Text = "ذخیره";
            this.btnSave.Size = new Size(100, 32);
            this.btnSave.BackColor = BgPanel;
            this.btnSave.ForeColor = TextDark;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 1;
            this.btnSave.FlatAppearance.BorderColor = BorderLight;
            this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.BackColor = BgPanel;
            this.btnCancel.ForeColor = TextDark;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = BorderLight;
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxFileSizeMb)).EndInit();
        }

        private void PnlBottom_Resize(object sender, EventArgs e)
        {
            int y = (this.pnlBottom.Height - 32) / 2;
            this.btnSave.Location = new Point(this.pnlBottom.Width - 230, y);
            this.btnCancel.Location = new Point(this.pnlBottom.Width - 120, y);
        }

        private void BuildGeneralTab()
        {
            this.pnlGeneral.Dock = DockStyle.Fill;
            this.pnlGeneral.BackColor = BgPanel;
            this.pnlGeneral.AutoScroll = true;
            this.pnlGeneral.RightToLeft = RightToLeft.Yes;

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
            this.cmbWatchMode.RightToLeft = RightToLeft.Yes;
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
            this.cmbConflict.RightToLeft = RightToLeft.Yes;
            this.cmbConflict.Items.AddRange(new object[] { "overwrite", "skip", "rename", "keepboth" });
            y += h + gap;

            this.lblSubfolder.Text = "الگوی زیرپوشه (اختیاری):";
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
            this.pnlActions.BackColor = BgPanel;
            this.pnlActions.RightToLeft = RightToLeft.Yes;

            this.lblActionsHint.Text = "اکشن‌ها به ترتیب اجرا می‌شوند (حداکثر ۵ اکشن). روی یک ردیف دابل‌کلیک کنید تا ویرایش شود.";
            this.lblActionsHint.Dock = DockStyle.Top;
            this.lblActionsHint.Height = 30;
            this.lblActionsHint.Font = new Font("Tahoma", 9F);
            this.lblActionsHint.ForeColor = TextMedium;

            // DataGridView for actions
            this.dgvActions.Dock = DockStyle.Fill;
            this.dgvActions.AllowUserToAddRows = false;
            this.dgvActions.AllowUserToDeleteRows = false;
            this.dgvActions.ReadOnly = true;
            this.dgvActions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvActions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvActions.RowHeadersVisible = false;
            this.dgvActions.RowHeadersWidth = 4;
            this.dgvActions.BackgroundColor = BgPanel;
            this.dgvActions.BorderStyle = BorderStyle.FixedSingle;
            this.dgvActions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvActions.GridColor = BorderLight;
            this.dgvActions.EnableHeadersVisualStyles = false;
            this.dgvActions.ColumnHeadersDefaultCellStyle.BackColor = BgGridHeader;
            this.dgvActions.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            this.dgvActions.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.dgvActions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvActions.ColumnHeadersHeight = 32;
            this.dgvActions.RowTemplate.Height = 28;
            this.dgvActions.AlternatingRowsDefaultCellStyle.BackColor = BgGridAltRow;
            this.dgvActions.RightToLeft = RightToLeft.Yes;

            this.colActionEnabled.HeaderText = "فعال";
            this.colActionEnabled.Name = "colActionEnabled";
            this.colActionEnabled.Width = 50;

            this.colActionType.HeaderText = "نوع";
            this.colActionType.Name = "colActionType";
            this.colActionType.Width = 100;

            this.colActionName.HeaderText = "نام";
            this.colActionName.Name = "colActionName";
            this.colActionName.Width = 120;

            this.colActionDest.HeaderText = "مقصد / پارامتر";
            this.colActionDest.Name = "colActionDest";
            this.colActionDest.Width = 200;

            this.dgvActions.Columns.AddRange(new DataGridViewColumn[] {
                this.colActionEnabled, this.colActionType, this.colActionName, this.colActionDest
            });

            // Buttons panel
            this.pnlActionsButtons.Dock = DockStyle.Right;
            this.pnlActionsButtons.Width = 130;
            this.pnlActionsButtons.Padding = new Padding(5);

            int y = 10;
            int btnW = 115;

            this.btnAddAction.Text = "افزودن";
            this.btnAddAction.Size = new Size(btnW, 32);
            this.btnAddAction.Location = new Point(5, y);
            this.btnAddAction.BackColor = BgPanel;
            this.btnAddAction.ForeColor = TextDark;
            this.btnAddAction.FlatStyle = FlatStyle.Flat;
            this.btnAddAction.FlatAppearance.BorderSize = 1;
            this.btnAddAction.FlatAppearance.BorderColor = BorderLight;
            y += 38;

            this.btnEditAction.Text = "ویرایش";
            this.btnEditAction.Size = new Size(btnW, 32);
            this.btnEditAction.Location = new Point(5, y);
            this.btnEditAction.BackColor = BgPanel;
            this.btnEditAction.ForeColor = TextDark;
            this.btnEditAction.FlatStyle = FlatStyle.Flat;
            this.btnEditAction.FlatAppearance.BorderSize = 1;
            this.btnEditAction.FlatAppearance.BorderColor = BorderLight;
            y += 38;

            this.btnRemoveAction.Text = "حذف";
            this.btnRemoveAction.Size = new Size(btnW, 32);
            this.btnRemoveAction.Location = new Point(5, y);
            this.btnRemoveAction.BackColor = BgPanel;
            this.btnRemoveAction.ForeColor = TextDark;
            this.btnRemoveAction.FlatStyle = FlatStyle.Flat;
            this.btnRemoveAction.FlatAppearance.BorderSize = 1;
            this.btnRemoveAction.FlatAppearance.BorderColor = BorderLight;
            y += 50;

            this.btnMoveUp.Text = "↑ بالا";
            this.btnMoveUp.Size = new Size(btnW, 32);
            this.btnMoveUp.Location = new Point(5, y);
            this.btnMoveUp.BackColor = BgPanel;
            this.btnMoveUp.ForeColor = TextDark;
            this.btnMoveUp.FlatStyle = FlatStyle.Flat;
            this.btnMoveUp.FlatAppearance.BorderSize = 1;
            this.btnMoveUp.FlatAppearance.BorderColor = BorderLight;
            y += 38;

            this.btnMoveDown.Text = "↓ پایین";
            this.btnMoveDown.Size = new Size(btnW, 32);
            this.btnMoveDown.Location = new Point(5, y);
            this.btnMoveDown.BackColor = BgPanel;
            this.btnMoveDown.ForeColor = TextDark;
            this.btnMoveDown.FlatStyle = FlatStyle.Flat;
            this.btnMoveDown.FlatAppearance.BorderSize = 1;
            this.btnMoveDown.FlatAppearance.BorderColor = BorderLight;

            this.pnlActionsButtons.Controls.Add(this.btnAddAction);
            this.pnlActionsButtons.Controls.Add(this.btnEditAction);
            this.pnlActionsButtons.Controls.Add(this.btnRemoveAction);
            this.pnlActionsButtons.Controls.Add(this.btnMoveUp);
            this.pnlActionsButtons.Controls.Add(this.btnMoveDown);

            this.pnlActions.Controls.Add(this.dgvActions);
            this.pnlActions.Controls.Add(this.pnlActionsButtons);
            this.pnlActions.Controls.Add(this.lblActionsHint);

            this.tabActions.Controls.Add(this.pnlActions);
        }

        private void BuildDatabaseTab()
        {
            this.pnlDatabase.Dock = DockStyle.Fill;
            this.pnlDatabase.BackColor = BgPanel;
            this.pnlDatabase.AutoScroll = true;
            this.pnlDatabase.RightToLeft = RightToLeft.Yes;

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
            this.cmbDbMode.RightToLeft = RightToLeft.Yes;
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
            this.btnTestConnection.BackColor = BgPanel;
            this.btnTestConnection.ForeColor = TextDark;
            this.btnTestConnection.FlatStyle = FlatStyle.Flat;
            this.btnTestConnection.FlatAppearance.BorderSize = 1;
            this.btnTestConnection.FlatAppearance.BorderColor = BorderLight;

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
