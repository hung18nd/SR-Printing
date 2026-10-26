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
    }
}
