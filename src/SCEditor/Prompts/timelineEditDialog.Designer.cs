
namespace SCEditor.Prompts
{
    partial class timelineEditDialog
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
            this.timelineArrayBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.timelineDataBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataTypeTextBox = new System.Windows.Forms.TextBox();
            this.editDataTypeLabel = new System.Windows.Forms.Label();
            this.dataTypeEditSubmitButton = new System.Windows.Forms.Button();
            this.addFrameBeforeSelectedButton = new System.Windows.Forms.Button();
            this.addFrameAfterSelectedButton = new System.Windows.Forms.Button();
            this.deleteSelectedButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timelineArrayBox
            // 
            this.timelineArrayBox.FormattingEnabled = true;
            this.timelineArrayBox.Location = new System.Drawing.Point(12, 89);
            this.timelineArrayBox.Name = "timelineArrayBox";
            this.timelineArrayBox.Size = new System.Drawing.Size(170, 23);
            this.timelineArrayBox.TabIndex = 0;
            this.timelineArrayBox.SelectedIndexChanged += new System.EventHandler(this.timelineArrayBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Timeline Frames:";
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(345, 207);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(84, 33);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // timelineDataBox
            // 
            this.timelineDataBox.FormattingEnabled = true;
            this.timelineDataBox.ItemHeight = 15;
            this.timelineDataBox.Location = new System.Drawing.Point(12, 146);
            this.timelineDataBox.Name = "timelineDataBox";
            this.timelineDataBox.Size = new System.Drawing.Size(170, 94);
            this.timelineDataBox.TabIndex = 3;
            this.timelineDataBox.SelectedIndexChanged += new System.EventHandler(this.timelineDataBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Timeline Data:";
            // 
            // dataTypeTextBox
            // 
            this.dataTypeTextBox.Enabled = false;
            this.dataTypeTextBox.Location = new System.Drawing.Point(217, 89);
            this.dataTypeTextBox.Name = "dataTypeTextBox";
            this.dataTypeTextBox.Size = new System.Drawing.Size(141, 23);
            this.dataTypeTextBox.TabIndex = 4;
            this.dataTypeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTypeTextBox_KeyDown);
            // 
            // editDataTypeLabel
            // 
            this.editDataTypeLabel.AutoSize = true;
            this.editDataTypeLabel.Location = new System.Drawing.Point(217, 71);
            this.editDataTypeLabel.Name = "editDataTypeLabel";
            this.editDataTypeLabel.Size = new System.Drawing.Size(80, 15);
            this.editDataTypeLabel.TabIndex = 5;
            this.editDataTypeLabel.Text = "Edit Datatype:";
            // 
            // dataTypeEditSubmitButton
            // 
            this.dataTypeEditSubmitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.dataTypeEditSubmitButton.Enabled = false;
            this.dataTypeEditSubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dataTypeEditSubmitButton.Location = new System.Drawing.Point(332, 88);
            this.dataTypeEditSubmitButton.Margin = new System.Windows.Forms.Padding(1);
            this.dataTypeEditSubmitButton.Name = "dataTypeEditSubmitButton";
            this.dataTypeEditSubmitButton.Size = new System.Drawing.Size(86, 24);
            this.dataTypeEditSubmitButton.TabIndex = 6;
            this.dataTypeEditSubmitButton.Text = "Change";
            this.dataTypeEditSubmitButton.UseVisualStyleBackColor = false;
            this.dataTypeEditSubmitButton.Click += new System.EventHandler(this.dataTypeEditSubmitButton_Click);
            // 
            // addFrameBeforeSelectedButton
            // 
            this.addFrameBeforeSelectedButton.Enabled = false;
            this.addFrameBeforeSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addFrameBeforeSelectedButton.Location = new System.Drawing.Point(12, 12);
            this.addFrameBeforeSelectedButton.Name = "addFrameBeforeSelectedButton";
            this.addFrameBeforeSelectedButton.Size = new System.Drawing.Size(142, 34);
            this.addFrameBeforeSelectedButton.TabIndex = 8;
            this.addFrameBeforeSelectedButton.Text = "Add Before Selected";
            this.addFrameBeforeSelectedButton.UseVisualStyleBackColor = true;
            this.addFrameBeforeSelectedButton.Click += new System.EventHandler(this.addFrameBeforeSelectedButton_Click);
            // 
            // addFrameAfterSelectedButton
            // 
            this.addFrameAfterSelectedButton.Enabled = false;
            this.addFrameAfterSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addFrameAfterSelectedButton.Location = new System.Drawing.Point(160, 12);
            this.addFrameAfterSelectedButton.Name = "addFrameAfterSelectedButton";
            this.addFrameAfterSelectedButton.Size = new System.Drawing.Size(142, 34);
            this.addFrameAfterSelectedButton.TabIndex = 8;
            this.addFrameAfterSelectedButton.Text = "Add After Selected";
            this.addFrameAfterSelectedButton.UseVisualStyleBackColor = true;
            this.addFrameAfterSelectedButton.Click += new System.EventHandler(this.addFrameAfterSelectedButton_Click);
            // 
            // deleteSelectedButton
            // 
            this.deleteSelectedButton.Enabled = false;
            this.deleteSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteSelectedButton.Location = new System.Drawing.Point(322, 12);
            this.deleteSelectedButton.Name = "deleteSelectedButton";
            this.deleteSelectedButton.Size = new System.Drawing.Size(107, 34);
            this.deleteSelectedButton.TabIndex = 8;
            this.deleteSelectedButton.Text = "Delete Selected";
            this.deleteSelectedButton.UseVisualStyleBackColor = true;
            this.deleteSelectedButton.Click += new System.EventHandler(this.deleteSelectedButton_Click);
            // 
            // timelineEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(448, 255);
            this.Controls.Add(this.addFrameAfterSelectedButton);
            this.Controls.Add(this.deleteSelectedButton);
            this.Controls.Add(this.addFrameBeforeSelectedButton);
            this.Controls.Add(this.dataTypeEditSubmitButton);
            this.Controls.Add(this.editDataTypeLabel);
            this.Controls.Add(this.dataTypeTextBox);
            this.Controls.Add(this.timelineDataBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.timelineArrayBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "timelineEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Timeline - Frames Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.timelineEditDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox timelineArrayBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ListBox timelineDataBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox dataTypeTextBox;
        private System.Windows.Forms.Label editDataTypeLabel;
        private System.Windows.Forms.Button dataTypeEditSubmitButton;
        private System.Windows.Forms.Button addFrameBeforeSelectedButton;
        private System.Windows.Forms.Button addFrameAfterSelectedButton;
        private System.Windows.Forms.Button deleteSelectedButton;
    }
}