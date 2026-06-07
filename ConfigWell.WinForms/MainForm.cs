using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ConfigWell.WinForms.Controls;
using ConfigWell.WinForms.Models;
using ConfigWell.WinForms.Services;

namespace ConfigWell.WinForms
{
    public partial class MainForm : Form
    {
        private readonly FileService _fileService = new FileService();
        private readonly BackupService _backupService = new BackupService();
        private readonly ConfigWriterService _writerService = new ConfigWriterService();

        private static readonly string[] ProtectedPaths =
        {
            @"C:\Windows",
            @"C:\Program Files",
            @"C:\Program Files (x86)"
        };

        private static readonly string[] SupportedExtensions =
            { ".properties", ".conf", ".ini", ".definition", ".xml", ".config", ".bat", ".cmd", ".ps1" };

        public string OpenFileOnLoad { get; set; }

        private int _rightClickedTabIndex = -1;

        private class TabData
        {
            public ParsedConfig Config;
            public SettingsFormPanel Panel;
        }

        private TabData ActiveData => tabFiles.SelectedTab?.Tag as TabData;
        private ParsedConfig ActiveConfig => ActiveData?.Config;

        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;
            Load += (s, e) =>
            {
                try
                {
                    // Left panel (form) gets the bulk; right column ~380px for raw text + info + tree
                    int target = splitMain.Width - 380;
                    if (target > 300) splitMain.SplitterDistance = target;

                    // Raw text gets ~45% of right column height, info+tree share the rest
                    int rightH = splitRight.Height;
                    if (rightH > 200) splitRight.SplitterDistance = rightH * 45 / 100;
                    if (splitInfoTree.Height > 100) splitInfoTree.SplitterDistance = 110;
                }
                catch { }
                if (!string.IsNullOrEmpty(OpenFileOnLoad)) LoadFile(OpenFileOnLoad);
            };
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.Handled = true;
                var cfg = ActiveConfig;
                if (btnSave.Enabled && cfg != null) SaveFile(cfg.FilePath);
            }
        }

        private void tabFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSidePanel();
            UpdateButtons();
        }

        private void tabFiles_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabFiles.TabPages.Count; i++)
            {
                if (!tabFiles.GetTabRect(i).Contains(e.Location)) continue;

                if (e.Button == MouseButtons.Middle)
                {
                    CloseTabAt(i);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    _rightClickedTabIndex = i;
                    tabContextMenu.Show(tabFiles, e.Location);
                }
                break;
            }
        }

        private void menuItemCloseTab_Click(object sender, EventArgs e)
        {
            CloseTabAt(_rightClickedTabIndex);
        }

        private void menuItemCloseOthers_Click(object sender, EventArgs e)
        {
            for (int i = tabFiles.TabPages.Count - 1; i >= 0; i--)
            {
                if (i != _rightClickedTabIndex)
                    CloseTabAt(i);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Konfigurationsdatei öffnen";
                dlg.Filter =
                    "Alle unterstützten Dateien|*.properties;*.conf;*.ini;*.definition;*.xml;*.config;*.bat;*.cmd;*.ps1|" +
                    ".properties / .conf|*.properties;*.conf|" +
                    ".ini / .definition|*.ini;*.definition|" +
                    ".xml / .config|*.xml;*.config|" +
                    ".bat / .cmd|*.bat;*.cmd|" +
                    ".ps1|*.ps1|" +
                    "Alle Dateien|*.*";
                dlg.Multiselect = true;

                if (dlg.ShowDialog() != DialogResult.OK) return;
                foreach (string f in dlg.FileNames)
                    LoadFile(f);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Ordner mit Konfigurationsdateien wählen";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                var files = new List<string>();
                foreach (string ext in SupportedExtensions)
                    files.AddRange(Directory.GetFiles(dlg.SelectedPath, "*" + ext));

                if (files.Count == 0)
                {
                    MessageBox.Show(
                        "Keine unterstützten Konfigurationsdateien gefunden.",
                        "Ordner öffnen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (files.Count > 20)
                {
                    var res = MessageBox.Show(
                        $"{files.Count} Dateien gefunden. Alle öffnen?",
                        "Ordner öffnen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.No) return;
                }

                files.Sort(StringComparer.OrdinalIgnoreCase);
                foreach (string f in files)
                    LoadFile(f);
            }
        }

        private void LoadFile(string path)
        {
            // Activate existing tab if the file is already open
            foreach (TabPage tab in tabFiles.TabPages)
            {
                var existing = tab.Tag as TabData;
                if (existing?.Config?.FilePath != null &&
                    string.Equals(existing.Config.FilePath, path, StringComparison.OrdinalIgnoreCase))
                {
                    tabFiles.SelectedTab = tab;
                    return;
                }
            }

            try
            {
                var config = _fileService.OpenFile(path);
                AddTab(config);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Öffnen:\n" + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddTab(ParsedConfig config)
        {
            var panel = new SettingsFormPanel { Dock = DockStyle.Fill };
            var tabData = new TabData { Config = config, Panel = panel };
            panel.ConfigChanged += (s, e) => OnTabConfigChanged(tabData);

            var tabPage = new TabPage(Path.GetFileName(config.FilePath));
            tabPage.Tag = tabData;
            tabPage.Controls.Add(panel);

            tabFiles.TabPages.Add(tabPage);
            tabFiles.SelectedTab = tabPage;

            panel.LoadConfig(config);
            txtRaw.Text = config.RawText;

            RefreshSidePanel();
            UpdateButtons();
        }

        private void CloseTabAt(int index)
        {
            if (index < 0 || index >= tabFiles.TabPages.Count) return;

            var data = tabFiles.TabPages[index].Tag as TabData;
            if (data?.Config?.HasUnsavedChanges == true)
            {
                var res = MessageBox.Show(
                    $"'{Path.GetFileName(data.Config.FilePath)}' hat ungespeicherte Änderungen.\nTab trotzdem schließen?",
                    "Ungespeicherte Änderungen",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No) return;
            }

            tabFiles.TabPages.RemoveAt(index);
            RefreshSidePanel();
            UpdateButtons();
        }

        private void OnTabConfigChanged(TabData data)
        {
            // Update this tab's title with dirty marker
            int idx = FindTabIndex(data);
            if (idx >= 0)
            {
                string prefix = data.Config.HasUnsavedChanges ? "* " : "";
                tabFiles.TabPages[idx].Text = prefix + Path.GetFileName(data.Config.FilePath);
            }

            // Update shared raw preview and status only when this is the active tab
            if (ReferenceEquals(tabFiles.SelectedTab?.Tag, data))
            {
                txtRaw.Text = _writerService.ApplyChanges(data.Config);
                UpdateStatusBar();
                UpdateTitle();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var cfg = ActiveConfig;
            if (cfg != null) SaveFile(cfg.FilePath);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            var cfg = ActiveConfig;
            if (cfg == null) return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "Speichern unter";
                dlg.FileName = Path.GetFileName(cfg.FilePath);
                dlg.Filter = "Alle Dateien|*.*";
                dlg.InitialDirectory = Path.GetDirectoryName(cfg.FilePath);

                if (dlg.ShowDialog() != DialogResult.OK) return;
                SaveFile(dlg.FileName);
            }
        }

        private void btnReparse_Click(object sender, EventArgs e)
        {
            var data = ActiveData;
            if (data == null) return;

            if (data.Config.HasUnsavedChanges)
            {
                var res = MessageBox.Show(
                    "Neu parsen verwirft alle nicht gespeicherten Änderungen. Fortfahren?",
                    "Änderungen verwerfen",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No) return;
            }

            string path = data.Config.FilePath;
            try
            {
                var newConfig = _fileService.OpenFile(path);
                data.Config = newConfig;
                data.Panel.LoadConfig(newConfig);

                int idx = FindTabIndex(data);
                if (idx >= 0) tabFiles.TabPages[idx].Text = Path.GetFileName(path);

                RefreshSidePanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Parsen:\n" + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveFile(string targetPath)
        {
            var data = ActiveData;
            if (data == null || string.IsNullOrEmpty(targetPath)) return;
            var config = data.Config;

            if (IsProtectedPath(targetPath))
            {
                var res = MessageBox.Show(
                    "Die Datei liegt in einem geschützten Systemverzeichnis:\n" + targetPath +
                    "\n\nSind Sie sicher, dass Sie diese Datei bearbeiten möchten?",
                    "Systemverzeichnis",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No) return;
            }

            string backupPath = null;
            if (File.Exists(targetPath))
            {
                try
                {
                    string backupSource = string.Equals(targetPath, config.FilePath,
                        StringComparison.OrdinalIgnoreCase) ? targetPath : config.FilePath;
                    if (File.Exists(backupSource))
                        backupPath = _backupService.CreateBackup(backupSource);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Backup konnte nicht erstellt werden:\n" + ex.Message +
                        "\n\nDie Datei wurde NICHT gespeichert.",
                        "Backup fehlgeschlagen",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                string newText = _writerService.ApplyChanges(config);
                File.WriteAllText(targetPath, newText, config.Encoding);

                config.RawText = newText;
                config.FilePath = targetPath;
                config.HasUnsavedChanges = false;

                txtRaw.Text = newText;

                int idx = FindTabIndex(data);
                if (idx >= 0) tabFiles.TabPages[idx].Text = Path.GetFileName(targetPath);

                UpdateStatusBar();
                UpdateTitle();

                string msg = "Gespeichert: " + targetPath;
                if (!string.IsNullOrEmpty(backupPath))
                    msg += "\nBackup: " + backupPath;

                MessageBox.Show(msg, "Gespeichert", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Keine Schreibrechte für:\n" + targetPath +
                    "\n\nBitte starten Sie ConfigWell als Administrator oder wählen Sie einen anderen Speicherort.",
                    "Zugriff verweigert",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshSidePanel()
        {
            var config = ActiveConfig;
            PopulateTree(config);
            UpdateFileInfo(config);
            UpdateStatusBar();
            UpdateTitle();
            txtRaw.Text = config != null ? _writerService.ApplyChanges(config) : string.Empty;
        }

        private int FindTabIndex(TabData data)
        {
            for (int i = 0; i < tabFiles.TabPages.Count; i++)
                if (ReferenceEquals(tabFiles.TabPages[i].Tag, data)) return i;
            return -1;
        }

        private void PopulateTree(ParsedConfig config)
        {
            treeGroups.Nodes.Clear();
            if (config == null) return;

            var root = new TreeNode(Path.GetFileName(config.FilePath));
            root.Expand();

            foreach (var section in config.Sections)
            {
                var sectionNode = new TreeNode($"{section.Name} ({section.Entries.Count})");
                root.Nodes.Add(sectionNode);
                sectionNode.Expand();
            }

            treeGroups.Nodes.Add(root);
        }

        private void UpdateFileInfo(ParsedConfig config)
        {
            if (config == null)
            {
                lblInfoTitle.Text = "ConfigWell";
                lblInfoDetails.Text = "Öffnen Sie eine Konfigurationsdatei.";
                return;
            }

            lblInfoTitle.Text = Path.GetFileName(config.FilePath);
            lblInfoDetails.Text =
                $"Typ: {config.FileType}\r\n" +
                $"Encoding: {config.Encoding.WebName.ToUpper()}\r\n" +
                $"Einstellungen: {config.TotalEntryCount}\r\n" +
                $"Abschnitte: {config.Sections.Count}";
        }

        private void UpdateStatusBar()
        {
            var config = ActiveConfig;
            if (config == null)
            {
                lblStatusPath.Text = "Keine Datei geöffnet";
                lblStatusType.Text = "";
                lblStatusEncoding.Text = "";
                lblStatusChanged.Text = "";
                return;
            }

            lblStatusPath.Text = config.FilePath;
            lblStatusType.Text = config.FileType.ToString();
            lblStatusEncoding.Text = config.Encoding.WebName.ToUpper();
            lblStatusChanged.Text = config.HasUnsavedChanges ? "● Ungespeichert" : "";
        }

        private void UpdateTitle()
        {
            var config = ActiveConfig;
            if (config == null)
            {
                Text = "ConfigWell";
                return;
            }
            string changed = config.HasUnsavedChanges ? "* " : "";
            Text = changed + Path.GetFileName(config.FilePath) + " — ConfigWell";
        }

        private void UpdateButtons()
        {
            bool hasTab = tabFiles.TabPages.Count > 0;
            btnSave.Enabled = hasTab;
            btnSaveAs.Enabled = hasTab;
            btnReparse.Enabled = hasTab;
        }

        private bool IsProtectedPath(string path)
        {
            foreach (string p in ProtectedPaths)
                if (path.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
