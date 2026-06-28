using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Core;
using FileCollector.Models;

namespace FileCollector.Forms
{
    public partial class FolderConfigForm : Form
    {
        private readonly FolderConfig _folder;

        public FolderConfigForm(FolderConfig folder)
        {
            _folder = folder;
            InitializeComponent();
            WireEvents();
            LoadData();
        }

        private void WireEvents()
        {
            btnBrowseSource.Click += btnBrowseSource_Click;
            btnBrowseDest.Click += btnBrowseDest_Click;
            btnBrowseShare.Click += btnBrowseShare_Click;
            btnTestConnection.Click += btnTestConnection_Click;
            btnAddAction.Click += btnAddAction_Click;
            btnEditAction.Click += btnEditAction_Click;
            btnRemoveAction.Click += btnRemoveAction_Click;
            btnMoveUp.Click += btnMoveUp_Click;
            btnMoveDown.Click += btnMoveDown_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnVariables.Click += btnVariables_Click;
        }

        private void LoadData()
        {
            txtName.Text = _folder.Name;
            txtSourcePath.Text = _folder.SourcePath;
            chkIncludeSubfolders.Checked = _folder.IncludeSubfolders;
            txtFileFilter.Text = _folder.FileFilter;
            numMinSize.Value = _folder.MinSizeBytes;
            numMaxSize.Value = _folder.MaxSizeBytes;

            cmbWatchMode.SelectedItem = _folder.WatchMode;
            if (cmbWatchMode.SelectedIndex < 0 && cmbWatchMode.Items.Count > 0)
                cmbWatchMode.SelectedIndex = 0;

            numIntervalSeconds.Value = _folder.IntervalSeconds;
            chkEnabled.Checked = _folder.Enabled;

            cmbConflict.SelectedItem = _folder.ConflictStrategy;
            if (cmbConflict.SelectedIndex < 0 && cmbConflict.Items.Count > 0)
                cmbConflict.SelectedIndex = 0;

            txtDestination.Text = _folder.DestinationPath;
            txtSubfolderPattern.Text = _folder.DestinationSubfolderPattern;
            txtFilenamePattern.Text = _folder.DestinationFilenamePattern;

            chkEnableDedup.Checked = _folder.EnableDeduplication;

            // Text processing
            if (_folder.TextProcessing == null)
                _folder.TextProcessing = new TextProcessingConfig();

            var tp = _folder.TextProcessing;
            chkEnableTextProcessing.Checked = tp.Enabled;
            txtExtensions.Text = tp.Extensions;
            cmbEncoding.SelectedItem = tp.Encoding;
            if (cmbEncoding.SelectedIndex < 0 && cmbEncoding.Items.Count > 0)
                cmbEncoding.SelectedIndex = 0;
            chkBackup.Checked = tp.CreateBackup;
            chkFR.Checked = tp.EnableFindReplace;
            chkHeader.Checked = tp.EnableHeader;
            chkFooter.Checked = tp.EnableFooter;
            chkAppend.Checked = tp.EnableAppend;
            chkPrepend.Checked = tp.EnablePrepend;
            txtHeader.Text = tp.HeaderTemplate;
            txtFooter.Text = tp.FooterTemplate;
            txtAppend.Text = tp.AppendText;
            txtPrepend.Text = tp.PrependText;

            dgvFindReplace.Rows.Clear();
            foreach (var rule in tp.FindReplaceRules)
            {
                dgvFindReplace.Rows.Add(rule.Find, rule.Replace, rule.UseRegex, rule.CaseSensitive);
            }

            // Database
            if (_folder.DatabaseStorage == null)
                _folder.DatabaseStorage = new DatabaseConfig();

            var db = _folder.DatabaseStorage;
            chkEnableDb.Checked = db.Enabled;
            txtConnString.Text = db.ConnectionString;
            txtTableName.Text = db.TableName;
            cmbDbMode.SelectedItem = db.Mode;
            if (cmbDbMode.SelectedIndex < 0 && cmbDbMode.Items.Count > 0)
                cmbDbMode.SelectedIndex = 1;
            txtFileShare.Text = db.FileSharePath;
            numMaxFileSizeMb.Value = Math.Max(1, db.MaxFileSizeMb);
            chkSkipLarger.Checked = db.SkipLargerThanMax;
            chkCompress.Checked = db.CompressBeforeStoring;
            txtDbSubfolder.Text = db.SubfolderPattern;

            RefreshActionsList();
        }

        private void SaveData()
        {
            _folder.Name = txtName.Text;
            _folder.SourcePath = txtSourcePath.Text;
            _folder.IncludeSubfolders = chkIncludeSubfolders.Checked;
            _folder.FileFilter = string.IsNullOrWhiteSpace(txtFileFilter.Text) ? "*.*" : txtFileFilter.Text;
            _folder.MinSizeBytes = (long)numMinSize.Value;
            _folder.MaxSizeBytes = (long)numMaxSize.Value;
            _folder.WatchMode = cmbWatchMode.SelectedItem?.ToString() ?? "realtime";
            _folder.IntervalSeconds = (int)numIntervalSeconds.Value;
            _folder.Enabled = chkEnabled.Checked;
            _folder.ConflictStrategy = cmbConflict.SelectedItem?.ToString() ?? "rename";
            _folder.DestinationPath = txtDestination.Text;
            _folder.DestinationSubfolderPattern = txtSubfolderPattern.Text;
            _folder.DestinationFilenamePattern = txtFilenamePattern.Text;
            _folder.EnableDeduplication = chkEnableDedup.Checked;

            var tp = _folder.TextProcessing;
            tp.Enabled = chkEnableTextProcessing.Checked;
            tp.Extensions = txtExtensions.Text;
            tp.Encoding = cmbEncoding.SelectedItem?.ToString() ?? "utf-8";
            tp.CreateBackup = chkBackup.Checked;
            tp.EnableFindReplace = chkFR.Checked;
            tp.EnableHeader = chkHeader.Checked;
            tp.EnableFooter = chkFooter.Checked;
            tp.EnableAppend = chkAppend.Checked;
            tp.EnablePrepend = chkPrepend.Checked;
            tp.HeaderTemplate = txtHeader.Text;
            tp.FooterTemplate = txtFooter.Text;
            tp.AppendText = txtAppend.Text;
            tp.PrependText = txtPrepend.Text;

            tp.FindReplaceRules.Clear();
            foreach (DataGridViewRow row in dgvFindReplace.Rows)
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
                    tp.FindReplaceRules.Add(rule);
            }

            var db = _folder.DatabaseStorage;
            db.Enabled = chkEnableDb.Checked;
            db.ConnectionString = txtConnString.Text;
            db.TableName = txtTableName.Text;
            if (cmbDbMode.SelectedItem != null)
                db.Mode = (DatabaseStorageMode)cmbDbMode.SelectedItem;
            db.FileSharePath = txtFileShare.Text;
            db.MaxFileSizeMb = (int)numMaxFileSizeMb.Value;
            db.SkipLargerThanMax = chkSkipLarger.Checked;
            db.CompressBeforeStoring = chkCompress.Checked;
            db.SubfolderPattern = txtDbSubfolder.Text;
        }

        private void RefreshActionsList()
        {
            lstActions.Items.Clear();
            if (_folder.Actions != null)
            {
                foreach (var action in _folder.Actions)
                {
                    lstActions.Items.Add($"[{(action.Enabled ? "✓" : "✗")}] {action.Type} — {action.Name}");
                }
            }
        }

        // ===========================================================
        // EVENT HANDLERS
        // ===========================================================

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(txtSourcePath.Text)) dlg.SelectedPath = txtSourcePath.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtSourcePath.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowseDest_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(txtDestination.Text)) dlg.SelectedPath = txtDestination.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtDestination.Text = dlg.SelectedPath;
            }
        }

        private void btnBrowseShare_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(txtFileShare.Text)) dlg.SelectedPath = txtFileShare.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    txtFileShare.Text = dlg.SelectedPath;
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string conn = txtConnString.Text;
            if (string.IsNullOrEmpty(conn))
            {
                MessageBox.Show(this, "ابتدا Connection String وارد کنید.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (DatabaseManager.TestRemoteConnection(conn, out string error))
                {
                    var dbConfig = new DatabaseConfig
                    {
                        ConnectionString = conn,
                        TableName = txtTableName.Text,
                        Mode = (DatabaseStorageMode)cmbDbMode.SelectedItem
                    };
                    if (DatabaseManager.EnsureRemoteTable(dbConfig, out string tableErr))
                    {
                        MessageBox.Show(this, "اتصال موفق و جدول ساخته/تأیید شد.", "موفق",
                            MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                    else
                    {
                        MessageBox.Show(this, "اتصال موفق بود اما ساخت جدول ناموفق:\n" + tableErr, "هشدار",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    MessageBox.Show(this, "اتصال ناموفق:\n" + error, "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {
            if (_folder.Actions.Count >= 5)
            {
                MessageBox.Show(this, "حداکثر ۵ اکشن مجاز است.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            var action = new ActionConfig
            {
                Type = ActionType.Copy,
                Name = "Copy",
                Enabled = true
            };
            using (var form = new ActionEditorForm(action))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _folder.Actions.Add(action);
                    RefreshActionsList();
                }
            }
        }

        private void btnEditAction_Click(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndex < 0) return;
            var action = _folder.Actions[lstActions.SelectedIndex];
            using (var form = new ActionEditorForm(action))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshActionsList();
                }
            }
        }

        private void btnRemoveAction_Click(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndex < 0) return;
            _folder.Actions.RemoveAt(lstActions.SelectedIndex);
            RefreshActionsList();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int idx = lstActions.SelectedIndex;
            if (idx <= 0) return;
            var tmp = _folder.Actions[idx - 1];
            _folder.Actions[idx - 1] = _folder.Actions[idx];
            _folder.Actions[idx] = tmp;
            lstActions.SelectedIndex = idx - 1;
            RefreshActionsList();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int idx = lstActions.SelectedIndex;
            if (idx < 0 || idx >= _folder.Actions.Count - 1) return;
            var tmp = _folder.Actions[idx + 1];
            _folder.Actions[idx + 1] = _folder.Actions[idx];
            _folder.Actions[idx] = tmp;
            lstActions.SelectedIndex = idx + 1;
            RefreshActionsList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(this, "نام پوشه الزامی است.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtSourcePath.Text))
            {
                MessageBox.Show(this, "مسیر منبع الزامی است.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            SaveData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            var vars = string.Join("\n", VariableResolver.KnownVariables);
            MessageBox.Show(this, "متغیرهای قابل استفاده در الگوها:\n\n{" + vars.Replace("\n", "}\n{") + "}",
                "راهنمای متغیرها",
                MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }
}
