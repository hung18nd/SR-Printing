using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Cognex.DataMan.SDK.Utils;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK;
using System.Xml;

namespace DXApplication1.MachineCls.SM_Printing
{
    class BCRCognexCls
    {
        private static Thread ThreadPolling;
        private static bool mainThreadStart = false;

        private static ResultCollector _results;
        private static SynchronizationContext _syncContext = null;
        private static EthSystemDiscoverer _ethSystemDiscoverer = null;
        private static SerSystemDiscoverer _serSystemDiscoverer = null;
        private static ISystemConnector _connector = null;
        private static DataManSystem _system = null;
        private static object _currentResultInfoSyncLock = new object();
        private static bool checkConnect = false;
        private static string DataBarcode1Read = "";
        public static void Thread_Init()
        {
            mainThreadStart = true;
            ThreadPolling = new Thread(() => ThreadPolling_Process());
            ThreadPolling.Start();
        }
        public static void Thread_Release()
        {
            mainThreadStart = false;
        }

        private static string lst_blockid = "";
        public static string LastBlockID
        {
            get { return lst_blockid; }
            set { lst_blockid = value; }
        }

                #region TEST CONNECTION
        public static bool TestConnection()
        {
            return TestConnection(VarGlobal.McSMPrinting.BcrCognexIp, 23);
        }

        public static bool TestConnection(string ip, int port = 23)
        {
            bool bCommOK = false;
            try
            {
                if (string.IsNullOrEmpty(ip)) return false;

                IPAddress ipAddress;
                if (!IPAddress.TryParse(ip, out ipAddress))
                {
                    return false;
                }

                
                using (TcpClient tcpClient = new TcpClient())
                {
                    IAsyncResult ar = tcpClient.BeginConnect(ipAddress, port, null, null);
                    bool waitSuccess = ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                    if (!waitSuccess)
                    {
                        return false;
                    }
                    tcpClient.EndConnect(ar);
                }

               
                EthSystemConnector conn = new EthSystemConnector(ipAddress, port);
                conn.UserName = "admin";
                conn.Password = "";
                using (DataManSystem sys = new DataManSystem(conn))
                {
                    sys.DefaultTimeout = 2000;
                    sys.Connect(2000);
                    if (sys.State == ConnectionState.Connected)
                    {
                        DmccResponse resp = sys.SendCommand("GET DEVICE.NAME", 1000);
                        if (resp != null)
                        {
                            bCommOK = true;
                        }
                        sys.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile("[BCRCognex TestConnection] - " + ex.Message);
                bCommOK = false;
            }
            return bCommOK;
        }
        #endregion

        #region PLC PROCESS
        private static void OnSystemConnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                 delegate
                 {
                     checkConnect = true;
                 },
                 null);
        }
        private static void OnSystemDisconnected(object sender, EventArgs args)
        {
            _syncContext.Post(
                delegate
                {
                    checkConnect = false;
                },
                null);
        }
        private static string GetReadStringFromResultXml(string resultXml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(resultXml);

                XmlNode full_string_node = doc.SelectSingleNode("result/general/full_string");

                if (full_string_node != null)
                {
                    XmlAttribute encoding = full_string_node.Attributes["encoding"];
                    if (encoding != null && encoding.InnerText == "base64")
                    {
                        byte[] code = Convert.FromBase64String(full_string_node.InnerText);
                        return _system.Encoding.GetString(code, 0, code.Length);
                    }

                    return full_string_node.InnerText;
                }
            }
            catch
            {
            }

            return "";
        }
        static void Results_ComplexResultArrived(object sender, ResultInfo e)
        {
            _syncContext.Post(
            delegate
            {
                ShowResult(e);

            },
            null);
        }

        private static void ShowResult(ResultInfo e)
        {
            string read_result = null;

            // Take a reference or copy values from the locked result info object. This is done
            // so that the lock is used only for a short period of time.
            lock (_currentResultInfoSyncLock)
            {
                read_result = !String.IsNullOrEmpty(e.ReadString) ? e.ReadString : GetReadStringFromResultXml(e.XmlResult);
            }
            if (read_result != null)
            {
                DataBarcode1Read = read_result.Replace("\r","").Replace("\n", "");
            }
        }

        static void Results_PartialResultDropped(object sender, ResultInfo e)
        {
            _syncContext.Post(
                delegate
                {
                    if (e.SubResults != null)
                    {
                        foreach (var sub_result in e.SubResults)
                        {
                            ReportDroppedResult(sub_result);
                        }
                    }

                    ReportDroppedResult(e);
                },
                null);
        }
        static void ReportDroppedResult(ResultInfo e)
        {
            List<string> dropped = new List<string>();

            if (e.ReadString != null) dropped.Add(String.Format("read string (ResultId={0})", e.ResultId));
            if (e.XmlResult != null) dropped.Add(String.Format("xml result (ResultId={0})", e.ResultId));

        }
        private static void CleanupConnection()
        {
            if (_system != null)
            {
                _system.SystemConnected -= OnSystemConnected;
                _system.SystemDisconnected -= OnSystemDisconnected;
            }

            _connector = null;
            _system = null;
        }
        private static void ConnectCognex()
        {
            _syncContext = new SynchronizationContext();
            EthSystemConnector conn = new EthSystemConnector(IPAddress.Parse(VarGlobal.McSMPrinting.BcrCognexIp), 23);
            conn.UserName = "admin";
            conn.Password = "";
            _connector = conn;
            _system = new DataManSystem(_connector);
            _system.DefaultTimeout = 5000;
            _system.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
            _system.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);
            ResultTypes requested_result_types = ResultTypes.ReadXml;
            _results = new ResultCollector(_system, requested_result_types);
            _results.ComplexResultArrived += new ComplexResultArrivedEventHandler(Results_ComplexResultArrived);
            _results.PartialResultDropped += new PartialResultDroppedEventHandler(Results_PartialResultDropped);
            _system.SetKeepAliveOptions(false, 3000, 1000);
            _system.Connect(3000);

            _system.SetResultTypes(requested_result_types);
            _system.SendCommand("SET TRIGGER.TYPE 0", 300);
        }
        private static void ThreadPolling_Process()
        {
            Stopwatch stoploc = new Stopwatch();
            try
            {
                ConnectCognex();
            }
            catch (Exception ex)
            {

            }

            bool isConnected = false;
            
            while (mainThreadStart)
            {
                try
                {
                                        if (_system == null || _system.State != ConnectionState.Connected)
                    {
                        ConnectCognex();
                    }

                    if (_system != null && _system.State == ConnectionState.Connected && checkConnect)
                    {
                      
                        DmccResponse rxTest = _system.SendCommand("GET DEVICE.NAME", 500);
                        if (rxTest != null)
                        {
                            isConnected = true;
                        }
                        else
                        {
                            isConnected = false;
                        }
                    }
                    else
                    {
                        isConnected = false;
                    }

                    if (isConnected)
                    {
                        if (VarGlobal.McSMPrinting.IsTrigger && VarGlobal.McSMPrinting.BcrSelect == 1)
                        {
                            DmccResponse res = _system.SendCommand("TRIGGER ON", 500);
                            //string blockid = Conv.atos(res.PayLoad).Replace("\r","").Replace("\n", "");
                            string blockid = DataBarcode1Read;

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
                                _system.SendCommand("TRIGGER OFF", 200);
                                DataBarcode1Read = "";
                                lst_blockid = blockid;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }

                if (!mainThreadStart) break;
                if (isConnected)
                {
                    VarGlobal.McSMPrinting.IsConnectedCognex = true;
                    Thread.Sleep(1000);
                }
                else
                {
                    VarGlobal.McSMPrinting.IsConnectedCognex = false;
                    Thread.Sleep(2000);
                }
            }//End of while true
            if (_system != null && _system.State == ConnectionState.Connected)
            {
                _system.Disconnect();

                CleanupConnection();

                _results.ClearCachedResults();
                _results = null;
            }    
        }
        #endregion
    }
}
