using DevExpress.XtraEditors;
using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DXApplication1
{
    public partial class FrmTraceBlockID : DevExpress.XtraEditors.XtraForm
    {
        public FrmTraceBlockID()
        {
            InitializeComponent();
        }

        private TimeSpan lastCPU_TS;
        private System.DateTime lastCPU_Time = DateTime.Now;
        private System.Windows.Forms.Timer timer1s = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timerUpdateView = new System.Windows.Forms.Timer();

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            ////string[] test = new string[] { "12599", "14392", "18999", "12333", "12336", "13622", "12333", "12592", "12589" };
            ////string[] test2 = new string[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
            ////string a = Conv.StringInt2StringChar(test);

            ////string b = Conv.StringInt2StringCharRevert(test2);
            Debug.Print("OK");

            #region TEST
            //try
            //{
            //    TcpClient client = new TcpClient();
            //    client.Connect("10.126.18.150", 5002);
            //    if (client.Connected)
            //    {
            //        string sRX = "";
            //        Stream stream = client.GetStream();
            //        byte[] data;

            //        //Heartbeet:
            //        //TX: 500000FF03FF00 0019 000014010001M*00300000010 = M3000
            //        //RX: D00000FF03FF00 0004 0000
            //        string cmd = Read_XReg_Binary3E("0", 108);
            //        Debug.Print("[TX] " + cmd); //DEBUG
            //        data = Conv.HexToBytes(cmd);
            //        stream.Write(data, 0, data.Length);
            //        int datalen = 0;
            //        Stopwatch stw = new Stopwatch();
            //        stw.Restart();
            //        while (stw.ElapsedMilliseconds < 1000)
            //        {
            //            if (client.Available > 0)
            //            {
            //                data = new byte[client.Available];
            //                stream.Read(data, 0, data.Length);
            //                sRX += Conv.BytesToStringHex(data);

            //                if (sRX.Length >= 18)
            //                {
            //                    string strlen = sRX.Substring(14, 4);
            //                    //int datalen = Conv.atoi32(sRX.Substring(14, 4), 0, 16);
            //                    datalen = Conv.SHtoi16($"{strlen[2]}{strlen[3]}{strlen[0]}{strlen[1]}", 0, 16);
            //                    if (datalen * 2 + 18 == sRX.Length)
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //            Thread.Sleep(5);
            //        }

            //        client.Close();

            //        MessageBox.Show($"TX: {cmd}\r\n{stw.ElapsedMilliseconds}\r\n{sRX} [{sRX.Length}] [{datalen}]");
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            #endregion

            this.Text = $"Machine Data Mixing Checking Software - Version: {Global.VersionSoftware}";        

            Timer1sInit();
            LoadCbbProcess();
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void LoadCbbProcess()
        {
            DataTable dtab = new DataTable();
            dtab.Columns.Add("Display");
            dtab.Columns.Add("Value");
            dtab.Rows.Add("Auto Checker", "TblAuto_EChecker");
            dtab.Rows.Add("Auto Pasting", "TblAuto_Pasting");
            dtab.Rows.Add("Copper Bright", "TbCopper_Bright");
            dtab.Rows.Add("Electroless Copper", "TblElectrolessCopper");
            dtab.Rows.Add("Electroless Gold", "TblElectrolessGold");
            dtab.Rows.Add("RTR Etching", "TblRTR_Etching");
            dtab.Rows.Add("HotPress", "TblHotPress");
            dtab.Rows.Add("Inkjet", "Tbl_Inkjet");
            dtab.Rows.Add("Layup", "TblLayup");
            dtab.Rows.Add("Micro Etching", "TblMicro_Etching");  
            dtab.Rows.Add("RTR Develop", "TblRTR_Develop");
            dtab.Rows.Add("Exposing DoubleSide", "TblExposing_DSide");
            dtab.Rows.Add("RTR Stripping", "Tbl_RTRStripping");
            dtab.Rows.Add("RTR Surface", "TblRTR_Surface");
            dtab.Rows.Add("RTR Vacuum Laminte", "TblRTRVacuumLaminte");
            dtab.Rows.Add("RTS Develop", "TblRTS_Develop");
            dtab.Rows.Add("RTR Shadow", "Tbl_Shadow");
            dtab.Rows.Add("SR Printing", "TblSR_Printing");
            dtab.Rows.Add("SR Develop", "TblSR_Develop");
            dtab.Rows.Add("SR Exposing", "TblSR_Exposing");
            dtab.Rows.Add("STS Shadow", "TblSTS_Shadow");
            dtab.Rows.Add("STS Stripping", "TblSTS_Stripping");
            dtab.Rows.Add("STS Surface", "TblSTS_Surface");
            dtab.Rows.Add("UV Laser", "TblUVLaser");
            cbbProcess.DataSource = dtab;
            cbbProcess.DisplayMember = "Display";
            cbbProcess.ValueMember = "Value";
        }

        private void btnMachineCfg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        #region TIMER
        private void Timer1sInit()
        {
            timer1s.Interval = 1000;
            timer1s.Tick += timer1s_Tick;
            timer1s.Enabled = true;
        }
        private void Timer1sRelease()
        {
            timer1s.Stop();
        }
        private void timer1s_Tick(object sender, EventArgs e)
        {
            bsiDatetime.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //Thong ke tai nguyen da su dung:
            bsiThreadCount.Caption = System.Diagnostics.Process.GetCurrentProcess().Threads.Count.ToString();

            TimeSpan currCPU_TS = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime - lastCPU_TS;
            lastCPU_TS = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
            TimeSpan currTotal_TS = DateTime.Now - lastCPU_Time;
            lastCPU_Time = DateTime.Now;

            int pcount = System.Environment.ProcessorCount;
            if (pcount != 0 && currTotal_TS.Ticks != 0)
                bsiCPUUsage.Caption = ((currCPU_TS.Ticks / pcount) * 100 / currTotal_TS.Ticks).ToString("0.0") + "%";

            string sBuff = "";
            sBuff = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("0.0");
            sBuff += "+" + (System.Diagnostics.Process.GetCurrentProcess().PagedMemorySize64 / 1048576).ToString("0.0");
            sBuff += "+" + (GC.GetTotalMemory(false) / 1048576).ToString("0.0");
            bsiRAM.Caption = sBuff + "MB";
        }

        

        #endregion

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool ret = SQLFunc.TestConnection();
            if (ret)
            {
                MessageBox.Show("Test connection OK");
            }
            else
            {
                MessageBox.Show("Test connection NG");
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if(txtMachineID.Text == "")
            //{
            //    MessageBox.Show("Chưa có MachineID!","MESSAGE",MessageBoxButtons.OK,MessageBoxIcon.Error);
            //    return;
            //}
            if (dtpFromTime.Value > dtpToTime.Value)
            {
                MessageBox.Show("Thời gian tìm kiếm không hợp lệ\r\n[FromTime] > [ToTime]!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string tbl = cbbProcess.SelectedValue.ToString();
            string fromtime = dtpFromTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string totime = dtpToTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string blockId = txtBlockID.Text;
            string sql = "";
            if (tbl != "")
            {
                if (blockId != "")
                {
                    sql = $"SELECT * FROM {tbl} WHERE BlockID = '{blockId}' ORDER BY pkid DESC";
                }
                else
                {
                    sql = $"SELECT * FROM {tbl} WHERE CreateDate > '{fromtime}' AND CreateDate < '{totime}' ORDER BY pkid DESC";
                }
            }
            
            DataTable dtab = SQLFunc.ExecuteDataTable(sql);
            if(dtab.Columns.Count > 0)
            {
                dgrDataa.DataSource = null;
                dgrDataa.DataSource = dtab;
                dgrDataa.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgrDataa.ReadOnly = false;
            }
            else
            {
                MessageBox.Show("Lỗi lấy dữ liệu từ DB!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dtab = (DataTable)dgrDataa.DataSource;
            if (dtab != null)
            {
                if (dtab.Rows.Count > 0)
                {
                    string sHeader = "";
                    string sContents = "";
                    for (int i = 0; i < dtab.Columns.Count; i++)
                    {
                        sHeader += Conv.atos(dtab.Columns[i].ColumnName) + ",";
                    }
                    for (int i = 0; i < dtab.Rows.Count; i++)
                    {
                        string sContent = "";
                        for (int j = 0; j < dtab.Columns.Count; j++)
                        {
                            sContent += Conv.atos(dtab.Rows[i][j]) + ",";
                        }
                        sContents += sContent + "\r\n";
                    }

                    if(sHeader.Length >0 && sContents.Length > 0)
                    {
                        try
                        {
                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.Filter = "csv|*.csv";
                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    if (sfd.FileName != "")
                                    {
                                        if (File.Exists(sfd.FileName))
                                        {
                                            File.Delete(sfd.FileName);
                                            File.WriteAllText(sfd.FileName, sHeader + "\r\n" + sContents);
                                        }
                                        else
                                        {
                                            File.WriteAllText(sfd.FileName, sHeader + "\r\n" + sContents);
                                        }

                                        MessageBox.Show($"Lưu dữ liệu thành công!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lưu dữ liệu lỗi!\r\n{ex.Message}", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                       
                    }
                }
            }
        }
    }
}