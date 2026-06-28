using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;
using Newtonsoft.Json;

namespace FileCollector.Forms
{
    /// <summary>
    /// Editor dialog for a single ActionConfig in the action chain.
    /// Shows different parameter groups depending on the selected ActionType.
    /// </summary>
    public class ActionEditorForm : Form
    {
        private readonly ActionConfig _action;

        // Common controls
        private ComboBox cmbType;
        private TextBox txtName;
        private CheckBox chkEnabled;

        // Common parameters (Copy/Move/Zip/etc.)
        private GroupBox grpCommon;
        private TextBox txtDestPath;
        private TextBox txtFilename;

        // Custom command parameters
        private GroupBox grpCommand;
        private TextBox txtCommandExe;
        private TextBox txtCommandArgs;
        private TextBox txtWorkDir;
        private CheckBox chkWaitForExit;
        private NumericUpDown numTimeout;

        // API Upload parameters
        private GroupBox grpApiUpload;
        private TextBox txtApiUrl;
        private ComboBox cmbApiMode;
        private TextBox txtApiHeaders;
        private TextBox txtApiJsonTemplate;
        private NumericUpDown numApiTimeout;

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
        private Label lblTextRules;

        // Advanced
        private GroupBox grpAdvanced;
        private NumericUpDown numRetry;
        private NumericUpDown numRetryDelay;
        private CheckBox chkContinueOnFail;

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
            cmbType.SelectedIndexChanged += (s, e) => UpdateGroupVisibility();
        }

        private void InitializeComponent()
        {
            this.cmbType = new ComboBox();
            this.txtName = new TextBox();
            this.chkEnabled = new CheckBox();
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
            this.txtApiUrl = new TextBox();
            this.cmbApiMode = new ComboBox();
            this.txtApiHeaders = new TextBox();
            this.txtApiJsonTemplate = new TextBox();
            this.numApiTimeout = new NumericUpDown();
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
            this.lblTextRules = new Label();
            this.grpAdvanced = new GroupBox();
            this.numRetry = new NumericUpDown();
            this.numRetryDelay = new NumericUpDown();
            this.chkContinueOnFail = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).BeginInit();

            // ----- Form -----
            this.Text = "تنظیمات اکشن";
            this.Size = new Size(740, 820);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9.75F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = BgForm;

            // ----- Type + Name -----
            var lblType = new Label { Text = "نوع اکشن:", Location = new Point(20, 15), Size = new Size(100, 24) };
            this.cmbType.Location = new Point(130, 15);
            this.cmbType.Size = new Size(200, 24);
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.RightToLeft = RightToLeft.Yes;
            this.cmbType.Items.AddRange(Enum.GetNames(typeof(ActionType)));

            var lblName = new Label { Text = "نام:", Location = new Point(20, 50), Size = new Size(100, 24) };
            this.txtName.Location = new Point(130, 50);
            this.txtName.Size = new Size(300, 24);

            this.chkEnabled.Text = "فعال";
            this.chkEnabled.Location = new Point(450, 50);
            this.chkEnabled.Size = new Size(80, 24);

            // ----- Common parameters group (Copy/Move/Zip/etc.) -----
            this.grpCommon.Text = "پارامترهای فایل (مسیر مقصد و نام فایل)";
            this.grpCommon.Location = new Point(20, 85);
            this.grpCommon.Size = new Size(680, 100);
            this.grpCommon.BackColor = BgPanel;
            this.grpCommon.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpCommon.RightToLeft = RightToLeft.Yes;

            var lblDest = new Label { Text = "مسیر مقصد:", Location = new Point(10, 28), Size = new Size(100, 24) };
            this.txtDestPath.Location = new Point(120, 28);
            this.txtDestPath.Size = new Size(540, 24);
            this.txtDestPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblFile = new Label { Text = "الگوی نام فایل:", Location = new Point(10, 60), Size = new Size(100, 24) };
            this.txtFilename.Location = new Point(120, 60);
            this.txtFilename.Size = new Size(540, 24);
            this.txtFilename.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.grpCommon.Controls.Add(lblDest);
            this.grpCommon.Controls.Add(this.txtDestPath);
            this.grpCommon.Controls.Add(lblFile);
            this.grpCommon.Controls.Add(this.txtFilename);

            // ----- Command group -----
            this.grpCommand.Text = "پارامترهای Command سفارشی";
            this.grpCommand.Location = new Point(20, 195);
            this.grpCommand.Size = new Size(680, 150);
            this.grpCommand.BackColor = BgPanel;
            this.grpCommand.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpCommand.RightToLeft = RightToLeft.Yes;

            this.grpCommand.Controls.Add(new Label { Text = "Executable:", Location = new Point(10, 25), Size = new Size(100, 24) });
            this.grpCommand.Controls.Add(new Label { Text = "Arguments:", Location = new Point(10, 55), Size = new Size(100, 24) });
            this.grpCommand.Controls.Add(new Label { Text = "Working Dir:", Location = new Point(10, 85), Size = new Size(100, 24) });
            this.grpCommand.Controls.Add(new Label { Text = "Timeout (s):", Location = new Point(10, 115), Size = new Size(100, 24) });

            this.txtCommandExe.Location = new Point(120, 25);
            this.txtCommandExe.Size = new Size(540, 24);
            this.txtCommandArgs.Location = new Point(120, 55);
            this.txtCommandArgs.Size = new Size(540, 24);
            this.txtWorkDir.Location = new Point(120, 85);
            this.txtWorkDir.Size = new Size(540, 24);
            this.numTimeout.Location = new Point(120, 115);
            this.numTimeout.Size = new Size(80, 24);
            this.numTimeout.Minimum = 0;
            this.numTimeout.Maximum = 86400;

            this.chkWaitForExit.Text = "Wait for exit";
            this.chkWaitForExit.Location = new Point(220, 115);
            this.chkWaitForExit.Size = new Size(120, 24);

            this.grpCommand.Controls.Add(this.txtCommandExe);
            this.grpCommand.Controls.Add(this.txtCommandArgs);
            this.grpCommand.Controls.Add(this.txtWorkDir);
            this.grpCommand.Controls.Add(this.numTimeout);
            this.grpCommand.Controls.Add(this.chkWaitForExit);

            // ----- API Upload group -----
            this.grpApiUpload.Text = "پارامترهای API (آپلود فایل)";
            this.grpApiUpload.Location = new Point(20, 355);
            this.grpApiUpload.Size = new Size(680, 200);
            this.grpApiUpload.BackColor = BgPanel;
            this.grpApiUpload.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpApiUpload.RightToLeft = RightToLeft.Yes;

            this.grpApiUpload.Controls.Add(new Label { Text = "URL:", Location = new Point(10, 25), Size = new Size(100, 24) });
            this.txtApiUrl.Location = new Point(120, 25);
            this.txtApiUrl.Size = new Size(540, 24);

            this.grpApiUpload.Controls.Add(new Label { Text = "حالت آپلود:", Location = new Point(10, 55), Size = new Size(100, 24) });
            this.cmbApiMode.Location = new Point(120, 55);
            this.cmbApiMode.Size = new Size(150, 24);
            this.cmbApiMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbApiMode.RightToLeft = RightToLeft.Yes;
            this.cmbApiMode.Items.AddRange(new object[] { "multipart", "base64" });

            this.grpApiUpload.Controls.Add(new Label { Text = "Timeout (s):", Location = new Point(290, 55), Size = new Size(80, 24) });
            this.numApiTimeout.Location = new Point(380, 55);
            this.numApiTimeout.Size = new Size(80, 24);
            this.numApiTimeout.Minimum = 5;
            this.numApiTimeout.Maximum = 3600;
            this.numApiTimeout.Value = 60;

            this.grpApiUpload.Controls.Add(new Label { Text = "Headers (JSON):", Location = new Point(10, 85), Size = new Size(100, 24) });
            this.txtApiHeaders.Location = new Point(120, 85);
            this.txtApiHeaders.Size = new Size(540, 24);
            this.txtApiHeaders.Font = new Font("Consolas", 9F);
            this.txtApiHeaders.Text = "{}";

            this.grpApiUpload.Controls.Add(new Label { Text = "JSON Template (base64):", Location = new Point(10, 115), Size = new Size(110, 24) });
            this.txtApiJsonTemplate.Location = new Point(120, 115);
            this.txtApiJsonTemplate.Size = new Size(540, 70);
            this.txtApiJsonTemplate.Multiline = true;
            this.txtApiJsonTemplate.ScrollBars = ScrollBars.Vertical;
            this.txtApiJsonTemplate.Font = new Font("Consolas", 9F);

            this.grpApiUpload.Controls.Add(this.txtApiUrl);
            this.grpApiUpload.Controls.Add(this.cmbApiMode);
            this.grpApiUpload.Controls.Add(this.numApiTimeout);
            this.grpApiUpload.Controls.Add(this.txtApiHeaders);
            this.grpApiUpload.Controls.Add(this.txtApiJsonTemplate);

            // ----- Text Processing group -----
            this.grpTextProcessing.Text = "پارامترهای پردازش متن";
            this.grpTextProcessing.Location = new Point(20, 565);
            this.grpTextProcessing.Size = new Size(680, 200);
            this.grpTextProcessing.BackColor = BgPanel;
            this.grpTextProcessing.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpTextProcessing.RightToLeft = RightToLeft.Yes;

            this.grpTextProcessing.Controls.Add(new Label { Text = "پسوندها:", Location = new Point(10, 25), Size = new Size(80, 24) });
            this.txtTextExtensions.Location = new Point(100, 25);
            this.txtTextExtensions.Size = new Size(250, 24);

            this.grpTextProcessing.Controls.Add(new Label { Text = "Encoding:", Location = new Point(360, 25), Size = new Size(70, 24) });
            this.cmbTextEncoding.Location = new Point(440, 25);
            this.cmbTextEncoding.Size = new Size(150, 24);
            this.cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTextEncoding.RightToLeft = RightToLeft.Yes;
            this.cmbTextEncoding.Items.AddRange(new object[] { "utf-8", "utf-8-bom", "utf-16", "utf-16-be", "ascii", "windows-1256" });

            this.chkTextBackup.Text = "پشتیبان (.bak)";
            this.chkTextBackup.Location = new Point(100, 55);
            this.chkTextBackup.Size = new Size(120, 24);

            this.chkTextFR.Text = "Find/Replace";
            this.chkTextFR.Location = new Point(230, 55);
            this.chkTextFR.Size = new Size(110, 24);

            this.chkTextHeader.Text = "Header";
            this.chkTextHeader.Location = new Point(350, 55);
            this.chkTextHeader.Size = new Size(70, 24);

            this.chkTextFooter.Text = "Footer";
            this.chkTextFooter.Location = new Point(430, 55);
            this.chkTextFooter.Size = new Size(70, 24);

            this.chkTextAppend.Text = "Append";
            this.chkTextAppend.Location = new Point(510, 55);
            this.chkTextAppend.Size = new Size(70, 24);

            this.chkTextPrepend.Text = "Prepend";
            this.chkTextPrepend.Location = new Point(590, 55);
            this.chkTextPrepend.Size = new Size(70, 24);

            this.txtTextHeader.Location = new Point(350, 85);
            this.txtTextHeader.Size = new Size(310, 24);
            this.txtTextFooter.Location = new Point(350, 115);
            this.txtTextFooter.Size = new Size(310, 24);
            this.txtTextAppend.Location = new Point(510, 145);
            this.txtTextAppend.Size = new Size(150, 24);
            this.txtTextPrepend.Location = new Point(510, 175);
            this.txtTextPrepend.Size = new Size(150, 24);

            this.lblTextRules.Text = "Find/Replace rules (Find, Replace, Regex, CaseSensitive):";
            this.lblTextRules.Location = new Point(10, 85);
            this.lblTextRules.Size = new Size(330, 20);
            this.lblTextRules.Font = new Font("Tahoma", 8.5F);
            this.lblTextRules.ForeColor = TextMedium;

            this.dgvTextRules.Location = new Point(10, 105);
            this.dgvTextRules.Size = new Size(330, 85);
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

            this.grpTextProcessing.Controls.Add(this.txtTextExtensions);
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
            this.grpTextProcessing.Controls.Add(this.lblTextRules);
            this.grpTextProcessing.Controls.Add(this.dgvTextRules);

            // ----- Advanced group -----
            this.grpAdvanced.Text = "تنظیمات پیشرفته (Retry و خطا)";
            this.grpAdvanced.Location = new Point(20, 775);
            this.grpAdvanced.Size = new Size(680, 80);
            this.grpAdvanced.BackColor = BgPanel;
            this.grpAdvanced.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            this.grpAdvanced.RightToLeft = RightToLeft.Yes;

            this.grpAdvanced.Controls.Add(new Label { Text = "Retry:", Location = new Point(10, 28), Size = new Size(80, 24) });
            this.numRetry.Location = new Point(100, 28);
            this.numRetry.Size = new Size(80, 24);
            this.numRetry.Minimum = 0;
            this.numRetry.Maximum = 100;

            this.grpAdvanced.Controls.Add(new Label { Text = "Retry Delay (ms):", Location = new Point(220, 28), Size = new Size(120, 24) });
            this.numRetryDelay.Location = new Point(350, 28);
            this.numRetryDelay.Size = new Size(80, 24);
            this.numRetryDelay.Minimum = 0;
            this.numRetryDelay.Maximum = 3600000;

            this.chkContinueOnFail.Text = "ادامه زنجیره در صورت خطا";
            this.chkContinueOnFail.Location = new Point(450, 28);
            this.chkContinueOnFail.Size = new Size(200, 24);

            this.grpAdvanced.Controls.Add(this.numRetry);
            this.grpAdvanced.Controls.Add(this.numRetryDelay);
            this.grpAdvanced.Controls.Add(this.chkContinueOnFail);

            // ----- OK / Cancel -----
            this.btnOK.Text = "تأیید";
            this.btnOK.Size = new Size(100, 32);
            this.btnOK.Location = new Point(490, 870);
            this.btnOK.BackColor = BgPanel;
            this.btnOK.ForeColor = TextDark;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 1;
            this.btnOK.FlatAppearance.BorderColor = BorderLight;
            this.btnOK.Click += BtnOK_Click;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.Location = new Point(600, 870);
            this.btnCancel.BackColor = BgPanel;
            this.btnCancel.ForeColor = TextDark;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = BorderLight;
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // ----- Add to form -----
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.grpCommon);
            this.Controls.Add(this.grpCommand);
            this.Controls.Add(this.grpApiUpload);
            this.Controls.Add(this.grpTextProcessing);
            this.Controls.Add(this.grpAdvanced);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            // Also add the labels
            this.Controls.Add(new Label { Text = "نوع اکشن:", Location = new Point(20, 15), Size = new Size(100, 24) });
            this.Controls.Add(new Label { Text = "نام:", Location = new Point(20, 50), Size = new Size(100, 24) });

            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numApiTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTextRules)).EndInit();
        }

        /// <summary>
        /// Shows/hides parameter groups based on the selected ActionType.
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

                case ActionType.DatabaseStore:
                case ActionType.Delete:
                case ActionType.Recycle:
                    // No additional parameters needed
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
            txtApiUrl.Text = _action.ApiUrl;
            cmbApiMode.SelectedItem = _action.ApiUploadMode;
            if (cmbApiMode.SelectedIndex < 0 && cmbApiMode.Items.Count > 0)
                cmbApiMode.SelectedIndex = 0;
            txtApiHeaders.Text = _action.ApiHeaders;
            txtApiJsonTemplate.Text = _action.ApiJsonTemplate;
            numApiTimeout.Value = Math.Min(Math.Max(5, _action.ApiTimeoutSeconds), 3600);

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

            // Load find/replace rules
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
            _action.ApiUrl = txtApiUrl.Text;
            _action.ApiUploadMode = cmbApiMode.SelectedItem?.ToString() ?? "multipart";
            _action.ApiHeaders = txtApiHeaders.Text;
            _action.ApiJsonTemplate = txtApiJsonTemplate.Text;
            _action.ApiTimeoutSeconds = (int)numApiTimeout.Value;

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

            // Save find/replace rules as JSON
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
