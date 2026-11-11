using System.Diagnostics;
using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class FrmConfig2 : Form
    {
        public FrmConfig2()
        {
            InitializeComponent();
        }

        private void FrmConfig2_Shown(object sender, EventArgs e)
        {
            ReloadConfig();
        }

        private void ReloadConfig()
        {
            //test
            txtMachineID.Text = VarGlobal.McID;
            txtPLCIp.Text = VarGlobal.McSMPrinting.PlcIp;
            txtPLCPort.Text = VarGlobal.McSMPrinting.PlcPort.ToString();
            txtSRXIp.Text = VarGlobal.McSMPrinting.BcrSrx300Ip;
            txtSRXPort.Text = VarGlobal.McSMPrinting.BcrSrx300Port.ToString();
            txtCognexIp.Text = VarGlobal.McSMPrinting.BcrCognexIp.ToString();
            txtCognexPort.Text = "";
            txtKVIp.Text = "10.126.1.184";
            txtKVPort.Text = "8501";
            txtDBIp.Text = SQLFunc.DBHostName;
            txtNumBlock.Value = VarGlobal.McSMPrinting.BlockNumPerPrints;
            if(VarGlobal.McSMPrinting.BcrMode == 0)
            {
                cbbBcrMode.SelectedIndex = 0;
            }
            else if (VarGlobal.McSMPrinting.BcrMode == 1)
            {
                cbbBcrMode.SelectedIndex = 1;
            }
            if (VarGlobal.McSMPrinting.BcrSelect == 0)
            {
                cbbBCRSelect.SelectedIndex = 0;
            }
            else if (VarGlobal.McSMPrinting.BcrSelect == 1)
            {
                cbbBCRSelect.SelectedIndex = 1;
            }

            txtNumBlock.ReadOnly = true;
            cbbBcrMode.Enabled = false;
            cbbBCRSelect.Enabled = false;
            btnSave.Enabled = false;

            txtPLCIp.ReadOnly = true;
            txtPLCPort.ReadOnly = true;
            txtSRXIp.ReadOnly = true;
            txtSRXPort.ReadOnly = true;
            txtCognexIp.ReadOnly = true;
            txtCognexPort.ReadOnly = true;

            // Load Update Config
            AutoUpdateManager.LoadConfig();
            chkAutoCheckUpdate.Checked = AutoUpdateManager.AutoCheckUpdate;
            txtServerUpdatePath.Text = AutoUpdateManager.ServerUpdatePath;
            txtServerUser.Text = AutoUpdateManager.ServerUser;
            txtServerPass.Text = AutoUpdateManager.ServerPass;
            chkAutoCheckUpdate.Enabled = false;
            txtServerUpdatePath.ReadOnly = true;
            txtServerUser.ReadOnly = true;
            txtServerPass.ReadOnly = true;
            btnBrowseUpdatePath.Enabled = false;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            ReloadConfig();
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            string pass = Global.PasswordInput();
            if(pass != "")
            {
                if(pass == Global.PassworSetting)
                {
                    txtNumBlock.ReadOnly = false;
                    cbbBcrMode.Enabled = true;
                    cbbBCRSelect.Enabled = true;
                    btnSave.Enabled = true;

                    chkAutoCheckUpdate.Enabled = true;
                    txtServerUpdatePath.ReadOnly = false;
                    txtServerUser.ReadOnly = false;
                    txtServerPass.ReadOnly = false;
                    btnBrowseUpdatePath.Enabled = true;
                }
                else if (pass == "seev@123")
                {
                    txtNumBlock.ReadOnly = false;
                    cbbBcrMode.Enabled = true;
                    cbbBCRSelect.Enabled = true;
                    btnSave.Enabled = true;

                    txtPLCIp.ReadOnly = false;
                    txtPLCPort.ReadOnly = false;
                    txtSRXIp.ReadOnly = false;
                    txtSRXPort.ReadOnly = false;
                    txtCognexIp.ReadOnly = false;
                    txtCognexPort.ReadOnly = false;

                    chkAutoCheckUpdate.Enabled = true;
                    txtServerUpdatePath.ReadOnly = false;
                    txtServerUser.ReadOnly = false;
                    txtServerPass.ReadOnly = false;
                    btnBrowseUpdatePath.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Mật khẩu không chính xác!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Xác nhận lưu cài đặt?","YÊU CẦU XÁC NHẬN",MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
            if(dialog == DialogResult.OK)
            {
                try
                {
                    string iniFile = Global.ApplicationPath() + @"\AppConfig" + $@"\{VarGlobal.McSMPrinting.MachineID}.ini";
                    IniFile iniFile1 = new IniFile(iniFile);

                    iniFile1.IniWriteValue("PARAM", "IpPLCMachine", txtPLCIp.Text);
                    iniFile1.IniWriteValue("PARAM", "PortPLCMachine", txtPLCPort.Text);
                    iniFile1.IniWriteValue("PARAM", "IpPLCBlock", txtKVIp.Text);
                    iniFile1.IniWriteValue("PARAM", "PortPLCBlock", txtKVPort.Text);
                    iniFile1.IniWriteValue("PARAM", "BcrSrx300Ip", txtSRXIp.Text);
                    iniFile1.IniWriteValue("PARAM", "BcrSrx300Port", txtSRXPort.Text);
                    iniFile1.IniWriteValue("PARAM", "BcrCognexIp", txtCognexIp.Text);
                    iniFile1.IniWriteValue("PARAM", "BcrCognexPort", txtCognexPort.Text);
                    iniFile1.IniWriteValue("PARAM", "NumBlockPerPrint", txtNumBlock.Value.ToString());
                    iniFile1.IniWriteValue("PARAM", "BcrMode", cbbBcrMode.SelectedIndex.ToString());
                    iniFile1.IniWriteValue("PARAM", "BcrSelect", cbbBCRSelect.SelectedIndex.ToString());

                    // Lưu Update Config
                    AutoUpdateManager.SaveConfig(chkAutoCheckUpdate.Checked, txtServerUpdatePath.Text.Trim(), txtServerUser.Text.Trim(), txtServerPass.Text.Trim());

                    MessageBox.Show("Lưu cài đặt thành công!","MESSAGE",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    VarGlobal.McSMPrinting.Read_Config();
                    ReloadConfig();
                }
                catch (Exception)
                {

                }
                
            }
            
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            FrmChangePass frmChangePass = new FrmChangePass();
            frmChangePass.ShowDialog();
        }

        private void btnBrowseUpdatePath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Chọn thư mục chứa bản cập nhật phần mềm trên Server:";
                fbd.ShowNewFolderButton = false;
                if (!string.IsNullOrWhiteSpace(txtServerUpdatePath.Text) && Directory.Exists(txtServerUpdatePath.Text.Trim()))
                {
                    fbd.SelectedPath = txtServerUpdatePath.Text.Trim();
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtServerUpdatePath.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnTestUpdatePath_Click(object sender, EventArgs e)
        {
            string path = txtServerUpdatePath.Text.Trim();
            string user = txtServerUser.Text.Trim();
            string pass = txtServerPass.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập đường dẫn file cập nhật trên server!", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AutoUpdateManager.TestConnection(path, user, pass, out string msg))
            {
                MessageBox.Show(msg, "KIỂM TRA KẾT NỐI SERVER THÀNH CÔNG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(msg, "LỖI KẾT NỐI SERVER", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnOpenServerFolder_Click(object sender, EventArgs e)
        {
            string path = txtServerUpdatePath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Vui lòng nhập hoặc chọn đường dẫn thư mục Server trước!", "THÔNG BÁO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string host = AutoUpdateManager.GetHostFromPath(path);
                string user = txtServerUser.Text.Trim();
                string pass = txtServerPass.Text.Trim();

                // 1. Luu thong tin vao Windows Credential Manager
                if (!string.IsNullOrEmpty(host))
                {
                    AutoUpdateManager.SaveCredentialsToWindowsVault(host, user, pass);
                }

                // 2. Mo Explorer de Windows thuc hien dang nhap va ghi nho (Remember)
                Process.Start("explorer.exe", path);

                MessageBox.Show("Đã mở thư mục Server trong Windows Explorer.\r\n\r\nNếu Windows hiện hộp thoại đăng nhập mạng, bạn hãy nhập User/Pass và tích vào ô [Remember my credentials / Ghi nhớ thông tin] để Windows lưu vĩnh viễn!", "HƯỚNG DẪN GHI NHỚ TÀI KHOẢN", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở thư mục Server: {ex.Message}", "LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
