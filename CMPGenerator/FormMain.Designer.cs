namespace CMPGenerator
{
    partial class FormMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMap1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openMap2 = new System.Windows.Forms.ToolStripMenuItem();
            this.openTileset1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openTileset2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMap1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMap2 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTileset1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTileset2 = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ignoreIdenticalTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.regenerateTSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMap1,
            this.openMap2,
            this.openTileset1,
            this.openTileset2});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openMap1
            // 
            this.openMap1.Name = "openMap1";
            this.openMap1.Size = new System.Drawing.Size(126, 22);
            this.openMap1.Text = "Map 1...";
            this.openMap1.Click += new System.EventHandler(this.openMap1_Click);
            // 
            // openMap2
            // 
            this.openMap2.Enabled = false;
            this.openMap2.Name = "openMap2";
            this.openMap2.Size = new System.Drawing.Size(126, 22);
            this.openMap2.Text = "Map 2..";
            this.openMap2.Click += new System.EventHandler(this.openMap2_Click);
            // 
            // openTileset1
            // 
            this.openTileset1.Name = "openTileset1";
            this.openTileset1.Size = new System.Drawing.Size(126, 22);
            this.openTileset1.Text = "Tileset 1...";
            // 
            // openTileset2
            // 
            this.openTileset2.Enabled = false;
            this.openTileset2.Name = "openTileset2";
            this.openTileset2.Size = new System.Drawing.Size(126, 22);
            this.openTileset2.Text = "Tileset 2...";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exportToolStripMenuItem.Text = "Export TSC As...";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.regenerateTSCToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewMap1,
            this.viewMap2,
            this.viewTileset1,
            this.viewTileset2});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // viewMap1
            // 
            this.viewMap1.CheckOnClick = true;
            this.viewMap1.Name = "viewMap1";
            this.viewMap1.Size = new System.Drawing.Size(117, 22);
            this.viewMap1.Text = "Map 1";
            // 
            // viewMap2
            // 
            this.viewMap2.CheckOnClick = true;
            this.viewMap2.Name = "viewMap2";
            this.viewMap2.Size = new System.Drawing.Size(117, 22);
            this.viewMap2.Text = "Map 2";
            // 
            // viewTileset1
            // 
            this.viewTileset1.CheckOnClick = true;
            this.viewTileset1.Name = "viewTileset1";
            this.viewTileset1.Size = new System.Drawing.Size(117, 22);
            this.viewTileset1.Text = "Tileset 1";
            // 
            // viewTileset2
            // 
            this.viewTileset2.CheckOnClick = true;
            this.viewTileset2.Name = "viewTileset2";
            this.viewTileset2.Size = new System.Drawing.Size(117, 22);
            this.viewTileset2.Text = "Tileset 2";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modeToolStripMenuItem,
            this.ignoreIdenticalTilesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cMPToolStripMenuItem,
            this.lMPToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // cMPToolStripMenuItem
            // 
            this.cMPToolStripMenuItem.Checked = true;
            this.cMPToolStripMenuItem.CheckOnClick = true;
            this.cMPToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cMPToolStripMenuItem.Name = "cMPToolStripMenuItem";
            this.cMPToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.cMPToolStripMenuItem.Text = "<CMP";
            this.cMPToolStripMenuItem.Click += new System.EventHandler(this.cMPToolStripMenuItem_Click);
            // 
            // lMPToolStripMenuItem
            // 
            this.lMPToolStripMenuItem.CheckOnClick = true;
            this.lMPToolStripMenuItem.Name = "lMPToolStripMenuItem";
            this.lMPToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.lMPToolStripMenuItem.Text = "<LMP";
            this.lMPToolStripMenuItem.Click += new System.EventHandler(this.lMPToolStripMenuItem_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(13, 28);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(259, 221);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // ignoreIdenticalTilesToolStripMenuItem
            // 
            this.ignoreIdenticalTilesToolStripMenuItem.Checked = true;
            this.ignoreIdenticalTilesToolStripMenuItem.CheckOnClick = true;
            this.ignoreIdenticalTilesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreIdenticalTilesToolStripMenuItem.Name = "ignoreIdenticalTilesToolStripMenuItem";
            this.ignoreIdenticalTilesToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.ignoreIdenticalTilesToolStripMenuItem.Text = "Ignore Identical Tiles";
            this.ignoreIdenticalTilesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ignoreIdenticalTilesToolStripMenuItem_CheckedChanged);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(154, 6);
            // 
            // regenerateTSCToolStripMenuItem
            // 
            this.regenerateTSCToolStripMenuItem.Enabled = false;
            this.regenerateTSCToolStripMenuItem.Name = "regenerateTSCToolStripMenuItem";
            this.regenerateTSCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.regenerateTSCToolStripMenuItem.Text = "Regenerate TSC";
            this.regenerateTSCToolStripMenuItem.Click += new System.EventHandler(this.regenerateTSCToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Name = "FormMain";
            this.Text = "CMP Generator Main Windows";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMap1;
        private System.Windows.Forms.ToolStripMenuItem viewMap2;
        private System.Windows.Forms.ToolStripMenuItem viewTileset1;
        private System.Windows.Forms.ToolStripMenuItem viewTileset2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMap1;
        private System.Windows.Forms.ToolStripMenuItem openMap2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTileset1;
        private System.Windows.Forms.ToolStripMenuItem openTileset2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreIdenticalTilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem regenerateTSCToolStripMenuItem;
    }
}

