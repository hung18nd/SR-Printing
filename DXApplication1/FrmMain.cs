using DevExpress.XtraEditors;
using DXApplication1.Common;
using DXApplication1.MachineCls.SM_Printing;
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
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private TimeSpan lastCPU_TS;
        private System.DateTime lastCPU_Time = DateTime.Now;
        private System.Windows.Forms.Timer timer1s = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timerUpdateView = new System.Windows.Forms.Timer();

        Thread threadScanLog;
        private bool cmdThreadStart = false;
        private bool isRefresh = false;

        bool isCloseForm = false;

        string SavePath = "";
        int Counter = 0;

        FrmAlert frmAlert = new FrmAlert("");
        private DateTime LastTimeBtnPassPressed = DateTime.Now;
        private bool IsBtnPassReadPressed = false;
        private void FrmMain_Shown(object sender, EventArgs e)
        {
            ////string[] test = new string[] { "12599", "14392", "18999", "12333", "12336", "13622", "12333", "12592", "12589" };
            ////string[] test2 = new string[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
            ////string a = Conv.StringInt2StringChar(test);

            ////string b = Conv.StringInt2StringCharRevert(test2);
            ///


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

            this.Text = $"SR Printing Data Loging Software - Version: {Global.VersionSoftware}";

            Global.DoubleBuffered(dgrConn, true);

            Timer1sInit();
            TimerUpdateViewInit();

            txtSRPrintingMcID.Text = VarGlobal.McID;
            //this.WindowState = FormWindowState.Maximized;
            ReInitialNew();
            cbbBCRSelect.SelectedIndex = VarGlobal.McSMPrinting.BcrSelect;

            string file = Global.ApplicationPath() + @"\" + "App.app";
            IniFile ini = new IniFile(file);
            string tmp = ini.IniReadValue("SETTING", "PASS");
            if (tmp != "")
            {
                Global.PassworSetting = tmp;
            }

            // Khoi dong BlockId Ingestion Worker
            BlockId.BlockIdQueueManager.StartWorker();
            BlockId.BlockIdQueueManager.OnBlockIdAlert += (alertMsg, alertSev) =>
            {
                try
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(alertMsg, "Canh bao BlockId API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }));
                }
                catch { }
            };
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            string password = Global.PasswordInput();
            if (password == "1")
            {
                Timer1sRelease();
                TimerrUpdateViewRelease();               
                KillAllThreadsNew();
                BlockId.BlockIdQueueManager.StopWorker();
                isCloseForm = true;
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("Sai mật khẩu!", "LỖI THAO TÁC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnMachineCfg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmConfig frmConfig = new FrmConfig();
            frmConfig.ShowDialog();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var frm = new BlockId.FrmBlockIdConfig())
            {
                frm.ShowDialog();
            }
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

            bsiPlcIP.Caption = $"{VarGlobal.McSMPrinting.PlcIp}";
            if (BlockId.BlockIdConfig.Instance.Enable)
            {
                bsiPLCStatus.Caption = $"BlockId: {BlockId.BlockIdQueueManager.LastStatus} [Queue: {BlockId.BlockIdQueueManager.PendingCount}]";
            }

            string message = "";
            if(txtPLCStatMonitor.Text == "DISCONNECTED")
            {
                message += "Mất kết nối với PLC!\r\n";
            }
            if (txtSRXStatMonitor.Text == "DISCONNECTED")
            {
                message += "Mất kết nối với BCR SRX300!\r\n";
            }
            if (txtCOGStatMonitor.Text == "DISCONNECTED")
            {
                message += "Mất kết nối với BCR Cognex!\r\n";
            }
            if (VarGlobal.McSMPrinting.EnablePlcKV && txtKVStatMonitor.Text == "DISCONNECTED")
            {
                message += "Mất kết nối với PLC KV!\r\n";
            }
            if (txtDBStatMonitor.Text == "DISCONNECTED")
            {
                message += "Mất kết nối với Database!\r\n";
            }
            if (message.Length > 0)
            {
                if ((Application.OpenForms["FrmAlert"] as FrmAlert) != null)
                {
                    //Form is already open
                    try
                    {
                        frmAlert.SetMessage(message);
                        frmAlert.WindowState = FormWindowState.Normal;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        frmAlert = new FrmAlert(message);
                        frmAlert.Show();
                    }
                    catch
                    {

                    }
                    // Form is not open
                }
            }
            else
            {
                if ((Application.OpenForms["FrmAlert"] as FrmAlert) != null)
                {
                    //Form is already open
                    try
                    {
                        frmAlert.Close();
                    }
                    catch
                    {

                    }
                }
            }

            
        }

        private void TimerUpdateViewInit()
        {
            timerUpdateView.Interval = 200;
            timerUpdateView.Tick += timerUpdateView_Tick;
            timerUpdateView.Enabled = true;
        }
        private void TimerrUpdateViewRelease()
        {
            timerUpdateView.Stop();
        }

        private void timerUpdateView_Tick(object sender, EventArgs e)
        {
            timerUpdateView.Enabled = false;

            btnStart.BackColor = VarGlobal.McSMPrinting.IsSwStart ? Color.Lime : DefaultBackColor;
            txtCounter.Text = VarGlobal.McSMPrinting.Counter.ToString();
            txtPLCStatMonitor.Text = $"{(VarGlobal.McSMPrinting.IsConnected ? "CONNECTED" : "DISCONNECTED")}";
            txtSRXStatMonitor.Text = $"{(VarGlobal.McSMPrinting.IsConnectedSRX ? "CONNECTED" : "DISCONNECTED")}";
            txtCOGStatMonitor.Text = $"{(VarGlobal.McSMPrinting.IsConnectedCognex ? "CONNECTED" : "DISCONNECTED")}";
            if (VarGlobal.McSMPrinting.EnablePlcKV)
            {
                txtKVStatMonitor.Text = $"{(VarGlobal.McSMPrinting.IsConnectedPlcKV ? "CONNECTED" : "DISCONNECTED")}";
                txtKVStatMonitor.BackColor = (VarGlobal.McSMPrinting.IsConnectedPlcKV ? Color.Lime : Color.Red);
            }
            else
            {
                txtKVStatMonitor.Text = "NOT USED";
                txtKVStatMonitor.BackColor = Color.LightGray;
            }
            txtDBStatMonitor.Text = $"{(VarGlobal.IsConnectedDB ? "CONNECTED" : "DISCONNECTED")}";

            txtPLCStatMonitor.BackColor = (VarGlobal.McSMPrinting.IsConnected ? Color.Lime : Color.Red);
            txtSRXStatMonitor.BackColor = (VarGlobal.McSMPrinting.IsConnectedSRX ? Color.Lime : Color.Red);
            txtCOGStatMonitor.BackColor = (VarGlobal.McSMPrinting.IsConnectedCognex ? Color.Lime : Color.Red);
            txtDBStatMonitor.BackColor = (VarGlobal.IsConnectedDB ? Color.Lime : Color.Red);

            txtItemNamePlusA.Text = VarGlobal.McSMPrinting.arrItem[0].ItemName;
            txtItemCodePlusA.Text = VarGlobal.McSMPrinting.arrItem[0].ItemCode;
            txtItemLotPlusA.Text = VarGlobal.McSMPrinting.arrItem[0].ItemLot;
            txtItemSeqPlusA.Text = VarGlobal.McSMPrinting.arrItem[0].ItemSeq;


            lblSignalStart.BackColor = VarGlobal.McSMPrinting.IsMcStart ? Color.Lime : Color.Gray;

            txtBlockNumPerCycle.Text = VarGlobal.McSMPrinting.BlockNumPerPrints.ToString();
            txtBcrMode.Text = VarGlobal.McSMPrinting.BcrMode == 0 ? "USE BARCODE" : "NOT USE";
            txtBcrSelect.Text = VarGlobal.McSMPrinting.BcrSelect == 0 ? "UP" : "DOWN";

            bool checkOk = true;
            
            for(int i  =0; i< VarGlobal.McSMPrinting.BlockNumPerPrints; i++)
            {
                if(VarGlobal.McSMPrinting.BlockIDs[i].BlockID != "" && !VarGlobal.McSMPrinting.BlockIDs[i].BlockID.StartsWith("999999"))
                {
                    bool isBcrMixlot = false;
                    if (VarGlobal.McSMPrinting.EnableMixing)
                    {
                        isBcrMixlot = VarGlobal.CheckMixing(VarGlobal.McSMPrinting.MachineID, VarGlobal.McSMPrinting.Factory, VarGlobal.McSMPrinting.BlockIDs[i].BlockID, VarGlobal.McSMPrinting.arrItem, VarGlobal.McSMPrinting.EnableMixing);
                        if (isBcrMixlot)
                        {
                            checkOk = false;
                            Global.WriteLogFile($"Error FPC mixing lot!\r\nBlockID: { VarGlobal.McSMPrinting.BlockIDs[i].BlockID}\r\nItemCode (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemCode}\r\nLot (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemLot}\r\nSequence (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemSeq}");
                            MessageBox.Show($"Error FPC mixing lot!\r\nBlockID: { VarGlobal.McSMPrinting.BlockIDs[i].BlockID}\r\nItemCode (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemCode}\r\nLot (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemLot}\r\nSequence (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemSeq}", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            VarGlobal.McSMPrinting.BlockIDs[i].BlockID = "";
                        }
                        else VarGlobal.McSMPrinting.ReqAlarmMixing = false;
                    }    
                    else VarGlobal.McSMPrinting.ReqAlarmMixing = false;

                    if (!isBcrMixlot && VarGlobal.McSMPrinting.EnableReinput)
                    {
                        VarGlobal.McSMPrinting.ReqAlarmMixing = false;
                        // check reinput
                        bool isBcrReinput = VarGlobal.McSMPrinting.CheckReInput(VarGlobal.McSMPrinting.BlockIDs[i].BlockID, VarGlobal.McSMPrinting.arrItem[0], VarGlobal.McSMPrinting.BlockIDs[i].LastBlockID);
                        if (isBcrReinput)
                        {
                            checkOk = false;
                            Global.WriteLogFile($"Error FPC re-input!\r\nBlockID: { VarGlobal.McSMPrinting.BlockIDs[i].BlockID}\r\nItemCode (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemCode}\r\nLot (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemLot}\r\nSequence (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemSeq}");
                            MessageBox.Show($"Error FPC re-input!\r\nBlockID: { VarGlobal.McSMPrinting.BlockIDs[i].BlockID}\r\nItemCode (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemCode}\r\nLot (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemLot}\r\nSequence (PlusA): {VarGlobal.McSMPrinting.arrItem[0].ItemSeq}", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            VarGlobal.McSMPrinting.BlockIDs[i].BlockID = "";
                        }
                    }
                }
            }

            if (checkOk)
            {
                txtBlockID.Text = VarGlobal.McSMPrinting.BlockIDs[0].BlockID;
                txtBlockID2.Text = VarGlobal.McSMPrinting.BlockIDs[1].BlockID;
                txtBlockID3.Text = VarGlobal.McSMPrinting.BlockIDs[2].BlockID;
                txtBlockID4.Text = VarGlobal.McSMPrinting.BlockIDs[3].BlockID;
                txtBlockID5.Text = VarGlobal.McSMPrinting.BlockIDs[4].BlockID;
                txtBlockID6.Text = VarGlobal.McSMPrinting.BlockIDs[5].BlockID;
                txtBlockID7.Text = VarGlobal.McSMPrinting.BlockIDs[6].BlockID;
                txtBlockID8.Text = VarGlobal.McSMPrinting.BlockIDs[7].BlockID;
            }
            
           

            txtParm_PrintingTime.Text = VarGlobal.McSMPrintingParam.PrintingTime.ToString();
            txtParm_StencilAngle.Text = VarGlobal.McSMPrintingParam.StencilAngle.ToString();
            txtParm_SquSpeed.Text = VarGlobal.McSMPrintingParam.SqueezeSpeed.ToString();
            txtParm_ReturnSpeed.Text = VarGlobal.McSMPrintingParam.ReturnSpeed.ToString();
            txtParm_SquePress.Text = VarGlobal.McSMPrintingParam.SqueezePressure.ToString();
            txtParm_SquePrinDistStart.Text = VarGlobal.McSMPrintingParam.SqueezePringtingDistanceStart.ToString();
            txtParm_SquePrinDistFinish.Text = VarGlobal.McSMPrintingParam.SqueezePringtingDistanceFinish.ToString();
            txtParm_CoaterPress.Text = VarGlobal.McSMPrintingParam.CoaterPressure.ToString();
            txtParm_Gap.Text = VarGlobal.McSMPrintingParam.Gap.ToString();
            txtParm_McAirPress.Text = VarGlobal.McSMPrintingParam.MachineAirPressure.ToString();

            if (LastTimeBtnPassPressed.AddSeconds(2) < DateTime.Now && IsBtnPassReadPressed && VarGlobal.McSMPrinting.IsSwStart && VarGlobal.McSMPrinting.IsMcReadyStart)
            {
                if (!VarGlobal.McSMPrinting.IsPassSignal)
                {
                    Global.WriteLogFile("Người dùng chọn [PASS]");
                }
                VarGlobal.McSMPrinting.IsPassSignal = true;
            }
            btnPassRead.BackColor = VarGlobal.McSMPrinting.IsPassSignal ? Color.Lime : DefaultBackColor;

            if (txtTroleyID.Text != "" && txtTroleyID.ReadOnly && VarGlobal.McSMPrinting.IsSwStart)
            {
                if (CheckExistsData(VarGlobal.McSMPrinting.BlockNumPerPrints))
                {
                    VarGlobal.McSMPrinting.IsTrigger = false;
                    if(VarGlobal.McSMPrinting.IsMcReadyStart)
                        VarGlobal.McSMPrinting.IsOKSignal = true;
                }
                else
                {
                    VarGlobal.McSMPrinting.IsTrigger = true;
                }
            }
            else  VarGlobal.McSMPrinting.IsTrigger = false;

            if(VarGlobal.McSMPrinting.IsMcStart && !VarGlobal.McSMPrinting.IsLastMcStart && VarGlobal.McSMPrinting.IsSwStart)
            {
                bool isHaveData = false;
                for (int i = 0; i < VarGlobal.McSMPrinting.BlockIDs.Length; i++)
                {
                    if (VarGlobal.McSMPrinting.BlockIDs[i].BlockID != VarGlobal.McSMPrinting.BlockIDs[i].LastBlockID)
                    {
                        if (VarGlobal.McSMPrinting.BlockIDs[i].BlockID != "")
                        {
                            isHaveData = true;
                            // save log local
                            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string content = $"{dtime},{txtSRPrintingMcID.Text},{txtTroleyID.Text},{VarGlobal.McSMPrinting.BlockIDs[i].BlockID},{txtOpratorID.Text},{VarGlobal.McSMPrinting.arrItem[0].ItemSeq}";
                            if (!VarGlobal.McSMPrinting.BlockIDs[i].BlockID.Contains("999999"))
                            {
                                if (SaveHistoryDatas(content, VarGlobal.McSMPrinting.BlockIDs[i].BlockID))
                                {
                                    dgrConn.Rows.Insert(0, dgrConn.Rows.Count + 1, dtime, txtSRPrintingMcID.Text, txtTroleyID.Text, VarGlobal.McSMPrinting.BlockIDs[i].BlockID, txtOpratorID.Text);
                                    if (VarGlobal.McSMPrinting.EnablePlcKV)
                                    {
                                        VarGlobal.McSMPrinting.BlockID2KVList.Add(new BlockID2KV() { BlockID = VarGlobal.McSMPrinting.BlockIDs[i].BlockID });
                                    }

                                    // BlockId Ingestion API
                                    if (BlockId.BlockIdConfig.Instance.Enable)
                                    {
                                        BlockId.BlockIdQueueManager.EnqueueRecord(
                                            VarGlobal.McSMPrinting.BlockIDs[i].BlockID,
                                            VarGlobal.McSMPrinting.arrItem[0]?.ItemLot ?? "",
                                            VarGlobal.McSMPrintingParam,
                                            txtSRPrintingMcID.Text
                                        );
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Lỗi lưu dữ liệu!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        VarGlobal.McSMPrinting.BlockIDs[i].LastBlockID = VarGlobal.McSMPrinting.BlockIDs[i].BlockID;
                        // save lastblockID
                        VarGlobal.McSMPrinting.Save_LastBlockIDs();
                    }
                }
                VarGlobal.McSMPrinting.IsPassSignal = false;
                VarGlobal.McSMPrinting.IsOKSignal = false;
                for (int i = 0; i < VarGlobal.McSMPrinting.BlockIDs.Length; i++)
                {
                    VarGlobal.McSMPrinting.BlockIDs[i].BlockID = "";
                }

                BCRSrx300Cls.LastBlockID = ""; // clear buffer
                BCRCognexCls.LastBlockID = "";
                //////if(isHaveData)
                //////    Counter++;
                //////if (Counter == 50)
                //////{
                //////    btnReset_Click(null,null);
                //////    Counter = 0;
                //////}
            }
            VarGlobal.McSMPrinting.IsLastMcStart = VarGlobal.McSMPrinting.IsMcStart;

            if (!isCloseForm) timerUpdateView.Enabled = true;
        }

        private bool CheckExistsData(int numb)
        {
            bool ret = false;
            if(numb == 1)
            {
                return txtBlockID.Text != "";
            }
            else if (numb == 2)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "";
            }
            else if (numb == 3)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "";
            }
            else if (numb == 4)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "" && txtBlockID4.Text != "";
            }
            else if (numb == 5)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "" && txtBlockID4.Text != "" && txtBlockID5.Text != "";
            }
            else if (numb == 6)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "" && txtBlockID4.Text != "" && txtBlockID5.Text != "" && txtBlockID6.Text != "";
            }
            else if (numb == 7)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "" && txtBlockID4.Text != "" && txtBlockID5.Text != "" && txtBlockID6.Text != "" && txtBlockID7.Text != "";
            }
            else if (numb == 8)
            {
                return txtBlockID.Text != "" && txtBlockID2.Text != "" && txtBlockID3.Text != "" && txtBlockID4.Text != "" && txtBlockID5.Text != "" && txtBlockID6.Text != "" && txtBlockID7.Text != "" && txtBlockID8.Text != "";
            }
            return ret;
        }

        #endregion

        private void ReInitialNew()
        {

            cmdThreadStart = true;
            threadScanLog = new Thread(() => ThreadScanLogProcess());
            threadScanLog.Start();
        }
        private void KillAllThreadsNew()
        {
            cmdThreadStart = false;

        }

        private void ThreadScanLogProcess()
        {
            while (cmdThreadStart)
            {
                if (!SQLFunc.IsConnected)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                        if (!cmdThreadStart) break;
                    }
                    continue;
                }

                string path = Global.ApplicationPath() + @"\HistoryData";
                if (Directory.Exists(path))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    FileInfo[] files = dir.GetFiles("*.txt", SearchOption.TopDirectoryOnly).ToArray();
                    if(files.Length > 0)
                    {
                        foreach(FileInfo file in files)
                        {
                            if (!cmdThreadStart) break;

                            string[] slines = File.ReadAllLines(file.FullName);
                            if (slines.Length > 0)
                            {
                                string[] sItems = slines[0].Split(',');
                                if (sItems.Length >= 6)
                                {
                                    string strQuery = $"INSERT INTO Tbl_Printing_Baking_Mapping (printing_machineId,troleyId,blockId,operatorId,printing_time,printing_seq) VALUES ('{sItems[1]}','{sItems[2]}','{sItems[3]}','{sItems[4]}','{sItems[0]}','{sItems[5]}')";
                                    if (SQLFunc.ExecuteNonQuery(strQuery))
                                    {
                                        string readOKPath = path + "\\READ_OK" + @"\" + DateTime.Now.ToString("yyyy-MM-dd");
                                        try
                                        {
                                            if (!Directory.Exists(readOKPath)) Directory.CreateDirectory(readOKPath);
                                            string readOkfile = readOKPath + "\\" + file.Name;
                                            if (File.Exists(readOkfile)) File.Delete(readOkfile);
                                            File.Move(file.FullName, readOkfile);
                                        }
                                        catch (Exception ex)
                                        {
                                            Global.WriteLogFile($"[Move File Error] {file.Name} - {ex.Message}");
                                        }

                                        //test
                                        if (VarGlobal.McSMPrinting.EnablePlcKV)
                                        {
                                            VarGlobal.McSMPrinting.BlockID2KVList.Add(new BlockID2KV() { BlockID =  sItems[3] });
                                        }
                                    }
                                    else
                                    {
                                        SQLFunc.IsConnected = false;
                                        VarGlobal.IsConnectedDB = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // xoa sau 6 thang
                    string readOKPath1 = path + "\\READ_OK";
                    if (Directory.Exists(readOKPath1))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(readOKPath1);
                        DirectoryInfo[] directoryInfos = dirInfo.GetDirectories("*.*",SearchOption.TopDirectoryOnly);
                        if(directoryInfos.Length > 0)
                        {
                            for(int i  = 0; i < directoryInfos.Length; i++)
                            {
                                if(directoryInfos[i].LastWriteTime.AddMonths(6) < DateTime.Now)
                                {
                                    try
                                    {
                                        Directory.Delete(directoryInfos[i].FullName, true);
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    
                                }
                            }
                        }
                    }
                }

                for (int i =0; i<2; i++)
                {
                    Thread.Sleep(1000);
                    if (!cmdThreadStart) break;
                }
                
            }
        }


        #region LOGFILE
        private void SaveRegValueLog2(string header, string data, string MachineID)
        {
            DateTime dateTimeSaveLog = DateTime.Now;
            if (dateTimeSaveLog.Hour < 17)
            {
                dateTimeSaveLog = dateTimeSaveLog.AddDays(-1);
            }
            string MyPath = Global.ApplicationPath() + @"\DataLog" + @"\" + MachineID + @"\" + dateTimeSaveLog.ToString("yyyyMMdd");
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += @"\" + dateTimeSaveLog.ToString("yyyyMMdd") + "_" + MachineID + "_[Value].csv";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "TIME," + header + "\r\n";
                    File.WriteAllText(MyPath, createText);
                }

                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + data + "\r\n";
                File.AppendAllText(MyPath, sBuff);

                //File.AppendAllText(MyPath, "-".PadRight(100, '-'));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        private void SaveRegValueLog3(string header, string data, string MachineID)
        {
            DateTime dateTimeSaveLog = DateTime.Now;
            if (dateTimeSaveLog.Hour < 17)
            {
                dateTimeSaveLog = dateTimeSaveLog.AddDays(-1);
            }
            string MyPath = Global.ApplicationPath() + @"\DataLog" + @"\" + MachineID + @"\" + dateTimeSaveLog.ToString("yyyyMMdd");
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += @"\" + dateTimeSaveLog.ToString("yyyyMMdd") + "_" + MachineID + "_[ValueNew].csv";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "TIME," + header + "\r\n";
                    File.WriteAllText(MyPath, createText);
                }

                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + data + "\r\n";
                File.AppendAllText(MyPath, sBuff);

                //File.AppendAllText(MyPath, "-".PadRight(100, '-'));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        private void SaveRegValueLog4(string header, string data, string MachineID)
        {
            DateTime dateTimeSaveLog = DateTime.Now;
            if (dateTimeSaveLog.Hour < 17)
            {
                dateTimeSaveLog = dateTimeSaveLog.AddDays(-1);
            }
            string MyPath = Global.ApplicationPath() + @"\DataLog" + @"\" + MachineID + @"\" + dateTimeSaveLog.ToString("yyyyMMdd");
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += @"\" + dateTimeSaveLog.ToString("yyyyMMdd") + "_" + MachineID + "_[ValueNew2].csv";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "TIME," + header + "\r\n";
                    File.WriteAllText(MyPath, createText);
                }

                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + data + "\r\n";
                File.AppendAllText(MyPath, sBuff);

                //File.AppendAllText(MyPath, "-".PadRight(100, '-'));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SaveLogSTSCopperBlockID(string header, string data, string MachineID, string filename)
        {
            DateTime dateTimeSaveLog = DateTime.Now;
            if (dateTimeSaveLog.Hour < 17)
            {
                dateTimeSaveLog = dateTimeSaveLog.AddDays(-1);
            }
            string MyPath = Global.ApplicationPath() + @"\DataLog" + @"\" + MachineID + @"\" + dateTimeSaveLog.ToString("yyyyMMdd") + @"\BLOCK";
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += @"\" + filename + "_"+ dateTimeSaveLog.ToString("yyyyMMdd-HHmmss") + ".csv";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "TIME," + header + "\r\n";
                    File.WriteAllText(MyPath, createText);
                }

                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + data + "\r\n";
                File.AppendAllText(MyPath, sBuff);

                //File.AppendAllText(MyPath, "-".PadRight(100, '-'));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        string strNumber = "0123456789";
        private void SaveLogBlockID(string pathSave, string header, string data, string MachineID, string blockID)
        {
            if (blockID.Length >= 12)
            {
                if (!strNumber.Contains(blockID[2])) //semi
                {
                    string[] arrblock = blockID.Split('-');
                    if (arrblock.Length >= 2)
                    {
                        string MyPath = pathSave + @"\" + $"{arrblock[0]}";
                        try
                        {
                            if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                            MyPath += @"\" + blockID + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";
                            if (File.Exists(MyPath) == false)
                            {
                                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + $"{MachineID}," + data + "\r\n";
                                // Create a file to write to.
                                string createText = "TIME, MACHINE ID," + header + "\r\n";
                                File.WriteAllText(MyPath, createText + sBuff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.WriteLogFile(ex.ToString());
                        }
                    }
                }
                else
                {
                    string[] arrblock = blockID.Split('-');
                    if (arrblock.Length >= 2)
                    {
                        string MyPath = pathSave + @"\" + $"{arrblock[0]}-{arrblock[1]}";
                        try
                        {
                            if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                            MyPath += @"\" + blockID + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv";
                            if (File.Exists(MyPath) == false)
                            {
                                string sBuff = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",") + $"{MachineID}," + data + "\r\n";
                                // Create a file to write to.
                                string createText = "TIME, MACHINE ID," + header + "\r\n";
                                File.WriteAllText(MyPath, createText + sBuff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.WriteLogFile(ex.ToString());
                        }
                    }
                }
            }
        }

        private void SaveHistoryBlockID(string MachineID, string section, string blockID)
        {
            string MyPath = Global.ApplicationPath() + @"\HistoryData";
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                IniFile iniFile = new IniFile(MyPath + @"\" + MachineID + ".ini");
                iniFile.IniWriteValue("HISTORY", section, blockID);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SaveHMIEventLogAll(string mess, string machineID)
        {
            DateTime dateTimeSaveLog = DateTime.Now;
            if (dateTimeSaveLog.Hour < 17)
            {
                dateTimeSaveLog = dateTimeSaveLog.AddDays(-1);
            }
            string MyPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machineID + @"\" + dateTimeSaveLog.ToString("yyyyMMdd");
            try
            {
                //string appendText = "\r\n" + "-".PadRight(100, '-') + "\r\n" + DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] - ") + stext + "\r\n" + "\r\n";
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += @"\" + dateTimeSaveLog.ToString("yyyyMMdd") + "_" + machineID + "_[HMI].csv";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "Time,".PadRight(30, ' ') + "ErrorCode" + "\r\n";
                    File.WriteAllText(MyPath, createText);
                }
                string sBuff = mess;
                File.AppendAllText(MyPath, sBuff);

                //File.AppendAllText(MyPath, "-".PadRight(100, '-'));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void TransferHMIEventLog(string machine)
        {
            DateTime dateTimeTransfer = DateTime.Now;

            try
            {
                string SourPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.ToString("yyyyMMdd");
                string DestPath = SavePath + @"\" + machine + @"\" + dateTimeTransfer.ToString("yyyyMMdd");

                
                if (Directory.Exists(SourPath))
                {
                    if (!Directory.Exists(DestPath)) Directory.CreateDirectory(DestPath);

                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly);
                    if (fileInfos.Length > 0)
                    {
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            string destFile = DestPath + @"\" + fileInfo.Name;
                            if (!File.Exists(destFile))
                            {
                                try
                                {
                                    File.Copy(fileInfo.FullName, destFile, true);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                FileInfo fileInfo1 = new FileInfo(destFile);

                                if (fileInfo.LastWriteTime > fileInfo1.LastWriteTime)
                                {
                                    try
                                    {
                                        File.Copy(fileInfo.FullName, destFile, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }

                    }
                }
                // transfer file cua ngay hom truoc
                SourPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.AddDays(-1).ToString("yyyyMMdd");
                DestPath = SavePath + @"\" + machine + @"\" + dateTimeTransfer.AddDays(-1).ToString("yyyyMMdd");

                
                if (Directory.Exists(SourPath))
                {
                    if (!Directory.Exists(DestPath)) Directory.CreateDirectory(DestPath);


                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly);
                    if (fileInfos.Length > 0)
                    {
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            string destFile = DestPath + @"\" + fileInfo.Name;
                            if (!File.Exists(destFile))
                            {
                                try
                                {
                                    File.Copy(fileInfo.FullName, destFile, true);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                FileInfo fileInfo1 = new FileInfo(destFile);

                                if (fileInfo.LastWriteTime > fileInfo1.LastWriteTime)
                                {
                                    try
                                    {
                                        File.Copy(fileInfo.FullName, destFile, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {

            }


            try
            {
                string SourPathDel = Global.ApplicationPath() + @"\DataLog" + @"\" + machine;
                if (Directory.Exists(SourPathDel))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPathDel);
                    DirectoryInfo[] arrDir = directoryInfo.GetDirectories().Where(x => x.CreationTime < dateTimeTransfer.AddDays(-7)).ToArray();
                    foreach (DirectoryInfo directoryInfo1 in arrDir)
                    {
                        try
                        {
                            Directory.Delete(directoryInfo1.FullName, true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void TransferHMIEventLog2(string machine)
        {
            DateTime dateTimeTransfer = DateTime.Now;

            try
            {
                string SourPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.ToString("yyyyMMdd") + @"\BLOCK";
                string DestPath = SavePath + @"\" + machine + @"\" + dateTimeTransfer.ToString("yyyyMMdd") + @"\BLOCK";
                string BackupPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.ToString("yyyyMMdd") + @"\BLOCK\BACKUP";

                if (Directory.Exists(SourPath))
                {
                    if (!Directory.Exists(DestPath)) Directory.CreateDirectory(DestPath);
                    if (!Directory.Exists(BackupPath)) Directory.CreateDirectory(BackupPath);

                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*.csv",SearchOption.TopDirectoryOnly);
                    if (fileInfos.Length > 0)
                    {
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            string destFile = DestPath + @"\" + fileInfo.Name;
                            if (!File.Exists(destFile))
                            {
                                try
                                {
                                    File.Copy(fileInfo.FullName, destFile, true);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                FileInfo fileInfo1 = new FileInfo(destFile);

                                if (fileInfo.LastWriteTime > fileInfo1.LastWriteTime)
                                {
                                    try
                                    {
                                        File.Copy(fileInfo.FullName, destFile, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }
                            File.Move(fileInfo.FullName, BackupPath + @"\" + fileInfo.Name);
                        }

                    }
                }
                // transfer file cua ngay hom truoc
                SourPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.AddDays(-1).ToString("yyyyMMdd") + @"\BLOCK";
                DestPath = SavePath + @"\" + machine + @"\" + dateTimeTransfer.AddDays(-1).ToString("yyyyMMdd") + @"\BLOCK";
                BackupPath = Global.ApplicationPath() + @"\DataLog" + @"\" + machine + @"\" + dateTimeTransfer.AddDays(-1).ToString("yyyyMMdd") + @"\BLOCK\BACKUP";

                if (Directory.Exists(SourPath))
                {
                    if (!Directory.Exists(DestPath)) Directory.CreateDirectory(DestPath);
                    if (!Directory.Exists(BackupPath)) Directory.CreateDirectory(BackupPath);

                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly);
                    if (fileInfos.Length > 0)
                    {
                        foreach (FileInfo fileInfo in fileInfos)
                        {
                            string destFile = DestPath + @"\" + fileInfo.Name;
                            if (!File.Exists(destFile))
                            {
                                try
                                {
                                    File.Copy(fileInfo.FullName, destFile, true);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                FileInfo fileInfo1 = new FileInfo(destFile);

                                if (fileInfo.LastWriteTime > fileInfo1.LastWriteTime)
                                {
                                    try
                                    {
                                        File.Copy(fileInfo.FullName, destFile, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }

                            File.Move(fileInfo.FullName, BackupPath + @"\" + fileInfo.Name);
                        }

                    }
                }
            }
            catch (Exception)
            {

            }


            try
            {
                string SourPathDel = Global.ApplicationPath() + @"\DataLog" + @"\" + machine;
                if (Directory.Exists(SourPathDel))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(SourPathDel);
                    DirectoryInfo[] arrDir = directoryInfo.GetDirectories().Where(x => x.CreationTime < dateTimeTransfer.AddDays(-7)).ToArray();
                    foreach (DirectoryInfo directoryInfo1 in arrDir)
                    {
                        try
                        {
                            Directory.Delete(directoryInfo1.FullName, true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private bool SaveHistoryDatas(string content, string blockID)
        {
            bool ret = false;
            string MyPath = Global.ApplicationPath() + @"\HistoryData";
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                string safeBlockId = string.Join("_", blockID.Split(Path.GetInvalidFileNameChars()));
                string MyFile = MyPath + $"\\SRPrinting_{safeBlockId}_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.txt";
                File.WriteAllText(MyFile, content);

                Global.WriteLogFile(content);

                ret = true;
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[SaveHistoryDatas Error] BlockID: {blockID} - {ex.Message}");
            }
            return ret;
        }
        
        #endregion



        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Hide();
            FrmConfig2 frmConfig2 = new FrmConfig2();
            frmConfig2.ShowDialog();
            this.Show();
            simpleButton2_Click(null,null);
        }

        private void ppGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmTrace frmTrace = new FrmTrace();
            frmTrace.ShowDialog();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnDisableAllMachine_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmTraceBlockID frmTrace = new FrmTraceBlockID();
            frmTrace.ShowDialog();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmTraceBlockIDHist frmTrace = new FrmTraceBlockIDHist();
            frmTrace.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtTroleyID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string troleyId = txtTroleyID.Text.Replace("\r", "").Replace("\n", "");
                string strQuery = $"SELECT [printing_machineId],[troleyId],[blockId],[operatorId],[printing_time] FROM [Tbl_Printing_Baking_Mapping] WHERE [troleyId] = '{troleyId}' AND [baking_machineId] is NULL order by pkid asc";
                DataTable dtab = SQLFunc.ExecuteDataTable(strQuery);
                if(dtab.Columns.Count > 0)
                {
                    if(dtab.Rows.Count > 0)
                    {
                        DialogResult dialog = MessageBox.Show("Mã xe này chưa kết thúc ở công đoạn Baking\nXác nhận sử dụng tiếp hoặc làm mới?", "MESSAGE", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialog == DialogResult.Yes)
                        {
                            for (int i = 0; i < dtab.Rows.Count; i++)
                            {
                                dgrConn.Rows.Insert(0, dgrConn.Rows.Count + 1
                                    , Conv.atos(dtab.Rows[i]["printing_time"])
                                    , Conv.atos(dtab.Rows[i]["printing_machineId"])
                                    , Conv.atos(dtab.Rows[i]["troleyId"])
                                    , Conv.atos(dtab.Rows[i]["blockId"])
                                    , Conv.atos(dtab.Rows[i]["operatorId"]));
                            }
                        }
                        else
                        {
                            strQuery = $"UPDATE [Tbl_Printing_Baking_Mapping] SET baking_machineId = 'UserConfirm' , baking_time = GETDATE()  WHERE [troleyId] = '{troleyId}' AND [baking_machineId] is NULL";
                            SQLFunc.ExecuteNonQuery(strQuery);
                        }
                    }
                }
                txtTroleyID.ReadOnly = true;
            }
        }

        private void txtBlockID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(txtTroleyID.Text == "")
                {
                    MessageBox.Show("Hãy scan dữ liệu trước khi bắt đầu!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // save log local
                string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string content = $"{dtime},{txtSRPrintingMcID.Text},{txtTroleyID.Text},{txtBlockID.Text},{txtOpratorID.Text}";
                if(SaveHistoryDatas(content, txtBlockID.Text))
                {
                    dgrConn.Rows.Insert(0, dgrConn.Rows.Count + 1, dtime, txtSRPrintingMcID.Text, txtTroleyID.Text, txtBlockID.Text, txtOpratorID.Text);
                }
                else
                {
                    MessageBox.Show("Lỗi lưu dữ liệu!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtOpratorID.ReadOnly = false;
            txtOpratorID.Clear();
            dgrConn.Rows.Clear();
            txtTroleyID.ReadOnly = false;
            txtTroleyID.Clear();
            txtTroleyID.Focus();
            VarGlobal.McSMPrinting.IsSwStart = false;
            VarGlobal.McSMPrinting.Counter = 0;
            VarGlobal.McSMPrinting.IsOKSignal = false;
            VarGlobal.McSMPrinting.LastOKSignal = false;
            VarGlobal.McSMPrinting.IsPassSignal = false;
            VarGlobal.McSMPrinting.LastPassSignal = false;
        }

        private void txtOpratorID_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (txtOpratorID.Text.Length > 0)
                {
                    txtOpratorID.ReadOnly = true;
                    txtTroleyID.Focus();
                }
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //txtBlockID.ReadOnly = false;
            //txtBlockID2.ReadOnly = false;
            //txtBlockID3.ReadOnly = false;
            //txtBlockID4.ReadOnly = false;
            //txtBlockID5.ReadOnly = false;
            //txtBlockID6.ReadOnly = false;
            //txtBlockID7.ReadOnly = false;
            //txtBlockID8.ReadOnly = false;

            for(int i = 0; i< VarGlobal.McSMPrinting.BlockIDs.Length; i++)
            {
                VarGlobal.McSMPrinting.BlockIDs[i].BlockID = "";
            }
            VarGlobal.McSMPrinting.IsOKSignal = false;
            VarGlobal.McSMPrinting.LastOKSignal = false;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (txtTroleyID.Text == "")
                return;
            Global.WriteLogFile($"BUTTON START: SRPrintingMachine {txtSRPrintingMcID.Text}, TrolleyID {txtTroleyID.Text}, BCRMode {txtBcrMode.Text} - {cbbBCRSelect.Text}");
            VarGlobal.McSMPrinting.IsSwStart = true;
            cbbBCRSelect.Enabled = false;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            cbbBCRSelect.Enabled = true;
            VarGlobal.McSMPrinting.IsSwStart = false;
            VarGlobal.McSMPrinting.IsPassSignal = false;
        }

        private void groupControl2_Paint(object sender, PaintEventArgs e)
        {

        }

   
        private void btnPassRead_MouseDown(object sender, MouseEventArgs e)
        {
            if(VarGlobal.McSMPrinting.IsSwStart)
            {
                LastTimeBtnPassPressed = DateTime.Now;
                IsBtnPassReadPressed = true;
            }
        }

        private void btnPassRead_MouseUp(object sender, MouseEventArgs e)
        {
            IsBtnPassReadPressed = false;
            
        }

        private void cbbBCRSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            VarGlobal.McSMPrinting.BcrSelect = cbbBCRSelect.SelectedIndex;
            Global.WriteLogFile($"Người dùng chọn chế độ code [{(VarGlobal.McSMPrinting.BcrSelect == 0 ? "TRÊN" : "DƯỚI")}]");
        }

        private void btnPassRead_Click(object sender, EventArgs e)
        {

        }

        private void lblPassRead_Click(object sender, EventArgs e)
        {

        }
    }
}
