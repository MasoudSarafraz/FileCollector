using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;
using Newtonsoft.Json;

namespace FileCollector.Forms
{
    /// <summary>
    /// Editor dialog for a single ActionConfig.
    /// Uses TableLayoutPanel exclusively (no absolute positioning) so RTL
    /// works correctly. The form is resizable and DPI-aware.
    /// </summary>
    public partial class ActionEditorForm : Form
    {
        private readonly ActionConfig _action;

        // ===== Controls =====
        private ComboBox cmbType;
        private TextBox txtName;
        private CheckBox chkEnabled;
        private Label lblDescription;

        // Common group
        private GroupBox grpCommon;
        private TextBox txtDestPath;
        private Button btnBrowseDest;
        private TextBox txtFilename;
        private TextBox txtZipPassword;
        private NumericUpDown numCompressionLevel;

        // Command group
        private GroupBox grpCommand;
        private TextBox txtCommandExe;
        private Button btnBrowseExe;
        private TextBox txtCommandArgs;
        private TextBox txtWorkDir;
        private Button btnBrowseWorkDir;
        private NumericUpDown numTimeout;
        private CheckBox chkWaitForExit;

        // API Upload group
        private GroupBox grpApiUpload;
        private ComboBox cmbApiMethod;
        private TextBox txtApiUrl;
        private ComboBox cmbApiMode;
        private NumericUpDown numApiTimeout;
        private ComboBox cmbAuthType;
        private Panel pnlAuthFields;
        private TextBox txtAuthUsername;
        private TextBox txtAuthPassword;
        private TextBox txtAuthToken;
        private TextBox txtAuthKeyName;
        private TextBox txtAuthKeyValue;
        private TextBox txtApiHeaders;
        private TextBox txtApiJsonTemplate;
        private Label lblApiJsonHint;

        // Text Processing group
        private GroupBox grpTextProcessing;
        private TextBox txtTextExtensions;
        private ComboBox cmbTextEncoding;
        private CheckBox chkTextBackup;
        private CheckBox chkTextFR;
        private CheckBox chkTextHeader;
        private CheckBox chkTextFooter;
        private CheckBox chkTextAppend;
        private CheckBox chkTextPrepend;
        private TextBox txtTextHeader;
        private TextBox txtTextFooter;
        private TextBox txtTextAppend;
        private TextBox txtTextPrepend;
        private DataGridView dgvTextRules;
        private Button btnAddRule;
        private Button btnRemoveRule;

        // Advanced group
        private GroupBox grpAdvanced;
        private NumericUpDown numRetry;
        private NumericUpDown numRetryDelay;
        private CheckBox chkContinueOnFail;

        // Empty hint
        private Panel pnlEmptyHint;
        private Label lblEmptyHint;

        // Master stack
        private TableLayoutPanel tlpStack;
        private Panel pnlContent;

        // Buttons
        private Button btnOK;
        private Button btnCancel;

        public ActionEditorForm(ActionConfig action)
        {
            _action = action;
            InitializeComponent();
            LoadData();
            UpdateVisibility();
            cmbType.SelectedIndexChanged += (s, e) => { UpdateVisibility(); UpdateDescription(); };
            cmbAuthType.SelectedIndexChanged += (s, e) => RebuildAuthFields();
            cmbApiMode.SelectedIndexChanged += (s, e) => UpdateJsonTemplateVisibility();
            WireCheckboxEnableDisable();
        }

        private void InitializeComponent()
        {
            // Create all controls
            this.cmbType = new ComboBox();
            this.txtName = new TextBox();
            this.chkEnabled = new CheckBox();
            this.lblDescription = new Label();
            this.grpCommon = new GroupBox();
            this.txtDestPath = new TextBox();
            this.btnBrowseDest = new Button();
            this.txtFilename = new TextBox();
            this.txtZipPassword = new TextBox();
            this.numCompressionLevel = new NumericUpDown();
            this.grpCommand = new GroupBox();
            this.txtCommandExe = new TextBox();
            this.btnBrowseExe = new Button();
            this.txtCommandArgs = new TextBox();
            this.txtWorkDir = new TextBox();
            this.btnBrowseWorkDir = new Button();
            this.numTimeout = new NumericUpDown();
            this.chkWaitForExit = new CheckBox();
            this.grpApiUpload = new GroupBox();
            this.cmbApiMethod = new ComboBox();
            this.txtApiUrl = new TextBox();
            this.cmbApiMode = new ComboBox();
            this.numApiTimeout = new NumericUpDown();
            this.cmbAuthType = new ComboBox();
            this.pnlAuthFields = new Panel();
            this.txtAuthUsername = new TextBox();
            this.txtAuthPassword = new TextBox();
            this.txtAuthToken = new TextBox();
            this.txtAuthKeyName = new TextBox();
            this.txtAuthKeyValue = new TextBox();
            this.txtApiHeaders = new TextBox();
            this.txtApiJsonTemplate = new TextBox();
            this.lblApiJsonHint = new Label();
            this.grpTextProcessing = new GroupBox();
            this.txtTextExtensions = new TextBox();
            this.cmbTextEncoding = new ComboBox();
            this.chkTextBackup = new CheckBox();
            this.chkTextFR = new CheckBox();
            this.chkTextHeader = new CheckBox();
            this.chkTextFooter = new CheckBox();
            this.chkTextAppend = new CheckBox();
            this.chkTextPrepend = new CheckBox();
            this.txtTextHeader = new TextBox();
            this.txtTextFooter = new TextBox();
            this.txtTextAppend = new TextBox();
            this.txtTextPrepend = new TextBox();
            this.dgvTextRules = new DataGridView();
            this.btnAddRule = new Button();
            this.btnRemoveRule = new Button();
            this.grpAdvanced = new GroupBox();
            this.numRetry = new NumericUpDown();
            this.numRetryDelay = new NumericUpDown();
            this.chkContinueOnFail = new CheckBox();
            this.pnlEmptyHint = new Panel();
            this.lblEmptyHint = new Label();
            this.tlpStack = new TableLayoutPanel();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).BeginInit();

            // ----- Form -----
            this.Text = "تنظیمات اکشن";
            this.Size = new Size(780, 760);
            this.MinimumSize = new Size(720, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = UiTheme.FontRegular;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = UiTheme.BgForm;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Header panel (Dock=Top) =====
            var pnlHeader = BuildHeaderPanel();

            // ===== Content panel (Dock=Fill, AutoScroll) =====
            this.pnlContent = new Panel();
            this.pnlContent.Dock = DockStyle.Fill;
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BackColor = UiTheme.BgForm;
            this.pnlContent.Padding = new Padding(15, 8, 15, 8);

            // Master stack: 1 column, 6 rows
            this.tlpStack = new TableLayoutPanel();
            this.tlpStack.Dock = DockStyle.Top;
            this.tlpStack.ColumnCount = 1;
            this.tlpStack.RowCount = 6;
            this.tlpStack.AutoSize = true;
            this.tlpStack.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tlpStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 6; i++)
                this.tlpStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Build groups
            BuildCommonGroup();
            BuildCommandGroup();
            BuildApiUploadGroup();
            BuildTextProcessingGroup();
            BuildAdvancedGroup();
            BuildEmptyHint();

            // Add groups to stack (row 0..5)
            this.tlpStack.Controls.Add(this.grpCommon, 0, 0);
            this.tlpStack.Controls.Add(this.grpCommand, 0, 1);
            this.tlpStack.Controls.Add(this.grpApiUpload, 0, 2);
            this.tlpStack.Controls.Add(this.grpTextProcessing, 0, 3);
            this.tlpStack.Controls.Add(this.grpAdvanced, 0, 4);
            this.tlpStack.Controls.Add(this.pnlEmptyHint, 0, 5);

            // Set group widths to fill
            foreach (Control c in this.tlpStack.Controls)
            {
                c.Dock = DockStyle.Top;
                c.Width = this.tlpStack.Width;
            }

            this.pnlContent.Controls.Add(this.tlpStack);

            // ===== Buttons panel (Dock=Bottom) =====
            var pnlButtons = BuildButtonsPanel();

            // ===== Add to form (Dock order matters) =====
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);

            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numCompressionLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).EndInit();
        }

        // ===========================================================
        // HEADER
        // ===========================================================

        private Panel BuildHeaderPanel()
        {
            var pnl = new Panel();
            pnl.Dock = DockStyle.Top;
            pnl.Height = 78;
            pnl.BackColor = UiTheme.BgPanel;
            pnl.Padding = new Padding(16, 10, 16, 6);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 5;
            tbl.RowCount = 2;
            tbl.ColumnStyles.Clear();
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));   // Enabled
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));   // Type label
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));    // Type combo
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));   // Name label
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));    // Name textbox
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            // Enabled checkbox
            this.chkEnabled.Text = "فعال";
            this.chkEnabled.Dock = DockStyle.Fill;
            this.chkEnabled.TextAlign = ContentAlignment.MiddleRight;
            this.chkEnabled.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.chkEnabled, 0, 0);

            // Type label
            var lblType = new Label();
            lblType.Text = "نوع اکشن:";
            lblType.Dock = DockStyle.Fill;
            lblType.TextAlign = ContentAlignment.MiddleRight;
            lblType.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lblType, 1, 0);

            // Type combo
            this.cmbType.Dock = DockStyle.Fill;
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.RightToLeft = RightToLeft.Yes;
            this.cmbType.Items.AddRange(Enum.GetNames(typeof(ActionType)));
            tbl.Controls.Add(this.cmbType, 2, 0);

            // Name label
            var lblName = new Label();
            lblName.Text = "نام:";
            lblName.Dock = DockStyle.Fill;
            lblName.TextAlign = ContentAlignment.MiddleRight;
            lblName.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lblName, 3, 0);

            // Name textbox
            this.txtName.Dock = DockStyle.Fill;
            this.txtName.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtName, 4, 0);

            // Description (spans all columns, row 1)
            this.lblDescription.Dock = DockStyle.Fill;
            this.lblDescription.Font = UiTheme.FontSmallItalic;
            this.lblDescription.ForeColor = UiTheme.HintText;
            this.lblDescription.TextAlign = ContentAlignment.MiddleRight;
            this.lblDescription.RightToLeft = RightToLeft.Yes;
            this.lblDescription.Text = "";
            tbl.Controls.Add(this.lblDescription, 0, 1);
            tbl.SetColumnSpan(this.lblDescription, 5);

            pnl.Controls.Add(tbl);

            // Bottom border
            pnl.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.BorderLight, 1))
                {
                    e.Graphics.DrawLine(pen, 0, pnl.Height - 1, pnl.Width, pnl.Height - 1);
                }
            };

            return pnl;
        }

        // ===========================================================
        // COMMON GROUP (Copy, Move, Rename, Zip, ZipAndMove, Extract)
        // ===========================================================

        private void BuildCommonGroup()
        {
            this.grpCommon.Text = "پارامترهای فایل";
            this.grpCommon.BackColor = UiTheme.BgPanel;
            this.grpCommon.Font = UiTheme.FontGroupTitle;
            this.grpCommon.ForeColor = UiTheme.TextDark;
            this.grpCommon.RightToLeft = RightToLeft.Yes;
            this.grpCommon.Height = 200;
            this.grpCommon.Padding = new Padding(UiTheme.GroupPadding, 22, UiTheme.GroupPadding, 8);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 3;
            tbl.RowCount = 4;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35));
            for (int i = 0; i < 4; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            // Row 0: Destination path + browse
            AddLabel(tbl, "مسیر مقصد:", 0, 0);
            this.txtDestPath.Dock = DockStyle.Fill;
            this.txtDestPath.RightToLeft = RightToLeft.No; // LTR paths
            tbl.Controls.Add(this.txtDestPath, 1, 0);
            this.btnBrowseDest.Text = "...";
            this.btnBrowseDest.Dock = DockStyle.Fill;
            this.btnBrowseDest.Click += (s, e) => BrowseFolder(this.txtDestPath);
            tbl.Controls.Add(this.btnBrowseDest, 2, 0);

            // Row 1: Filename pattern + hint
            AddLabel(tbl, "الگوی نام فایل:", 0, 1);
            this.txtFilename.Dock = DockStyle.Fill;
            this.txtFilename.RightToLeft = RightToLeft.No;
            tbl.Controls.Add(this.txtFilename, 1, 1);
            tbl.SetColumnSpan(this.txtFilename, 2);

            // Row 2: Zip password
            AddLabel(tbl, "رمز ZIP:", 0, 2);
            this.txtZipPassword.Dock = DockStyle.Fill;
            this.txtZipPassword.UseSystemPasswordChar = true;
            tbl.Controls.Add(this.txtZipPassword, 1, 2);
            tbl.SetColumnSpan(this.txtZipPassword, 2);

            // Row 3: Compression level
            AddLabel(tbl, "سطح فشرده‌سازی:", 0, 3);
            this.numCompressionLevel.Dock = DockStyle.Left;
            this.numCompressionLevel.Width = 80;
            this.numCompressionLevel.Minimum = 0;
            this.numCompressionLevel.Maximum = 9;
            this.numCompressionLevel.Value = 6;
            tbl.Controls.Add(this.numCompressionLevel, 1, 3);
            tbl.SetColumnSpan(this.numCompressionLevel, 2);

            this.grpCommon.Controls.Add(tbl);
        }

        // ===========================================================
        // COMMAND GROUP (CustomCommand)
        // ===========================================================

        private void BuildCommandGroup()
        {
            this.grpCommand.Text = "پارامترهای Command سفارشی";
            this.grpCommand.BackColor = UiTheme.BgPanel;
            this.grpCommand.Font = UiTheme.FontGroupTitle;
            this.grpCommand.ForeColor = UiTheme.TextDark;
            this.grpCommand.RightToLeft = RightToLeft.Yes;
            this.grpCommand.Height = 200;
            this.grpCommand.Padding = new Padding(UiTheme.GroupPadding, 22, UiTheme.GroupPadding, 8);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 3;
            tbl.RowCount = 4;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35));
            for (int i = 0; i < 4; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            // Row 0: Executable + browse
            AddLabel(tbl, "فایل اجرایی:", 0, 0);
            this.txtCommandExe.Dock = DockStyle.Fill;
            this.txtCommandExe.RightToLeft = RightToLeft.No;
            tbl.Controls.Add(this.txtCommandExe, 1, 0);
            this.btnBrowseExe.Text = "...";
            this.btnBrowseExe.Dock = DockStyle.Fill;
            this.btnBrowseExe.Click += (s, e) => BrowseFile(this.txtCommandExe);
            tbl.Controls.Add(this.btnBrowseExe, 2, 0);

            // Row 1: Arguments
            AddLabel(tbl, "آرگومان‌ها:", 0, 1);
            this.txtCommandArgs.Dock = DockStyle.Fill;
            this.txtCommandArgs.RightToLeft = RightToLeft.No;
            tbl.Controls.Add(this.txtCommandArgs, 1, 1);
            tbl.SetColumnSpan(this.txtCommandArgs, 2);

            // Row 2: Working directory + browse
            AddLabel(tbl, "پوشه کاری:", 0, 2);
            this.txtWorkDir.Dock = DockStyle.Fill;
            this.txtWorkDir.RightToLeft = RightToLeft.No;
            tbl.Controls.Add(this.txtWorkDir, 1, 2);
            this.btnBrowseWorkDir.Text = "...";
            this.btnBrowseWorkDir.Dock = DockStyle.Fill;
            this.btnBrowseWorkDir.Click += (s, e) => BrowseFolder(this.txtWorkDir);
            tbl.Controls.Add(this.btnBrowseWorkDir, 2, 2);

            // Row 3: Timeout + WaitForExit
            AddLabel(tbl, "مهلت (ثانیه):", 0, 3);
            var pnlRow3 = new Panel { Dock = DockStyle.Fill };
            this.numTimeout.Dock = DockStyle.Left;
            this.numTimeout.Width = 80;
            this.numTimeout.Minimum = 0;
            this.numTimeout.Maximum = 86400;
            this.chkWaitForExit.Text = "صبر تا پایان";
            this.chkWaitForExit.Dock = DockStyle.Left;
            this.chkWaitForExit.Width = 120;
            this.chkWaitForExit.RightToLeft = RightToLeft.Yes;
            pnlRow3.Controls.Add(this.chkWaitForExit);
            pnlRow3.Controls.Add(this.numTimeout);
            tbl.Controls.Add(pnlRow3, 1, 3);
            tbl.SetColumnSpan(pnlRow3, 2);

            this.grpCommand.Controls.Add(tbl);
        }

        // ===========================================================
        // API UPLOAD GROUP
        // ===========================================================

        private void BuildApiUploadGroup()
        {
            this.grpApiUpload.Text = "پارامترهای API (آپلود فایل)";
            this.grpApiUpload.BackColor = UiTheme.BgPanel;
            this.grpApiUpload.Font = UiTheme.FontGroupTitle;
            this.grpApiUpload.ForeColor = UiTheme.TextDark;
            this.grpApiUpload.RightToLeft = RightToLeft.Yes;
            this.grpApiUpload.Height = 400;
            this.grpApiUpload.Padding = new Padding(UiTheme.GroupPadding, 22, UiTheme.GroupPadding, 8);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 7;  // rows 0-6: URL, mode, auth-type, auth-fields, headers, json-hint, json-template
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 7; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            // Row 0: Method + URL
            AddLabel(tbl, "متد و URL:", 0, 0);
            var pnlUrl = new Panel { Dock = DockStyle.Fill };
            this.cmbApiMethod.Dock = DockStyle.Left;
            this.cmbApiMethod.Width = 80;
            this.cmbApiMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbApiMethod.Items.AddRange(new object[] { "GET", "POST", "PUT", "PATCH", "DELETE" });
            this.txtApiUrl.Dock = DockStyle.Fill;
            this.txtApiUrl.RightToLeft = RightToLeft.No; // LTR URLs
            pnlUrl.Controls.Add(this.txtApiUrl);
            pnlUrl.Controls.Add(this.cmbApiMethod);
            tbl.Controls.Add(pnlUrl, 1, 0);

            // Row 1: Upload mode + timeout
            AddLabel(tbl, "حالت آپلود:", 0, 1);
            var pnlMode = new Panel { Dock = DockStyle.Fill };
            this.cmbApiMode.Dock = DockStyle.Left;
            this.cmbApiMode.Width = 110;
            this.cmbApiMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbApiMode.RightToLeft = RightToLeft.Yes;
            this.cmbApiMode.Items.AddRange(new object[] { "multipart", "base64" });
            this.numApiTimeout.Dock = DockStyle.Left;
            this.numApiTimeout.Width = 70;
            this.numApiTimeout.Minimum = 5;
            this.numApiTimeout.Maximum = 3600;
            this.numApiTimeout.Value = 60;
            var lblTimeout = new Label { Text = "مهلت:", Dock = DockStyle.Left, Width = 50, TextAlign = ContentAlignment.MiddleRight };
            pnlMode.Controls.Add(lblTimeout);
            pnlMode.Controls.Add(this.numApiTimeout);
            pnlMode.Controls.Add(this.cmbApiMode);
            tbl.Controls.Add(pnlMode, 1, 1);

            // Row 2: Auth type
            AddLabel(tbl, "احراز هویت:", 0, 2);
            this.cmbAuthType.Dock = DockStyle.Left;
            this.cmbAuthType.Width = 160;
            this.cmbAuthType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAuthType.RightToLeft = RightToLeft.Yes;
            this.cmbAuthType.Items.AddRange(new object[] { ApiAuthType.None, ApiAuthType.Basic, ApiAuthType.Bearer, ApiAuthType.ApiKeyHeader, ApiAuthType.ApiKeyQuery });
            tbl.Controls.Add(this.cmbAuthType, 1, 2);

            // Row 3: Auth fields (dynamic panel)
            this.pnlAuthFields.Dock = DockStyle.Fill;
            tbl.Controls.Add(this.pnlAuthFields, 0, 3);
            tbl.SetColumnSpan(this.pnlAuthFields, 2);

            // Row 4: Headers
            AddLabel(tbl, "هدرها (JSON):", 0, 4);
            this.txtApiHeaders.Dock = DockStyle.Fill;
            this.txtApiHeaders.Font = UiTheme.FontMono;
            this.txtApiHeaders.RightToLeft = RightToLeft.No; // LTR JSON
            this.txtApiHeaders.Text = "{}";
            tbl.Controls.Add(this.txtApiHeaders, 1, 4);

            // Row 5: JSON Template hint
            this.lblApiJsonHint.Text = "الگوی JSON (فقط حالت base64 — خالی = پیش‌فرض). متغیرها: {filename} {base64} {size} {md5}";
            this.lblApiJsonHint.Dock = DockStyle.Fill;
            this.lblApiJsonHint.Font = UiTheme.FontSmall;
            this.lblApiJsonHint.ForeColor = UiTheme.HintText;
            this.lblApiJsonHint.TextAlign = ContentAlignment.MiddleRight;
            this.lblApiJsonHint.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.lblApiJsonHint, 0, 5);
            tbl.SetColumnSpan(this.lblApiJsonHint, 2);

            // Row 6: JSON Template textbox
            this.txtApiJsonTemplate.Dock = DockStyle.Fill;
            this.txtApiJsonTemplate.Multiline = true;
            this.txtApiJsonTemplate.ScrollBars = ScrollBars.Vertical;
            this.txtApiJsonTemplate.Font = UiTheme.FontMono;
            this.txtApiJsonTemplate.RightToLeft = RightToLeft.No;
            tbl.Controls.Add(this.txtApiJsonTemplate, 0, 6);
            tbl.SetColumnSpan(this.txtApiJsonTemplate, 2);

            // Make row 6 taller for multiline
            tbl.RowStyles[6] = new RowStyle(SizeType.Absolute, 60);

            this.grpApiUpload.Controls.Add(tbl);
        }

        /// <summary>
        /// Rebuilds the auth fields panel based on selected AuthType.
        /// Only shows relevant fields — no empty rows.
        /// </summary>
        private void RebuildAuthFields()
        {
            this.pnlAuthFields.Controls.Clear();
            this.pnlAuthFields.Height = UiTheme.RowHeight;

            if (cmbAuthType.SelectedItem == null) return;
            if (!Enum.TryParse<ApiAuthType>(cmbAuthType.SelectedItem.ToString(), out var t)) return;

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 4;
            tbl.RowCount = 1;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            switch (t)
            {
                case ApiAuthType.Basic:
                    AddLabel(tbl, "نام کاربری:", 0, 0);
                    this.txtAuthUsername.Dock = DockStyle.Fill;
                    this.txtAuthUsername.RightToLeft = RightToLeft.Yes;
                    tbl.Controls.Add(this.txtAuthUsername, 1, 0);
                    AddLabel(tbl, "رمز:", 2, 0);
                    this.txtAuthPassword.Dock = DockStyle.Fill;
                    this.txtAuthPassword.UseSystemPasswordChar = true;
                    tbl.Controls.Add(this.txtAuthPassword, 3, 0);
                    break;

                case ApiAuthType.Bearer:
                    AddLabel(tbl, "توکن:", 0, 0);
                    this.txtAuthToken.Dock = DockStyle.Fill;
                    this.txtAuthToken.RightToLeft = RightToLeft.No;
                    tbl.Controls.Add(this.txtAuthToken, 1, 0);
                    tbl.SetColumnSpan(this.txtAuthToken, 3);
                    break;

                case ApiAuthType.ApiKeyHeader:
                case ApiAuthType.ApiKeyQuery:
                    AddLabel(tbl, "نام کلید:", 0, 0);
                    this.txtAuthKeyName.Dock = DockStyle.Fill;
                    this.txtAuthKeyName.RightToLeft = RightToLeft.Yes;
                    tbl.Controls.Add(this.txtAuthKeyName, 1, 0);
                    AddLabel(tbl, "مقدار:", 2, 0);
                    this.txtAuthKeyValue.Dock = DockStyle.Fill;
                    this.txtAuthKeyValue.RightToLeft = RightToLeft.No;
                    tbl.Controls.Add(this.txtAuthKeyValue, 3, 0);
                    break;

                case ApiAuthType.None:
                    var lblNone = new Label();
                    lblNone.Text = "بدون احراز هویت";
                    lblNone.Dock = DockStyle.Fill;
                    lblNone.TextAlign = ContentAlignment.MiddleRight;
                    lblNone.Font = UiTheme.FontSmallItalic;
                    lblNone.ForeColor = UiTheme.HintText;
                    lblNone.RightToLeft = RightToLeft.Yes;
                    tbl.Controls.Add(lblNone, 0, 0);
                    tbl.SetColumnSpan(lblNone, 4);
                    break;
            }

            this.pnlAuthFields.Controls.Add(tbl);
        }

        private void UpdateJsonTemplateVisibility()
        {
            bool showJson = cmbApiMode.SelectedItem?.ToString() == "base64";
            lblApiJsonHint.Visible = showJson;
            txtApiJsonTemplate.Visible = showJson;
        }

        // ===========================================================
        // TEXT PROCESSING GROUP
        // ===========================================================

        private void BuildTextProcessingGroup()
        {
            this.grpTextProcessing.Text = "پارامترهای پردازش متن";
            this.grpTextProcessing.BackColor = UiTheme.BgPanel;
            this.grpTextProcessing.Font = UiTheme.FontGroupTitle;
            this.grpTextProcessing.ForeColor = UiTheme.TextDark;
            this.grpTextProcessing.RightToLeft = RightToLeft.Yes;
            this.grpTextProcessing.Height = 460;
            this.grpTextProcessing.Padding = new Padding(UiTheme.GroupPadding, 22, UiTheme.GroupPadding, 8);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 8;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 8; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            // Row 0: Extensions + Encoding
            AddLabel(tbl, "پسوندها:", 0, 0);
            var pnlExt = new Panel { Dock = DockStyle.Fill };
            this.txtTextExtensions.Dock = DockStyle.Fill;
            this.txtTextExtensions.RightToLeft = RightToLeft.No;
            var lblEnc = new Label { Text = "انکودینگ:", Dock = DockStyle.Left, Width = 70, TextAlign = ContentAlignment.MiddleRight };
            this.cmbTextEncoding.Dock = DockStyle.Left;
            this.cmbTextEncoding.Width = 120;
            this.cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTextEncoding.RightToLeft = RightToLeft.Yes;
            this.cmbTextEncoding.Items.AddRange(new object[] { "utf-8", "utf-8-bom", "utf-16", "utf-16-be", "ascii", "windows-1256" });
            pnlExt.Controls.Add(this.txtTextExtensions);
            pnlExt.Controls.Add(this.cmbTextEncoding);
            pnlExt.Controls.Add(lblEnc);
            tbl.Controls.Add(pnlExt, 1, 0);

            // Row 1: Operations checkboxes
            AddLabel(tbl, "عملیات فعال:", 0, 1);
            var pnlOps = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = false, WrapContents = false };
            this.chkTextFR.Text = "جستجو/جایگزینی";
            this.chkTextFR.Margin = new Padding(2);
            this.chkTextFR.RightToLeft = RightToLeft.Yes;
            this.chkTextHeader.Text = "هدر";
            this.chkTextHeader.Margin = new Padding(2);
            this.chkTextHeader.RightToLeft = RightToLeft.Yes;
            this.chkTextFooter.Text = "فوتر";
            this.chkTextFooter.Margin = new Padding(2);
            this.chkTextFooter.RightToLeft = RightToLeft.Yes;
            this.chkTextAppend.Text = "افزودن به انتها";
            this.chkTextAppend.Margin = new Padding(2);
            this.chkTextAppend.RightToLeft = RightToLeft.Yes;
            this.chkTextPrepend.Text = "افزودن به ابتدا";
            this.chkTextPrepend.Margin = new Padding(2);
            this.chkTextPrepend.RightToLeft = RightToLeft.Yes;
            this.chkTextBackup.Text = "پشتیبان (.bak)";
            this.chkTextBackup.Margin = new Padding(2);
            this.chkTextBackup.RightToLeft = RightToLeft.Yes;
            pnlOps.Controls.Add(this.chkTextFR);
            pnlOps.Controls.Add(this.chkTextHeader);
            pnlOps.Controls.Add(this.chkTextFooter);
            pnlOps.Controls.Add(this.chkTextAppend);
            pnlOps.Controls.Add(this.chkTextPrepend);
            pnlOps.Controls.Add(this.chkTextBackup);
            tbl.Controls.Add(pnlOps, 1, 1);

            // Row 2: Header template
            AddLabel(tbl, "الگوی هدر:", 0, 2);
            this.txtTextHeader.Dock = DockStyle.Fill;
            this.txtTextHeader.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtTextHeader, 1, 2);

            // Row 3: Footer template
            AddLabel(tbl, "الگوی فوتر:", 0, 3);
            this.txtTextFooter.Dock = DockStyle.Fill;
            this.txtTextFooter.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtTextFooter, 1, 3);

            // Row 4: Append text
            AddLabel(tbl, "متن افزودن به انتها:", 0, 4);
            this.txtTextAppend.Dock = DockStyle.Fill;
            this.txtTextAppend.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtTextAppend, 1, 4);

            // Row 5: Prepend text
            AddLabel(tbl, "متن افزودن به ابتدا:", 0, 5);
            this.txtTextPrepend.Dock = DockStyle.Fill;
            this.txtTextPrepend.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtTextPrepend, 1, 5);

            // Row 6: Find/Replace rules label
            var lblRules = new Label();
            lblRules.Text = "قوانین جستجو/جایگزینی:";
            lblRules.Dock = DockStyle.Fill;
            lblRules.TextAlign = ContentAlignment.MiddleRight;
            lblRules.Font = UiTheme.FontSmall;
            lblRules.ForeColor = UiTheme.TextMedium;
            lblRules.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lblRules, 0, 6);

            // Row 7: DGV + buttons
            var pnlRules = new Panel { Dock = DockStyle.Fill };
            this.dgvTextRules.Dock = DockStyle.Fill;
            this.dgvTextRules.AllowUserToAddRows = false;
            this.dgvTextRules.AllowUserToDeleteRows = false;
            this.dgvTextRules.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTextRules.RowHeadersVisible = false;
            this.dgvTextRules.RowHeadersWidth = 4;
            this.dgvTextRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTextRules.BackgroundColor = UiTheme.BgPanel;
            this.dgvTextRules.BorderStyle = BorderStyle.FixedSingle;
            this.dgvTextRules.EnableHeadersVisualStyles = false;
            this.dgvTextRules.RightToLeft = RightToLeft.Yes;
            this.dgvTextRules.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.BgGridHeader;
            this.dgvTextRules.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.TextDark;
            this.dgvTextRules.ColumnHeadersDefaultCellStyle.Font = UiTheme.FontBold;
            var colFind = new DataGridViewTextBoxColumn { HeaderText = "متن جستجو", Name = "find" };
            var colReplace = new DataGridViewTextBoxColumn { HeaderText = "متن جایگزین", Name = "replace" };
            var colRegex = new DataGridViewCheckBoxColumn { HeaderText = "Regex", Name = "regex" };
            var colCase = new DataGridViewCheckBoxColumn { HeaderText = "حساس به حروف", Name = "case" };
            this.dgvTextRules.Columns.AddRange(new DataGridViewColumn[] { colFind, colReplace, colRegex, colCase });

            // Buttons on right of DGV
            this.btnAddRule.Text = "+ افزودن";
            this.btnAddRule.Dock = DockStyle.Right;
            this.btnAddRule.Width = 90;
            this.btnAddRule.BackColor = UiTheme.BgPanel;
            this.btnAddRule.ForeColor = UiTheme.TextDark;
            this.btnAddRule.FlatStyle = FlatStyle.Flat;
            this.btnAddRule.FlatAppearance.BorderColor = UiTheme.BorderLight;
            this.btnAddRule.Click += (s, e) => dgvTextRules.Rows.Add("", "", false, false);

            this.btnRemoveRule.Text = "حذف";
            this.btnRemoveRule.Dock = DockStyle.Right;
            this.btnRemoveRule.Width = 60;
            this.btnRemoveRule.BackColor = UiTheme.BgPanel;
            this.btnRemoveRule.ForeColor = UiTheme.TextDark;
            this.btnRemoveRule.FlatStyle = FlatStyle.Flat;
            this.btnRemoveRule.FlatAppearance.BorderColor = UiTheme.BorderLight;
            this.btnRemoveRule.Click += (s, e) =>
            {
                if (dgvTextRules.SelectedRows.Count > 0)
                    dgvTextRules.Rows.Remove(dgvTextRules.SelectedRows[0]);
            };

            pnlRules.Controls.Add(this.dgvTextRules);
            pnlRules.Controls.Add(this.btnRemoveRule);
            pnlRules.Controls.Add(this.btnAddRule);
            tbl.Controls.Add(pnlRules, 0, 7);
            tbl.SetColumnSpan(pnlRules, 2);

            // Make row 7 taller for the DGV
            tbl.RowStyles[7] = new RowStyle(SizeType.Absolute, 160);

            this.grpTextProcessing.Controls.Add(tbl);
        }

        /// <summary>
        /// Wires checkboxes to enable/disable their corresponding textboxes.
        /// </summary>
        private void WireCheckboxEnableDisable()
        {
            chkTextHeader.CheckedChanged += (s, e) => ToggleTextBox(txtTextHeader, chkTextHeader.Checked);
            chkTextFooter.CheckedChanged += (s, e) => ToggleTextBox(txtTextFooter, chkTextFooter.Checked);
            chkTextAppend.CheckedChanged += (s, e) => ToggleTextBox(txtTextAppend, chkTextAppend.Checked);
            chkTextPrepend.CheckedChanged += (s, e) => ToggleTextBox(txtTextPrepend, chkTextPrepend.Checked);
            chkTextFR.CheckedChanged += (s, e) =>
            {
                dgvTextRules.Enabled = chkTextFR.Checked;
                btnAddRule.Enabled = chkTextFR.Checked;
                btnRemoveRule.Enabled = chkTextFR.Checked;
                dgvTextRules.BackColor = chkTextFR.Checked ? UiTheme.BgPanel : UiTheme.DisabledBg;
            };

            // WaitForExit controls whether Timeout is meaningful
            chkWaitForExit.CheckedChanged += (s, e) =>
            {
                numTimeout.Enabled = chkWaitForExit.Checked;
                numTimeout.BackColor = chkWaitForExit.Checked ? UiTheme.BgPanel : UiTheme.DisabledBg;
            };
        }

        private void ToggleTextBox(TextBox txt, bool enabled)
        {
            txt.Enabled = enabled;
            txt.BackColor = enabled ? UiTheme.BgPanel : UiTheme.DisabledBg;
        }

        // ===========================================================
        // ADVANCED GROUP
        // ===========================================================

        private void BuildAdvancedGroup()
        {
            this.grpAdvanced.Text = "تنظیمات پیشرفته (Retry و خطا)";
            this.grpAdvanced.BackColor = UiTheme.BgPanel;
            this.grpAdvanced.Font = UiTheme.FontGroupTitle;
            this.grpAdvanced.ForeColor = UiTheme.TextDark;
            this.grpAdvanced.RightToLeft = RightToLeft.Yes;
            this.grpAdvanced.Height = 90;
            this.grpAdvanced.Padding = new Padding(UiTheme.GroupPadding, 22, UiTheme.GroupPadding, 8);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 1;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.LabelColumnWidth));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, UiTheme.RowHeight));

            AddLabel(tbl, "Retry:", 0, 0);
            var pnl = new Panel { Dock = DockStyle.Fill };
            this.numRetry.Dock = DockStyle.Left;
            this.numRetry.Width = 70;
            this.numRetry.Minimum = 0;
            this.numRetry.Maximum = 100;
            var lblDelay = new Label { Text = "تأخیر (ms):", Dock = DockStyle.Left, Width = 80, TextAlign = ContentAlignment.MiddleRight };
            this.numRetryDelay.Dock = DockStyle.Left;
            this.numRetryDelay.Width = 90;
            this.numRetryDelay.Minimum = 0;
            this.numRetryDelay.Maximum = 3600000;
            this.chkContinueOnFail.Text = "ادامه زنجیره در صورت خطا";
            this.chkContinueOnFail.Dock = DockStyle.Left;
            this.chkContinueOnFail.Width = 180;
            this.chkContinueOnFail.RightToLeft = RightToLeft.Yes;
            pnl.Controls.Add(this.chkContinueOnFail);
            pnl.Controls.Add(this.numRetryDelay);
            pnl.Controls.Add(lblDelay);
            pnl.Controls.Add(this.numRetry);
            tbl.Controls.Add(pnl, 1, 0);

            this.grpAdvanced.Controls.Add(tbl);
        }

        // ===========================================================
        // EMPTY HINT (for Delete/Recycle)
        // ===========================================================

        private void BuildEmptyHint()
        {
            this.pnlEmptyHint.BackColor = UiTheme.BgGroupAlt;
            this.pnlEmptyHint.Height = 80;
            this.pnlEmptyHint.Padding = new Padding(20);

            this.lblEmptyHint.Dock = DockStyle.Fill;
            this.lblEmptyHint.Text = "این اکشن پارامتری ندارد.";
            this.lblEmptyHint.Font = UiTheme.FontSmallItalic;
            this.lblEmptyHint.ForeColor = UiTheme.HintText;
            this.lblEmptyHint.TextAlign = ContentAlignment.MiddleCenter;
            this.lblEmptyHint.RightToLeft = RightToLeft.Yes;

            this.pnlEmptyHint.Controls.Add(this.lblEmptyHint);
        }

        // ===========================================================
        // BUTTONS PANEL
        // ===========================================================

        private Panel BuildButtonsPanel()
        {
            var pnl = new Panel();
            pnl.Dock = DockStyle.Bottom;
            pnl.Height = 56;
            pnl.BackColor = UiTheme.BgPanel;
            pnl.Padding = new Padding(16, 10, 16, 10);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 3;
            tbl.RowCount = 1;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.ButtonWidth + 10));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UiTheme.ButtonWidth + 10));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Spacer
            tbl.Controls.Add(new Label(), 0, 0);

            // OK button
            this.btnOK.Text = "تأیید";
            this.btnOK.Dock = DockStyle.Fill;
            this.btnOK.BackColor = UiTheme.BgPanel;
            this.btnOK.ForeColor = UiTheme.TextDark;
            this.btnOK.Font = UiTheme.FontBold;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 1;
            this.btnOK.FlatAppearance.BorderColor = UiTheme.Accent;
            this.btnOK.Click += BtnOK_Click;
            tbl.Controls.Add(this.btnOK, 1, 0);

            // Cancel button
            this.btnCancel.Text = "انصراف";
            this.btnCancel.Dock = DockStyle.Fill;
            this.btnCancel.BackColor = UiTheme.BgPanel;
            this.btnCancel.ForeColor = UiTheme.TextDark;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = UiTheme.BorderLight;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            tbl.Controls.Add(this.btnCancel, 2, 0);

            pnl.Controls.Add(tbl);

            // Top border
            pnl.Paint += (s, e) =>
            {
                using (var pen = new Pen(UiTheme.BorderLight, 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, pnl.Width, 0);
                }
            };

            return pnl;
        }

        // ===========================================================
        // VISIBILITY & LAYOUT
        // ===========================================================

        private void UpdateVisibility()
        {
            if (cmbType.SelectedItem == null) return;
            if (!Enum.TryParse<ActionType>(cmbType.SelectedItem.ToString(), out var t)) return;

            // Hide all groups first
            SetRowVisible(grpCommon, 0, false);
            SetRowVisible(grpCommand, 1, false);
            SetRowVisible(grpApiUpload, 2, false);
            SetRowVisible(grpTextProcessing, 3, false);
            SetRowVisible(pnlEmptyHint, 5, false);

            // Advanced always visible
            SetRowVisible(grpAdvanced, 4, true);

            switch (t)
            {
                case ActionType.Copy:
                case ActionType.Move:
                case ActionType.Rename:
                case ActionType.Zip:
                case ActionType.ZipAndMove:
                case ActionType.Extract:
                    SetRowVisible(grpCommon, 0, true);
                    break;
                case ActionType.CustomCommand:
                    SetRowVisible(grpCommand, 1, true);
                    break;
                case ActionType.ApiUpload:
                    SetRowVisible(grpApiUpload, 2, true);
                    RebuildAuthFields();
                    UpdateJsonTemplateVisibility();
                    break;
                case ActionType.TextProcessing:
                    SetRowVisible(grpTextProcessing, 3, true);
                    break;
                case ActionType.Delete:
                case ActionType.Recycle:
                    SetRowVisible(pnlEmptyHint, 5, true);
                    break;
            }

            UpdateDescription();
        }

        /// <summary>
        /// Shows or hides a control in the master stack by setting its
        /// row's RowStyle height to 0 (or back to AutoSize).
        /// </summary>
        private void SetRowVisible(Control control, int rowIndex, bool visible)
        {
            control.Visible = visible;
            if (visible)
            {
                tlpStack.RowStyles[rowIndex] = new RowStyle(SizeType.AutoSize);
            }
            else
            {
                tlpStack.RowStyles[rowIndex] = new RowStyle(SizeType.Absolute, 0);
            }
        }

        private void UpdateDescription()
        {
            if (cmbType.SelectedItem == null) return;
            if (!Enum.TryParse<ActionType>(cmbType.SelectedItem.ToString(), out var t)) return;

            switch (t)
            {
                case ActionType.Copy:
                    lblDescription.Text = "کپی فایل به مسیر مقصد. ساختار زیرپوشه‌ها حفظ می‌شود.";
                    break;
                case ActionType.Move:
                    lblDescription.Text = "انتقال فایل به مسیر مقصد. ساختار زیرپوشه‌ها حفظ می‌شود.";
                    break;
                case ActionType.Rename:
                    lblDescription.Text = "تغییر نام فایل در همان پوشه (بدون جابجایی). الگوی نام فایل را تنظیم کنید.";
                    break;
                case ActionType.Delete:
                    lblDescription.Text = "حذف دائمی فایل از دیسک.";
                    break;
                case ActionType.Recycle:
                    lblDescription.Text = "ارسال فایل به سطل بازیافت ویندوز.";
                    break;
                case ActionType.Zip:
                    lblDescription.Text = "فشرده‌سازی فایل به ZIP. فایل اصلی باقی می‌ماند.";
                    break;
                case ActionType.ZipAndMove:
                    lblDescription.Text = "فشرده‌سازی فایل به ZIP و سپس حذف فایل اصلی.";
                    break;
                case ActionType.Extract:
                    lblDescription.Text = "باز کردن فایل ZIP در مسیر مقصد.";
                    break;
                case ActionType.CustomCommand:
                    lblDescription.Text = "اجرای فایل اجرایی دلخواه. از {filepath} برای مسیر فایل استفاده کنید.";
                    break;
                case ActionType.TextProcessing:
                    lblDescription.Text = "پردازش فایل‌های متنی: Find/Replace، Header/Footer، Append/Prepend.";
                    break;
                case ActionType.ApiUpload:
                    lblDescription.Text = "آپلود فایل به API. دو حالت: Multipart (form-data) یا Base64 (JSON).";
                    break;
                default:
                    lblDescription.Text = "";
                    break;
            }
        }

        // ===========================================================
        // HELPERS
        // ===========================================================

        private void AddLabel(TableLayoutPanel tbl, string text, int col, int row)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lbl, col, row);
        }

        private void BrowseFolder(TextBox target)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(target.Text))
                    dlg.SelectedPath = target.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    target.Text = dlg.SelectedPath;
            }
        }

        private void BrowseFile(TextBox target)
        {
            using (var dlg = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(target.Text))
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(target.Text);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    target.Text = dlg.FileName;
            }
        }

        // ===========================================================
        // DATA LOAD / SAVE
        // ===========================================================

        private void LoadData()
        {
            cmbType.SelectedItem = _action.Type.ToString();
            if (cmbType.SelectedIndex < 0 && cmbType.Items.Count > 0)
                cmbType.SelectedIndex = 0;

            txtName.Text = _action.Name;
            chkEnabled.Checked = _action.Enabled;

            // Common
            txtDestPath.Text = _action.DestinationPath;
            txtFilename.Text = _action.FilenamePattern;
            txtZipPassword.Text = _action.ZipPassword;
            numCompressionLevel.Value = Math.Min(Math.Max(0, _action.CompressionLevel), 9);

            // Command
            txtCommandExe.Text = _action.CommandExecutable;
            txtCommandArgs.Text = _action.CommandArguments;
            txtWorkDir.Text = _action.WorkingDirectory;
            chkWaitForExit.Checked = _action.WaitForExit;
            numTimeout.Value = Math.Min(Math.Max(0, _action.TimeoutSeconds), 86400);
            numTimeout.Enabled = chkWaitForExit.Checked;
            numTimeout.BackColor = chkWaitForExit.Checked ? UiTheme.BgPanel : UiTheme.DisabledBg;

            // API Upload
            cmbApiMethod.SelectedItem = _action.ApiMethod;
            if (cmbApiMethod.SelectedIndex < 0 && cmbApiMethod.Items.Count > 0)
                cmbApiMethod.SelectedIndex = 1; // POST
            txtApiUrl.Text = _action.ApiUrl;
            cmbApiMode.SelectedItem = _action.ApiUploadMode;
            if (cmbApiMode.SelectedIndex < 0 && cmbApiMode.Items.Count > 0)
                cmbApiMode.SelectedIndex = 0;
            numApiTimeout.Value = Math.Min(Math.Max(5, _action.ApiTimeoutSeconds), 3600);
            cmbAuthType.SelectedItem = _action.AuthType;
            if (cmbAuthType.SelectedIndex < 0 && cmbAuthType.Items.Count > 0)
                cmbAuthType.SelectedIndex = 0;
            txtApiHeaders.Text = _action.ApiHeaders;
            txtApiJsonTemplate.Text = _action.ApiJsonTemplate;

            // Load auth credential fields BEFORE RebuildAuthFields reparents them.
            // Without this, opening an existing ApiUpload action and clicking OK
            // would silently erase all credentials.
            txtAuthUsername.Text = _action.AuthUsername;
            txtAuthPassword.Text = _action.AuthPassword;
            txtAuthToken.Text = _action.AuthToken;
            txtAuthKeyName.Text = _action.AuthKeyName;
            txtAuthKeyValue.Text = _action.AuthKeyValue;

            // Auth fields (need to rebuild after cmbAuthType is set)
            RebuildAuthFields();

            // Text Processing
            txtTextExtensions.Text = _action.TextExtensions;
            cmbTextEncoding.SelectedItem = _action.TextEncoding;
            if (cmbTextEncoding.SelectedIndex < 0 && cmbTextEncoding.Items.Count > 0)
                cmbTextEncoding.SelectedIndex = 0;
            chkTextBackup.Checked = _action.TextCreateBackup;
            chkTextFR.Checked = _action.TextEnableFindReplace;
            chkTextHeader.Checked = _action.TextEnableHeader;
            chkTextFooter.Checked = _action.TextEnableFooter;
            chkTextAppend.Checked = _action.TextEnableAppend;
            chkTextPrepend.Checked = _action.TextEnablePrepend;
            txtTextHeader.Text = _action.TextHeaderTemplate;
            txtTextFooter.Text = _action.TextFooterTemplate;
            txtTextAppend.Text = _action.TextAppendContent;
            txtTextPrepend.Text = _action.TextPrependContent;

            dgvTextRules.Rows.Clear();
            try
            {
                if (!string.IsNullOrEmpty(_action.TextFindReplaceRulesJson))
                {
                    var rules = JsonConvert.DeserializeObject<List<FindReplaceRule>>(_action.TextFindReplaceRulesJson);
                    if (rules != null)
                    {
                        foreach (var rule in rules)
                        {
                            dgvTextRules.Rows.Add(rule.Find, rule.Replace, rule.UseRegex, rule.CaseSensitive);
                        }
                    }
                }
            }
            catch { }

            // Apply checkbox-to-enabled state
            ToggleTextBox(txtTextHeader, chkTextHeader.Checked);
            ToggleTextBox(txtTextFooter, chkTextFooter.Checked);
            ToggleTextBox(txtTextAppend, chkTextAppend.Checked);
            ToggleTextBox(txtTextPrepend, chkTextPrepend.Checked);
            dgvTextRules.Enabled = chkTextFR.Checked;
            btnAddRule.Enabled = chkTextFR.Checked;
            btnRemoveRule.Enabled = chkTextFR.Checked;
            dgvTextRules.BackColor = chkTextFR.Checked ? UiTheme.BgPanel : UiTheme.DisabledBg;

            // Advanced
            numRetry.Value = Math.Min(Math.Max(0, _action.RetryCount), 100);
            numRetryDelay.Value = Math.Min(Math.Max(0, _action.RetryDelayMs), 3600000);
            chkContinueOnFail.Checked = _action.ContinueOnFailure;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Validate required fields based on action type
            if (Enum.TryParse<ActionType>(cmbType.SelectedItem?.ToString(), out var t))
            {
                string error = null;
                switch (t)
                {
                    case ActionType.ApiUpload:
                        if (string.IsNullOrWhiteSpace(txtApiUrl.Text))
                            error = "URL برای اکشن API الزامی است.";
                        break;
                    case ActionType.CustomCommand:
                        if (string.IsNullOrWhiteSpace(txtCommandExe.Text))
                            error = "فایل اجرایی برای اکشن Command الزامی است.";
                        break;
                    case ActionType.Copy:
                    case ActionType.Move:
                    case ActionType.Zip:
                    case ActionType.ZipAndMove:
                    case ActionType.Extract:
                        if (string.IsNullOrWhiteSpace(txtDestPath.Text))
                            error = "مسیر مقصد برای این اکشن الزامی است.";
                        break;
                    case ActionType.Rename:
                        if (string.IsNullOrWhiteSpace(txtFilename.Text))
                            error = "الگوی نام فایل برای Rename الزامی است.";
                        break;
                }

                if (error != null)
                {
                    MessageBox.Show(this, error, "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }
            }

            SaveData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveData()
        {
            if (Enum.TryParse<ActionType>(cmbType.SelectedItem?.ToString(), out var t))
                _action.Type = t;
            _action.Name = txtName.Text;
            _action.Enabled = chkEnabled.Checked;

            // Common
            _action.DestinationPath = txtDestPath.Text;
            _action.FilenamePattern = txtFilename.Text;
            _action.ZipPassword = txtZipPassword.Text;
            _action.CompressionLevel = (int)numCompressionLevel.Value;

            // Command
            _action.CommandExecutable = txtCommandExe.Text;
            _action.CommandArguments = txtCommandArgs.Text;
            _action.WorkingDirectory = txtWorkDir.Text;
            _action.WaitForExit = chkWaitForExit.Checked;
            _action.TimeoutSeconds = (int)numTimeout.Value;

            // API Upload
            _action.ApiMethod = cmbApiMethod.SelectedItem?.ToString() ?? "POST";
            _action.ApiUrl = txtApiUrl.Text;
            _action.ApiUploadMode = cmbApiMode.SelectedItem?.ToString() ?? "multipart";
            _action.ApiTimeoutSeconds = (int)numApiTimeout.Value;
            if (cmbAuthType.SelectedItem != null)
                _action.AuthType = (ApiAuthType)cmbAuthType.SelectedItem;
            _action.AuthUsername = txtAuthUsername.Text;
            _action.AuthPassword = txtAuthPassword.Text;
            _action.AuthToken = txtAuthToken.Text;
            _action.AuthKeyName = txtAuthKeyName.Text;
            _action.AuthKeyValue = txtAuthKeyValue.Text;
            _action.ApiHeaders = txtApiHeaders.Text;
            _action.ApiJsonTemplate = txtApiJsonTemplate.Text;

            // Text Processing
            _action.TextExtensions = txtTextExtensions.Text;
            _action.TextEncoding = cmbTextEncoding.SelectedItem?.ToString() ?? "utf-8";
            _action.TextCreateBackup = chkTextBackup.Checked;
            _action.TextEnableFindReplace = chkTextFR.Checked;
            _action.TextEnableHeader = chkTextHeader.Checked;
            _action.TextEnableFooter = chkTextFooter.Checked;
            _action.TextEnableAppend = chkTextAppend.Checked;
            _action.TextEnablePrepend = chkTextPrepend.Checked;
            _action.TextHeaderTemplate = txtTextHeader.Text;
            _action.TextFooterTemplate = txtTextFooter.Text;
            _action.TextAppendContent = txtTextAppend.Text;
            _action.TextPrependContent = txtTextPrepend.Text;

            var rules = new List<FindReplaceRule>();
            foreach (DataGridViewRow row in dgvTextRules.Rows)
            {
                if (row.IsNewRow) continue;
                var rule = new FindReplaceRule
                {
                    Find = row.Cells[0].Value?.ToString() ?? "",
                    Replace = row.Cells[1].Value?.ToString() ?? "",
                    UseRegex = row.Cells[2].Value != null && (bool)row.Cells[2].Value,
                    CaseSensitive = row.Cells[3].Value != null && (bool)row.Cells[3].Value
                };
                if (!string.IsNullOrEmpty(rule.Find))
                    rules.Add(rule);
            }
            _action.TextFindReplaceRulesJson = JsonConvert.SerializeObject(rules);

            // Advanced
            _action.RetryCount = (int)numRetry.Value;
            _action.RetryDelayMs = (int)numRetryDelay.Value;
            _action.ContinueOnFailure = chkContinueOnFail.Checked;
        }
    }
}
