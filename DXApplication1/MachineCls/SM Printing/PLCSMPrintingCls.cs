using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCProtocol;

namespace DXApplication1.MachineCls.SM_Printing
{
    class PLCSMPrintingCls
    {
        private static Thread ThreadPollingPLC;
        private static F1E PLC = new F1E("192.168.100.100",5000);
        private static bool mainThreadStart = false;

        public static void Thread_Init()
        {
            VarGlobal.McSMPrinting.Read_Config();
            VarGlobal.McSMPrinting.Read_LastIdxBcr();
            VarGlobal.McSMPrinting.Read_LastBlockIDs();
            mainThreadStart = true;
            ThreadPollingPLC = new Thread(() => ThreadPollingPLC_Process());
            ThreadPollingPLC.IsBackground = true;
            ThreadPollingPLC.Start();
        }
        public static void Thread_Release()
        {
            mainThreadStart = false;
            try
            {
                if (PLC != null) PLC.Close();
            }
            catch { }
            try
            {
                if (ThreadPollingPLC != null && ThreadPollingPLC.IsAlive)
                {
                    ThreadPollingPLC.Abort();
                }
            }
            catch { }
        }

        #region PLC PROCESS
        private static void ThreadPollingPLC_Process()
        {
            Stopwatch stoploc = new Stopwatch();

            bool isConnected = false;
            long x = 0;
            string sRX;
            UInt16[] rx_dd = new UInt16[0];
            while (mainThreadStart)
            {
                try
                {
                    if (VarGlobal.McSMPrinting.Connection)
                    {
                        #region CONNECTION INIT
                        if (!isConnected)
                        {
                            try
                            {
                                VarGlobal.McSMPrinting.Message = "CONNECTING...";
                                PLC.Close();
                                Thread.Sleep(200);
                                PLC = new F1E(VarGlobal.McSMPrinting.PlcIp, VarGlobal.McSMPrinting.PlcPort);
                                PLC.Open();
                            }
                            catch (Exception ex)
                            {
                                VarGlobal.McSMPrinting.Message = ex.Message;
                            }

                        }
                        #endregion

                        if (PLC.IsConnected)
                        {
                            isConnected = true;
                            stoploc.Restart();
                            try
                            {
                                //rx_dd = (UInt16[])PLC.Read("D7030", Protocol.VarType.Word, 60);
                                //if (rx_dd != null)
                                //{
                                //    if (rx_dd.Length == 60)
                                //    {
                                //        UInt16[] rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 0, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[0].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 10, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[1].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 20, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[2].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 30, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[3].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 40, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[4].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 50, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[5].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //    }
                                //    else
                                //    {
                                //        isConnected = false;
                                //    }
                                //}
                                //else isConnected = false;
                                //rx_dd = (UInt16[])PLC.Read("D7090", Protocol.VarType.Word, 20);
                                //if (rx_dd != null)
                                //{
                                //    if (rx_dd.Length == 20)
                                //    {
                                //        UInt16[] rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 0, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[6].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //        rx_temp = new UInt16[10];
                                //        Array.Copy(rx_dd, 10, rx_temp, 0, 10);
                                //        VarGlobal.McSMPrinting.BlockIDs[7].BlockID = Conv.Int2StringCharRevert(rx_temp);
                                //    }
                                //}

                                bool[] rxStatus = (bool[])PLC.Read("M5024", Protocol.VarType.Bit, 10);
                                if (rxStatus != null)
                                {
                                    if (rxStatus.Length == 10)
                                    {
                                        VarGlobal.McSMPrinting.IsMcStart = rxStatus[1];
                                    }
                                    else isConnected = false;
                                }
                                else isConnected = false;
                                bool[] rxStatus1 = (bool[])PLC.Read("X25", Protocol.VarType.Bit, 2);
                                if (rxStatus1 != null)
                                {
                                    if (rxStatus1.Length == 2)
                                    {
                                        VarGlobal.McSMPrinting.IsMcReadyStart = rxStatus1[0];
                                    }
                                }
                                if (VarGlobal.McSMPrinting.BcrMode == 1)
                                {
                                    PLC.Write("M5024", Protocol.VarType.Bit, true);
                                }    
                                else if (VarGlobal.McSMPrinting.BcrMode == 0 && VarGlobal.McSMPrinting.IsSwStart )
                                {
                                    if (!VarGlobal.McSMPrinting.LastPassSignal && VarGlobal.McSMPrinting.IsPassSignal)
                                    {
                                        PLC.Write("M5024", Protocol.VarType.Bit, true);
                                    }
                                    if (!VarGlobal.McSMPrinting.LastOKSignal && VarGlobal.McSMPrinting.IsOKSignal)
                                    {
                                        PLC.Write("M5024", Protocol.VarType.Bit, true);
                                    }
                                }
                                else if (!VarGlobal.McSMPrinting.IsSwStart)
                                {
                                    PLC.Write("M5024", Protocol.VarType.Bit, false);
                                }
                                VarGlobal.McSMPrinting.LastPassSignal = VarGlobal.McSMPrinting.IsPassSignal;
                                VarGlobal.McSMPrinting.LastOKSignal = VarGlobal.McSMPrinting.IsOKSignal;

                                #region READ INFO
                                rx_dd = (UInt16[])PLC.Read("D600", Protocol.VarType.Word, 60);
                                if (rx_dd != null)
                                {
                                    if (rx_dd.Length == 60)
                                    {
                                        UInt16[] rx_temp = new UInt16[10];
                                        Array.Copy(rx_dd, 0, rx_temp, 0, 10);
                                        VarGlobal.McSMPrinting.arrItem[0].ItemName = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[6];
                                        Array.Copy(rx_dd, 10, rx_temp, 0, 6);
                                        VarGlobal.McSMPrinting.arrItem[0].ItemCode = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[4];
                                        Array.Copy(rx_dd, 16, rx_temp, 0, 4);
                                        VarGlobal.McSMPrinting.arrItem[0].ItemLot = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[2];
                                        Array.Copy(rx_dd, 20, rx_temp, 0, 2);
                                        VarGlobal.McSMPrinting.arrItem[0].ItemSeq = Conv.Int2StringCharRevert(rx_temp);

                                        rx_temp = new UInt16[10];
                                        Array.Copy(rx_dd, 30, rx_temp, 0, 10);
                                        VarGlobal.McSMPrinting.arrItem[1].ItemName = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[6];
                                        Array.Copy(rx_dd, 40, rx_temp, 0, 6);
                                        VarGlobal.McSMPrinting.arrItem[1].ItemCode = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[4];
                                        Array.Copy(rx_dd, 46, rx_temp, 0, 4);
                                        VarGlobal.McSMPrinting.arrItem[1].ItemLot = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[2];
                                        Array.Copy(rx_dd, 50, rx_temp, 0, 2);
                                        VarGlobal.McSMPrinting.arrItem[1].ItemSeq = Conv.Int2StringCharRevert(rx_temp);
                                    }
                                }
                                rx_dd = (UInt16[])PLC.Read("D660", Protocol.VarType.Word, 60);
                                if (rx_dd != null)
                                {
                                    if (rx_dd.Length == 60)
                                    {
                                        UInt16[] rx_temp = new UInt16[10];
                                        Array.Copy(rx_dd, 0, rx_temp, 0, 10);
                                        VarGlobal.McSMPrinting.arrItem[2].ItemName = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[6];
                                        Array.Copy(rx_dd, 10, rx_temp, 0, 6);
                                        VarGlobal.McSMPrinting.arrItem[2].ItemCode = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[4];
                                        Array.Copy(rx_dd, 16, rx_temp, 0, 4);
                                        VarGlobal.McSMPrinting.arrItem[2].ItemLot = Conv.Int2StringCharRevert(rx_temp);
                                        rx_temp = new UInt16[2];
                                        Array.Copy(rx_dd, 20, rx_temp, 0, 2);
                                        VarGlobal.McSMPrinting.arrItem[2].ItemSeq = Conv.Int2StringCharRevert(rx_temp);
                                    }
                                }
                                #endregion

                                #region READ PARAMETER
                                McSMPrintingParamCls mcParamTmp = new McSMPrintingParamCls();
                                bool read1 = false;
                                bool read2 = false;
                                bool read3 = false;
                                rx_dd = (UInt16[])PLC.Read("D2000", Protocol.VarType.Word, 30);
                                if (rx_dd != null)
                                {
                                    if (rx_dd.Length == 30)
                                    {
                                        mcParamTmp.PrintingTime = rx_dd[2026 - 2000];
                                        mcParamTmp.StencilAngle = rx_dd[2010 - 2000];
                                        mcParamTmp.SqueezeSpeed = rx_dd[2020 - 2000];
                                        mcParamTmp.ReturnSpeed = rx_dd[2021 - 2000];
                                        mcParamTmp.SqueezePressure = rx_dd[2022 - 2000];
                                        mcParamTmp.SqueezePringtingDistanceStart = rx_dd[2004 - 2000];
                                        mcParamTmp.SqueezePringtingDistanceFinish = rx_dd[2005 - 2000];
                                        mcParamTmp.CoaterPressure = rx_dd[2023 - 2000];
                                        mcParamTmp.Gap = rx_dd[2012 - 2000];
                                        read1 = true;
                                    }
                                }
                                rx_dd = (UInt16[])PLC.Read("D6000", Protocol.VarType.Word, 2);
                                if (rx_dd != null)
                                {
                                    if (rx_dd.Length == 2)
                                    {
                                        mcParamTmp.MachineAirPressure = rx_dd[6000 - 6000];
                                        read2 = true;
                                    }
                                }
                                if(read1 && read2)
                                {
                                    VarGlobal.McSMPrintingParam = mcParamTmp;
                                }
                                #endregion

                            }
                            catch (Exception ex)
                            {
                                isConnected = false;
                            }

                            VarGlobal.McSMPrinting.Cyctime = (int)stoploc.ElapsedMilliseconds;
                        }
                        else isConnected = false;
                    }
                    else
                    {
                        isConnected = false;
                        if (PLC != null)
                        {
                            PLC.Close();
                        }
                    }
                }
                catch (Exception)
                {

                }

                if (!mainThreadStart) break;
                if (isConnected)
                {
                    VarGlobal.McSMPrinting.Status = VarGlobal.E000_CONNECTED;
                    VarGlobal.McSMPrinting.Message = "";
                    VarGlobal.McSMPrinting.IsConnected = true;
                    Thread.Sleep(100);
                }
                else
                {
                    VarGlobal.McSMPrinting.Status = VarGlobal.E001_DISCONNECT;
                    if (!VarGlobal.McSMPrinting.Connection)
                    {
                        VarGlobal.McSMPrinting.Status = VarGlobal.E002_DISABLE;
                    }
                    VarGlobal.McSMPrinting.Message = VarGlobal.E001_DISCONNECT_PLC;
                    VarGlobal.McSMPrinting.IsConnected = false;
                    Thread.Sleep(2000);
                }
            }//End of while true

            if (PLC != null)
            {
                PLC.Close();
            }
        }
        #endregion
    }
}
