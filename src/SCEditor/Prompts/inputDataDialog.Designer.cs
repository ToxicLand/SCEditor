
namespace SCEditor.Prompts
{
    partial class inputDataDialog
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
            this.inputValueLabel = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.submitButtom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputValueLabel
            // 
            this.inputValueLabel.AutoSize = true;
            this.inputValueLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.inputValueLabel.Location = new System.Drawing.Point(12, 9);
            this.inputValueLabel.Name = "inputValueLabel";
            this.inputValueLabel.Size = new System.Drawing.Size(79, 19);
            this.inputValueLabel.TabIndex = 0;
            this.inputValueLabel.Text = "Input Value";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(12, 31);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(154, 23);
            this.inputTextBox.TabIndex = 1;
            this.inputTextBox.TextChanged += new System.EventHandler(this.inputTextBox_TextChanged);
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputTextBox_KeyDown);
            // 
            // submitButtom
            // 
            this.submitButtom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.submitButtom.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.submitButtom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.submitButtom.Location = new System.Drawing.Point(256, 22);
            this.submitButtom.Name = "submitButtom";
            this.submitButtom.Size = new System.Drawing.Size(75, 32);
            this.submitButtom.TabIndex = 2;
            this.submitButtom.Text = "Submit";
            this.submitButtom.UseVisualStyleBackColor = false;
            // 
            // inputDataDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(343, 70);
            this.Controls.Add(this.submitButtom);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.inputValueLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "inputDataDialog";
            this.Text = "inputDataDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label inputValueLabel;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button submitButtom;
    }
}