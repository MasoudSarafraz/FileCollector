using System;
using System.Drawing;
using System.Windows.Forms;
using FileCollector.Core;
using FileCollector.Models;
using Newtonsoft.Json;

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
            btnBrowseShare.Click += btnBrowseShare_Click;
            btnTestConnection.Click += btnTestConnection_Click;
            btnAddAction.Click += btnAddAction_Click;
            btnEditAction.Click += btnEditAction_Click;
            btnRemoveAction.Click += btnRemoveAction_Click;
            btnMoveUp.Click += btnMoveUp_Click;
            btnMoveDown.Click += btnMoveDown_Click;
            dgvActions.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) btnEditAction_Click(s, e); };
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnVariables.Click += btnVariables_Click;

            // Auto-set folder name from source path when the path changes
            // (only if name is still empty or default "پوشه جدید")
            txtSourcePath.Leave += (s, e) =>
            {
                if ((string.IsNullOrWhiteSpace(txtName.Text) || txtName.Text == "پوشه جدید")
                    && !string.IsNullOrWhiteSpace(txtSourcePath.Text))
                {
                    try
                    {
                        txtName.Text = System.IO.Path.GetFileName(
                            txtSourcePath.Text.TrimEnd('\\', '/'));
                    }
                    catch { }
                }
            };
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

            // Database
            if (_folder.DatabaseStorage == null)
                _folder.DatabaseStorage = new DatabaseConfig();

            var db = _folder.DatabaseStorage;
            chkEnableDb.Checked = db.Enabled;
            txtConnString.Text = db.ConnectionString;
            txtTableName.Text = db.TableName;
            // Find the DbModeItem whose Mode matches db.Mode
            cmbDbMode.SelectedIndex = -1;
            for (int i = 0; i < cmbDbMode.Items.Count; i++)
            {
                if (cmbDbMode.Items[i] is DbModeItem item && item.Mode == db.Mode)
                {
                    cmbDbMode.SelectedIndex = i;
                    break;
                }
            }
            if (cmbDbMode.SelectedIndex < 0 && cmbDbMode.Items.Count > 0)
                cmbDbMode.SelectedIndex = 1;
            txtFileShare.Text = db.FileSharePath;
            numMaxFileSizeMb.Value = Math.Min(10240, Math.Max(1, db.MaxFileSizeMb));
            chkSkipLarger.Checked = db.SkipLargerThanMax;
            chkCompress.Checked = db.CompressBeforeStoring;
            txtDbSubfolder.Text = db.SubfolderPattern;

            RefreshActionsGrid();
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

            // Database
            var db = _folder.DatabaseStorage;
            db.Enabled = chkEnableDb.Checked;
            db.ConnectionString = txtConnString.Text;
            db.TableName = txtTableName.Text;
            if (cmbDbMode.SelectedItem is DbModeItem modeItem)
                db.Mode = modeItem.Mode;
            db.FileSharePath = txtFileShare.Text;
            db.MaxFileSizeMb = (int)numMaxFileSizeMb.Value;
            db.SkipLargerThanMax = chkSkipLarger.Checked;
            db.CompressBeforeStoring = chkCompress.Checked;
            db.SubfolderPattern = txtDbSubfolder.Text;
        }

        /// <summary>
        /// Refreshes the actions DataGridView from the folder's Actions list.
        /// </summary>
        private void RefreshActionsGrid()
        {
            dgvActions.Rows.Clear();
            if (_folder.Actions != null)
            {
                foreach (var action in _folder.Actions)
                {
                    // Show a summary of the action's destination/parameters
                    string destSummary = GetActionSummary(action);

                    int rowIdx = dgvActions.Rows.Add();
                    var row = dgvActions.Rows[rowIdx];
                    row.Cells[0].Value = action.Enabled ? "✓" : "✗";
                    row.Cells[1].Value = action.Type.ToString();
                    row.Cells[2].Value = action.Name;
                    row.Cells[3].Value = destSummary;
                }
            }
        }

        /// <summary>
        /// Builds a short summary string for an action (destination, command, API URL, etc.)
        /// </summary>
        private string GetActionSummary(ActionConfig action)
        {
            switch (action.Type)
            {
                case ActionType.Copy:
                case ActionType.Move:
                case ActionType.Zip:
                case ActionType.ZipAndMove:
                case ActionType.Extract:
                    return action.DestinationPath;

                case ActionType.Rename:
                    return action.FilenamePattern;

                case ActionType.CustomCommand:
                    return action.CommandExecutable + " " + action.CommandArguments;

                case ActionType.ApiUpload:
                    return action.ApiUploadMode + " → " + action.ApiUrl;

                case ActionType.TextProcessing:
                    var tp = action.TextProcessingConfig;
                    var parts = new System.Collections.Generic.List<string>();
                    if (tp.EnableFindReplace) parts.Add("Find/Replace");
                    if (tp.EnableHeader) parts.Add("Header");
                    if (tp.EnableFooter) parts.Add("Footer");
                    if (tp.EnableAppend) parts.Add("Append");
                    if (tp.EnablePrepend) parts.Add("Prepend");
                    return parts.Count > 0 ? string.Join(", ", parts) : "(no operations enabled)";

                default:
                    return "";
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
                        Mode = (cmbDbMode.SelectedItem is DbModeItem mi) ? mi.Mode : DatabaseStorageMode.Hybrid
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
                    RefreshActionsGrid();
                }
            }
        }

        private void btnEditAction_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count == 0) return;
            int idx = dgvActions.SelectedRows[0].Index;
            if (idx < 0 || idx >= _folder.Actions.Count) return;

            var action = _folder.Actions[idx];
            using (var form = new ActionEditorForm(action))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshActionsGrid();
                }
            }
        }

        private void btnRemoveAction_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count == 0) return;
            int idx = dgvActions.SelectedRows[0].Index;
            if (idx < 0 || idx >= _folder.Actions.Count) return;

            _folder.Actions.RemoveAt(idx);
            RefreshActionsGrid();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count == 0) return;
            int idx = dgvActions.SelectedRows[0].Index;
            if (idx <= 0) return;
            var tmp = _folder.Actions[idx - 1];
            _folder.Actions[idx - 1] = _folder.Actions[idx];
            _folder.Actions[idx] = tmp;
            RefreshActionsGrid();
            dgvActions.Rows[idx - 1].Selected = true;
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count == 0) return;
            int idx = dgvActions.SelectedRows[0].Index;
            if (idx < 0 || idx >= _folder.Actions.Count - 1) return;
            var tmp = _folder.Actions[idx + 1];
            _folder.Actions[idx + 1] = _folder.Actions[idx];
            _folder.Actions[idx] = tmp;
            RefreshActionsGrid();
            dgvActions.Rows[idx + 1].Selected = true;
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

    /// <summary>
    /// Helper class for displaying DatabaseStorageMode enum values
    /// with localized Persian names in the combo box.
    /// </summary>
    public class DbModeItem
    {
        public DatabaseStorageMode Mode { get; set; }
        public string DisplayName { get; set; }

        public DbModeItem(DatabaseStorageMode mode, string displayName)
        {
            Mode = mode;
            DisplayName = displayName;
        }

        public override string ToString() => DisplayName;
    }
}
