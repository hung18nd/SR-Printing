using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1.BlockId
{
    public partial class FrmBlockIdConfig : Form
    {
        public FrmBlockIdConfig()
        {
            InitializeComponent();
            LoadConfigToUI();
        }

        private void chkEnablePlcKV_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnablePlcKV.Checked && chkEnable.Checked)
            {
                chkEnable.Checked = false;
            }
        }

        private void chkEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnable.Checked && chkEnablePlcKV.Checked)
            {
                chkEnablePlcKV.Checked = false;
            }
        }

        private void chkShowKey_CheckedChanged(object sender, EventArgs e)
        {
            txtApiKey.UseSystemPasswordChar = !chkShowKey.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadConfigToUI()
        {
            var cfg = BlockIdConfig.Instance;
            chkEnablePlcKV.Checked = VarGlobal.McSMPrinting.EnablePlcKV;
            chkEnable.Checked = cfg.Enable;
            txtApiUrl.Text = cfg.ApiUrl;
            txtApiKey.Text = cfg.ApiKey;
            txtMachineCode.Text = !string.IsNullOrEmpty(cfg.MachineCode) ? cfg.MachineCode : VarGlobal.McID;
            txtSourceUrn.Text = cfg.Source;
            cbbSendMode.SelectedIndex = (cfg.SendMode == 2) ? 1 : 0;

            txtCodePrintingTime.Text = cfg.CodePrintingTime;
            txtCodeSqueezePressure.Text = cfg.CodeSqueezePressure;
            txtCodeSqueezeSpeed.Text = cfg.CodeSqueezeSpeed;
            txtCodeGap.Text = cfg.CodeGap;
            txtCodeAirPressure.Text = cfg.CodeMachineAirPressure;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var cfg = BlockIdConfig.Instance;
            cfg.EnablePlcKV = chkEnablePlcKV.Checked;
            cfg.Enable = chkEnable.Checked;
            cfg.ApiUrl = txtApiUrl.Text.Trim();
            cfg.ApiKey = txtApiKey.Text.Trim();
            cfg.MachineCode = txtMachineCode.Text.Trim();
            cfg.Source = txtSourceUrn.Text.Trim();
            cfg.SendMode = (cbbSendMode.SelectedIndex == 1) ? 2 : 1;

            cfg.CodePrintingTime = txtCodePrintingTime.Text.Trim();
            cfg.CodeSqueezePressure = txtCodeSqueezePressure.Text.Trim();
            cfg.CodeSqueezeSpeed = txtCodeSqueezeSpeed.Text.Trim();
            cfg.CodeGap = txtCodeGap.Text.Trim();
            cfg.CodeMachineAirPressure = txtCodeAirPressure.Text.Trim();

            // Đồng bộ trạng thái PLC KV
            VarGlobal.McSMPrinting.EnablePlcKV = chkEnablePlcKV.Checked;
            try
            {
                VarGlobal.McSMPrinting.Write_Config();
            }
            catch { }

            cfg.Save();
            MessageBox.Show("Configuration saved successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = false;
            txtTestLog.Text = "Sending test payload to BlockId API...";

            string url = txtApiUrl.Text.Trim();
            string apiKey = txtApiKey.Text.Trim();
            string mc = txtMachineCode.Text.Trim();

            Task.Run(() =>
            {
                var res = BlockIdQueueManager.TestConnection(url, apiKey, mc);

                this.BeginInvoke(new Action(() =>
                {
                    btnTestConnection.Enabled = true;
                    if (res.IsSuccess)
                    {
                        txtTestLog.Text = $"[SUCCESS] HTTP Status: {res.StatusCode} - {(res.IsDuplicate ? "Duplicate Record" : "Accepted")}\r\nResponse: {res.ResponseBody}";
                    }
                    else
                    {
                        txtTestLog.Text = $"[FAILED] HTTP Status: {res.StatusCode}\r\nDetails: {res.ErrorMessage}\r\nResponse: {res.ResponseBody}";
                    }
                }));
            });
        }
    }
}
