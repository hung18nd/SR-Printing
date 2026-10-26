using DevExpress.XtraEditors;
using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class FrmChangePass : DevExpress.XtraEditors.XtraForm
    {
        public FrmChangePass( )
        {
            InitializeComponent();
        }

        private void btnPassRead_Click(object sender, EventArgs e)
        {
            string oldpass = txtOldPass.Text.Replace("\r", "").Replace("\n", "");
            string newpass = txtNewPass.Text.Replace("\r", "").Replace("\n", "");
            string cfgpass = txtCfmPass.Text.Replace("\r", "").Replace("\n", "");
            if (oldpass != Global.PassworSetting)
            {
                MessageBox.Show("Mật khẩu hiện tại không chính xác!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (oldpass == newpass)
            {
                MessageBox.Show("Mật khẩu mới trùng với mật khẩu cũ!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cfgpass != newpass)
            {
                MessageBox.Show("Mật khẩu nhập lại khác mật khẩu mới!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string file = Global.ApplicationPath() + @"\" + "App.app";
                IniFile ini = new IniFile(file);
                ini.IniWriteValue("SETTING", "PASS", newpass);
                string tmp = ini.IniReadValue("SETTING", "PASS");
                if (tmp != "")
                {
                    Global.PassworSetting = tmp;
                }
                MessageBox.Show("Lưu mật khẩu mới thành công!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lưu mật khẩu mới lỗi!\r\n{ex.Message}", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}