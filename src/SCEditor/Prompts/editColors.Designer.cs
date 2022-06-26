namespace SCEditor.Prompts
{
    partial class editColors
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
            this.colorsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.selectId = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.toColorNumR = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.toColorNumG = new System.Windows.Forms.NumericUpDown();
            this.toColorNumB = new System.Windows.Forms.NumericUpDown();
            this.toColorNumA = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.fromColorNumA = new System.Windows.Forms.NumericUpDown();
            this.fromColorNumB = new System.Windows.Forms.NumericUpDown();
            this.fromColorNumG = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.fromColorNumR = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.colorAlphaNum = new System.Windows.Forms.NumericUpDown();
            this.colorIdNum = new System.Windows.Forms.NumericUpDown();
            this.transformIDNum = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorIdNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transformIDNum)).BeginInit();
            this.SuspendLayout();
            // 
            // colorsListBox
            // 
            this.colorsListBox.FormattingEnabled = true;
            this.colorsListBox.ItemHeight = 15;
            this.colorsListBox.Location = new System.Drawing.Point(12, 87);
            this.colorsListBox.Name = "colorsListBox";
            this.colorsListBox.Size = new System.Drawing.Size(231, 229);
            this.colorsListBox.TabIndex = 0;
            this.colorsListBox.SelectedIndexChanged += new System.EventHandler(this.colorsListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select ID:";
            // 
            // selectId
            // 
            this.selectId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.selectId.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectId.Location = new System.Drawing.Point(168, 25);
            this.selectId.Name = "selectId";
            this.selectId.Size = new System.Drawing.Size(75, 23);
            this.selectId.TabIndex = 3;
            this.selectId.Text = "Select";
            this.selectId.UseVisualStyleBackColor = false;
            this.selectId.Click += new System.EventHandler(this.selectId_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(380, 276);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 40);
            this.button1.TabIndex = 4;
            this.button1.Text = "Add Color";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(271, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Color (to color):";
            // 
            // toColorNumR
            // 
            this.toColorNumR.Location = new System.Drawing.Point(294, 57);
            this.toColorNumR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.toColorNumR.Name = "toColorNumR";
            this.toColorNumR.Size = new System.Drawing.Size(53, 23);
            this.toColorNumR.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(271, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "R";
            // 
            // toColorNumG
            // 
            this.toColorNumG.Location = new System.Drawing.Point(391, 58);
            this.toColorNumG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.toColorNumG.Name = "toColorNumG";
            this.toColorNumG.Size = new System.Drawing.Size(67, 23);
            this.toColorNumG.TabIndex = 9;
            // 
            // toColorNumB
            // 
            this.toColorNumB.Location = new System.Drawing.Point(295, 87);
            this.toColorNumB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.toColorNumB.Name = "toColorNumB";
            this.toColorNumB.Size = new System.Drawing.Size(53, 23);
            this.toColorNumB.TabIndex = 10;
            // 
            // toColorNumA
            // 
            this.toColorNumA.Location = new System.Drawing.Point(391, 87);
            this.toColorNumA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.toColorNumA.Name = "toColorNumA";
            this.toColorNumA.Size = new System.Drawing.Size(67, 23);
            this.toColorNumA.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(366, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 19);
            this.label4.TabIndex = 12;
            this.label4.Text = "G";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(271, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 19);
            this.label5.TabIndex = 13;
            this.label5.Text = "B";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(367, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 19);
            this.label6.TabIndex = 14;
            this.label6.Text = "A";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(367, 206);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 19);
            this.label7.TabIndex = 23;
            this.label7.Text = "A";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(271, 205);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 19);
            this.label8.TabIndex = 22;
            this.label8.Text = "B";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(366, 177);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 19);
            this.label9.TabIndex = 21;
            this.label9.Text = "G";
            // 
            // fromColorNumA
            // 
            this.fromColorNumA.Location = new System.Drawing.Point(391, 205);
            this.fromColorNumA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fromColorNumA.Name = "fromColorNumA";
            this.fromColorNumA.Size = new System.Drawing.Size(67, 23);
            this.fromColorNumA.TabIndex = 20;
            // 
            // fromColorNumB
            // 
            this.fromColorNumB.Location = new System.Drawing.Point(294, 205);
            this.fromColorNumB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fromColorNumB.Name = "fromColorNumB";
            this.fromColorNumB.Size = new System.Drawing.Size(53, 23);
            this.fromColorNumB.TabIndex = 19;
            // 
            // fromColorNumG
            // 
            this.fromColorNumG.Location = new System.Drawing.Point(391, 176);
            this.fromColorNumG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fromColorNumG.Name = "fromColorNumG";
            this.fromColorNumG.Size = new System.Drawing.Size(67, 23);
            this.fromColorNumG.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label10.Location = new System.Drawing.Point(271, 176);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 19);
            this.label10.TabIndex = 17;
            this.label10.Text = "R";
            // 
            // fromColorNumR
            // 
            this.fromColorNumR.Location = new System.Drawing.Point(294, 176);
            this.fromColorNumR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fromColorNumR.Name = "fromColorNumR";
            this.fromColorNumR.Size = new System.Drawing.Size(53, 23);
            this.fromColorNumR.TabIndex = 16;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(278, 149);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 19);
            this.label11.TabIndex = 15;
            this.label11.Text = "Color (from color):";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label12.Location = new System.Drawing.Point(274, 268);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 19);
            this.label12.TabIndex = 24;
            this.label12.Text = "Color Alpha:";
            // 
            // colorAlphaNum
            // 
            this.colorAlphaNum.Location = new System.Drawing.Point(278, 293);
            this.colorAlphaNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.colorAlphaNum.Name = "colorAlphaNum";
            this.colorAlphaNum.Size = new System.Drawing.Size(80, 23);
            this.colorAlphaNum.TabIndex = 25;
            // 
            // colorIdNum
            // 
            this.colorIdNum.Location = new System.Drawing.Point(14, 26);
            this.colorIdNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.colorIdNum.Name = "colorIdNum";
            this.colorIdNum.Size = new System.Drawing.Size(148, 23);
            this.colorIdNum.TabIndex = 26;
            // 
            // transformIDNum
            // 
            this.transformIDNum.Location = new System.Drawing.Point(109, 58);
            this.transformIDNum.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.transformIDNum.Name = "transformIDNum";
            this.transformIDNum.Size = new System.Drawing.Size(134, 23);
            this.transformIDNum.TabIndex = 28;
            this.transformIDNum.ValueChanged += new System.EventHandler(this.transformIDNum_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(12, 58);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(91, 19);
            this.label13.TabIndex = 27;
            this.label13.Text = "Transform ID:";
            // 
            // editColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(482, 337);
            this.Controls.Add(this.transformIDNum);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.colorIdNum);
            this.Controls.Add(this.colorAlphaNum);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.fromColorNumA);
            this.Controls.Add(this.fromColorNumB);
            this.Controls.Add(this.fromColorNumG);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.fromColorNumR);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.toColorNumA);
            this.Controls.Add(this.toColorNumB);
            this.Controls.Add(this.toColorNumG);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.toColorNumR);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.selectId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.colorsListBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "editColors";
            this.Text = "Colors List / Add-Edit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.editColors_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toColorNumA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromColorNumR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorAlphaNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorIdNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transformIDNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox colorsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectId;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown toColorNumR;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown toColorNumG;
        private System.Windows.Forms.NumericUpDown toColorNumB;
        private System.Windows.Forms.NumericUpDown toColorNumA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown fromColorNumA;
        private System.Windows.Forms.NumericUpDown fromColorNumB;
        private System.Windows.Forms.NumericUpDown fromColorNumG;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown fromColorNumR;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown colorAlphaNum;
        private System.Windows.Forms.NumericUpDown colorIdNum;
        private System.Windows.Forms.NumericUpDown transformIDNum;
        private System.Windows.Forms.Label label13;
    }
}