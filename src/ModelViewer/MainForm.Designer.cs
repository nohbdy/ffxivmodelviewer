namespace ModelViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.cacheToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheStatusMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheReloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cartographerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileNavigator = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.btnRenderModel = new System.Windows.Forms.ToolStripButton();
            this.btnRenderSkeleton = new System.Windows.Forms.ToolStripButton();
            this.btnAnimatedSkeleton = new System.Windows.Forms.ToolStripButton();
            this.btnDecompileShader = new System.Windows.Forms.ToolStripButton();
            this.btnViewTexture = new System.Windows.Forms.ToolStripButton();
            this.btnPlaySound = new System.Windows.Forms.ToolStripButton();
            this.btnSoundPause = new System.Windows.Forms.ToolStripButton();
            this.btnSoundStop = new System.Windows.Forms.ToolStripButton();
            this.btnSaveScript = new System.Windows.Forms.ToolStripButton();
            this.leftSideContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.npcSearchDataGrid = new System.Windows.Forms.DataGridView();
            this.searchToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtNpcSearch = new System.Windows.Forms.ToolStripTextBox();
            this.debugText = new System.Windows.Forms.TextBox();
            this.rootContainer = new System.Windows.Forms.SplitContainer();
            this.charaSelector = new System.Windows.Forms.TreeView();
            this.charaToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnHideChara = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRenderChara = new System.Windows.Forms.ToolStripButton();
            this.btnCharaAnimation = new System.Windows.Forms.ToolStripButton();
            this.menuStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.leftSideContainer.Panel1.SuspendLayout();
            this.leftSideContainer.Panel2.SuspendLayout();
            this.leftSideContainer.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.npcSearchDataGrid)).BeginInit();
            this.searchToolStrip.SuspendLayout();
            this.rootContainer.Panel1.SuspendLayout();
            this.rootContainer.Panel2.SuspendLayout();
            this.rootContainer.SuspendLayout();
            this.charaToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(976, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.clearLogToolStripMenuItem,
            this.toolStripMenuItem4,
            this.cacheToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // clearLogToolStripMenuItem
            // 
            this.clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            this.clearLogToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.clearLogToolStripMenuItem.Text = "Clear Log";
            this.clearLogToolStripMenuItem.Click += new System.EventHandler(this.clearLogToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(152, 6);
            // 
            // cacheToolStripMenuItem1
            // 
            this.cacheToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cacheStatusMenuItem,
            this.cacheReloadMenuItem});
            this.cacheToolStripMenuItem1.Name = "cacheToolStripMenuItem1";
            this.cacheToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.cacheToolStripMenuItem1.Text = "Cache";
            // 
            // cacheStatusMenuItem
            // 
            this.cacheStatusMenuItem.Name = "cacheStatusMenuItem";
            this.cacheStatusMenuItem.Size = new System.Drawing.Size(146, 22);
            this.cacheStatusMenuItem.Text = "Status";
            this.cacheStatusMenuItem.Click += new System.EventHandler(this.cacheStatusMenuItem_Click);
            // 
            // cacheReloadMenuItem
            // 
            this.cacheReloadMenuItem.Name = "cacheReloadMenuItem";
            this.cacheReloadMenuItem.Size = new System.Drawing.Size(146, 22);
            this.cacheReloadMenuItem.Text = "Reload Cache";
            this.cacheReloadMenuItem.Click += new System.EventHandler(this.cacheReloadMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelListToolStripMenuItem,
            this.cartographerToolStripMenuItem,
            this.toolStripMenuItem2,
            this.preferencesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // modelListToolStripMenuItem
            // 
            this.modelListToolStripMenuItem.Name = "modelListToolStripMenuItem";
            this.modelListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.modelListToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.modelListToolStripMenuItem.Text = "Toggle Model List";
            this.modelListToolStripMenuItem.Click += new System.EventHandler(this.modelListToolStripMenuItem_Click);
            // 
            // cartographerToolStripMenuItem
            // 
            this.cartographerToolStripMenuItem.Name = "cartographerToolStripMenuItem";
            this.cartographerToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.cartographerToolStripMenuItem.Text = "Cartographer";
            this.cartographerToolStripMenuItem.Click += new System.EventHandler(this.cartographerToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(211, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All files|*.*|DAT Files|*.dat";
            // 
            // fileNavigator
            // 
            this.fileNavigator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileNavigator.Location = new System.Drawing.Point(0, 0);
            this.fileNavigator.Name = "fileNavigator";
            this.fileNavigator.Size = new System.Drawing.Size(225, 382);
            this.fileNavigator.TabIndex = 0;
            this.fileNavigator.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileNavigator_AfterSelect);
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
            this.splitContainer1.Panel1.Controls.Add(this.fileNavigator);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(225, 662);
            this.splitContainer1.SplitterDistance = 382;
            this.splitContainer1.TabIndex = 1;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(225, 276);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.btnRenderModel,
            this.btnRenderSkeleton,
            this.btnAnimatedSkeleton,
            this.btnDecompileShader,
            this.btnViewTexture,
            this.btnPlaySound,
            this.btnSoundPause,
            this.btnSoundStop,
            this.btnSaveScript});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(976, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // btnRenderModel
            // 
            this.btnRenderModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRenderModel.Image = ((System.Drawing.Image)(resources.GetObject("btnRenderModel.Image")));
            this.btnRenderModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRenderModel.Name = "btnRenderModel";
            this.btnRenderModel.Size = new System.Drawing.Size(23, 22);
            this.btnRenderModel.Text = "Render Mesh";
            this.btnRenderModel.Visible = false;
            this.btnRenderModel.Click += new System.EventHandler(this.btnRenderModel_Click);
            // 
            // btnRenderSkeleton
            // 
            this.btnRenderSkeleton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRenderSkeleton.Image = ((System.Drawing.Image)(resources.GetObject("btnRenderSkeleton.Image")));
            this.btnRenderSkeleton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRenderSkeleton.Name = "btnRenderSkeleton";
            this.btnRenderSkeleton.Size = new System.Drawing.Size(23, 22);
            this.btnRenderSkeleton.Text = "Render Skeleton";
            this.btnRenderSkeleton.Visible = false;
            this.btnRenderSkeleton.Click += new System.EventHandler(this.btnRenderSkeleton_Click);
            // 
            // btnAnimatedSkeleton
            // 
            this.btnAnimatedSkeleton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAnimatedSkeleton.Image = ((System.Drawing.Image)(resources.GetObject("btnAnimatedSkeleton.Image")));
            this.btnAnimatedSkeleton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAnimatedSkeleton.Name = "btnAnimatedSkeleton";
            this.btnAnimatedSkeleton.Size = new System.Drawing.Size(23, 22);
            this.btnAnimatedSkeleton.Text = "Animated Skeleton";
            this.btnAnimatedSkeleton.Visible = false;
            this.btnAnimatedSkeleton.Click += new System.EventHandler(this.btnAnimatedSkeleton_Click);
            // 
            // btnDecompileShader
            // 
            this.btnDecompileShader.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDecompileShader.Image = ((System.Drawing.Image)(resources.GetObject("btnDecompileShader.Image")));
            this.btnDecompileShader.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDecompileShader.Name = "btnDecompileShader";
            this.btnDecompileShader.Size = new System.Drawing.Size(23, 22);
            this.btnDecompileShader.Text = "Decompile Shader";
            this.btnDecompileShader.Visible = false;
            this.btnDecompileShader.Click += new System.EventHandler(this.btnDecompileShader_Click);
            // 
            // btnViewTexture
            // 
            this.btnViewTexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnViewTexture.Image = ((System.Drawing.Image)(resources.GetObject("btnViewTexture.Image")));
            this.btnViewTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewTexture.Name = "btnViewTexture";
            this.btnViewTexture.Size = new System.Drawing.Size(23, 22);
            this.btnViewTexture.Text = "View Texture";
            this.btnViewTexture.Visible = false;
            this.btnViewTexture.Click += new System.EventHandler(this.btnViewTexture_Click);
            // 
            // btnPlaySound
            // 
            this.btnPlaySound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlaySound.Enabled = false;
            this.btnPlaySound.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaySound.Image")));
            this.btnPlaySound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlaySound.Name = "btnPlaySound";
            this.btnPlaySound.Size = new System.Drawing.Size(23, 22);
            this.btnPlaySound.Text = "Play Sound";
            this.btnPlaySound.Click += new System.EventHandler(this.btnPlaySound_Click);
            // 
            // btnSoundPause
            // 
            this.btnSoundPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSoundPause.Image = ((System.Drawing.Image)(resources.GetObject("btnSoundPause.Image")));
            this.btnSoundPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSoundPause.Name = "btnSoundPause";
            this.btnSoundPause.Size = new System.Drawing.Size(23, 22);
            this.btnSoundPause.Text = "Pause Sound";
            this.btnSoundPause.Click += new System.EventHandler(this.btnSoundPause_Click);
            // 
            // btnSoundStop
            // 
            this.btnSoundStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSoundStop.Image = ((System.Drawing.Image)(resources.GetObject("btnSoundStop.Image")));
            this.btnSoundStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSoundStop.Name = "btnSoundStop";
            this.btnSoundStop.Size = new System.Drawing.Size(23, 22);
            this.btnSoundStop.Text = "Stop Sound";
            this.btnSoundStop.Click += new System.EventHandler(this.btnSoundStop_Click);
            // 
            // btnSaveScript
            // 
            this.btnSaveScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveScript.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveScript.Image")));
            this.btnSaveScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveScript.Name = "btnSaveScript";
            this.btnSaveScript.Size = new System.Drawing.Size(23, 22);
            this.btnSaveScript.Text = "Save Script";
            this.btnSaveScript.Visible = false;
            this.btnSaveScript.Click += new System.EventHandler(this.btnSaveScript_Click);
            // 
            // leftSideContainer
            // 
            this.leftSideContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSideContainer.Location = new System.Drawing.Point(0, 0);
            this.leftSideContainer.Name = "leftSideContainer";
            // 
            // leftSideContainer.Panel1
            // 
            this.leftSideContainer.Panel1.Controls.Add(this.splitContainer1);
            // 
            // leftSideContainer.Panel2
            // 
            this.leftSideContainer.Panel2.Controls.Add(this.splitContainer2);
            this.leftSideContainer.Size = new System.Drawing.Size(814, 662);
            this.leftSideContainer.SplitterDistance = 225;
            this.leftSideContainer.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.npcSearchDataGrid);
            this.splitContainer2.Panel1.Controls.Add(this.searchToolStrip);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.debugText);
            this.splitContainer2.Size = new System.Drawing.Size(585, 662);
            this.splitContainer2.SplitterDistance = 487;
            this.splitContainer2.TabIndex = 1;
            // 
            // npcSearchDataGrid
            // 
            this.npcSearchDataGrid.AllowUserToAddRows = false;
            this.npcSearchDataGrid.AllowUserToDeleteRows = false;
            this.npcSearchDataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.npcSearchDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.npcSearchDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.npcSearchDataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.npcSearchDataGrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.npcSearchDataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.npcSearchDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.npcSearchDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.npcSearchDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.npcSearchDataGrid.GridColor = System.Drawing.SystemColors.ControlLight;
            this.npcSearchDataGrid.Location = new System.Drawing.Point(0, 25);
            this.npcSearchDataGrid.MultiSelect = false;
            this.npcSearchDataGrid.Name = "npcSearchDataGrid";
            this.npcSearchDataGrid.ReadOnly = true;
            this.npcSearchDataGrid.RowHeadersVisible = false;
            this.npcSearchDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.npcSearchDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.npcSearchDataGrid.Size = new System.Drawing.Size(585, 462);
            this.npcSearchDataGrid.TabIndex = 1;
            this.npcSearchDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.npcSearchDataGrid_CellContentClick);
            // 
            // searchToolStrip
            // 
            this.searchToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.searchToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txtNpcSearch});
            this.searchToolStrip.Location = new System.Drawing.Point(0, 0);
            this.searchToolStrip.Name = "searchToolStrip";
            this.searchToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.searchToolStrip.Size = new System.Drawing.Size(585, 25);
            this.searchToolStrip.TabIndex = 0;
            this.searchToolStrip.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.toolStripLabel1.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel1.Text = "NPC Search:";
            // 
            // txtNpcSearch
            // 
            this.txtNpcSearch.MaxLength = 64;
            this.txtNpcSearch.Name = "txtNpcSearch";
            this.txtNpcSearch.Size = new System.Drawing.Size(150, 25);
            this.txtNpcSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNpcSearch_KeyPress);
            // 
            // debugText
            // 
            this.debugText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugText.Location = new System.Drawing.Point(0, 0);
            this.debugText.Multiline = true;
            this.debugText.Name = "debugText";
            this.debugText.ReadOnly = true;
            this.debugText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.debugText.Size = new System.Drawing.Size(585, 171);
            this.debugText.TabIndex = 0;
            // 
            // rootContainer
            // 
            this.rootContainer.BackColor = System.Drawing.Color.Transparent;
            this.rootContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootContainer.Location = new System.Drawing.Point(0, 49);
            this.rootContainer.Name = "rootContainer";
            // 
            // rootContainer.Panel1
            // 
            this.rootContainer.Panel1.Controls.Add(this.leftSideContainer);
            // 
            // rootContainer.Panel2
            // 
            this.rootContainer.Panel2.Controls.Add(this.charaSelector);
            this.rootContainer.Panel2.Controls.Add(this.charaToolStrip);
            this.rootContainer.Size = new System.Drawing.Size(976, 662);
            this.rootContainer.SplitterDistance = 814;
            this.rootContainer.TabIndex = 4;
            // 
            // charaSelector
            // 
            this.charaSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charaSelector.Location = new System.Drawing.Point(0, 25);
            this.charaSelector.Name = "charaSelector";
            this.charaSelector.Size = new System.Drawing.Size(158, 637);
            this.charaSelector.TabIndex = 0;
            this.charaSelector.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.charaSelector_AfterSelect);
            // 
            // charaToolStrip
            // 
            this.charaToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.charaToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnHideChara,
            this.toolStripSeparator1,
            this.btnRenderChara,
            this.btnCharaAnimation});
            this.charaToolStrip.Location = new System.Drawing.Point(0, 0);
            this.charaToolStrip.Name = "charaToolStrip";
            this.charaToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.charaToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.charaToolStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.charaToolStrip.Size = new System.Drawing.Size(158, 25);
            this.charaToolStrip.TabIndex = 1;
            this.charaToolStrip.Text = "toolStrip2";
            // 
            // btnHideChara
            // 
            this.btnHideChara.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHideChara.Image = ((System.Drawing.Image)(resources.GetObject("btnHideChara.Image")));
            this.btnHideChara.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHideChara.Name = "btnHideChara";
            this.btnHideChara.Size = new System.Drawing.Size(23, 22);
            this.btnHideChara.Text = "Close";
            this.btnHideChara.Click += new System.EventHandler(this.btnHideChara_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRenderChara
            // 
            this.btnRenderChara.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRenderChara.Enabled = false;
            this.btnRenderChara.Image = ((System.Drawing.Image)(resources.GetObject("btnRenderChara.Image")));
            this.btnRenderChara.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRenderChara.Name = "btnRenderChara";
            this.btnRenderChara.Size = new System.Drawing.Size(23, 22);
            this.btnRenderChara.Text = "Render";
            this.btnRenderChara.Click += new System.EventHandler(this.btnRenderChara_Click);
            // 
            // btnCharaAnimation
            // 
            this.btnCharaAnimation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCharaAnimation.Enabled = false;
            this.btnCharaAnimation.Image = ((System.Drawing.Image)(resources.GetObject("btnCharaAnimation.Image")));
            this.btnCharaAnimation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCharaAnimation.Name = "btnCharaAnimation";
            this.btnCharaAnimation.Size = new System.Drawing.Size(23, 22);
            this.btnCharaAnimation.Text = "View Animated (EXPERIMENTAL)";
            this.btnCharaAnimation.Click += new System.EventHandler(this.btnCharaAnimation_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(976, 711);
            this.Controls.Add(this.rootContainer);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "FFXIV Model Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.leftSideContainer.Panel1.ResumeLayout(false);
            this.leftSideContainer.Panel2.ResumeLayout(false);
            this.leftSideContainer.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.npcSearchDataGrid)).EndInit();
            this.searchToolStrip.ResumeLayout(false);
            this.searchToolStrip.PerformLayout();
            this.rootContainer.Panel1.ResumeLayout(false);
            this.rootContainer.Panel2.ResumeLayout(false);
            this.rootContainer.Panel2.PerformLayout();
            this.rootContainer.ResumeLayout(false);
            this.charaToolStrip.ResumeLayout(false);
            this.charaToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TreeView fileNavigator;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer leftSideContainer;
        private System.Windows.Forms.TextBox debugText;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton btnDecompileShader;
        private System.Windows.Forms.ToolStripButton btnViewTexture;
        private System.Windows.Forms.ToolStripMenuItem clearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnPlaySound;
        private System.Windows.Forms.ToolStripButton btnSoundPause;
        private System.Windows.Forms.ToolStripButton btnSoundStop;
        private System.Windows.Forms.ToolStripButton btnRenderModel;
        private System.Windows.Forms.SplitContainer rootContainer;
        private System.Windows.Forms.TreeView charaSelector;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStrip charaToolStrip;
        private System.Windows.Forms.ToolStripButton btnRenderChara;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnHideChara;
        private System.Windows.Forms.ToolStripButton btnRenderSkeleton;
        private System.Windows.Forms.ToolStripButton btnSaveScript;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem cacheToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cacheStatusMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cacheReloadMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStrip searchToolStrip;
        private System.Windows.Forms.ToolStripTextBox txtNpcSearch;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.DataGridView npcSearchDataGrid;
        private System.Windows.Forms.ToolStripButton btnAnimatedSkeleton;
        private System.Windows.Forms.ToolStripButton btnCharaAnimation;
        private System.Windows.Forms.ToolStripMenuItem cartographerToolStripMenuItem;
    }
}

