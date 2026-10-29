namespace DXApplication1.BlockId
{
    partial class FrmBlockIdConfig
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
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.chkEnablePlcKV = new System.Windows.Forms.CheckBox();
            this.chkEnable = new System.Windows.Forms.CheckBox();
            this.lblApiUrl = new System.Windows.Forms.Label();
            this.txtApiUrl = new System.Windows.Forms.TextBox();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.chkShowKey = new System.Windows.Forms.CheckBox();
            this.lblMachineCode = new System.Windows.Forms.Label();
            this.txtMachineCode = new System.Windows.Forms.TextBox();
            this.lblSourceUrn = new System.Windows.Forms.Label();
            this.txtSourceUrn = new System.Windows.Forms.TextBox();
            this.lblMode = new System.Windows.Forms.Label();
            this.cbbSendMode = new System.Windows.Forms.ComboBox();
            this.grpCodes = new System.Windows.Forms.GroupBox();
            this.lblCodePrintingTime = new System.Windows.Forms.Label();
            this.txtCodePrintingTime = new System.Windows.Forms.TextBox();
            this.lblCodeSqueezePressure = new System.Windows.Forms.Label();
            this.txtCodeSqueezePressure = new System.Windows.Forms.TextBox();
            this.lblCodeSqueezeSpeed = new System.Windows.Forms.Label();
            this.txtCodeSqueezeSpeed = new System.Windows.Forms.TextBox();
            this.lblCodeGap = new System.Windows.Forms.Label();
            this.txtCodeGap = new System.Windows.Forms.TextBox();
            this.lblCodeAirPressure = new System.Windows.Forms.Label();
            this.txtCodeAirPressure = new System.Windows.Forms.TextBox();
            this.grpTest = new System.Windows.Forms.GroupBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtTestLog = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpGeneral.SuspendLayout();
            this.grpCodes.SuspendLayout();
            this.grpTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGeneral
            // 
            this.grpGeneral.Controls.Add(this.chkEnablePlcKV);
            this.grpGeneral.Controls.Add(this.chkEnable);
            this.grpGeneral.Controls.Add(this.lblApiUrl);
            this.grpGeneral.Controls.Add(this.txtApiUrl);
            this.grpGeneral.Controls.Add(this.lblApiKey);
            this.grpGeneral.Controls.Add(this.txtApiKey);
            this.grpGeneral.Controls.Add(this.chkShowKey);
            this.grpGeneral.Controls.Add(this.lblMachineCode);
            this.grpGeneral.Controls.Add(this.txtMachineCode);
            this.grpGeneral.Controls.Add(this.lblSourceUrn);
            this.grpGeneral.Controls.Add(this.txtSourceUrn);
            this.grpGeneral.Controls.Add(this.lblMode);
            this.grpGeneral.Controls.Add(this.cbbSendMode);
            this.grpGeneral.Location = new System.Drawing.Point(15, 10);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(675, 230);
            this.grpGeneral.TabIndex = 0;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "1. BlockId Connection Settings";
            // 
            // chkEnablePlcKV
            // 
            this.chkEnablePlcKV.AutoSize = true;
            this.chkEnablePlcKV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnablePlcKV.Location = new System.Drawing.Point(160, 22);
            this.chkEnablePlcKV.Name = "chkEnablePlcKV";
            this.chkEnablePlcKV.Size = new System.Drawing.Size(107, 17);
            this.chkEnablePlcKV.TabIndex = 0;
            this.chkEnablePlcKV.Text = "Enable PLC KV";
            this.chkEnablePlcKV.UseVisualStyleBackColor = true;
            this.chkEnablePlcKV.CheckedChanged += new System.EventHandler(this.chkEnablePlcKV_CheckedChanged);
            // 
            // chkEnable
            // 
            this.chkEnable.AutoSize = true;
            this.chkEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnable.Location = new System.Drawing.Point(310, 22);
            this.chkEnable.Name = "chkEnable";
            this.chkEnable.Size = new System.Drawing.Size(199, 17);
            this.chkEnable.TabIndex = 1;
            this.chkEnable.Text = "Enable BlockId Data Ingestion";
            this.chkEnable.UseVisualStyleBackColor = true;
            this.chkEnable.CheckedChanged += new System.EventHandler(this.chkEnable_CheckedChanged);
            // 
            // lblApiUrl
            // 
            this.lblApiUrl.AutoSize = true;
            this.lblApiUrl.Location = new System.Drawing.Point(20, 53);
            this.lblApiUrl.Name = "lblApiUrl";
            this.lblApiUrl.Size = new System.Drawing.Size(72, 13);
            this.lblApiUrl.TabIndex = 2;
            this.lblApiUrl.Text = "API Endpoint:";
            // 
            // txtApiUrl
            // 
            this.txtApiUrl.Location = new System.Drawing.Point(160, 50);
            this.txtApiUrl.Name = "txtApiUrl";
            this.txtApiUrl.Size = new System.Drawing.Size(500, 20);
            this.txtApiUrl.TabIndex = 3;
            // 
            // lblApiKey
            // 
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(20, 85);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(48, 13);
            this.lblApiKey.TabIndex = 4;
            this.lblApiKey.Text = "API Key:";
            // 
            // txtApiKey
            // 
            this.txtApiKey.Location = new System.Drawing.Point(160, 82);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.Size = new System.Drawing.Size(410, 20);
            this.txtApiKey.TabIndex = 5;
            this.txtApiKey.UseSystemPasswordChar = true;
            // 
            // chkShowKey
            // 
            this.chkShowKey.AutoSize = true;
            this.chkShowKey.Location = new System.Drawing.Point(580, 84);
            this.chkShowKey.Name = "chkShowKey";
            this.chkShowKey.Size = new System.Drawing.Size(74, 17);
            this.chkShowKey.TabIndex = 6;
            this.chkShowKey.Text = "Show Key";
            this.chkShowKey.UseVisualStyleBackColor = true;
            this.chkShowKey.CheckedChanged += new System.EventHandler(this.chkShowKey_CheckedChanged);
            // 
            // lblMachineCode
            // 
            this.lblMachineCode.AutoSize = true;
            this.lblMachineCode.Location = new System.Drawing.Point(20, 117);
            this.lblMachineCode.Name = "lblMachineCode";
            this.lblMachineCode.Size = new System.Drawing.Size(65, 13);
            this.lblMachineCode.TabIndex = 7;
            this.lblMachineCode.Text = "Machine ID:";
            // 
            // txtMachineCode
            // 
            this.txtMachineCode.Location = new System.Drawing.Point(160, 114);
            this.txtMachineCode.Name = "txtMachineCode";
            this.txtMachineCode.Size = new System.Drawing.Size(500, 20);
            this.txtMachineCode.TabIndex = 8;
            // 
            // lblSourceUrn
            // 
            this.lblSourceUrn.AutoSize = true;
            this.lblSourceUrn.Location = new System.Drawing.Point(20, 149);
            this.lblSourceUrn.Name = "lblSourceUrn";
            this.lblSourceUrn.Size = new System.Drawing.Size(73, 13);
            this.lblSourceUrn.TabIndex = 9;
            this.lblSourceUrn.Text = "Client Source:";
            // 
            // txtSourceUrn
            // 
            this.txtSourceUrn.Location = new System.Drawing.Point(160, 146);
            this.txtSourceUrn.Name = "txtSourceUrn";
            this.txtSourceUrn.Size = new System.Drawing.Size(500, 20);
            this.txtSourceUrn.TabIndex = 10;
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(20, 180);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(65, 13);
            this.lblMode.TabIndex = 11;
            this.lblMode.Text = "Send Mode:";
            // 
            // cbbSendMode
            // 
            this.cbbSendMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbSendMode.FormattingEnabled = true;
            this.cbbSendMode.Items.AddRange(new object[] {
            "Single Barcode",
            "Tray / Lot Batch"});
            this.cbbSendMode.Location = new System.Drawing.Point(160, 177);
            this.cbbSendMode.Name = "cbbSendMode";
            this.cbbSendMode.Size = new System.Drawing.Size(340, 21);
            this.cbbSendMode.TabIndex = 12;
            // 
            // grpCodes
            // 
            this.grpCodes.Controls.Add(this.lblCodePrintingTime);
            this.grpCodes.Controls.Add(this.txtCodePrintingTime);
            this.grpCodes.Controls.Add(this.lblCodeSqueezePressure);
            this.grpCodes.Controls.Add(this.txtCodeSqueezePressure);
            this.grpCodes.Controls.Add(this.lblCodeSqueezeSpeed);
            this.grpCodes.Controls.Add(this.txtCodeSqueezeSpeed);
            this.grpCodes.Controls.Add(this.lblCodeGap);
            this.grpCodes.Controls.Add(this.txtCodeGap);
            this.grpCodes.Controls.Add(this.lblCodeAirPressure);
            this.grpCodes.Controls.Add(this.txtCodeAirPressure);
            this.grpCodes.Location = new System.Drawing.Point(15, 250);
            this.grpCodes.Name = "grpCodes";
            this.grpCodes.Size = new System.Drawing.Size(675, 130);
            this.grpCodes.TabIndex = 1;
            this.grpCodes.TabStop = false;
            this.grpCodes.Text = "2. BlockId Parameter Code Mapping";
            // 
            // lblCodePrintingTime
            // 
            this.lblCodePrintingTime.AutoSize = true;
            this.lblCodePrintingTime.Location = new System.Drawing.Point(20, 28);
            this.lblCodePrintingTime.Name = "lblCodePrintingTime";
            this.lblCodePrintingTime.Size = new System.Drawing.Size(71, 13);
            this.lblCodePrintingTime.TabIndex = 0;
            this.lblCodePrintingTime.Text = "Printing Time:";
            // 
            // txtCodePrintingTime
            // 
            this.txtCodePrintingTime.Location = new System.Drawing.Point(160, 25);
            this.txtCodePrintingTime.Name = "txtCodePrintingTime";
            this.txtCodePrintingTime.Size = new System.Drawing.Size(160, 20);
            this.txtCodePrintingTime.TabIndex = 1;
            // 
            // lblCodeSqueezePressure
            // 
            this.lblCodeSqueezePressure.AutoSize = true;
            this.lblCodeSqueezePressure.Location = new System.Drawing.Point(350, 28);
            this.lblCodeSqueezePressure.Name = "lblCodeSqueezePressure";
            this.lblCodeSqueezePressure.Size = new System.Drawing.Size(81, 13);
            this.lblCodeSqueezePressure.TabIndex = 2;
            this.lblCodeSqueezePressure.Text = "Squeeze Press:";
            // 
            // txtCodeSqueezePressure
            // 
            this.txtCodeSqueezePressure.Location = new System.Drawing.Point(480, 25);
            this.txtCodeSqueezePressure.Name = "txtCodeSqueezePressure";
            this.txtCodeSqueezePressure.Size = new System.Drawing.Size(170, 20);
            this.txtCodeSqueezePressure.TabIndex = 3;
            // 
            // lblCodeSqueezeSpeed
            // 
            this.lblCodeSqueezeSpeed.AutoSize = true;
            this.lblCodeSqueezeSpeed.Location = new System.Drawing.Point(20, 63);
            this.lblCodeSqueezeSpeed.Name = "lblCodeSqueezeSpeed";
            this.lblCodeSqueezeSpeed.Size = new System.Drawing.Size(86, 13);
            this.lblCodeSqueezeSpeed.TabIndex = 4;
            this.lblCodeSqueezeSpeed.Text = "Squeeze Speed:";
            // 
            // txtCodeSqueezeSpeed
            // 
            this.txtCodeSqueezeSpeed.Location = new System.Drawing.Point(160, 60);
            this.txtCodeSqueezeSpeed.Name = "txtCodeSqueezeSpeed";
            this.txtCodeSqueezeSpeed.Size = new System.Drawing.Size(160, 20);
            this.txtCodeSqueezeSpeed.TabIndex = 5;
            // 
            // lblCodeGap
            // 
            this.lblCodeGap.AutoSize = true;
            this.lblCodeGap.Location = new System.Drawing.Point(350, 63);
            this.lblCodeGap.Name = "lblCodeGap";
            this.lblCodeGap.Size = new System.Drawing.Size(73, 13);
            this.lblCodeGap.TabIndex = 6;
            this.lblCodeGap.Text = "Snap-off Gap:";
            // 
            // txtCodeGap
            // 
            this.txtCodeGap.Location = new System.Drawing.Point(480, 60);
            this.txtCodeGap.Name = "txtCodeGap";
            this.txtCodeGap.Size = new System.Drawing.Size(170, 20);
            this.txtCodeGap.TabIndex = 7;
            // 
            // lblCodeAirPressure
            // 
            this.lblCodeAirPressure.AutoSize = true;
            this.lblCodeAirPressure.Location = new System.Drawing.Point(20, 98);
            this.lblCodeAirPressure.Name = "lblCodeAirPressure";
            this.lblCodeAirPressure.Size = new System.Drawing.Size(66, 13);
            this.lblCodeAirPressure.TabIndex = 8;
            this.lblCodeAirPressure.Text = "Air Pressure:";
            // 
            // txtCodeAirPressure
            // 
            this.txtCodeAirPressure.Location = new System.Drawing.Point(160, 95);
            this.txtCodeAirPressure.Name = "txtCodeAirPressure";
            this.txtCodeAirPressure.Size = new System.Drawing.Size(160, 20);
            this.txtCodeAirPressure.TabIndex = 9;
            // 
            // grpTest
            // 
            this.grpTest.Controls.Add(this.btnTestConnection);
            this.grpTest.Controls.Add(this.txtTestLog);
            this.grpTest.Location = new System.Drawing.Point(15, 390);
            this.grpTest.Name = "grpTest";
            this.grpTest.Size = new System.Drawing.Size(675, 135);
            this.grpTest.TabIndex = 2;
            this.grpTest.TabStop = false;
            this.grpTest.Text = "3. Connection Test & API Response";
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(20, 25);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(200, 30);
            this.btnTestConnection.TabIndex = 0;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.BtnTestConnection_Click);
            // 
            // txtTestLog
            // 
            this.txtTestLog.Location = new System.Drawing.Point(20, 60);
            this.txtTestLog.Multiline = true;
            this.txtTestLog.Name = "txtTestLog";
            this.txtTestLog.ReadOnly = true;
            this.txtTestLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTestLog.Size = new System.Drawing.Size(635, 65);
            this.txtTestLog.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(480, 535);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save Config";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(590, 535);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmBlockIdConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 581);
            this.Controls.Add(this.grpGeneral);
            this.Controls.Add(this.grpCodes);
            this.Controls.Add(this.grpTest);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmBlockIdConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BlockId API Configuration";
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpCodes.ResumeLayout(false);
            this.grpCodes.PerformLayout();
            this.grpTest.ResumeLayout(false);
            this.grpTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpGeneral;
        private System.Windows.Forms.CheckBox chkEnablePlcKV;
        private System.Windows.Forms.CheckBox chkEnable;
        private System.Windows.Forms.Label lblApiUrl;
        private System.Windows.Forms.TextBox txtApiUrl;
        private System.Windows.Forms.Label lblApiKey;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.CheckBox chkShowKey;
        private System.Windows.Forms.Label lblMachineCode;
        private System.Windows.Forms.TextBox txtMachineCode;
        private System.Windows.Forms.Label lblSourceUrn;
        private System.Windows.Forms.TextBox txtSourceUrn;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cbbSendMode;
        private System.Windows.Forms.GroupBox grpCodes;
        private System.Windows.Forms.Label lblCodePrintingTime;
        private System.Windows.Forms.TextBox txtCodePrintingTime;
        private System.Windows.Forms.Label lblCodeSqueezePressure;
        private System.Windows.Forms.TextBox txtCodeSqueezePressure;
        private System.Windows.Forms.Label lblCodeSqueezeSpeed;
        private System.Windows.Forms.TextBox txtCodeSqueezeSpeed;
        private System.Windows.Forms.Label lblCodeGap;
        private System.Windows.Forms.TextBox txtCodeGap;
        private System.Windows.Forms.Label lblCodeAirPressure;
        private System.Windows.Forms.TextBox txtCodeAirPressure;
        private System.Windows.Forms.GroupBox grpTest;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.TextBox txtTestLog;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
