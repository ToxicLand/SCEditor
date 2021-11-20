using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEditor.Prompts
{
    public partial class inputDataDialog : Form
    {
        public int inputTextBoxInt { get; private set; }
        public string inputTextBoxString { get; private set; }
        public int inputTextBoxType { get; set; }
        public inputDataDialog(int inputType)
        {
            InitializeComponent();

            inputTextBoxType = inputType;
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            if (inputTextBoxType == 1)
            {
                if (!string.IsNullOrEmpty(inputTextBox.Text))
                    inputTextBoxInt = int.Parse(inputTextBox.Text);
                else
                    inputTextBoxInt = 0;
            }  
            else if (inputTextBoxType == 0)
                inputTextBoxString = inputTextBox.Text;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputTextBoxType == 1)
            {
                if (e.KeyData != Keys.Back)
                {
                    if (int.TryParse(Convert.ToString((char)e.KeyData), out int _))
                    {
                        if (Convert.ToInt64(string.Format("{0}{1}", inputTextBoxInt, int.Parse(Convert.ToString((char)e.KeyData)))) >= Int32.MaxValue)
                        {
                            e.SuppressKeyPress = true;
                        }
                    }
                    else
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }

        public void setLabelText(string text)
        {
            this.inputValueLabel.Text = text;
        }
    }
}
