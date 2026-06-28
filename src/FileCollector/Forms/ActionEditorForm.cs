using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;
using Newtonsoft.Json;

namespace FileCollector.Forms
{
    /// <summary>
    /// Editor dialog for a single ActionConfig in the action chain.
    /// Uses a TableLayoutPanel for clean, predictable layout.
    /// </summary>
    public class ActionEditorForm : Form
    {
        private readonly ActionConfig _action;

        // Common controls
        private ComboBox cmbType;
        private TextBox txtName;
        private CheckBox chkEnabled;
        private Label lblDescription;

        // Common parameters
        private GroupBox grpCommon;
        private TextBox txtDestPath;
        private TextBox txtFilename;

        // Command parameters
        private GroupBox grpCommand;
        private TextBox txtCommandExe;
        private TextBox txtCommandArgs;
        private TextBox txtWorkDir;
        private CheckBox chkWaitForExit;
        private NumericUpDown numTimeout;

        // API Upload parameters
        private GroupBox grpApiUpload;
        private ComboBox cmbApiMethod;
        private TextBox txtApiUrl;
        private ComboBox cmbApiMode;
        private NumericUpDown numApiTimeout;
        private ComboBox cmbAuthType;
        private TextBox txtAuthUsername;
        private TextBox txtAuthPassword;
        private TextBox txtAuthToken;
        private TextBox txtAuthKeyName;
        private TextBox txtAuthKeyValue;
        private TextBox txtApiHeaders;
        private TextBox txtApiJsonTemplate;
        private Label lblApiJsonHint;

        // Text Processing parameters
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

        // Advanced
        private GroupBox grpAdvanced;
        private NumericUpDown numRetry;
        private NumericUpDown numRetryDelay;
        private CheckBox chkContinueOnFail;

        // Layout panel
        private Panel pnlScroll;

        // Buttons
        private Button btnOK;
        private Button btnCancel;

        // Color palette
        private static readonly Color BgForm = Color.FromArgb(245, 247, 250);
        private static readonly Color BgPanel = Color.White;
        private static readonly Color BorderLight = Color.FromArgb(220, 220, 215);
        private static readonly Color TextDark = Color.FromArgb(51, 51, 51);
        private static readonly Color TextMedium = Color.FromArgb(90, 90, 90);
        private static readonly Color BgGridHeader = Color.FromArgb(245, 245, 242);

        public ActionEditorForm(ActionConfig action)
        {
            _action = action;
            InitializeComponent();
            LoadData();
            UpdateGroupVisibility();
            UpdateDescription();
            cmbType.SelectedIndexChanged += (s, e) => { UpdateGroupVisibility(); UpdateDescription(); };
            cmbAuthType.SelectedIndexChanged += (s, e) => UpdateAuthVisibility();
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
            this.txtFilename = new TextBox();
            this.grpCommand = new GroupBox();
            this.txtCommandExe = new TextBox();
            this.txtCommandArgs = new TextBox();
            this.txtWorkDir = new TextBox();
            this.chkWaitForExit = new CheckBox();
            this.numTimeout = new NumericUpDown();
            this.grpApiUpload = new GroupBox();
            this.cmbApiMethod = new ComboBox();
            this.txtApiUrl = new TextBox();
            this.cmbApiMode = new ComboBox();
            this.numApiTimeout = new NumericUpDown();
            this.cmbAuthType = new ComboBox();
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
            this.grpAdvanced = new GroupBox();
            this.numRetry = new NumericUpDown();
            this.numRetryDelay = new NumericUpDown();
            this.chkContinueOnFail = new CheckBox();
            this.pnlScroll = new Panel();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).BeginInit();

            // ----- Form -----
            this.Text = "تنظیمات اکشن";
            this.Size = new Size(760, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9.75F);
            this.RightToLeft = RightToLeft.Yes;
            // NOTE: Do NOT set RightToLeftLayout = true — it breaks absolute positioning
            // of dynamically-repositioned controls. We handle RTL manually via
            // RightToLeft on individual controls and text alignment.
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = BgForm;

            // Header panel (fixed at top, not scrolled) — uses TableLayoutPanel
            // for clean, predictable layout regardless of RTL settings.
            var pnlHeader = new TableLayoutPanel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 80;
            pnlHeader.BackColor = BgPanel;
            pnlHeader.ColumnCount = 7;
            pnlHeader.RowCount = 2;
            pnlHeader.Padding = new Padding(15, 10, 15, 5);

            // Define column widths (in pixels, left-to-right visual order)
            // Columns: [Enabled][spacer][Name-label][Name-input][spacer][Type-label][Type-input]
            pnlHeader.ColumnStyles.Clear();
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));   // col0: Enabled checkbox
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15));   // col1: spacer
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));   // col2: "نام:" label
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));    // col3: Name textbox
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15));   // col4: spacer
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));   // col5: "نوع اکشن:" label
            pnlHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));    // col6: Type combo

            pnlHeader.RowStyles.Clear();
            pnlHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // row0: controls
            pnlHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));  // row1: description

            // Enabled checkbox (col0, row0) — spans 1 cell
            this.chkEnabled.Text = "فعال";
            this.chkEnabled.Dock = DockStyle.Fill;
            this.chkEnabled.TextAlign = ContentAlignment.MiddleRight;
            this.chkEnabled.RightToLeft = RightToLeft.Yes;
            pnlHeader.Controls.Add(this.chkEnabled, 0, 0);

            // "نام:" label (col2, row0)
            var lblName = new Label();
            lblName.Text = "نام:";
            lblName.Dock = DockStyle.Fill;
            lblName.TextAlign = ContentAlignment.MiddleRight;
            lblName.RightToLeft = RightToLeft.Yes;
            pnlHeader.Controls.Add(lblName, 2, 0);

            // Name textbox (col3, row0)
            this.txtName.Dock = DockStyle.Fill;
            this.txtName.RightToLeft = RightToLeft.Yes;
            pnlHeader.Controls.Add(this.txtName, 3, 0);

            // "نوع اکشن:" label (col5, row0)
            var lblType = new Label();
            lblType.Text = "نوع اکشن:";
            lblType.Dock = DockStyle.Fill;
            lblType.TextAlign = ContentAlignment.MiddleRight;
            lblType.RightToLeft = RightToLeft.Yes;
            pnlHeader.Controls.Add(lblType, 5, 0);

            // Type combo (col6, row0)
            this.cmbType.Dock = DockStyle.Fill;
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.RightToLeft = RightToLeft.Yes;
            this.cmbType.Items.AddRange(Enum.GetNames(typeof(ActionType)));
            pnlHeader.Controls.Add(this.cmbType, 6, 0);

            // Description (col0..col6, row1) — spans all columns
            this.lblDescription.Dock = DockStyle.Fill;
            this.lblDescription.Font = new Font("Tahoma", 8.5F);
            this.lblDescription.ForeColor = TextMedium;
            this.lblDescription.TextAlign = ContentAlignment.MiddleRight;
            this.lblDescription.RightToLeft = RightToLeft.Yes;
            this.lblDescription.Text = "";
            pnlHeader.Controls.Add(this.lblDescription, 0, 1);
            pnlHeader.SetColumnSpan(this.lblDescription, 7);

            // Scrollable content panel (holds all groups)
            this.pnlScroll.Dock = DockStyle.Fill;
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.BackColor = BgForm;
            this.pnlScroll.Padding = new Padding(15, 5, 15, 5);

            // Bottom buttons panel
            var pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 50;
            pnlButtons.BackColor = BgPanel;
            pnlButtons.Padding = new Padding(15, 8, 15, 8);

            this.btnOK.Text = "تأیید";
            this.btnOK.Size = new Size(100, 32);
            this.btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnOK.BackColor = BgPanel;
            this.btnOK.ForeColor = TextDark;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 1;
            this.btnOK.FlatAppearance.BorderColor = BorderLight;
            this.btnOK.Click += BtnOK_Click;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnCancel.BackColor = BgPanel;
            this.btnCancel.ForeColor = TextDark;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = BorderLight;
            this.btnCancel.DialogResult = DialogResult.Cancel;

            pnlButtons.Controls.Add(this.btnCancel);
            pnlButtons.Controls.Add(this.btnOK);
            pnlButtons.Layout += (s, e) =>
            {
                // Place buttons at right edge (RTL: visually leftmost)
                this.btnOK.Location = new Point(pnlButtons.Width - 230, 9);
                this.btnCancel.Location = new Point(pnlButtons.Width - 120, 9);
            };

            // Build groups (no positioning yet — UpdateGroupVisibility handles layout)
            BuildCommonGroup();
            BuildCommandGroup();
            BuildApiUploadGroup();
            BuildTextProcessingGroup();
            BuildAdvancedGroup();

            // Add groups to scroll panel
            this.pnlScroll.Controls.Add(this.grpCommon);
            this.pnlScroll.Controls.Add(this.grpCommand);
            this.pnlScroll.Controls.Add(this.grpApiUpload);
            this.pnlScroll.Controls.Add(this.grpTextProcessing);
            this.pnlScroll.Controls.Add(this.grpAdvanced);

            // Add panels to form (order matters for Dock)
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);

            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).EndInit();
        }

        private void BuildCommonGroup()
        {
            this.grpCommon.Text = "پارامترهای فایل";
            this.grpCommon.Size = new Size(700, 100);
            this.grpCommon.BackColor = BgPanel;
            this.grpCommon.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpCommon.RightToLeft = RightToLeft.Yes;

            // Use a TableLayoutPanel inside the group for clean RTL layout
            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 2;
            tbl.Padding = new Padding(10, 20, 10, 5);
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));  // label column
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // input column
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            var lblDest = new Label();
            lblDest.Text = "مسیر مقصد:";
            lblDest.Dock = DockStyle.Fill;
            lblDest.TextAlign = ContentAlignment.MiddleRight;
            lblDest.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lblDest, 0, 0);

            this.txtDestPath.Dock = DockStyle.Fill;
            this.txtDestPath.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtDestPath, 1, 0);

            var lblFile = new Label();
            lblFile.Text = "الگوی نام فایل:";
            lblFile.Dock = DockStyle.Fill;
            lblFile.TextAlign = ContentAlignment.MiddleRight;
            lblFile.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lblFile, 0, 1);

            this.txtFilename.Dock = DockStyle.Fill;
            this.txtFilename.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtFilename, 1, 1);

            this.grpCommon.Controls.Add(tbl);
        }

        private void BuildCommandGroup()
        {
            this.grpCommand.Text = "پارامترهای Command سفارشی";
            this.grpCommand.Size = new Size(700, 150);
            this.grpCommand.BackColor = BgPanel;
            this.grpCommand.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpCommand.RightToLeft = RightToLeft.Yes;

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 4;
            tbl.Padding = new Padding(10, 20, 10, 5);
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 4; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            Action<string, Control, int> addRow = (labelText, ctrl, row) =>
            {
                var lbl = new Label();
                lbl.Text = labelText;
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleRight;
                lbl.RightToLeft = RightToLeft.Yes;
                tbl.Controls.Add(lbl, 0, row);
                ctrl.Dock = DockStyle.Fill;
                if (ctrl is TextBox tb) tb.RightToLeft = RightToLeft.Yes;
                tbl.Controls.Add(ctrl, 1, row);
            };

            addRow("Executable:", this.txtCommandExe, 0);
            addRow("Arguments:", this.txtCommandArgs, 1);
            addRow("Working Dir:", this.txtWorkDir, 2);

            // Row 3: Timeout + WaitForExit in the input cell
            var pnlRow3 = new Panel { Dock = DockStyle.Fill };
            this.numTimeout.Dock = DockStyle.Left;
            this.numTimeout.Size = new Size(80, 24);
            this.numTimeout.Minimum = 0;
            this.numTimeout.Maximum = 86400;
            this.chkWaitForExit.Text = "Wait for exit";
            this.chkWaitForExit.Dock = DockStyle.Left;
            this.chkWaitForExit.Size = new Size(120, 24);
            this.chkWaitForExit.RightToLeft = RightToLeft.Yes;
            pnlRow3.Controls.Add(this.chkWaitForExit);
            pnlRow3.Controls.Add(this.numTimeout);
            addRow("Timeout (s):", pnlRow3, 3);

            this.grpCommand.Controls.Add(tbl);
        }

        private void BuildApiUploadGroup()
        {
            this.grpApiUpload.Text = "پارامترهای API (آپلود فایل)";
            this.grpApiUpload.Size = new Size(700, 320);
            this.grpApiUpload.BackColor = BgPanel;
            this.grpApiUpload.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpApiUpload.RightToLeft = RightToLeft.Yes;

            // Main table: label column (120px) + input column (fill)
            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 2;
            tbl.RowCount = 9;
            tbl.Padding = new Padding(10, 20, 10, 5);
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 9; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            // Row 0: Method + URL (in a sub-panel)
            var pnlUrl = new Panel { Dock = DockStyle.Fill };
            this.cmbApiMethod.Dock = DockStyle.Left;
            this.cmbApiMethod.Size = new Size(80, 24);
            this.cmbApiMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbApiMethod.RightToLeft = RightToLeft.Yes;
            this.cmbApiMethod.Items.AddRange(new object[] { "GET", "POST", "PUT", "PATCH", "DELETE" });
            this.txtApiUrl.Dock = DockStyle.Fill;
            this.txtApiUrl.RightToLeft = RightToLeft.Yes;
            pnlUrl.Controls.Add(this.txtApiUrl);
            pnlUrl.Controls.Add(this.cmbApiMethod);
            AddTableRow(tbl, "متد و URL:", pnlUrl, 0);

            // Row 1: Mode + Timeout
            var pnlMode = new Panel { Dock = DockStyle.Fill };
            this.cmbApiMode.Dock = DockStyle.Left;
            this.cmbApiMode.Size = new Size(120, 24);
            this.cmbApiMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbApiMode.RightToLeft = RightToLeft.Yes;
            this.cmbApiMode.Items.AddRange(new object[] { "multipart", "base64" });
            this.numApiTimeout.Dock = DockStyle.Left;
            this.numApiTimeout.Size = new Size(80, 24);
            this.numApiTimeout.Minimum = 5;
            this.numApiTimeout.Maximum = 3600;
            this.numApiTimeout.Value = 60;
            var lblTimeout = new Label { Text = "Timeout (s):", Dock = DockStyle.Left, Width = 80, TextAlign = ContentAlignment.MiddleRight };
            pnlMode.Controls.Add(lblTimeout);
            pnlMode.Controls.Add(this.numApiTimeout);
            pnlMode.Controls.Add(this.cmbApiMode);
            AddTableRow(tbl, "حالت آپلود:", pnlMode, 1);

            // Row 2: Auth type
            this.cmbAuthType.Dock = DockStyle.Left;
            this.cmbAuthType.Size = new Size(150, 24);
            this.cmbAuthType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbAuthType.RightToLeft = RightToLeft.Yes;
            this.cmbAuthType.Items.AddRange(new object[] { ApiAuthType.None, ApiAuthType.Basic, ApiAuthType.Bearer, ApiAuthType.ApiKeyHeader, ApiAuthType.ApiKeyQuery });
            AddTableRow(tbl, "نوع احراز هویت:", this.cmbAuthType, 2);

            // Row 3: Auth username/password (Basic)
            var pnlBasic = new Panel { Dock = DockStyle.Fill };
            this.txtAuthUsername.Dock = DockStyle.Fill;
            this.txtAuthUsername.RightToLeft = RightToLeft.Yes;
            this.txtAuthUsername.Name = "txtAuthUser";
            this.txtAuthPassword.Dock = DockStyle.Left;
            this.txtAuthPassword.Size = new Size(200, 24);
            this.txtAuthPassword.Name = "txtAuthPass";
            this.txtAuthPassword.UseSystemPasswordChar = true;
            var lblPass = new Label { Text = "Password:", Dock = DockStyle.Left, Width = 80, TextAlign = ContentAlignment.MiddleRight };
            pnlBasic.Controls.Add(this.txtAuthUsername);
            pnlBasic.Controls.Add(this.txtAuthPassword);
            pnlBasic.Controls.Add(lblPass);
            AddTableRow(tbl, "Username:", pnlBasic, 3);
            // Mark the label so we can show/hide it
            foreach (Control c in tbl.Controls) { if (c is Label l && l.Text == "Username:") l.Name = "lblAuthUser"; }

            // Row 4: Bearer token
            this.txtAuthToken.Dock = DockStyle.Fill;
            this.txtAuthToken.RightToLeft = RightToLeft.Yes;
            this.txtAuthToken.Name = "txtAuthToken";
            AddTableRow(tbl, "Token:", this.txtAuthToken, 4);
            foreach (Control c in tbl.Controls) { if (c is Label l && l.Text == "Token:") l.Name = "lblAuthToken"; }

            // Row 5: API Key name + value
            var pnlKey = new Panel { Dock = DockStyle.Fill };
            this.txtAuthKeyName.Dock = DockStyle.Fill;
            this.txtAuthKeyName.RightToLeft = RightToLeft.Yes;
            this.txtAuthKeyName.Name = "txtAuthKeyName";
            this.txtAuthKeyValue.Dock = DockStyle.Left;
            this.txtAuthKeyValue.Size = new Size(200, 24);
            this.txtAuthKeyValue.Name = "txtAuthKeyValue";
            var lblKeyValue = new Label { Text = "Key Value:", Dock = DockStyle.Left, Width = 80, TextAlign = ContentAlignment.MiddleRight };
            pnlKey.Controls.Add(this.txtAuthKeyName);
            pnlKey.Controls.Add(this.txtAuthKeyValue);
            pnlKey.Controls.Add(lblKeyValue);
            AddTableRow(tbl, "Key Name:", pnlKey, 5);
            foreach (Control c in tbl.Controls) { if (c is Label l && l.Text == "Key Name:") l.Name = "lblAuthKeyName"; }

            // Row 6: Headers
            this.txtApiHeaders.Dock = DockStyle.Fill;
            this.txtApiHeaders.Font = new Font("Consolas", 9F);
            this.txtApiHeaders.Text = "{}";
            this.txtApiHeaders.RightToLeft = RightToLeft.Yes;
            AddTableRow(tbl, "Headers (JSON):", this.txtApiHeaders, 6);

            // Row 7: JSON Template hint
            this.lblApiJsonHint.Text = "JSON Template (فقط حالت base64 — خالی=builtin). متغیرها: {filename} {base64} {size} {md5}";
            this.lblApiJsonHint.Dock = DockStyle.Fill;
            this.lblApiJsonHint.Font = new Font("Tahoma", 8F);
            this.lblApiJsonHint.ForeColor = TextMedium;
            this.lblApiJsonHint.TextAlign = ContentAlignment.MiddleRight;
            this.lblApiJsonHint.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.lblApiJsonHint, 0, 7);
            tbl.SetColumnSpan(this.lblApiJsonHint, 2);

            // Row 8: JSON Template textbox
            this.txtApiJsonTemplate.Dock = DockStyle.Fill;
            this.txtApiJsonTemplate.Multiline = true;
            this.txtApiJsonTemplate.ScrollBars = ScrollBars.Vertical;
            this.txtApiJsonTemplate.Font = new Font("Consolas", 9F);
            this.txtApiJsonTemplate.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(this.txtApiJsonTemplate, 0, 8);
            tbl.SetColumnSpan(this.txtApiJsonTemplate, 2);

            this.grpApiUpload.Controls.Add(tbl);
        }

        private void AddTableRow(TableLayoutPanel tbl, string labelText, Control input, int row)
        {
            var lbl = new Label();
            lbl.Text = labelText;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.RightToLeft = RightToLeft.Yes;
            tbl.Controls.Add(lbl, 0, row);
            tbl.Controls.Add(input, 1, row);
        }

        private void BuildTextProcessingGroup()
        {
            this.grpTextProcessing.Text = "پارامترهای پردازش متن";
            this.grpTextProcessing.Size = new Size(700, 250);
            this.grpTextProcessing.BackColor = BgPanel;
            this.grpTextProcessing.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpTextProcessing.RightToLeft = RightToLeft.Yes;

            var lblExt = new Label();
            lblExt.Text = "پسوندها:";
            lblExt.Location = new Point(580, 22);
            lblExt.Size = new Size(80, 24);
            lblExt.TextAlign = ContentAlignment.MiddleRight;

            this.txtTextExtensions.Location = new Point(330, 22);
            this.txtTextExtensions.Size = new Size(240, 24);

            var lblEnc = new Label();
            lblEnc.Text = "Encoding:";
            lblEnc.Location = new Point(310, 22);
            lblEnc.Size = new Size(70, 24);
            lblEnc.TextAlign = ContentAlignment.MiddleRight;

            this.cmbTextEncoding.Location = new Point(160, 22);
            this.cmbTextEncoding.Size = new Size(140, 24);
            this.cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTextEncoding.RightToLeft = RightToLeft.Yes;
            this.cmbTextEncoding.Items.AddRange(new object[] { "utf-8", "utf-8-bom", "utf-16", "utf-16-be", "ascii", "windows-1256" });

            // Checkboxes
            this.chkTextBackup.Text = "پشتیبان (.bak)";
            this.chkTextBackup.Location = new Point(570, 55);
            this.chkTextBackup.Size = new Size(110, 24);

            this.chkTextFR.Text = "Find/Replace";
            this.chkTextFR.Location = new Point(450, 55);
            this.chkTextFR.Size = new Size(110, 24);

            this.chkTextHeader.Text = "Header";
            this.chkTextHeader.Location = new Point(370, 55);
            this.chkTextHeader.Size = new Size(70, 24);

            this.chkTextFooter.Text = "Footer";
            this.chkTextFooter.Location = new Point(290, 55);
            this.chkTextFooter.Size = new Size(70, 24);

            this.chkTextAppend.Text = "Append";
            this.chkTextAppend.Location = new Point(210, 55);
            this.chkTextAppend.Size = new Size(70, 24);

            this.chkTextPrepend.Text = "Prepend";
            this.chkTextPrepend.Location = new Point(130, 55);
            this.chkTextPrepend.Size = new Size(70, 24);

            // Header/Footer/Append/Prepend textboxes (right side)
            this.txtTextHeader.Location = new Point(370, 85);
            this.txtTextHeader.Size = new Size(320, 24);

            this.txtTextFooter.Location = new Point(370, 115);
            this.txtTextFooter.Size = new Size(320, 24);

            this.txtTextAppend.Location = new Point(370, 145);
            this.txtTextAppend.Size = new Size(320, 24);

            this.txtTextPrepend.Location = new Point(370, 175);
            this.txtTextPrepend.Size = new Size(320, 24);

            // Find/Replace rules label + grid (left side)
            var lblRules = new Label();
            lblRules.Text = "قوانین Find/Replace:";
            lblRules.Location = new Point(30, 85);
            lblRules.Size = new Size(330, 20);
            lblRules.Font = new Font("Tahoma", 8.5F);
            lblRules.ForeColor = TextMedium;
            lblRules.TextAlign = ContentAlignment.MiddleRight;

            this.dgvTextRules.Location = new Point(30, 105);
            this.dgvTextRules.Size = new Size(330, 115);
            this.dgvTextRules.AllowUserToAddRows = true;
            this.dgvTextRules.AllowUserToDeleteRows = true;
            this.dgvTextRules.RowHeadersVisible = false;
            this.dgvTextRules.RowHeadersWidth = 4;
            this.dgvTextRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTextRules.BackgroundColor = BgPanel;
            this.dgvTextRules.BorderStyle = BorderStyle.FixedSingle;
            this.dgvTextRules.EnableHeadersVisualStyles = false;
            this.dgvTextRules.RightToLeft = RightToLeft.Yes;
            this.dgvTextRules.ColumnHeadersDefaultCellStyle.BackColor = BgGridHeader;
            this.dgvTextRules.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            this.dgvTextRules.Columns.Add("find", "Find");
            this.dgvTextRules.Columns.Add("replace", "Replace");
            this.dgvTextRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Regex", Name = "regex" });
            this.dgvTextRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "حساس", Name = "case" });

            this.grpTextProcessing.Controls.Add(lblExt);
            this.grpTextProcessing.Controls.Add(this.txtTextExtensions);
            this.grpTextProcessing.Controls.Add(lblEnc);
            this.grpTextProcessing.Controls.Add(this.cmbTextEncoding);
            this.grpTextProcessing.Controls.Add(this.chkTextBackup);
            this.grpTextProcessing.Controls.Add(this.chkTextFR);
            this.grpTextProcessing.Controls.Add(this.chkTextHeader);
            this.grpTextProcessing.Controls.Add(this.chkTextFooter);
            this.grpTextProcessing.Controls.Add(this.chkTextAppend);
            this.grpTextProcessing.Controls.Add(this.chkTextPrepend);
            this.grpTextProcessing.Controls.Add(this.txtTextHeader);
            this.grpTextProcessing.Controls.Add(this.txtTextFooter);
            this.grpTextProcessing.Controls.Add(this.txtTextAppend);
            this.grpTextProcessing.Controls.Add(this.txtTextPrepend);
            this.grpTextProcessing.Controls.Add(lblRules);
            this.grpTextProcessing.Controls.Add(this.dgvTextRules);
        }

        private void BuildAdvancedGroup()
        {
            this.grpAdvanced.Text = "تنظیمات پیشرفته (Retry و خطا)";
            this.grpAdvanced.Size = new Size(700, 80);
            this.grpAdvanced.BackColor = BgPanel;
            this.grpAdvanced.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpAdvanced.RightToLeft = RightToLeft.Yes;

            var lblRetry = new Label();
            lblRetry.Text = "Retry:";
            lblRetry.Location = new Point(580, 28);
            lblRetry.Size = new Size(80, 24);
            lblRetry.TextAlign = ContentAlignment.MiddleRight;

            this.numRetry.Location = new Point(470, 28);
            this.numRetry.Size = new Size(100, 24);
            this.numRetry.Minimum = 0;
            this.numRetry.Maximum = 100;

            var lblRetryDelay = new Label();
            lblRetryDelay.Text = "Retry Delay (ms):";
            lblRetryDelay.Location = new Point(440, 28);
            lblRetryDelay.Size = new Size(120, 24);
            lblRetryDelay.TextAlign = ContentAlignment.MiddleRight;

            this.numRetryDelay.Location = new Point(310, 28);
            this.numRetryDelay.Size = new Size(120, 24);
            this.numRetryDelay.Minimum = 0;
            this.numRetryDelay.Maximum = 3600000;

            this.chkContinueOnFail.Text = "ادامه زنجیره در صورت خطا";
            this.chkContinueOnFail.Location = new Point(30, 28);
            this.chkContinueOnFail.Size = new Size(260, 24);

            this.grpAdvanced.Controls.Add(lblRetry);
            this.grpAdvanced.Controls.Add(this.numRetry);
            this.grpAdvanced.Controls.Add(lblRetryDelay);
            this.grpAdvanced.Controls.Add(this.numRetryDelay);
            this.grpAdvanced.Controls.Add(this.chkContinueOnFail);
        }

        /// <summary>
        /// Repositions visible groups to stack vertically from top of scroll panel.
        /// </summary>
        private void UpdateGroupVisibility()
        {
            if (cmbType.SelectedItem == null) return;
            if (!Enum.TryParse<ActionType>(cmbType.SelectedItem.ToString(), out var t)) return;

            // Hide all optional groups first
            grpCommon.Visible = false;
            grpCommand.Visible = false;
            grpApiUpload.Visible = false;
            grpTextProcessing.Visible = false;

            // Show relevant groups based on action type
            switch (t)
            {
                case ActionType.Copy:
                case ActionType.Move:
                case ActionType.Rename:
                case ActionType.Zip:
                case ActionType.ZipAndMove:
                case ActionType.Extract:
                    grpCommon.Visible = true;
                    break;
                case ActionType.CustomCommand:
                    grpCommon.Visible = true;
                    grpCommand.Visible = true;
                    break;
                case ActionType.ApiUpload:
                    grpApiUpload.Visible = true;
                    break;
                case ActionType.TextProcessing:
                    grpTextProcessing.Visible = true;
                    break;
            }

            // Stack visible groups from top
            int y = 5;
            int gap = 8;
            int x = 0; // left-aligned within scroll panel

            if (grpCommon.Visible)
            {
                grpCommon.Location = new Point(x, y);
                y += grpCommon.Height + gap;
            }
            if (grpCommand.Visible)
            {
                grpCommand.Location = new Point(x, y);
                y += grpCommand.Height + gap;
            }
            if (grpApiUpload.Visible)
            {
                grpApiUpload.Location = new Point(x, y);
                y += grpApiUpload.Height + gap;
            }
            if (grpTextProcessing.Visible)
            {
                grpTextProcessing.Location = new Point(x, y);
                y += grpTextProcessing.Height + gap;
            }

            // Advanced group always visible
            grpAdvanced.Location = new Point(x, y);
            grpAdvanced.Visible = true;
            y += grpAdvanced.Height + gap;
        }

        private void UpdateAuthVisibility()
        {
            if (cmbAuthType.SelectedItem == null) return;
            if (!Enum.TryParse<ApiAuthType>(cmbAuthType.SelectedItem.ToString(), out var t)) return;

            // Hide all auth fields first
            txtAuthUsername.Visible = false;
            txtAuthPassword.Visible = false;
            txtAuthToken.Visible = false;
            txtAuthKeyName.Visible = false;
            txtAuthKeyValue.Visible = false;

            // Find the TableLayoutPanel inside grpApiUpload and search its controls
            TableLayoutPanel tbl = null;
            foreach (Control c in grpApiUpload.Controls)
            {
                if (c is TableLayoutPanel t) { tbl = t; break; }
            }

            if (tbl != null)
            {
                foreach (Control c in tbl.Controls)
                {
                    if (c is Label lbl && lbl.Name.StartsWith("lblAuth"))
                        lbl.Visible = false;
                }
            }

            switch (t)
            {
                case ApiAuthType.Basic:
                    txtAuthUsername.Visible = true;
                    txtAuthPassword.Visible = true;
                    ShowAuthLabel(tbl, "lblAuthUser");
                    break;
                case ApiAuthType.Bearer:
                    txtAuthToken.Visible = true;
                    ShowAuthLabel(tbl, "lblAuthToken");
                    break;
                case ApiAuthType.ApiKeyHeader:
                case ApiAuthType.ApiKeyQuery:
                    txtAuthKeyName.Visible = true;
                    txtAuthKeyValue.Visible = true;
                    ShowAuthLabel(tbl, "lblAuthKeyName");
                    break;
            }
        }

        private void ShowAuthLabel(TableLayoutPanel tbl, string name)
        {
            if (tbl == null) return;
            foreach (Control c in tbl.Controls)
            {
                if (c is Label lbl && lbl.Name == name)
                {
                    lbl.Visible = true;
                    return;
                }
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
                    lblDescription.Text = "اجرای فایل اجرایی دلخواه با آرگومان‌ها. از {filepath} برای مسیر فایل استفاده کنید.";
                    break;
                case ActionType.TextProcessing:
                    lblDescription.Text = "پردازش محتوای فایل‌های متنی (Find/Replace، Header/Footer، Append/Prepend).";
                    break;
                case ActionType.ApiUpload:
                    lblDescription.Text = "آپلود فایل به یک API از طریق HTTP. دو حالت: Multipart (form-data) یا Base64 (JSON).";
                    break;
                default:
                    lblDescription.Text = "";
                    break;
            }
        }

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

            // Command
            txtCommandExe.Text = _action.CommandExecutable;
            txtCommandArgs.Text = _action.CommandArguments;
            txtWorkDir.Text = _action.WorkingDirectory;
            chkWaitForExit.Checked = _action.WaitForExit;
            numTimeout.Value = Math.Min(Math.Max(0, _action.TimeoutSeconds), 86400);

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
            txtAuthUsername.Text = _action.AuthUsername;
            txtAuthPassword.Text = _action.AuthPassword;
            txtAuthToken.Text = _action.AuthToken;
            txtAuthKeyName.Text = _action.AuthKeyName;
            txtAuthKeyValue.Text = _action.AuthKeyValue;
            txtApiHeaders.Text = _action.ApiHeaders;
            txtApiJsonTemplate.Text = _action.ApiJsonTemplate;

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
                    var rules = JsonConvert.DeserializeObject<System.Collections.Generic.List<FindReplaceRule>>(_action.TextFindReplaceRulesJson);
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

            // Advanced
            numRetry.Value = Math.Min(Math.Max(0, _action.RetryCount), 100);
            numRetryDelay.Value = Math.Min(Math.Max(0, _action.RetryDelayMs), 3600000);
            chkContinueOnFail.Checked = _action.ContinueOnFailure;

            UpdateAuthVisibility();
            UpdateDescription();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
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

            var rules = new System.Collections.Generic.List<FindReplaceRule>();
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
