namespace SCEditor.Prompts
{
    partial class editChildrenData
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
            this.childrenIdListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.childrenNameListBox = new System.Windows.Forms.ListBox();
            this.childrenNameTextBox = new System.Windows.Forms.TextBox();
            this.childrenIdTextBox = new System.Windows.Forms.TextBox();
            this.addChildrenAfter = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.addChildrenBefore = new System.Windows.Forms.Button();
            this.deleteChildrenButtom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // childrenIdListBox
            // 
            this.childrenIdListBox.FormattingEnabled = true;
            this.childrenIdListBox.ItemHeight = 15;
            this.childrenIdListBox.Location = new System.Drawing.Point(22, 38);
            this.childrenIdListBox.Name = "childrenIdListBox";
            this.childrenIdListBox.Size = new System.Drawing.Size(152, 124);
            this.childrenIdListBox.TabIndex = 0;
            this.childrenIdListBox.SelectedIndexChanged += new System.EventHandler(this.childrenIdListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Children IDs:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(204, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Children Names:";
            // 
            // childrenNameListBox
            // 
            this.childrenNameListBox.FormattingEnabled = true;
            this.childrenNameListBox.ItemHeight = 15;
            this.childrenNameListBox.Location = new System.Drawing.Point(204, 38);
            this.childrenNameListBox.Name = "childrenNameListBox";
            this.childrenNameListBox.Size = new System.Drawing.Size(152, 124);
            this.childrenNameListBox.TabIndex = 3;
            this.childrenNameListBox.SelectedIndexChanged += new System.EventHandler(this.childrenNameListBox_SelectedIndexChanged);
            // 
            // childrenNameTextBox
            // 
            this.childrenNameTextBox.Enabled = false;
            this.childrenNameTextBox.Location = new System.Drawing.Point(204, 180);
            this.childrenNameTextBox.Name = "childrenNameTextBox";
            this.childrenNameTextBox.Size = new System.Drawing.Size(152, 23);
            this.childrenNameTextBox.TabIndex = 4;
            // 
            // childrenIdTextBox
            // 
            this.childrenIdTextBox.Enabled = false;
            this.childrenIdTextBox.Location = new System.Drawing.Point(22, 180);
            this.childrenIdTextBox.Name = "childrenIdTextBox";
            this.childrenIdTextBox.Size = new System.Drawing.Size(152, 23);
            this.childrenIdTextBox.TabIndex = 5;
            this.childrenIdTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.childrenIdTextBox_KeyDown);
            // 
            // addChildrenAfter
            // 
            this.addChildrenAfter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.addChildrenAfter.Enabled = false;
            this.addChildrenAfter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addChildrenAfter.Location = new System.Drawing.Point(142, 225);
            this.addChildrenAfter.Name = "addChildrenAfter";
            this.addChildrenAfter.Size = new System.Drawing.Size(84, 36);
            this.addChildrenAfter.TabIndex = 6;
            this.addChildrenAfter.Text = "Add After";
            this.addChildrenAfter.UseVisualStyleBackColor = false;
            this.addChildrenAfter.Click += new System.EventHandler(this.addChildrenAfter_Click);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(74)))), ((int)(((byte)(74)))));
            this.saveButton.Enabled = false;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Location = new System.Drawing.Point(259, 225);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(97, 36);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // addChildrenBefore
            // 
            this.addChildrenBefore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.addChildrenBefore.Enabled = false;
            this.addChildrenBefore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addChildrenBefore.Location = new System.Drawing.Point(22, 225);
            this.addChildrenBefore.Name = "addChildrenBefore";
            this.addChildrenBefore.Size = new System.Drawing.Size(84, 36);
            this.addChildrenBefore.TabIndex = 8;
            this.addChildrenBefore.Text = "Add Before";
            this.addChildrenBefore.UseVisualStyleBackColor = false;
            this.addChildrenBefore.Click += new System.EventHandler(this.addChildrenBefore_Click);
            // 
            // deleteChildrenButtom
            // 
            this.deleteChildrenButtom.BackColor = System.Drawing.Color.LightCoral;
            this.deleteChildrenButtom.Enabled = false;
            this.deleteChildrenButtom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteChildrenButtom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.deleteChildrenButtom.ForeColor = System.Drawing.Color.White;
            this.deleteChildrenButtom.Location = new System.Drawing.Point(112, 225);
            this.deleteChildrenButtom.Name = "deleteChildrenButtom";
            this.deleteChildrenButtom.Size = new System.Drawing.Size(24, 36);
            this.deleteChildrenButtom.TabIndex = 9;
            this.deleteChildrenButtom.Text = "X";
            this.deleteChildrenButtom.UseVisualStyleBackColor = false;
            this.deleteChildrenButtom.Click += new System.EventHandler(this.deleteChildrenButtom_Click);
            // 
            // editChildrenData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(379, 291);
            this.Controls.Add(this.deleteChildrenButtom);
            this.Controls.Add(this.addChildrenBefore);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.addChildrenAfter);
            this.Controls.Add(this.childrenIdTextBox);
            this.Controls.Add(this.childrenNameTextBox);
            this.Controls.Add(this.childrenNameListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.childrenIdListBox);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "editChildrenData";
            this.Text = "Edit MovieClip Chidlren ID & Name";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.editChildrenData_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox childrenIdListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox childrenNameListBox;
        private System.Windows.Forms.TextBox childrenNameTextBox;
        private System.Windows.Forms.TextBox childrenIdTextBox;
        private System.Windows.Forms.Button addChildrenAfter;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button addChildrenBefore;
        private System.Windows.Forms.Button deleteChildrenButtom;
    }
}