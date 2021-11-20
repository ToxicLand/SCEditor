
namespace SCEditor.Prompts
{
    partial class scMergeSelection
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
            this.exportsListBox = new System.Windows.Forms.CheckedListBox();
            this.importButton = new System.Windows.Forms.Button();
            this.checkAllButton = new System.Windows.Forms.Button();
            this.scaleFactorTextBox = new System.Windows.Forms.TextBox();
            this.newTextureCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exportsListBox
            // 
            this.exportsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.exportsListBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.exportsListBox.ForeColor = System.Drawing.Color.White;
            this.exportsListBox.FormattingEnabled = true;
            this.exportsListBox.Location = new System.Drawing.Point(12, 12);
            this.exportsListBox.Name = "exportsListBox";
            this.exportsListBox.Size = new System.Drawing.Size(276, 384);
            this.exportsListBox.TabIndex = 0;
            // 
            // importButton
            // 
            this.importButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importButton.Location = new System.Drawing.Point(155, 511);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(133, 38);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = false;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // checkAllButton
            // 
            this.checkAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.checkAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkAllButton.Location = new System.Drawing.Point(12, 392);
            this.checkAllButton.Name = "checkAllButton";
            this.checkAllButton.Size = new System.Drawing.Size(276, 30);
            this.checkAllButton.TabIndex = 1;
            this.checkAllButton.Text = "Check All";
            this.checkAllButton.UseVisualStyleBackColor = false;
            this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
            // 
            // scaleFactorTextBox
            // 
            this.scaleFactorTextBox.Location = new System.Drawing.Point(12, 464);
            this.scaleFactorTextBox.Name = "scaleFactorTextBox";
            this.scaleFactorTextBox.Size = new System.Drawing.Size(162, 23);
            this.scaleFactorTextBox.TabIndex = 2;
            this.scaleFactorTextBox.TextChanged += new System.EventHandler(this.scaleFactorTextBox_TextChanged);
            this.scaleFactorTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scaleFactorTextBox_KeyDown);
            // 
            // newTextureCheckBox
            // 
            this.newTextureCheckBox.AutoSize = true;
            this.newTextureCheckBox.Location = new System.Drawing.Point(197, 464);
            this.newTextureCheckBox.Name = "newTextureCheckBox";
            this.newTextureCheckBox.Size = new System.Drawing.Size(91, 19);
            this.newTextureCheckBox.TabIndex = 3;
            this.newTextureCheckBox.Text = "New Texture";
            this.newTextureCheckBox.UseVisualStyleBackColor = true;
            this.newTextureCheckBox.CheckedChanged += new System.EventHandler(this.newTextureCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 446);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Chunks Scale Factor:";
            // 
            // scMergeSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(300, 561);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.newTextureCheckBox);
            this.Controls.Add(this.scaleFactorTextBox);
            this.Controls.Add(this.checkAllButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportsListBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "scMergeSelection";
            this.Text = "scMergeSelection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox exportsListBox;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button checkAllButton;
        private System.Windows.Forms.TextBox scaleFactorTextBox;
        private System.Windows.Forms.CheckBox newTextureCheckBox;
        private System.Windows.Forms.Label label1;
    }
}