using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXApplication1.MachineCls
{
    class PLCKV_1_184Cls
    {
        private static Thread ThreadPollingPLCKV;
        private static TcpClient PLCKV = new TcpClient();
        private static bool mainThreadStart = false;


        public static void Thread_Init(ConnCfg conn)
        {
            mainThreadStart = true;
            ThreadPollingPLCKV = new Thread(() => ThreadPollingPLCKV_Process(conn));
            ThreadPollingPLCKV.Start();
        }
        public static void Thread_Release()
        {
            mainThreadStart = false;
        }

        #region PLCKV PROCESS
        private static void ThreadPollingPLCKV_Process(ConnCfg conn)
        {
            Stopwatch stoploc = new Stopwatch();

            bool isConnected = false;
            long x = 0;
            string[] sRX;
            string[] rxTemp = new string[0];
            DateTime lstDateTime2SendBlock = DateTime.Now;
            DateTime lstDateTime2SendParam = DateTime.Now;
            while (mainThreadStart)
            {
                if (!VarGlobal.McSMPrinting.EnablePlcKV)
                {
                    isConnected = false;
                    VarGlobal.McSMPrinting.IsConnectedPlcKV = false;
                    try
                    {
                        if (PLCKV != null && PLCKV.Client != null) PLCKV.Close();
                    }
                    catch { }
                    Thread.Sleep(1000);
                    continue;
                }
                try
                {
                    if (!isConnected)
                    {
                        try
                        {
                            if (PLCKV.Client != null)
                            {
                                PLCKV.Close();
                            }
                            PLCKV = new TcpClient();
                            PLCKV.Connect(VarGlobal.McSMPrinting.PlcIpKV, VarGlobal.McSMPrinting.PlcPortKV);
                            if (PLCKV.Connected) isConnected = true;
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (PLCKV.Connected)
                    {
                        stoploc.Restart();

                        string[] rxDatas = VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_ReadDregMessage(1000,2)).Split(' ');
                        if (rxDatas.Length == 2)
                        {
                            isConnected = true;
                        }
                        else isConnected = false;

                        if (lstDateTime2SendBlock.AddSeconds(2) < DateTime.Now)
                        {
                            if (VarGlobal.McSMPrinting.BlockID2KVList.Count > 0)
                            {
                                bool isSend = false;
                                foreach (BlockID2KV block in VarGlobal.McSMPrinting.BlockID2KVList.ToList())
                                {
                                    if (!block.IsSync)
                                    {
                                        if (!isSend)
                                        {
                                            int[] arrTx = Conv.StringChar2Int(block.BlockID, 10);
                                            string rxData = VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteMultyRegMessage(VarGlobal.McSMPrinting.RegBlockID, arrTx.Length, arrTx));
                                            if (rxData.Contains("OK"))
                                            {
                                                block.IsSync = true;
                                                isSend = true;
                                                lstDateTime2SendBlock = DateTime.Now;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        VarGlobal.McSMPrinting.BlockID2KVList.Remove(block);
                                    }
                                }

                            }
                        }
                        if (lstDateTime2SendParam.AddSeconds(2) < DateTime.Now)
                        {
                            if (VarGlobal.McSMPrinting.IsConnected)
                            {
                                if(VarGlobal.McSMPrinting.RegPrintingTime != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegPrintingTime, VarGlobal.McSMPrintingParam.PrintingTime));
                                if (VarGlobal.McSMPrinting.RegStencilAngle != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegStencilAngle, VarGlobal.McSMPrintingParam.StencilAngle));
                                if (VarGlobal.McSMPrinting.RegSqueezeSpeed != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegSqueezeSpeed, VarGlobal.McSMPrintingParam.SqueezeSpeed));
                                if (VarGlobal.McSMPrinting.RegReturnSpeed != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegReturnSpeed, VarGlobal.McSMPrintingParam.ReturnSpeed));
                                if (VarGlobal.McSMPrinting.RegSqueezePressure != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegSqueezePressure, VarGlobal.McSMPrintingParam.SqueezePressure));
                                if (VarGlobal.McSMPrinting.RegCoaterPressure != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegCoaterPressure, VarGlobal.McSMPrintingParam.CoaterPressure));
                                if (VarGlobal.McSMPrinting.RegSqueezePringtingDistanceStart != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegSqueezePringtingDistanceStart, VarGlobal.McSMPrintingParam.SqueezePringtingDistanceStart));
                                if (VarGlobal.McSMPrinting.RegSqueezePringtingDistanceFinish != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegSqueezePringtingDistanceFinish, VarGlobal.McSMPrintingParam.SqueezePringtingDistanceFinish));
                                if (VarGlobal.McSMPrinting.RegGap != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegGap, VarGlobal.McSMPrintingParam.Gap));
                                if (VarGlobal.McSMPrinting.RegMachineAirPressure != "") VarGlobal.Keyence_ExcuteCommand(PLCKV, VarGlobal.KEYENCE_WriteSingleRegMessage(VarGlobal.McSMPrinting.RegMachineAirPressure, VarGlobal.McSMPrintingParam.MachineAirPressure));
                                lstDateTime2SendParam = DateTime.Now;
                            }
                            
                        }

                        conn.Cyctime = (int)stoploc.ElapsedMilliseconds;
                    }
                }
                catch (Exception)
                {

                }

                if (!mainThreadStart) break;
                if (isConnected)
                {
                    conn.IsConnected = true;
                    conn.IsOnline = true;
                    conn.Status = "CONNECTED";
                    conn.ErrorCode = "";
                    conn.Message = "";
                    VarGlobal.McSMPrinting.IsConnectedPlcKV = true;

                    Thread.Sleep(100);
                }
                else
                {
                    conn.IsConnected = false;
                    conn.IsOnline = false;
                    conn.ErrorCode = VarGlobal.E001_DISCONNECT_PLC;
                    conn.Status = VarGlobal.E001_DISCONNECT;
                    VarGlobal.McSMPrinting.IsConnectedPlcKV = false;
                    Thread.Sleep(2000);
                }
            }//End of while true

            if (PLCKV != null)
            {
                if (PLCKV.Client != null)
                {
                    PLCKV.Close();
                }
            }
        }
        #endregion
    }
}
