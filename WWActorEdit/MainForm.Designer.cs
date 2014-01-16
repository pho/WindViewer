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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("room.dzs");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("DZS", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("room.dzb");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("DZB", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Room0", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("event_list.dat");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("DAT", new System.Windows.Forms.TreeNode[] {
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Room1", new System.Windows.Forms.TreeNode[] {
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("stage.dzs");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("DZS", new System.Windows.Forms.TreeNode[] {
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Stage", new System.Windows.Forms.TreeNode[] {
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("MiniHyo", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode8,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Kui");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Kui");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("flower");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Actors", new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14,
            treeNode15});
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("[0]");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("[1]");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Spawnpoints", new System.Windows.Forms.TreeNode[] {
            treeNode17,
            treeNode18});
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Misc. (FILI)");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Exits");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.glControl = new OpenTK.GLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRoomRARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openStageRARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.openArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.environmentLightingEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.spawnEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sharedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showReadmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.floatConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.treeView3 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel5 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(1020, 713);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.glControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyDown);
            this.glControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl_KeyUp);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1249, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRoomRARCToolStripMenuItem,
            this.openStageRARCToolStripMenuItem,
            this.toolStripSeparator5,
            this.openArchiveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveChangesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.resetToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openRoomRARCToolStripMenuItem
            // 
            this.openRoomRARCToolStripMenuItem.Name = "openRoomRARCToolStripMenuItem";
            this.openRoomRARCToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.openRoomRARCToolStripMenuItem.Text = "&Open Room RARC(s)...";
            this.openRoomRARCToolStripMenuItem.Click += new System.EventHandler(this.openRoomRARCToolStripMenuItem_Click);
            // 
            // openStageRARCToolStripMenuItem
            // 
            this.openStageRARCToolStripMenuItem.Name = "openStageRARCToolStripMenuItem";
            this.openStageRARCToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.openStageRARCToolStripMenuItem.Text = "Open S&tage RARC...";
            this.openStageRARCToolStripMenuItem.Click += new System.EventHandler(this.openStageRARCToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(190, 6);
            // 
            // openArchiveToolStripMenuItem
            // 
            this.openArchiveToolStripMenuItem.Name = "openArchiveToolStripMenuItem";
            this.openArchiveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.openArchiveToolStripMenuItem.Text = "Open Archive";
            this.openArchiveToolStripMenuItem.Click += new System.EventHandler(this.openArchiveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(190, 6);
            // 
            // saveChangesToolStripMenuItem
            // 
            this.saveChangesToolStripMenuItem.Enabled = false;
            this.saveChangesToolStripMenuItem.Name = "saveChangesToolStripMenuItem";
            this.saveChangesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.saveChangesToolStripMenuItem.Text = "&Save Changes";
            this.saveChangesToolStripMenuItem.Click += new System.EventHandler(this.saveChangesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(190, 6);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(190, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
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
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // renderModelsToolStripMenuItem
            // 
            this.renderModelsToolStripMenuItem.Checked = true;
            this.renderModelsToolStripMenuItem.CheckOnClick = true;
            this.renderModelsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderModelsToolStripMenuItem.Name = "renderModelsToolStripMenuItem";
            this.renderModelsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.renderModelsToolStripMenuItem.Text = "Render &Models";
            // 
            // renderCollisionToolStripMenuItem
            // 
            this.renderCollisionToolStripMenuItem.Checked = true;
            this.renderCollisionToolStripMenuItem.CheckOnClick = true;
            this.renderCollisionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderCollisionToolStripMenuItem.Name = "renderCollisionToolStripMenuItem";
            this.renderCollisionToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.renderCollisionToolStripMenuItem.Text = "Render C&ollision";
            // 
            // renderRoomActorsToolStripMenuItem
            // 
            this.renderRoomActorsToolStripMenuItem.Checked = true;
            this.renderRoomActorsToolStripMenuItem.CheckOnClick = true;
            this.renderRoomActorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderRoomActorsToolStripMenuItem.Name = "renderRoomActorsToolStripMenuItem";
            this.renderRoomActorsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.renderRoomActorsToolStripMenuItem.Text = "Render &Room Actors";
            // 
            // renderStageActorsToolStripMenuItem
            // 
            this.renderStageActorsToolStripMenuItem.Checked = true;
            this.renderStageActorsToolStripMenuItem.CheckOnClick = true;
            this.renderStageActorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderStageActorsToolStripMenuItem.Name = "renderStageActorsToolStripMenuItem";
            this.renderStageActorsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.renderStageActorsToolStripMenuItem.Text = "Render &Stage Actors";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(181, 6);
            // 
            // autoCenterCameraToolStripMenuItem
            // 
            this.autoCenterCameraToolStripMenuItem.Checked = true;
            this.autoCenterCameraToolStripMenuItem.CheckOnClick = true;
            this.autoCenterCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoCenterCameraToolStripMenuItem.Name = "autoCenterCameraToolStripMenuItem";
            this.autoCenterCameraToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.autoCenterCameraToolStripMenuItem.Text = "Auto-&Center Camera";
            this.autoCenterCameraToolStripMenuItem.ToolTipText = "Automatically center camera on selected chunk element when applicable (ex. PLYR, " +
    "ACTR, TGDR).";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stageToolStripMenuItem,
            this.toolStripSeparator1,
            this.environmentLightingEditorToolStripMenuItem,
            this.roomToolStripMenuItem,
            this.toolStripSeparator2,
            this.spawnEditorToolStripMenuItem,
            this.sharedToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitEditorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // stageToolStripMenuItem
            // 
            this.stageToolStripMenuItem.Enabled = false;
            this.stageToolStripMenuItem.Name = "stageToolStripMenuItem";
            this.stageToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.stageToolStripMenuItem.Text = "Stage";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // environmentLightingEditorToolStripMenuItem
            // 
            this.environmentLightingEditorToolStripMenuItem.Name = "environmentLightingEditorToolStripMenuItem";
            this.environmentLightingEditorToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.environmentLightingEditorToolStripMenuItem.Text = "&Environment Lighting Editor";
            this.environmentLightingEditorToolStripMenuItem.Click += new System.EventHandler(this.environmentLightingEditorToolStripMenuItem_Click);
            // 
            // roomToolStripMenuItem
            // 
            this.roomToolStripMenuItem.Enabled = false;
            this.roomToolStripMenuItem.Name = "roomToolStripMenuItem";
            this.roomToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.roomToolStripMenuItem.Text = "Room";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // spawnEditorToolStripMenuItem
            // 
            this.spawnEditorToolStripMenuItem.Name = "spawnEditorToolStripMenuItem";
            this.spawnEditorToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.spawnEditorToolStripMenuItem.Text = "Spawn Editor";
            this.spawnEditorToolStripMenuItem.Click += new System.EventHandler(this.spawnEditorToolStripMenuItem_Click);
            // 
            // sharedToolStripMenuItem
            // 
            this.sharedToolStripMenuItem.Enabled = false;
            this.sharedToolStripMenuItem.Name = "sharedToolStripMenuItem";
            this.sharedToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.sharedToolStripMenuItem.Text = "Shared";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(220, 6);
            // 
            // exitEditorToolStripMenuItem
            // 
            this.exitEditorToolStripMenuItem.Name = "exitEditorToolStripMenuItem";
            this.exitEditorToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.exitEditorToolStripMenuItem.Text = "Exit Editor";
            this.exitEditorToolStripMenuItem.Click += new System.EventHandler(this.exitEditorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showReadmeToolStripMenuItem,
            this.wikiToolStripMenuItem,
            this.toolStripMenuItem5,
            this.aboutToolStripMenuItem,
            this.toolStripSeparator4,
            this.floatConverterToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // showReadmeToolStripMenuItem
            // 
            this.showReadmeToolStripMenuItem.Name = "showReadmeToolStripMenuItem";
            this.showReadmeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.showReadmeToolStripMenuItem.Text = "&Show Readme";
            this.showReadmeToolStripMenuItem.Click += new System.EventHandler(this.showReadmeToolStripMenuItem_Click);
            // 
            // wikiToolStripMenuItem
            // 
            this.wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            this.wikiToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.wikiToolStripMenuItem.Text = "&Wiki";
            this.wikiToolStripMenuItem.Click += new System.EventHandler(this.wikiToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(152, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(152, 6);
            // 
            // floatConverterToolStripMenuItem
            // 
            this.floatConverterToolStripMenuItem.Name = "floatConverterToolStripMenuItem";
            this.floatConverterToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.floatConverterToolStripMenuItem.Text = "Float Converter";
            this.floatConverterToolStripMenuItem.Click += new System.EventHandler(this.floatConverterToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 737);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1249, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1234, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "---";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.Visible = false;
            // 
            // treeView2
            // 
            this.treeView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Location = new System.Drawing.Point(3, 3);
            this.treeView2.Margin = new System.Windows.Forms.Padding(6);
            this.treeView2.Name = "treeView2";
            treeNode1.Name = "Node18";
            treeNode1.Text = "room.dzs";
            treeNode2.Name = "Node8";
            treeNode2.Text = "DZS";
            treeNode3.Name = "Node17";
            treeNode3.Text = "room.dzb";
            treeNode4.Name = "Node16";
            treeNode4.Text = "DZB";
            treeNode5.Name = "Node7";
            treeNode5.Text = "Room0";
            treeNode6.Name = "Node19";
            treeNode6.Text = "event_list.dat";
            treeNode7.Name = "Node10";
            treeNode7.Text = "DAT";
            treeNode8.Name = "Node9";
            treeNode8.Text = "Room1";
            treeNode9.Name = "Node21";
            treeNode9.Text = "stage.dzs";
            treeNode10.Name = "Node20";
            treeNode10.Text = "DZS";
            treeNode11.Name = "Node11";
            treeNode11.Text = "Stage";
            treeNode12.Name = "Room1";
            treeNode12.Text = "MiniHyo";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode12});
            this.treeView2.Size = new System.Drawing.Size(219, 268);
            this.treeView2.TabIndex = 7;
            this.treeView2.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView2_NodeMouseClick);
            // 
            // treeView3
            // 
            this.treeView3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView3.Location = new System.Drawing.Point(3, 3);
            this.treeView3.Name = "treeView3";
            treeNode13.Name = "Node4";
            treeNode13.Text = "Kui";
            treeNode14.Name = "Node5";
            treeNode14.Text = "Kui";
            treeNode15.Name = "Node6";
            treeNode15.Text = "flower";
            treeNode16.Name = "Node0";
            treeNode16.Text = "Actors";
            treeNode17.Name = "Node7";
            treeNode17.Text = "[0]";
            treeNode18.Name = "Node8";
            treeNode18.Text = "[1]";
            treeNode19.Name = "Node1";
            treeNode19.Text = "Spawnpoints";
            treeNode20.Name = "Node2";
            treeNode20.Text = "Misc. (FILI)";
            treeNode21.Name = "Node3";
            treeNode21.Text = "Exits";
            this.treeView3.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode16,
            treeNode19,
            treeNode20,
            treeNode21});
            this.treeView3.Size = new System.Drawing.Size(219, 429);
            this.treeView3.TabIndex = 8;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel4);
            this.splitContainer1.Panel1.Controls.Add(this.treeView3);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView2);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer1.Size = new System.Drawing.Size(225, 713);
            this.splitContainer1.SplitterDistance = 435;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel4.Controls.Add(this.button2);
            this.panel4.Controls.Add(this.button1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 402);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.panel4.Size = new System.Drawing.Size(219, 30);
            this.panel4.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button2.Location = new System.Drawing.Point(190, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "-";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(161, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "+";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel5);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.glControl);
            this.splitContainer2.Size = new System.Drawing.Size(1249, 713);
            this.splitContainer2.SplitterDistance = 225;
            this.splitContainer2.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.splitContainer1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(225, 713);
            this.panel5.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 759);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 451);
            this.Name = "MainForm";
            this.Text = "Wind Viewer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRoomRARCToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem showReadmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem environmentLightingEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem roomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sharedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem wikiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spawnEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem floatConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem openArchiveToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.TreeView treeView3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel5;
    }
}

