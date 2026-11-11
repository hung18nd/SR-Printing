using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCProtocol;
using Keyence.AutoID.SDK;

namespace DXApplication1.MachineCls.SM_Printing
{
    class BCRSrx300Cls
    {
        private static Thread ThreadPolling;
        private static ReaderAccessor m_reader = new ReaderAccessor();
        private static bool mainThreadStart = false;

        public static void Thread_Init()
        {
            mainThreadStart = true;
            ThreadPolling = new Thread(() => ThreadPolling_Process());
            ThreadPolling.IsBackground = true;
            ThreadPolling.Start();
        }
        public static void Thread_Release()
        {
            mainThreadStart = false;
            try
            {
                if (m_reader != null) m_reader.Disconnect();
            }
            catch { }
            try
            {
                if (ThreadPolling != null && ThreadPolling.IsAlive)
                {
                    ThreadPolling.Abort();
                }
            }
            catch { }
        }

        private static string lst_blockid = "";
        public static  string LastBlockID 
        {
            get { return lst_blockid; }
            set { lst_blockid = value; }
        }

                #region TEST CONNECTION
        public static bool TestConnection()
        {
            return TestConnection(VarGlobal.McSMPrinting.BcrSrx300Ip, VarGlobal.McSMPrinting.BcrSrx300Port);
        }

        public static bool TestConnection(string ip, int port)
        {
            bool bCommOK = false;
            try
            {
                if (string.IsNullOrEmpty(ip) || port <= 0) return false;

                using (ReaderAccessor testReader = new ReaderAccessor())
                {
                    testReader.IpAddress = ip;
                    testReader.DataPort = port;
                    testReader.Connect();
                    string rxData = testReader.ExecCommand("KEYENCE\r", 1500);
                    if (rxData != null && rxData.Contains("OK"))
                    {
                        bCommOK = true;
                    }
                    testReader.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile("[BCRSrx300 TestConnection] - " + ex.Message);
                bCommOK = false;
            }
            return bCommOK;
        }
        #endregion

        #region PLC PROCESS
        private static void ThreadPolling_Process()
        {
            Stopwatch stoploc = new Stopwatch();

            bool isConnected = false;
            m_reader.IpAddress = VarGlobal.McSMPrinting.BcrSrx300Ip;
            m_reader.DataPort = VarGlobal.McSMPrinting.BcrSrx300Port;
            
            DateTime LastTriggerTime = Global.Y2K_VALUE;
            while (mainThreadStart)
            {
                try
                {
                    if (!isConnected)
                    {
                        m_reader.Connect();
                    }
                    string rxData = m_reader.ExecCommand("KEYENCE\r");
                    if (rxData.Contains("OK"))
                    {
                        isConnected = true;
                    }
                    else isConnected = false;
                    if (isConnected)
                    {
                        if(VarGlobal.McSMPrinting.IsTrigger && VarGlobal.McSMPrinting.BcrSelect == 0)
                        {
                            if (LastTriggerTime.AddSeconds(2) < DateTime.Now)
                            {
                                string blockid = m_reader.ExecCommand("LON\r", 500).Replace("\r", "").Replace("\n", "");
                                if (blockid.Contains('-') && lst_blockid != blockid)
                                {
                                    for (int i = 0; i < VarGlobal.McSMPrinting.BlockNumPerPrints; i++)
                                    {
                                        if (blockid == VarGlobal.McSMPrinting.BlockIDs[i].BlockID) break;
                                        if (VarGlobal.McSMPrinting.BlockIDs[i].BlockID == "")
                                        {
                                            VarGlobal.McSMPrinting.Counter++;
                                            VarGlobal.McSMPrinting.BlockIDs[i].BlockID = blockid;
                                            break;
                                        }
                                    }

                                    lst_blockid = blockid;
                                }
                                LastTriggerTime = DateTime.Now;
                            }
                            
                        }    
                       
                    }
                }
                catch (Exception)
                {

                }

                if (!mainThreadStart) break;
                if (isConnected)
                {
                    VarGlobal.McSMPrinting.IsConnectedSRX = true;
                    Thread.Sleep(200);
                }
                else
                {
                    VarGlobal.McSMPrinting.IsConnectedSRX = false;
                    Thread.Sleep(2000);
                }
            }//End of while true
            if(m_reader != null)
            {
                m_reader.Disconnect();
            }
        }
        #endregion
    }
}
