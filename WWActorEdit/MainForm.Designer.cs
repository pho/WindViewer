namespace WWActorEdit
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.glControl1 = new OpenTK.GLControl();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRoomRARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openStageRARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderRoomActorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderStageActorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.autoCenterCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showReadmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(283, 28);
            this.glControl1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(1010, 541);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = true;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 28);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(279, 594);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1360, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRoomRARCToolStripMenuItem,
            this.openStageRARCToolStripMenuItem,
            this.toolStripMenuItem6,
            this.toolStripMenuItem2,
            this.saveChangesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.resetToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openRoomRARCToolStripMenuItem
            // 
            this.openRoomRARCToolStripMenuItem.Name = "openRoomRARCToolStripMenuItem";
            this.openRoomRARCToolStripMenuItem.Size = new System.Drawing.Size(224, 24);
            this.openRoomRARCToolStripMenuItem.Text = "&Open Room RARC(s)...";
            this.openRoomRARCToolStripMenuItem.Click += new System.EventHandler(this.openRoomRARCToolStripMenuItem_Click);
            // 
            // openStageRARCToolStripMenuItem
            // 
            this.openStageRARCToolStripMenuItem.Name = "openStageRARCToolStripMenuItem";
            this.openStageRARCToolStripMenuItem.Size = new System.Drawing.Size(224, 24);
            this.openStageRARCToolStripMenuItem.Text = "Open S&tage RARC...";
            this.openStageRARCToolStripMenuItem.Click += new System.EventHandler(this.openStageRARCToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(224, 24);
            this.toolStripMenuItem6.Text = "Open Demo RARC...";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(221, 6);
            // 
            // saveChangesToolStripMenuItem
            // 
            this.saveChangesToolStripMenuItem.Name = "saveChangesToolStripMenuItem";
            this.saveChangesToolStripMenuItem.Size = new System.Drawing.Size(224, 24);
            this.saveChangesToolStripMenuItem.Text = "&Save Changes";
            this.saveChangesToolStripMenuItem.Click += new System.EventHandler(this.saveChangesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(221, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(224, 24);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(221, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 24);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderModelsToolStripMenuItem,
            this.renderCollisionToolStripMenuItem,
            this.renderRoomActorsToolStripMenuItem,
            this.renderStageActorsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.autoCenterCameraToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // renderModelsToolStripMenuItem
            // 
            this.renderModelsToolStripMenuItem.Checked = true;
            this.renderModelsToolStripMenuItem.CheckOnClick = true;
            this.renderModelsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderModelsToolStripMenuItem.Name = "renderModelsToolStripMenuItem";
            this.renderModelsToolStripMenuItem.Size = new System.Drawing.Size(215, 24);
            this.renderModelsToolStripMenuItem.Text = "Render &Models";
            // 
            // renderCollisionToolStripMenuItem
            // 
            this.renderCollisionToolStripMenuItem.Checked = true;
            this.renderCollisionToolStripMenuItem.CheckOnClick = true;
            this.renderCollisionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderCollisionToolStripMenuItem.Name = "renderCollisionToolStripMenuItem";
            this.renderCollisionToolStripMenuItem.Size = new System.Drawing.Size(215, 24);
            this.renderCollisionToolStripMenuItem.Text = "Render C&ollision";
            // 
            // renderRoomActorsToolStripMenuItem
            // 
            this.renderRoomActorsToolStripMenuItem.Checked = true;
            this.renderRoomActorsToolStripMenuItem.CheckOnClick = true;
            this.renderRoomActorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderRoomActorsToolStripMenuItem.Name = "renderRoomActorsToolStripMenuItem";
            this.renderRoomActorsToolStripMenuItem.Size = new System.Drawing.Size(215, 24);
            this.renderRoomActorsToolStripMenuItem.Text = "Render &Room Actors";
            // 
            // renderStageActorsToolStripMenuItem
            // 
            this.renderStageActorsToolStripMenuItem.Checked = true;
            this.renderStageActorsToolStripMenuItem.CheckOnClick = true;
            this.renderStageActorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderStageActorsToolStripMenuItem.Name = "renderStageActorsToolStripMenuItem";
            this.renderStageActorsToolStripMenuItem.Size = new System.Drawing.Size(215, 24);
            this.renderStageActorsToolStripMenuItem.Text = "Render &Stage Actors";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(212, 6);
            // 
            // autoCenterCameraToolStripMenuItem
            // 
            this.autoCenterCameraToolStripMenuItem.Checked = true;
            this.autoCenterCameraToolStripMenuItem.CheckOnClick = true;
            this.autoCenterCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoCenterCameraToolStripMenuItem.Name = "autoCenterCameraToolStripMenuItem";
            this.autoCenterCameraToolStripMenuItem.Size = new System.Drawing.Size(215, 24);
            this.autoCenterCameraToolStripMenuItem.Text = "Auto-&Center Camera";
            this.autoCenterCameraToolStripMenuItem.ToolTipText = "Automatically center camera on selected chunk element when applicable (ex. PLYR, " +
                "ACTR, TGDR).";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showReadmeToolStripMenuItem,
            this.toolStripMenuItem5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // showReadmeToolStripMenuItem
            // 
            this.showReadmeToolStripMenuItem.Name = "showReadmeToolStripMenuItem";
            this.showReadmeToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            this.showReadmeToolStripMenuItem.Text = "&Show Readme";
            this.showReadmeToolStripMenuItem.Click += new System.EventHandler(this.showReadmeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(170, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(173, 24);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 622);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1360, 25);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1340, 20);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "---";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 19);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(279, 28);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 594);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1293, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(67, 594);
            this.panel1.TabIndex = 5;
            this.panel1.Visible = false;
            // 
            // panel2
            // 
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(283, 569);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1010, 53);
            this.panel2.TabIndex = 6;
            this.panel2.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 647);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1061, 546);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRoomRARCToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoCenterCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openStageRARCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem renderModelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renderRoomActorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renderCollisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem renderStageActorsToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem showReadmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
    }
}

