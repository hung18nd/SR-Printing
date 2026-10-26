namespace DXApplication1
{
    partial class frmInputPassword
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.chkPWChar = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.txtInput.Location = new System.Drawing.Point(12, 12);
            this.txtInput.Name = "txtInput";
            this.txtInput.PasswordChar = '*';
            this.txtInput.Size = new System.Drawing.Size(412, 27);
            this.txtInput.TabIndex = 0;
            this.txtInput.Text = "p@ssword";
            this.txtInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // chkPWChar
            // 
            this.chkPWChar.AutoSize = true;
            this.chkPWChar.Location = new System.Drawing.Point(12, 50);
            this.chkPWChar.Name = "chkPWChar";
            this.chkPWChar.Size = new System.Drawing.Size(130, 17);
            this.chkPWChar.TabIndex = 1;
            this.chkPWChar.Text = "Show password chars";
            this.chkPWChar.UseVisualStyleBackColor = true;
            this.chkPWChar.CheckedChanged += new System.EventHandler(this.chkPWChar_CheckedChanged);
            this.chkPWChar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(363, 45);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(61, 25);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmInputPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 75);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkPWChar);
            this.Controls.Add(this.txtInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmInputPassword";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INPUT PASSWORD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmInputPassword_FormClosing);
            this.Load += new System.EventHandler(this.frmInputPassword_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkPWChar;
        public System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnOK;
    }
}