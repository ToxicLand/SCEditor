
namespace SCEditor.Prompts
{
    partial class editMatrixes
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
            this.matrixList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.saveMatrixButton = new System.Windows.Forms.Button();
            this.transformIDNum = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.matrixIdNum = new System.Windows.Forms.NumericUpDown();
            this.selectId = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.transformIDNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixIdNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            this.SuspendLayout();
            // 
            // matrixList
            // 
            this.matrixList.FormattingEnabled = true;
            this.matrixList.ItemHeight = 15;
            this.matrixList.Location = new System.Drawing.Point(12, 91);
            this.matrixList.Name = "matrixList";
            this.matrixList.Size = new System.Drawing.Size(229, 199);
            this.matrixList.TabIndex = 0;
            this.matrixList.SelectedIndexChanged += new System.EventHandler(this.matrixList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(255, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Value 1 (Scale X):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(407, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Value 2 (Rotation):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(255, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Value 3 (Rotation):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(407, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Value 4 (Scale Y):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Value 5 (-Left + Right):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(407, 169);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Value 6 (-Up +Down):";
            // 
            // saveMatrixButton
            // 
            this.saveMatrixButton.BackColor = System.Drawing.Color.Chartreuse;
            this.saveMatrixButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveMatrixButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.saveMatrixButton.ForeColor = System.Drawing.Color.Black;
            this.saveMatrixButton.Location = new System.Drawing.Point(407, 254);
            this.saveMatrixButton.Name = "saveMatrixButton";
            this.saveMatrixButton.Size = new System.Drawing.Size(127, 36);
            this.saveMatrixButton.TabIndex = 3;
            this.saveMatrixButton.Text = "Add Matrix";
            this.saveMatrixButton.UseVisualStyleBackColor = false;
            this.saveMatrixButton.Click += new System.EventHandler(this.saveMatrixButton_Click);
            // 
            // transformIDNum
            // 
            this.transformIDNum.Location = new System.Drawing.Point(107, 62);
            this.transformIDNum.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.transformIDNum.Name = "transformIDNum";
            this.transformIDNum.Size = new System.Drawing.Size(134, 23);
            this.transformIDNum.TabIndex = 33;
            this.transformIDNum.ValueChanged += new System.EventHandler(this.transformIDNum_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(10, 62);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(91, 19);
            this.label13.TabIndex = 32;
            this.label13.Text = "Transform ID:";
            // 
            // matrixIdNum
            // 
            this.matrixIdNum.Location = new System.Drawing.Point(12, 30);
            this.matrixIdNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.matrixIdNum.Name = "matrixIdNum";
            this.matrixIdNum.Size = new System.Drawing.Size(148, 23);
            this.matrixIdNum.TabIndex = 31;
            // 
            // selectId
            // 
            this.selectId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.selectId.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectId.Location = new System.Drawing.Point(166, 29);
            this.selectId.Name = "selectId";
            this.selectId.Size = new System.Drawing.Size(75, 23);
            this.selectId.TabIndex = 30;
            this.selectId.Text = "Select";
            this.selectId.UseVisualStyleBackColor = false;
            this.selectId.Click += new System.EventHandler(this.selectId_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 15);
            this.label7.TabIndex = 29;
            this.label7.Text = "Select ID:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown1.Location = new System.Drawing.Point(255, 50);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown1.TabIndex = 34;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown2.Location = new System.Drawing.Point(407, 50);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown2.TabIndex = 35;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown3.Location = new System.Drawing.Point(255, 117);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown3.TabIndex = 36;
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown4.Location = new System.Drawing.Point(407, 117);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown4.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown4.TabIndex = 37;
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown5.Location = new System.Drawing.Point(255, 187);
            this.numericUpDown5.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown5.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown5.TabIndex = 38;
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown6.Location = new System.Drawing.Point(407, 187);
            this.numericUpDown6.Maximum = new decimal(new int[] {
            6553501,
            0,
            0,
            131072});
            this.numericUpDown6.Minimum = new decimal(new int[] {
            6553501,
            0,
            0,
            -2147352576});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(125, 23);
            this.numericUpDown6.TabIndex = 39;
            // 
            // editMatrixes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(555, 310);
            this.Controls.Add(this.numericUpDown6);
            this.Controls.Add(this.numericUpDown5);
            this.Controls.Add(this.numericUpDown4);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.transformIDNum);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.matrixIdNum);
            this.Controls.Add(this.selectId);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.saveMatrixButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matrixList);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "editMatrixes";
            this.Text = "Matrix List and Edit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.editMatrixes_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.transformIDNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixIdNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox matrixList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button saveMatrixButton;
        private System.Windows.Forms.NumericUpDown transformIDNum;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown matrixIdNum;
        private System.Windows.Forms.Button selectId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
    }
}