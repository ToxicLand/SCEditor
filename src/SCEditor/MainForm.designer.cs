namespace SCEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LZMAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lzhamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZstandardtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPolygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllShapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTextureStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importExpotstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportShapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEditCharacter = new System.Windows.Forms.ToolStripMenuItem();
            this.editTimelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addEditMatrixtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editChildrenDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.fixPointsChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.textureToolStripMenuItem,
            this.chunkToolStripMenuItem,
            this.objectToolStripMenuItem,
            this.shapeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1073, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem1,
            this.openToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.compressionToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createToolStripMenuItem1
            // 
            this.createToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.createToolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.createToolStripMenuItem1.Name = "createToolStripMenuItem1";
            this.createToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.createToolStripMenuItem1.Text = "Create";
            this.createToolStripMenuItem1.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.reloadToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Visible = false;
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.saveToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Visible = false;
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // compressionToolStripMenuItem
            // 
            this.compressionToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.compressionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LZMAToolStripMenuItem,
            this.lzhamToolStripMenuItem,
            this.ZstandardtoolStripMenuItem});
            this.compressionToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.compressionToolStripMenuItem.Name = "compressionToolStripMenuItem";
            this.compressionToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.compressionToolStripMenuItem.Text = "Compress";
            this.compressionToolStripMenuItem.Visible = false;
            // 
            // LZMAToolStripMenuItem
            // 
            this.LZMAToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.LZMAToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.LZMAToolStripMenuItem.Name = "LZMAToolStripMenuItem";
            this.LZMAToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.LZMAToolStripMenuItem.Text = "LZMA ";
            this.LZMAToolStripMenuItem.Click += new System.EventHandler(this.LZMAToolStripMenuItem_Click);
            // 
            // lzhamToolStripMenuItem
            // 
            this.lzhamToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.lzhamToolStripMenuItem.CheckOnClick = true;
            this.lzhamToolStripMenuItem.Enabled = false;
            this.lzhamToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.lzhamToolStripMenuItem.Name = "lzhamToolStripMenuItem";
            this.lzhamToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.lzhamToolStripMenuItem.Text = "Lzham";
            this.lzhamToolStripMenuItem.Click += new System.EventHandler(this.lzhamToolStripMenuItem_Click);
            // 
            // ZstandardtoolStripMenuItem
            // 
            this.ZstandardtoolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ZstandardtoolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.ZstandardtoolStripMenuItem.Name = "ZstandardtoolStripMenuItem";
            this.ZstandardtoolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.ZstandardtoolStripMenuItem.Text = "Zstandard";
            this.ZstandardtoolStripMenuItem.Click += new System.EventHandler(this.ZstandardToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPolygonsToolStripMenuItem,
            this.exportAllShapeToolStripMenuItem,
            this.exportAllChunkToolStripMenuItem,
            this.exportAllAnimationToolStripMenuItem,
            this.scImportToolStripMenuItem});
            this.imageToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.imageToolStripMenuItem.Text = "Extras";
            // 
            // viewPolygonsToolStripMenuItem
            // 
            this.viewPolygonsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.viewPolygonsToolStripMenuItem.CheckOnClick = true;
            this.viewPolygonsToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.viewPolygonsToolStripMenuItem.Name = "viewPolygonsToolStripMenuItem";
            this.viewPolygonsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.viewPolygonsToolStripMenuItem.Text = "Display Polygons";
            this.viewPolygonsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewPolygonsToolStripMenuItem_CheckedChanged);
            // 
            // exportAllShapeToolStripMenuItem
            // 
            this.exportAllShapeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.exportAllShapeToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportAllShapeToolStripMenuItem.Name = "exportAllShapeToolStripMenuItem";
            this.exportAllShapeToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.exportAllShapeToolStripMenuItem.Text = "Export All Shape";
            this.exportAllShapeToolStripMenuItem.Visible = false;
            this.exportAllShapeToolStripMenuItem.Click += new System.EventHandler(this.exportAllShapeToolStripMenuItem_Click);
            // 
            // exportAllChunkToolStripMenuItem
            // 
            this.exportAllChunkToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.exportAllChunkToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportAllChunkToolStripMenuItem.Name = "exportAllChunkToolStripMenuItem";
            this.exportAllChunkToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.exportAllChunkToolStripMenuItem.Text = "Export All Chunk";
            this.exportAllChunkToolStripMenuItem.Visible = false;
            this.exportAllChunkToolStripMenuItem.Click += new System.EventHandler(this.exportAllChunkToolStripMenuItem_Click);
            // 
            // exportAllAnimationToolStripMenuItem
            // 
            this.exportAllAnimationToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.exportAllAnimationToolStripMenuItem.Enabled = false;
            this.exportAllAnimationToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportAllAnimationToolStripMenuItem.Name = "exportAllAnimationToolStripMenuItem";
            this.exportAllAnimationToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.exportAllAnimationToolStripMenuItem.Text = "Export All Animation";
            this.exportAllAnimationToolStripMenuItem.Visible = false;
            this.exportAllAnimationToolStripMenuItem.Click += new System.EventHandler(this.exportAllAnimationToolStripMenuItem_Click);
            // 
            // scImportToolStripMenuItem
            // 
            this.scImportToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.scImportToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.scImportToolStripMenuItem.Name = "scImportToolStripMenuItem";
            this.scImportToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.scImportToolStripMenuItem.Text = "Import SC";
            this.scImportToolStripMenuItem.Click += new System.EventHandler(this.scImportToolStripMenuItem_Click);
            // 
            // textureToolStripMenuItem
            // 
            this.textureToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.textureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createTextureStripMenuItem,
            this.addTextureToolStripMenuItem,
            this.replaceTextureToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.exportToolStripMenuItem1});
            this.textureToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            this.textureToolStripMenuItem.Size = new System.Drawing.Size(62, 21);
            this.textureToolStripMenuItem.Text = "Texture";
            this.textureToolStripMenuItem.Visible = false;
            // 
            // createTextureStripMenuItem
            // 
            this.createTextureStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.createTextureStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.createTextureStripMenuItem.Name = "createTextureStripMenuItem";
            this.createTextureStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.createTextureStripMenuItem.Text = "Create Texture";
            this.createTextureStripMenuItem.Click += new System.EventHandler(this.createTextureToolStripMenuItem_Click);
            // 
            // addTextureToolStripMenuItem
            // 
            this.addTextureToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.addTextureToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.addTextureToolStripMenuItem.Name = "addTextureToolStripMenuItem";
            this.addTextureToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addTextureToolStripMenuItem.Text = "Add Texture";
            this.addTextureToolStripMenuItem.Click += new System.EventHandler(this.addTextureToolStripMenuItem_Click);
            // 
            // replaceTextureToolStripMenuItem
            // 
            this.replaceTextureToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.replaceTextureToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.replaceTextureToolStripMenuItem.Name = "replaceTextureToolStripMenuItem";
            this.replaceTextureToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.replaceTextureToolStripMenuItem.Text = "Replace";
            this.replaceTextureToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.duplicateToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.duplicateToolStripMenuItem.Text = "Clone";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exportToolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.exportToolStripMenuItem1.Text = "Export...";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // chunkToolStripMenuItem
            // 
            this.chunkToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.chunkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem2,
            this.importToolStripMenuItem,
            this.changeTextureToolStripMenuItem,
            this.fixPointsChunkToolStripMenuItem});
            this.chunkToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.chunkToolStripMenuItem.Name = "chunkToolStripMenuItem";
            this.chunkToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.chunkToolStripMenuItem.Text = "Chunk";
            this.chunkToolStripMenuItem.Visible = false;
            // 
            // exportToolStripMenuItem2
            // 
            this.exportToolStripMenuItem2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exportToolStripMenuItem2.ForeColor = System.Drawing.Color.White;
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.exportToolStripMenuItem2.Text = "Export...";
            this.exportToolStripMenuItem2.Click += new System.EventHandler(this.exportToolStripMenuItem2_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.importToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // changeTextureToolStripMenuItem
            // 
            this.changeTextureToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.changeTextureToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.changeTextureToolStripMenuItem.Name = "changeTextureToolStripMenuItem";
            this.changeTextureToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.changeTextureToolStripMenuItem.Text = "Replace Texture...";
            this.changeTextureToolStripMenuItem.Click += new System.EventHandler(this.changeTextureToolStripMenuItem_Click);
            // 
            // objectToolStripMenuItem
            // 
            this.objectToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.objectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createExportToolStripMenuItem,
            this.importExpotstoolStripMenuItem,
            this.exportToolStripMenuItem,
            this.duplicateToolStripMenuItem1,
            this.exportShapeToolStripMenuItem,
            this.toolStripMenuItemEditCharacter,
            this.editTimelineToolStripMenuItem,
            this.addEditMatrixtoolStripMenuItem,
            this.editChildrenDataToolStripMenuItem});
            this.objectToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.objectToolStripMenuItem.Name = "objectToolStripMenuItem";
            this.objectToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.objectToolStripMenuItem.Text = "Export";
            this.objectToolStripMenuItem.Visible = false;
            // 
            // createExportToolStripMenuItem
            // 
            this.createExportToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.createExportToolStripMenuItem.Enabled = false;
            this.createExportToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.createExportToolStripMenuItem.Name = "createExportToolStripMenuItem";
            this.createExportToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.createExportToolStripMenuItem.Text = "Create Export";
            this.createExportToolStripMenuItem.Click += new System.EventHandler(this.createExportToolStripMenuItem_Click);
            // 
            // importExpotstoolStripMenuItem
            // 
            this.importExpotstoolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.importExpotstoolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.importExpotstoolStripMenuItem.Name = "importExpotstoolStripMenuItem";
            this.importExpotstoolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.importExpotstoolStripMenuItem.Text = "Import Export Json";
            this.importExpotstoolStripMenuItem.Click += new System.EventHandler(this.importExportToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exportToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exportToolStripMenuItem.Text = "Export Image";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click_1);
            // 
            // duplicateToolStripMenuItem1
            // 
            this.duplicateToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.duplicateToolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.duplicateToolStripMenuItem1.Name = "duplicateToolStripMenuItem1";
            this.duplicateToolStripMenuItem1.Size = new System.Drawing.Size(191, 22);
            this.duplicateToolStripMenuItem1.Text = "Clone";
            this.duplicateToolStripMenuItem1.Click += new System.EventHandler(this.duplicateToolStripMenuItem1_Click);
            // 
            // exportShapeToolStripMenuItem
            // 
            this.exportShapeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exportShapeToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.exportShapeToolStripMenuItem.Name = "exportShapeToolStripMenuItem";
            this.exportShapeToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exportShapeToolStripMenuItem.Text = "Export Shape";
            this.exportShapeToolStripMenuItem.Click += new System.EventHandler(this.exportShapeToolStripMenuItem_Click);
            // 
            // toolStripMenuItemEditCharacter
            // 
            this.toolStripMenuItemEditCharacter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.toolStripMenuItemEditCharacter.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItemEditCharacter.Name = "toolStripMenuItemEditCharacter";
            this.toolStripMenuItemEditCharacter.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemEditCharacter.Text = "Edit Character";
            this.toolStripMenuItemEditCharacter.Click += new System.EventHandler(this.toolStripMenuItemEditCharacter_Click);
            // 
            // editTimelineToolStripMenuItem
            // 
            this.editTimelineToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.editTimelineToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.editTimelineToolStripMenuItem.Name = "editTimelineToolStripMenuItem";
            this.editTimelineToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.editTimelineToolStripMenuItem.Text = "Edit Timeline/Frame";
            this.editTimelineToolStripMenuItem.Click += new System.EventHandler(this.editTimelineToolStripMenuItem_Click);
            // 
            // addEditMatrixtoolStripMenuItem
            // 
            this.addEditMatrixtoolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.addEditMatrixtoolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.addEditMatrixtoolStripMenuItem.Name = "addEditMatrixtoolStripMenuItem";
            this.addEditMatrixtoolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addEditMatrixtoolStripMenuItem.Text = "Matrixes";
            this.addEditMatrixtoolStripMenuItem.Click += new System.EventHandler(this.addEditMatrixtoolStripMenuItem_Click);
            // 
            // editChildrenDataToolStripMenuItem
            // 
            this.editChildrenDataToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.editChildrenDataToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.editChildrenDataToolStripMenuItem.Name = "editChildrenDataToolStripMenuItem";
            this.editChildrenDataToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.editChildrenDataToolStripMenuItem.Text = "Edit Children Data";
            this.editChildrenDataToolStripMenuItem.Click += new System.EventHandler(this.editChildrenDataToolStripMenuItem_Click);
            // 
            // shapeToolStripMenuItem
            // 
            this.shapeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.shapeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem3});
            this.shapeToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.shapeToolStripMenuItem.Name = "shapeToolStripMenuItem";
            this.shapeToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.shapeToolStripMenuItem.Text = "Shape";
            this.shapeToolStripMenuItem.Visible = false;
            // 
            // exportToolStripMenuItem3
            // 
            this.exportToolStripMenuItem3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.exportToolStripMenuItem3.ForeColor = System.Drawing.Color.White;
            this.exportToolStripMenuItem3.Name = "exportToolStripMenuItem3";
            this.exportToolStripMenuItem3.Size = new System.Drawing.Size(123, 22);
            this.exportToolStripMenuItem3.Text = "Export...";
            this.exportToolStripMenuItem3.Click += new System.EventHandler(this.exportToolStripMenuItem3_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(9, 36);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(1055, 648);
            this.splitContainer1.SplitterDistance = 289;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(1, 548);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 98);
            this.label1.TabIndex = 8;
            this.label1.Text = "SC Editor\r\nVersion: 0.1\r\n";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.LineColor = System.Drawing.Color.DimGray;
            this.treeView1.Location = new System.Drawing.Point(2, 1);
            this.treeView1.Margin = new System.Windows.Forms.Padding(2);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(284, 545);
            this.treeView1.TabIndex = 7;
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(305, 36);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(757, 644);
            this.panel1.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // fixPointsChunkToolStripMenuItem
            // 
            this.fixPointsChunkToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.fixPointsChunkToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.fixPointsChunkToolStripMenuItem.Name = "fixPointsChunkToolStripMenuItem";
            this.fixPointsChunkToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fixPointsChunkToolStripMenuItem.Text = "Fix Points";
            this.fixPointsChunkToolStripMenuItem.Click += new System.EventHandler(this.fixPointsChunkToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(1073, 693);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SC Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPolygonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chunkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem shapeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LZMAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lzhamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllShapeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllChunkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportShapeToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem ZstandardtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTextureStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importExpotstoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEditCharacter;
        private System.Windows.Forms.ToolStripMenuItem editTimelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addEditMatrixtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editChildrenDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixPointsChunkToolStripMenuItem;
    }
}

