
namespace SCEditor
{
    partial class createTextureDialog
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
            this.textureWidthTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textureHeightTextBox = new System.Windows.Forms.TextBox();
            this.textureTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.createTextureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textureWidthTextBox
            // 
            this.textureWidthTextBox.Location = new System.Drawing.Point(150, 44);
            this.textureWidthTextBox.Name = "textureWidthTextBox";
            this.textureWidthTextBox.Size = new System.Drawing.Size(100, 23);
            this.textureWidthTextBox.TabIndex = 0;
            this.textureWidthTextBox.TextChanged += new System.EventHandler(this.textureWidthTextBox_TextChanged);
            this.textureWidthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textureWidthTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(150, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width (px)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height (px)";
            // 
            // textureHeightTextBox
            // 
            this.textureHeightTextBox.Location = new System.Drawing.Point(270, 44);
            this.textureHeightTextBox.Name = "textureHeightTextBox";
            this.textureHeightTextBox.Size = new System.Drawing.Size(100, 23);
            this.textureHeightTextBox.TabIndex = 2;
            this.textureHeightTextBox.TextChanged += new System.EventHandler(this.textureHeightTextBox_TextChanged);
            this.textureHeightTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textureHeightTextBox_KeyDown);
            // 
            // textureTypeComboBox
            // 
            this.textureTypeComboBox.FormattingEnabled = true;
            this.textureTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textureTypeComboBox.Items.AddRange(new object[] {
            "ImageRGBA8888",
            "ImageRGBA5551",
            "ImageRGBA4444",
            "ImageRGB565",
            "ImageLuminance8",
            "ImageLuminance8Alpha8",
            "ScImage"});
            this.textureTypeComboBox.Location = new System.Drawing.Point(12, 44);
            this.textureTypeComboBox.Name = "textureTypeComboBox";
            this.textureTypeComboBox.Size = new System.Drawing.Size(121, 23);
            this.textureTypeComboBox.TabIndex = 4;
            this.textureTypeComboBox.SelectedValueChanged += new System.EventHandler(this.textureTypeComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Texture Type";
            // 
            // createTextureButton
            // 
            this.createTextureButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.createTextureButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.createTextureButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.createTextureButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.createTextureButton.Location = new System.Drawing.Point(251, 97);
            this.createTextureButton.Name = "createTextureButton";
            this.createTextureButton.Size = new System.Drawing.Size(119, 28);
            this.createTextureButton.TabIndex = 6;
            this.createTextureButton.Text = "CREATE TEXTURE";
            this.createTextureButton.UseVisualStyleBackColor = false;
            this.createTextureButton.Click += new System.EventHandler(this.createTextureButton_Click);
            // 
            // dialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.ClientSize = new System.Drawing.Size(385, 137);
            this.Controls.Add(this.createTextureButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textureTypeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textureHeightTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textureWidthTextBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "dialogForm";
            this.Text = "Create Texture";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textureWidthTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textureHeightTextBox;
        private System.Windows.Forms.ComboBox textureTypeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button createTextureButton;
    }
}