
namespace SCEditor.Prompts
{
    partial class frameEditDialog
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
            this.FramesArrayBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.frameTimelineDataBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataTypeTextBox = new System.Windows.Forms.TextBox();
            this.editDataTypeLabel = new System.Windows.Forms.Label();
            this.dataTypeEditSubmitButton = new System.Windows.Forms.Button();
            this.addFrameBeforeSelectedButton = new System.Windows.Forms.Button();
            this.addFrameAfterSelectedButton = new System.Windows.Forms.Button();
            this.deleteSelectedButton = new System.Windows.Forms.Button();
            this.changeAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FramesArrayBox
            // 
            this.FramesArrayBox.FormattingEnabled = true;
            this.FramesArrayBox.Location = new System.Drawing.Point(26, 103);
            this.FramesArrayBox.Name = "FramesArrayBox";
            this.FramesArrayBox.Size = new System.Drawing.Size(170, 23);
            this.FramesArrayBox.TabIndex = 0;
            this.FramesArrayBox.SelectedIndexChanged += new System.EventHandler(this.timelineArrayBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "MovieClip Frames:";
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(280, 224);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(117, 41);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // frameTimelineDataBox
            // 
            this.frameTimelineDataBox.FormattingEnabled = true;
            this.frameTimelineDataBox.ItemHeight = 15;
            this.frameTimelineDataBox.Location = new System.Drawing.Point(26, 155);
            this.frameTimelineDataBox.Name = "frameTimelineDataBox";
            this.frameTimelineDataBox.Size = new System.Drawing.Size(170, 109);
            this.frameTimelineDataBox.TabIndex = 3;
            this.frameTimelineDataBox.SelectedIndexChanged += new System.EventHandler(this.frameTimelineDataBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Frame Timeline Data:";
            // 
            // dataTypeTextBox
            // 
            this.dataTypeTextBox.Enabled = false;
            this.dataTypeTextBox.Location = new System.Drawing.Point(256, 104);
            this.dataTypeTextBox.Name = "dataTypeTextBox";
            this.dataTypeTextBox.Size = new System.Drawing.Size(141, 23);
            this.dataTypeTextBox.TabIndex = 4;
            this.dataTypeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTypeTextBox_KeyDown);
            // 
            // editDataTypeLabel
            // 
            this.editDataTypeLabel.AutoSize = true;
            this.editDataTypeLabel.Location = new System.Drawing.Point(256, 86);
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
            this.dataTypeEditSubmitButton.Location = new System.Drawing.Point(256, 133);
            this.dataTypeEditSubmitButton.Margin = new System.Windows.Forms.Padding(1);
            this.dataTypeEditSubmitButton.Name = "dataTypeEditSubmitButton";
            this.dataTypeEditSubmitButton.Size = new System.Drawing.Size(141, 33);
            this.dataTypeEditSubmitButton.TabIndex = 6;
            this.dataTypeEditSubmitButton.Text = "Change";
            this.dataTypeEditSubmitButton.UseVisualStyleBackColor = false;
            this.dataTypeEditSubmitButton.Click += new System.EventHandler(this.dataTypeEditSubmitButton_Click);
            // 
            // addFrameBeforeSelectedButton
            // 
            this.addFrameBeforeSelectedButton.Enabled = false;
            this.addFrameBeforeSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addFrameBeforeSelectedButton.Location = new System.Drawing.Point(26, 26);
            this.addFrameBeforeSelectedButton.Name = "addFrameBeforeSelectedButton";
            this.addFrameBeforeSelectedButton.Size = new System.Drawing.Size(118, 34);
            this.addFrameBeforeSelectedButton.TabIndex = 8;
            this.addFrameBeforeSelectedButton.Text = "Add Frame Before";
            this.addFrameBeforeSelectedButton.UseVisualStyleBackColor = true;
            this.addFrameBeforeSelectedButton.Click += new System.EventHandler(this.addFrameBeforeSelectedButton_Click);
            // 
            // addFrameAfterSelectedButton
            // 
            this.addFrameAfterSelectedButton.Enabled = false;
            this.addFrameAfterSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addFrameAfterSelectedButton.Location = new System.Drawing.Point(150, 26);
            this.addFrameAfterSelectedButton.Name = "addFrameAfterSelectedButton";
            this.addFrameAfterSelectedButton.Size = new System.Drawing.Size(122, 34);
            this.addFrameAfterSelectedButton.TabIndex = 8;
            this.addFrameAfterSelectedButton.Text = "Add Frame After";
            this.addFrameAfterSelectedButton.UseVisualStyleBackColor = true;
            this.addFrameAfterSelectedButton.Click += new System.EventHandler(this.addFrameAfterSelectedButton_Click);
            // 
            // deleteSelectedButton
            // 
            this.deleteSelectedButton.Enabled = false;
            this.deleteSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteSelectedButton.Location = new System.Drawing.Point(290, 26);
            this.deleteSelectedButton.Name = "deleteSelectedButton";
            this.deleteSelectedButton.Size = new System.Drawing.Size(107, 34);
            this.deleteSelectedButton.TabIndex = 8;
            this.deleteSelectedButton.Text = "Delete Frame";
            this.deleteSelectedButton.UseVisualStyleBackColor = true;
            this.deleteSelectedButton.Click += new System.EventHandler(this.deleteSelectedButton_Click);
            // 
            // changeAllButton
            // 
            this.changeAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.changeAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeAllButton.Location = new System.Drawing.Point(256, 168);
            this.changeAllButton.Margin = new System.Windows.Forms.Padding(1);
            this.changeAllButton.Name = "changeAllButton";
            this.changeAllButton.Size = new System.Drawing.Size(141, 28);
            this.changeAllButton.TabIndex = 6;
            this.changeAllButton.Text = "Change All";
            this.changeAllButton.UseVisualStyleBackColor = false;
            this.changeAllButton.Click += new System.EventHandler(this.changeAllButton_Click);
            // 
            // frameEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(422, 294);
            this.Controls.Add(this.addFrameAfterSelectedButton);
            this.Controls.Add(this.deleteSelectedButton);
            this.Controls.Add(this.addFrameBeforeSelectedButton);
            this.Controls.Add(this.changeAllButton);
            this.Controls.Add(this.dataTypeEditSubmitButton);
            this.Controls.Add(this.editDataTypeLabel);
            this.Controls.Add(this.dataTypeTextBox);
            this.Controls.Add(this.frameTimelineDataBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FramesArrayBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frameEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Timeline - Frames Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.timelineEditDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FramesArrayBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ListBox frameTimelineDataBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox dataTypeTextBox;
        private System.Windows.Forms.Label editDataTypeLabel;
        private System.Windows.Forms.Button dataTypeEditSubmitButton;
        private System.Windows.Forms.Button addFrameBeforeSelectedButton;
        private System.Windows.Forms.Button addFrameAfterSelectedButton;
        private System.Windows.Forms.Button deleteSelectedButton;
        private System.Windows.Forms.Button changeAllButton;
    }
}