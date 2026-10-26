
namespace DXApplication1
{
    partial class FrmConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfig));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.dgrConn = new System.Windows.Forms.DataGridView();
            this.IND = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MachineType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MachineID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MachineName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MachineIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MachinePort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Factory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnPassword = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrConn)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnReload,
            this.toolStripSeparator2,
            this.btnEdit,
            this.toolStripSeparator3,
            this.btnPassword,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1184, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // dgrConn
            // 
            this.dgrConn.AllowUserToAddRows = false;
            this.dgrConn.AllowUserToDeleteRows = false;
            this.dgrConn.AllowUserToResizeColumns = false;
            this.dgrConn.AllowUserToResizeRows = false;
            this.dgrConn.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LemonChiffon;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrConn.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrConn.ColumnHeadersHeight = 26;
            this.dgrConn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrConn.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IND,
            this.Enable,
            this.MachineType,
            this.MachineID,
            this.MachineName,
            this.MachineIP,
            this.MachinePort,
            this.Factory,
            this.Note});
            this.dgrConn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrConn.EnableHeadersVisualStyles = false;
            this.dgrConn.GridColor = System.Drawing.Color.Gainsboro;
            this.dgrConn.Location = new System.Drawing.Point(0, 25);
            this.dgrConn.MultiSelect = false;
            this.dgrConn.Name = "dgrConn";
            this.dgrConn.ReadOnly = true;
            this.dgrConn.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Tahoma", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrConn.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgrConn.RowHeadersWidth = 4;
            this.dgrConn.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgrConn.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgrConn.Size = new System.Drawing.Size(1184, 686);
            this.dgrConn.TabIndex = 95;
            // 
            // IND
            // 
            this.IND.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.IND.DefaultCellStyle = dataGridViewCellStyle2;
            this.IND.HeaderText = "#";
            this.IND.Name = "IND";
            this.IND.ReadOnly = true;
            this.IND.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IND.Width = 21;
            // 
            // Enable
            // 
            this.Enable.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Enable.HeaderText = "ENABLE";
            this.Enable.Name = "Enable";
            this.Enable.ReadOnly = true;
            this.Enable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Enable.Width = 50;
            // 
            // MachineType
            // 
            this.MachineType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MachineType.HeaderText = "TYPE";
            this.MachineType.Name = "MachineType";
            this.MachineType.ReadOnly = true;
            this.MachineType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MachineType.Width = 150;
            // 
            // MachineID
            // 
            this.MachineID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.MachineID.DefaultCellStyle = dataGridViewCellStyle3;
            this.MachineID.HeaderText = "MACHINE ID";
            this.MachineID.Name = "MachineID";
            this.MachineID.ReadOnly = true;
            this.MachineID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MachineID.Width = 150;
            // 
            // MachineName
            // 
            this.MachineName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.MachineName.DefaultCellStyle = dataGridViewCellStyle4;
            this.MachineName.HeaderText = "MACHINE NAME";
            this.MachineName.Name = "MachineName";
            this.MachineName.ReadOnly = true;
            this.MachineName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MachineName.Width = 150;
            // 
            // MachineIP
            // 
            this.MachineIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.MachineIP.DefaultCellStyle = dataGridViewCellStyle5;
            this.MachineIP.HeaderText = "IP";
            this.MachineIP.Name = "MachineIP";
            this.MachineIP.ReadOnly = true;
            this.MachineIP.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MachineIP.Width = 150;
            // 
            // MachinePort
            // 
            this.MachinePort.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.MachinePort.DefaultCellStyle = dataGridViewCellStyle6;
            this.MachinePort.HeaderText = "PORT";
            this.MachinePort.Name = "MachinePort";
            this.MachinePort.ReadOnly = true;
            this.MachinePort.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Factory
            // 
            this.Factory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Factory.DefaultCellStyle = dataGridViewCellStyle7;
            this.Factory.HeaderText = "FACTORY";
            this.Factory.Name = "Factory";
            this.Factory.ReadOnly = true;
            this.Factory.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Factory.Width = 80;
            // 
            // Note
            // 
            this.Note.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Note.HeaderText = "NOTE";
            this.Note.Name = "Note";
            this.Note.ReadOnly = true;
            this.Note.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnReload
            // 
            this.btnReload.Image = global::DXApplication1.Properties.Resources.refresh_16px;
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(63, 22);
            this.btnReload.Text = "Reload";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Image = global::DXApplication1.Properties.Resources.edit_16px;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(47, 22);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnPassword
            // 
            this.btnPassword.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPassword.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPassword.Image = global::DXApplication1.Properties.Resources.password_1_16px;
            this.btnPassword.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPassword.Name = "btnPassword";
            this.btnPassword.Size = new System.Drawing.Size(81, 22);
            this.btnPassword.Text = "Password";
            this.btnPassword.Click += new System.EventHandler(this.btnPassword_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::DXApplication1.Properties.Resources.save_close_16px;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 711);
            this.Controls.Add(this.dgrConn);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PMTT Machine Data Loging Software - Config";
            this.Shown += new System.EventHandler(this.FrmConfig_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrConn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnReload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnPassword;
        internal System.Windows.Forms.DataGridView dgrConn;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn IND;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enable;
        private System.Windows.Forms.DataGridViewTextBoxColumn MachineType;
        private System.Windows.Forms.DataGridViewTextBoxColumn MachineID;
        private System.Windows.Forms.DataGridViewTextBoxColumn MachineName;
        private System.Windows.Forms.DataGridViewTextBoxColumn MachineIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn MachinePort;
        private System.Windows.Forms.DataGridViewTextBoxColumn Factory;
        private System.Windows.Forms.DataGridViewTextBoxColumn Note;
    }
}