using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Models;

namespace FileCollector.Forms
{
    /// <summary>
    /// Editor dialog for a single ActionConfig in the action chain.
    /// </summary>
    public class ActionEditorForm : Form
    {
        private readonly ActionConfig _action;

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
        private Label lblType;
        private Label lblName;

        public ActionEditorForm(ActionConfig action)
        {
            _action = action;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.cmbType = new ComboBox();
            this.txtName = new TextBox();
            this.txtDestPath = new TextBox();
            this.txtFilename = new TextBox();
            this.txtCommandExe = new TextBox();
            this.txtCommandArgs = new TextBox();
            this.txtWorkDir = new TextBox();
            this.chkWaitForExit = new CheckBox();
            this.numTimeout = new NumericUpDown();
            this.numRetry = new NumericUpDown();
            this.numRetryDelay = new NumericUpDown();
            this.chkContinueOnFail = new CheckBox();
            this.chkEnabled = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.grpCommon = new GroupBox();
            this.grpCommand = new GroupBox();
            this.grpAdvanced = new GroupBox();
            this.lblType = new Label();
            this.lblName = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).BeginInit();

            // ----- Form -----
            this.Text = "تنظیمات اکشن";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9.75F);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ----- Type + Name -----
            this.lblType.Text = "نوع اکشن:";
            this.lblType.Location = new Point(20, 20);
            this.lblType.Size = new Size(100, 24);

            this.cmbType.Location = new Point(130, 20);
            this.cmbType.Size = new Size(200, 24);
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbType.Items.AddRange(Enum.GetNames(typeof(ActionType)));

            this.lblName.Text = "نام:";
            this.lblName.Location = new Point(20, 55);
            this.lblName.Size = new Size(100, 24);

            this.txtName.Location = new Point(130, 55);
            this.txtName.Size = new Size(300, 24);

            // ----- Common parameters group -----
            this.grpCommon.Text = "پارامترهای عمومی (مسیر مقصد و نام فایل)";
            this.grpCommon.Location = new Point(20, 90);
            this.grpCommon.Size = new Size(640, 110);
            this.grpCommon.BackColor = Color.White;
            this.grpCommon.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);

            this.txtDestPath.Location = new Point(120, 30);
            this.txtDestPath.Size = new Size(500, 24);
            this.txtDestPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.txtFilename.Location = new Point(120, 65);
            this.txtFilename.Size = new Size(500, 24);
            this.txtFilename.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var lblDest = new Label { Text = "مسیر مقصد:", Location = new Point(10, 30), Size = new Size(100, 24) };
            var lblFile = new Label { Text = "الگوی نام فایل:", Location = new Point(10, 65), Size = new Size(100, 24) };
            this.grpCommon.Controls.Add(lblDest);
            this.grpCommon.Controls.Add(lblFile);
            this.grpCommon.Controls.Add(this.txtDestPath);
            this.grpCommon.Controls.Add(this.txtFilename);

            // ----- Command group -----
            this.grpCommand.Text = "پارامترهای Command سفارشی";
            this.grpCommand.Location = new Point(20, 210);
            this.grpCommand.Size = new Size(640, 160);
            this.grpCommand.BackColor = Color.White;
            this.grpCommand.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);

            var lblExe = new Label { Text = "Executable:", Location = new Point(10, 30), Size = new Size(100, 24) };
            var lblArgs = new Label { Text = "Arguments:", Location = new Point(10, 65), Size = new Size(100, 24) };
            var lblWd = new Label { Text = "Working Dir:", Location = new Point(10, 100), Size = new Size(100, 24) };
            var lblTo = new Label { Text = "Timeout (s):", Location = new Point(10, 135), Size = new Size(100, 24) };

            this.txtCommandExe.Location = new Point(120, 30);
            this.txtCommandExe.Size = new Size(500, 24);
            this.txtCommandArgs.Location = new Point(120, 65);
            this.txtCommandArgs.Size = new Size(500, 24);
            this.txtWorkDir.Location = new Point(120, 100);
            this.txtWorkDir.Size = new Size(500, 24);
            this.numTimeout.Location = new Point(120, 135);
            this.numTimeout.Size = new Size(80, 24);

            this.chkWaitForExit.Text = "Wait for exit";
            this.chkWaitForExit.Location = new Point(220, 135);
            this.chkWaitForExit.Size = new Size(120, 24);

            this.grpCommand.Controls.Add(lblExe);
            this.grpCommand.Controls.Add(lblArgs);
            this.grpCommand.Controls.Add(lblWd);
            this.grpCommand.Controls.Add(lblTo);
            this.grpCommand.Controls.Add(this.txtCommandExe);
            this.grpCommand.Controls.Add(this.txtCommandArgs);
            this.grpCommand.Controls.Add(this.txtWorkDir);
            this.grpCommand.Controls.Add(this.numTimeout);
            this.grpCommand.Controls.Add(this.chkWaitForExit);

            // ----- Advanced group -----
            this.grpAdvanced.Text = "تنظیمات پیشرفته";
            this.grpAdvanced.Location = new Point(20, 380);
            this.grpAdvanced.Size = new Size(640, 110);
            this.grpAdvanced.BackColor = Color.White;
            this.grpAdvanced.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);

            var lblRetry = new Label { Text = "Retry:", Location = new Point(10, 30), Size = new Size(80, 24) };
            var lblRetryDelay = new Label { Text = "Retry Delay (ms):", Location = new Point(220, 30), Size = new Size(120, 24) };

            this.numRetry.Location = new Point(100, 30);
            this.numRetry.Size = new Size(80, 24);
            this.numRetryDelay.Location = new Point(350, 30);
            this.numRetryDelay.Size = new Size(80, 24);

            this.chkContinueOnFail.Text = "ادامه زنجیره در صورت خطا";
            this.chkContinueOnFail.Location = new Point(10, 65);
            this.chkContinueOnFail.Size = new Size(200, 24);

            this.chkEnabled.Text = "فعال";
            this.chkEnabled.Location = new Point(220, 65);
            this.chkEnabled.Size = new Size(100, 24);

            this.grpAdvanced.Controls.Add(lblRetry);
            this.grpAdvanced.Controls.Add(lblRetryDelay);
            this.grpAdvanced.Controls.Add(this.numRetry);
            this.grpAdvanced.Controls.Add(this.numRetryDelay);
            this.grpAdvanced.Controls.Add(this.chkContinueOnFail);
            this.grpAdvanced.Controls.Add(this.chkEnabled);

            // ----- OK / Cancel -----
            this.btnOK.Text = "تأیید";
            this.btnOK.Size = new Size(100, 32);
            this.btnOK.Location = new Point(440, 510);
            this.btnOK.BackColor = Color.White;
            this.btnOK.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderSize = 1;
            this.btnOK.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            this.btnOK.Click += BtnOK_Click;

            this.btnCancel.Text = "انصراف";
            this.btnCancel.Size = new Size(100, 32);
            this.btnCancel.Location = new Point(550, 510);
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.ForeColor = Color.FromArgb(51, 51, 51);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 215);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // ----- Add to form -----
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.grpCommon);
            this.Controls.Add(this.grpCommand);
            this.Controls.Add(this.grpAdvanced);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;

            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).EndInit();
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
