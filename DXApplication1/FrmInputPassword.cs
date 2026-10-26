using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class frmInputPassword : Form
    {
        bool key_entered = false;
        public frmInputPassword(string default_value = "", bool show_pass = false)
        {
            InitializeComponent();
            chkPWChar.Checked = show_pass;
            txtInput.Text = default_value;
        }

        private void chkPWChar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPWChar.Checked)
                txtInput.PasswordChar = '\0';
            else
                txtInput.PasswordChar = '*';
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                key_entered = true;
                this.Close();
            }
            else if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frmInputPassword_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!key_entered) txtInput.Text = "";
            
        }

        private void frmInputPassword_Load(object sender, EventArgs e)
        {
            chkPWChar_CheckedChanged(null, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            key_entered = true;
            this.Close();
        }

    }
}
