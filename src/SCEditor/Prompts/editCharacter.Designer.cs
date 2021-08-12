
namespace SCEditor.Prompts
{
    partial class editCharacter
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.editCharacterButtonUp = new System.Windows.Forms.Button();
            this.editCharacterButtonDown = new System.Windows.Forms.Button();
            this.editCharacterButtonLeft = new System.Windows.Forms.Button();
            this.editCharacterButtonRight = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.editCharacterButtonSizeIncrease = new System.Windows.Forms.Button();
            this.editCharacterButtonSizeDecrease = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(242, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(546, 484);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // editCharacterButtonUp
            // 
            this.editCharacterButtonUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonUp.Location = new System.Drawing.Point(84, 329);
            this.editCharacterButtonUp.Name = "editCharacterButtonUp";
            this.editCharacterButtonUp.Size = new System.Drawing.Size(75, 32);
            this.editCharacterButtonUp.TabIndex = 1;
            this.editCharacterButtonUp.Text = "Up";
            this.editCharacterButtonUp.UseVisualStyleBackColor = true;
            this.editCharacterButtonUp.Click += new System.EventHandler(this.editCharacterButtonUp_Click);
            // 
            // editCharacterButtonDown
            // 
            this.editCharacterButtonDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonDown.Location = new System.Drawing.Point(84, 405);
            this.editCharacterButtonDown.Name = "editCharacterButtonDown";
            this.editCharacterButtonDown.Size = new System.Drawing.Size(75, 30);
            this.editCharacterButtonDown.TabIndex = 1;
            this.editCharacterButtonDown.Text = "Down";
            this.editCharacterButtonDown.UseVisualStyleBackColor = true;
            this.editCharacterButtonDown.Click += new System.EventHandler(this.editCharacterButtonDown_Click);
            // 
            // editCharacterButtonLeft
            // 
            this.editCharacterButtonLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonLeft.Location = new System.Drawing.Point(30, 367);
            this.editCharacterButtonLeft.Name = "editCharacterButtonLeft";
            this.editCharacterButtonLeft.Size = new System.Drawing.Size(75, 32);
            this.editCharacterButtonLeft.TabIndex = 1;
            this.editCharacterButtonLeft.Text = "Left";
            this.editCharacterButtonLeft.UseVisualStyleBackColor = true;
            this.editCharacterButtonLeft.Click += new System.EventHandler(this.editCharacterButtonLeft_Click);
            // 
            // editCharacterButtonRight
            // 
            this.editCharacterButtonRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonRight.Location = new System.Drawing.Point(140, 367);
            this.editCharacterButtonRight.Name = "editCharacterButtonRight";
            this.editCharacterButtonRight.Size = new System.Drawing.Size(75, 32);
            this.editCharacterButtonRight.TabIndex = 1;
            this.editCharacterButtonRight.Text = "Right";
            this.editCharacterButtonRight.UseVisualStyleBackColor = true;
            this.editCharacterButtonRight.Click += new System.EventHandler(this.editCharacterButtonRight_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(224, 214);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // editCharacterButtonSizeIncrease
            // 
            this.editCharacterButtonSizeIncrease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonSizeIncrease.Location = new System.Drawing.Point(30, 260);
            this.editCharacterButtonSizeIncrease.Name = "editCharacterButtonSizeIncrease";
            this.editCharacterButtonSizeIncrease.Size = new System.Drawing.Size(75, 31);
            this.editCharacterButtonSizeIncrease.TabIndex = 3;
            this.editCharacterButtonSizeIncrease.Text = "Increase";
            this.editCharacterButtonSizeIncrease.UseVisualStyleBackColor = true;
            this.editCharacterButtonSizeIncrease.Click += new System.EventHandler(this.editCharacterButtonSizeIncrease_Click);
            // 
            // editCharacterButtonSizeDecrease
            // 
            this.editCharacterButtonSizeDecrease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editCharacterButtonSizeDecrease.Location = new System.Drawing.Point(140, 260);
            this.editCharacterButtonSizeDecrease.Name = "editCharacterButtonSizeDecrease";
            this.editCharacterButtonSizeDecrease.Size = new System.Drawing.Size(75, 31);
            this.editCharacterButtonSizeDecrease.TabIndex = 4;
            this.editCharacterButtonSizeDecrease.Text = "Decrease";
            this.editCharacterButtonSizeDecrease.UseVisualStyleBackColor = true;
            this.editCharacterButtonSizeDecrease.Click += new System.EventHandler(this.editCharacterButtonSizeDecrease_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(81, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Edit Position";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(91, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Edit Size";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(12, 465);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(224, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "SAVE";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // editCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(800, 508);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.editCharacterButtonSizeDecrease);
            this.Controls.Add(this.editCharacterButtonSizeIncrease);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.editCharacterButtonRight);
            this.Controls.Add(this.editCharacterButtonLeft);
            this.Controls.Add(this.editCharacterButtonDown);
            this.Controls.Add(this.editCharacterButtonUp);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "editCharacter";
            this.Text = "Edit Character";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.Button editCharacterButtonUp;
        internal System.Windows.Forms.Button editCharacterButtonDown;
        internal System.Windows.Forms.Button editCharacterButtonLeft;
        internal System.Windows.Forms.Button editCharacterButtonRight;
        internal System.Windows.Forms.TreeView treeView1;
        internal System.Windows.Forms.Button editCharacterButtonSizeIncrease;
        internal System.Windows.Forms.Button editCharacterButtonSizeDecrease;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Button button1;
    }
}