using System;
using System.Drawing;
using System.Windows.Forms;
using DXApplication1.Common;

namespace DXApplication1.BlockId
{
    public class FrmBlockIdConfig : DevExpress.XtraEditors.XtraForm
    {
        private System.ComponentModel.IContainer components = null;
        private CheckBox chkEnable;
        private TextBox txtApiUrl;
        private TextBox txtApiKey;
        private TextBox txtMachineCode;
        private TextBox txtSourceUrn;
        private ComboBox cbbSendMode;
        private TextBox txtCodePrintingTime;
        private TextBox txtCodeSqueezePressure;
        private TextBox txtCodeSqueezeSpeed;
        private TextBox txtCodeGap;
        private TextBox txtCodeAirPressure;
        private TextBox txtTestLog;
        private Button btnTestConnection;
        private Button btnSave;
        private Button btnCancel;
        private CheckBox chkShowKey;

        public FrmBlockIdConfig()
        {
            InitializeComponent();
            LoadConfigToUI();
        }

        private void InitializeComponent()
        {
            this.chkEnable = new CheckBox();
            this.txtApiUrl = new TextBox();
            this.txtApiKey = new TextBox();
            this.txtMachineCode = new TextBox();
            this.txtSourceUrn = new TextBox();
            this.cbbSendMode = new ComboBox();
            this.txtCodePrintingTime = new TextBox();
            this.txtCodeSqueezePressure = new TextBox();
            this.txtCodeSqueezeSpeed = new TextBox();
            this.txtCodeGap = new TextBox();
            this.txtCodeAirPressure = new TextBox();
            this.txtTestLog = new TextBox();
            this.btnTestConnection = new Button();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.chkShowKey = new CheckBox();

            // Form
            this.Text = "BlockId API Ingestion Protocol v1.0 Configuration";
            this.Size = new Size(720, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int leftLabel = 20;
            int leftBox = 160;
            int boxWidth = 500;

            // Group 1: General
            var grpGeneral = new GroupBox
            {
                Text = "1. BlockId Connection Settings",
                Location = new Point(15, 10),
                Size = new Size(675, 230)
            };

            chkEnable.Text = "Enable BlockId Data Ingestion";
            chkEnable.Location = new Point(leftBox, 22);
            chkEnable.AutoSize = true;
            chkEnable.Font = new Font(this.Font, FontStyle.Bold);

            AddRow(grpGeneral, "API Endpoint (URL):", txtApiUrl, 50, leftLabel, leftBox, boxWidth);
            AddRow(grpGeneral, "API Key (Secret):", txtApiKey, 82, leftLabel, leftBox, boxWidth - 90);
            txtApiKey.UseSystemPasswordChar = true;

            chkShowKey.Text = "Show Key";
            chkShowKey.Location = new Point(leftBox + boxWidth - 80, 82);
            chkShowKey.AutoSize = true;
            chkShowKey.CheckedChanged += (s, e) => { txtApiKey.UseSystemPasswordChar = !chkShowKey.Checked; };
            grpGeneral.Controls.Add(chkShowKey);

            AddRow(grpGeneral, "Machine ID (Subject):", txtMachineCode, 114, leftLabel, leftBox, boxWidth);
            AddRow(grpGeneral, "Client Source (URN):", txtSourceUrn, 146, leftLabel, leftBox, boxWidth);

            var lblMode = new Label { Text = "Send Mode:", Location = new Point(leftLabel, 180), AutoSize = true };
            cbbSendMode.Location = new Point(leftBox, 177);
            cbbSendMode.Width = 340;
            cbbSendMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbSendMode.Items.Add("Mode 1: Single Barcode (Recommended for SR Printing)");
            cbbSendMode.Items.Add("Mode 2: By Tray / Trolley");
            grpGeneral.Controls.Add(lblMode);
            grpGeneral.Controls.Add(cbbSendMode);
            grpGeneral.Controls.Add(chkEnable);

            // Group 2: Param Codes
            var grpCodes = new GroupBox
            {
                Text = "2. BlockId Parameter Code Mapping",
                Location = new Point(15, 250),
                Size = new Size(675, 130)
            };

            AddRow(grpCodes, "Printing Time (Code):", txtCodePrintingTime, 25, 20, 160, 160);
            AddRow(grpCodes, "Squeeze Press (Code):", txtCodeSqueezePressure, 25, 350, 480, 170);

            AddRow(grpCodes, "Squeeze Speed (Code):", txtCodeSqueezeSpeed, 60, 20, 160, 160);
            AddRow(grpCodes, "Snap-off Gap (Code):", txtCodeGap, 60, 350, 480, 170);

            AddRow(grpCodes, "Air Pressure (Code):", txtCodeAirPressure, 95, 20, 160, 160);

            // Group 3: Test & Log
            var grpTest = new GroupBox
            {
                Text = "3. Connection Test & API Response",
                Location = new Point(15, 390),
                Size = new Size(675, 135)
            };

            btnTestConnection.Text = "Test Connection";
            btnTestConnection.Location = new Point(20, 25);
            btnTestConnection.Size = new Size(200, 30);
            btnTestConnection.Click += BtnTestConnection_Click;
            grpTest.Controls.Add(btnTestConnection);

            txtTestLog.Location = new Point(20, 60);
            txtTestLog.Size = new Size(635, 65);
            txtTestLog.Multiline = true;
            txtTestLog.ScrollBars = ScrollBars.Vertical;
            txtTestLog.ReadOnly = true;
            grpTest.Controls.Add(txtTestLog);

            // Buttons
            btnSave.Text = "Save Config";
            btnSave.Location = new Point(480, 535);
            btnSave.Size = new Size(100, 32);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Close";
            btnCancel.Location = new Point(590, 535);
            btnCancel.Size = new Size(100, 32);
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(grpGeneral);
            this.Controls.Add(grpCodes);
            this.Controls.Add(grpTest);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void AddRow(Control parent, string labelText, TextBox box, int top, int labelLeft, int boxLeft, int boxWidth)
        {
            var lbl = new Label { Text = labelText, Location = new Point(labelLeft, top + 3), AutoSize = true };
            box.Location = new Point(boxLeft, top);
            box.Width = boxWidth;
            parent.Controls.Add(lbl);
            parent.Controls.Add(box);
        }

        private void LoadConfigToUI()
        {
            var cfg = BlockIdConfig.Instance;
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

            cfg.Save();
            MessageBox.Show("BlockId configuration saved successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = false;
            txtTestLog.Text = "Sending test payload to BlockId API...";

            string url = txtApiUrl.Text.Trim();
            string apiKey = txtApiKey.Text.Trim();
            string mc = txtMachineCode.Text.Trim();

            System.Threading.Tasks.Task.Run(() =>
            {
                var res = BlockIdQueueManager.TestConnection(url, apiKey, mc);
                this.Invoke(new Action(() =>
                {
                    btnTestConnection.Enabled = true;
                    if (res.IsSuccess)
                    {
                        txtTestLog.Text = $"[SUCCESS] HTTP Status: {res.StatusCode} ({(res.IsDuplicate ? "Duplicate / Already Accepted" : "Accepted")})\r\nResponse: {res.ResponseBody}";
                    }
                    else
                    {
                        txtTestLog.Text = $"[FAILED] HTTP Status: {res.StatusCode}\r\nDetails: {res.ErrorMessage}\r\nResponse: {res.ResponseBody}";
                    }
                }));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
