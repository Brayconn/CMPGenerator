namespace CMPGenerator
{
    partial class EditRange
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
            this.xOffsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.yOffsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.xRangeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.yRangeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.xOffsetNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yOffsetNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRangeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yRangeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // xOffsetNumericUpDown
            // 
            this.xOffsetNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xOffsetNumericUpDown.Location = new System.Drawing.Point(74, 12);
            this.xOffsetNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.xOffsetNumericUpDown.Name = "xOffsetNumericUpDown";
            this.xOffsetNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.xOffsetNumericUpDown.TabIndex = 0;
            this.xOffsetNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // yOffsetNumericUpDown
            // 
            this.yOffsetNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yOffsetNumericUpDown.Location = new System.Drawing.Point(74, 38);
            this.yOffsetNumericUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.yOffsetNumericUpDown.Name = "yOffsetNumericUpDown";
            this.yOffsetNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.yOffsetNumericUpDown.TabIndex = 1;
            this.yOffsetNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X Offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y Offset";
            // 
            // xRangeNumericUpDown
            // 
            this.xRangeNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xRangeNumericUpDown.Location = new System.Drawing.Point(74, 64);
            this.xRangeNumericUpDown.Name = "xRangeNumericUpDown";
            this.xRangeNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.xRangeNumericUpDown.TabIndex = 4;
            this.xRangeNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // yRangeNumericUpDown
            // 
            this.yRangeNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yRangeNumericUpDown.Location = new System.Drawing.Point(74, 90);
            this.yRangeNumericUpDown.Name = "yRangeNumericUpDown";
            this.yRangeNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.yRangeNumericUpDown.TabIndex = 5;
            this.yRangeNumericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "X Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Y Size";
            // 
            // EditRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 122);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.yRangeNumericUpDown);
            this.Controls.Add(this.xRangeNumericUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.yOffsetNumericUpDown);
            this.Controls.Add(this.xOffsetNumericUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditRange";
            this.ShowInTaskbar = false;
            this.Text = "EditRange";
            ((System.ComponentModel.ISupportInitialize)(this.xOffsetNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yOffsetNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xRangeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yRangeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown xOffsetNumericUpDown;
        public System.Windows.Forms.NumericUpDown yOffsetNumericUpDown;
        private System.Windows.Forms.NumericUpDown xRangeNumericUpDown;
        private System.Windows.Forms.NumericUpDown yRangeNumericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}