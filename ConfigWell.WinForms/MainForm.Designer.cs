namespace ConfigWell.WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.btnReparse = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatusPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusType = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusEncoding = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusChanged = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tabFiles = new System.Windows.Forms.TabControl();
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.lblRawHeader = new System.Windows.Forms.Label();
            this.txtRaw = new System.Windows.Forms.RichTextBox();
            this.splitInfoTree = new System.Windows.Forms.SplitContainer();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.lblInfoTitle = new System.Windows.Forms.Label();
            this.lblInfoDetails = new System.Windows.Forms.Label();
            this.treeGroups = new System.Windows.Forms.TreeView();
            this.tabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemCloseTab = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCloseOthers = new System.Windows.Forms.ToolStripMenuItem();

            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitInfoTree)).BeginInit();
            this.splitInfoTree.Panel1.SuspendLayout();
            this.splitInfoTree.Panel2.SuspendLayout();
            this.splitInfoTree.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            this.SuspendLayout();

            // toolStrip
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnOpen,
                this.btnOpenFolder,
                new System.Windows.Forms.ToolStripSeparator(),
                this.btnSave,
                this.btnSaveAs,
                new System.Windows.Forms.ToolStripSeparator(),
                this.btnReparse
            });
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1100, 27);
            this.toolStrip.TabIndex = 0;

            // btnOpen
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Text = "Öffnen";
            this.btnOpen.ToolTipText = "Konfigurationsdatei(en) öffnen";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);

            // btnOpenFolder
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Text = "Ordner öffnen";
            this.btnOpenFolder.ToolTipText = "Alle Konfigurationsdateien eines Ordners öffnen";
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);

            // btnSave
            this.btnSave.Name = "btnSave";
            this.btnSave.Text = "Speichern";
            this.btnSave.ToolTipText = "Änderungen speichern (Strg+S)";
            this.btnSave.Enabled = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnSaveAs
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Text = "Speichern unter";
            this.btnSaveAs.ToolTipText = "Unter neuem Namen speichern";
            this.btnSaveAs.Enabled = false;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);

            // btnReparse
            this.btnReparse.Name = "btnReparse";
            this.btnReparse.Text = "Neu parsen";
            this.btnReparse.ToolTipText = "Datei neu einlesen und Formular aktualisieren";
            this.btnReparse.Enabled = false;
            this.btnReparse.Click += new System.EventHandler(this.btnReparse_Click);

            // statusStrip
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatusPath,
                this.lblStatusType,
                this.lblStatusEncoding,
                this.lblStatusChanged
            });
            this.statusStrip.Location = new System.Drawing.Point(0, 640);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1100, 24);

            this.lblStatusPath.Name = "lblStatusPath";
            this.lblStatusPath.Text = "Keine Datei geöffnet";
            this.lblStatusPath.Spring = true;
            this.lblStatusPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblStatusType.Name = "lblStatusType";
            this.lblStatusType.Text = "";
            this.lblStatusType.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;

            this.lblStatusEncoding.Name = "lblStatusEncoding";
            this.lblStatusEncoding.Text = "";
            this.lblStatusEncoding.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;

            this.lblStatusChanged.Name = "lblStatusChanged";
            this.lblStatusChanged.Text = "";
            this.lblStatusChanged.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblStatusChanged.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;

            // splitMain: settings-tabs (left) | right column (raw+info+tree)
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 27);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.tabFiles);
            this.splitMain.Panel2.Controls.Add(this.splitRight);

            // tabFiles — fills left panel, one tab per open file
            this.tabFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.SelectedIndexChanged += new System.EventHandler(this.tabFiles_SelectedIndexChanged);
            this.tabFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabFiles_MouseDown);

            // splitRight — right column: raw text (top) | info+tree (bottom)
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.Panel1.Controls.Add(this.txtRaw);
            this.splitRight.Panel1.Controls.Add(this.lblRawHeader);
            this.splitRight.Panel2.Controls.Add(this.splitInfoTree);

            // lblRawHeader — thin caption above the raw text box
            this.lblRawHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRawHeader.Text = "Rohtext";
            this.lblRawHeader.Height = 20;
            this.lblRawHeader.Font = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Bold);
            this.lblRawHeader.ForeColor = System.Drawing.Color.Gray;
            this.lblRawHeader.Padding = new System.Windows.Forms.Padding(4, 2, 0, 0);
            this.lblRawHeader.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // txtRaw — raw text preview, fills Panel1 of splitRight
            this.txtRaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.ReadOnly = true;
            this.txtRaw.Font = new System.Drawing.Font("Consolas", 8.5f);
            this.txtRaw.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.txtRaw.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRaw.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            this.txtRaw.WordWrap = false;

            // splitInfoTree — info panel (top) | tree (bottom)
            this.splitInfoTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitInfoTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitInfoTree.Panel1.Controls.Add(this.panelInfo);
            this.splitInfoTree.Panel2.Controls.Add(this.treeGroups);

            // panelInfo
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInfo.Padding = new System.Windows.Forms.Padding(6);
            this.panelInfo.BackColor = System.Drawing.SystemColors.Control;
            this.panelInfo.Controls.Add(this.lblInfoDetails);
            this.panelInfo.Controls.Add(this.lblInfoTitle);

            // lblInfoTitle
            this.lblInfoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfoTitle.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.lblInfoTitle.Text = "ConfigWell";
            this.lblInfoTitle.Height = 22;
            this.lblInfoTitle.ForeColor = System.Drawing.Color.FromArgb(0, 80, 160);

            // lblInfoDetails
            this.lblInfoDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfoDetails.Text = "Öffnen Sie eine Konfigurationsdatei.";
            this.lblInfoDetails.Font = new System.Drawing.Font("Segoe UI", 8f);
            this.lblInfoDetails.ForeColor = System.Drawing.Color.Gray;

            // treeGroups
            this.treeGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGroups.Name = "treeGroups";
            this.treeGroups.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeGroups.Font = new System.Drawing.Font("Segoe UI", 8.5f);

            // tabContextMenu
            this.tabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuItemCloseTab,
                this.menuItemCloseOthers
            });
            this.tabContextMenu.Name = "tabContextMenu";

            this.menuItemCloseTab.Name = "menuItemCloseTab";
            this.menuItemCloseTab.Text = "Tab schließen";
            this.menuItemCloseTab.Click += new System.EventHandler(this.menuItemCloseTab_Click);

            this.menuItemCloseOthers.Name = "menuItemCloseOthers";
            this.menuItemCloseOthers.Text = "Alle anderen schließen";
            this.menuItemCloseOthers.Click += new System.EventHandler(this.menuItemCloseOthers_Click);

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 664);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.Text = "ConfigWell";

            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            this.splitInfoTree.Panel1.ResumeLayout(false);
            this.splitInfoTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitInfoTree)).EndInit();
            this.splitInfoTree.ResumeLayout(false);
            this.tabContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnOpenFolder;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.ToolStripButton btnReparse;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusPath;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusType;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusEncoding;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusChanged;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabFiles;
        private System.Windows.Forms.SplitContainer splitRight;
        private System.Windows.Forms.Label lblRawHeader;
        private System.Windows.Forms.RichTextBox txtRaw;
        private System.Windows.Forms.SplitContainer splitInfoTree;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblInfoTitle;
        private System.Windows.Forms.Label lblInfoDetails;
        private System.Windows.Forms.TreeView treeGroups;
        private System.Windows.Forms.ContextMenuStrip tabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemCloseTab;
        private System.Windows.Forms.ToolStripMenuItem menuItemCloseOthers;
    }
}
