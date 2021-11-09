
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
            this.exportsListBox.Size = new System.Drawing.Size(221, 526);
            this.exportsListBox.TabIndex = 0;
            // 
            // importButton
            // 
            this.importButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importButton.Location = new System.Drawing.Point(446, 490);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(183, 48);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = false;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // scMergeSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(641, 560);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.exportsListBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "scMergeSelection";
            this.Text = "scMergeSelection";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox exportsListBox;
        private System.Windows.Forms.Button importButton;
    }
}