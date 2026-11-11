using DXApplication1.Common;
using DXApplication1.MachineCls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXApplication1.MachineCls.SM_Printing;

using System.ComponentModel;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Data;


namespace DXApplication1
{
    public static class VarGlobal
    {
        public static string E000_CONNECTED = "CONNECTED";
        public static string E001_DISCONNECT = "DISCONNECT";
        public static string E002_DISABLE = "DISABLE";
        public static string E001_DISCONNECT_PLC = "[E001] Lose connection PC - PLC";


        public static string McID = "";
        public static string Factory = "F1";
        //public static string Factory = "F2";
        //public static string Factory = "F5";
        //public static string Factory = "FX";

        public static string STRNumber = "0123456789";
        public static string SavePath = "";

        public static string SavePathFTP = "";
        public static string FTPHost = "";
        public static string FTPUser = "";
        public static string FTPPass = "";

        private static bool mainThreadStart = false;

        public static bool ISUploadLog = true;

        public static bool IsConnectedDB = false;
        #region TCP Var

        public static List<ConnCfg> ListConn = new List<ConnCfg>();
        #endregion

        #region Machine Var

        #region SM Printing
        public static McSMPrintingCls McSMPrinting = new McSMPrintingCls() { MachineType = "McSMPrinting", PlcIpKV = "10.126.1.184", PlcIp = "10.126.4.229", PlcPort = 5002, Enable = false, Factory = "F1", MachineID = "PRMP", MachineName = "SM Printing" };
        public static McSMPrintingParamCls McSMPrintingParam = new McSMPrintingParamCls();

        #endregion

        #endregion

        public static void InitAllThreadPollingPLC()
        {
            /////McMicroEtchingNo3.CheckReInput("72019A-00023-012-2",null);
            ////byte[] datas = new byte[] { 0x00, 0x00 };
            ////string ree = Write_MultyDRegSigle_Binary3E("D700",1, datas);
            ////string reeee = Read_DReg_Binary3E("D700",1);
            mainThreadStart = true;

            //if (Factory == "F1")
            //{

            #region machine

            #region SM Printing
            PLCSMPrintingCls.Thread_Init();
            BCRSrx300Cls.Thread_Init();
            BCRCognexCls.Thread_Init();
            VarGlobal.IsConnectedDB = SQLFunc.TestConnection() ? true : false;
            BlockId.BlockIdQueueManager.StartWorker();
            #endregion

            Thread.Sleep(100);
            if (VarGlobal.McSMPrinting.EnablePlcKV)
            {
                ConnCfg conn = new ConnCfg() { Enable = true, MachineType = "PLCKV8000", MachineID = "PLCKV8000", PlcIp = VarGlobal.McSMPrinting.PlcIpKV, PlcPort = VarGlobal.McSMPrinting.PlcPortKV, Factory = "F1", MachineName = "PLC KV" };
                ListConn.Add(conn);
                PLCKV_1_184Cls.Thread_Init(conn);
            }
            #endregion
            // }
        }

        public static void StopAllThreadPollingPLC()
        {
            mainThreadStart = false;
            try
            {
                SQLFunc.StopThreadPollingDB();
                if (VarGlobal.McSMPrinting.EnablePlcKV)
                {
                    PLCKV_1_184Cls.Thread_Release();
                }
                PLCSMPrintingCls.Thread_Release();
                BCRSrx300Cls.Thread_Release();
                BCRCognexCls.Thread_Release();
                BlockId.BlockIdQueueManager.StopWorker();
            }
            catch (Exception)
            {
            }
        }

        public static string Keyence_ExcuteCommand(TcpClient client, string cmd, int timeout = 200)
        {
            string ret = "";
            try
            {
                if (client != null)
                {
                    if (client.Connected)
                    {
                        Stream stream = client.GetStream();
                        byte[] data = Conv.stringToBytes(cmd);
                        stream.Write(data, 0, data.Length);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        while (stopwatch.ElapsedMilliseconds < timeout)
                        {
                            if (client.Available > 0)
                            {
                                data = new byte[client.Available];
                                stream.Read(data, 0, data.Length);
                                ret += Conv.bytesToString(data);
                                if (ret.EndsWith("\n"))
                                {
                                    ret.Replace("\n", "").Replace("\r", "");
                                    break;
                                }
                            }
                            Thread.Sleep(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile(ex.Message);
            }

            return ret;
        }

        #region PLC MESSAGE KEYENCE
        public static string KEYENCE_ReadEMregMessage(int Address, int numbers)
        {
            return $"RDS EM{Address} {numbers}\r\n";
        }
        public static string KEYENCE_ReadZFregMessage(int Address, int numbers)
        {
            return $"RDS ZF{Address} {numbers}\r\n";
        }

        public static string KEYENCE_ReadLregMessage(int Address, int numbers)
        {
            return $"RDS L{Address} {numbers}\r\n";
        }

        public static string KEYENCE_ReadDregMessage(int Address, int numbers)
        {
            return $"RDS D{Address} {numbers}\r\n";
        }
        public static string KEYENCE_ReadWregMessage(string Address, int numbers)
        {
            return $"RDS W{Address} {numbers}\r\n";
        }

        public static string KEYENCE_ReadMregMessage(int Address, int numbers)
        {
            return $"RDS M{Address} {numbers}\r\n";
        }

        public static string KEYENCE_WriteSingleMregMessage(int Address, bool state)
        {
            return $"WR M{Address} {(state ? "1" : "0")}\r\n";
        }
        public static string KEYENCE_WriteMultyRegMessage(string deviceno, int number, int[] arrDatas)
        {
            //WRS DM7150.U 2 16706 17220$0D$0A
            string res = "";
            string data = "";
            for (int i = 0; i < arrDatas.Length; i++)
            {
                if (data.Length > 0) data += " ";
                data += arrDatas[i].ToString();
            }
            res = "WRS " + deviceno + " " + number.ToString() + " " + data + "\r";
            return res;
        }
        public static string KEYENCE_WriteSingleRegMessage(string deviceno, int Data)
        {
            //WRS DM7150.U 2 16706 17220$0D$0A
            string res = "";
            res = "WRS " + deviceno + " " + 1.ToString() + " " + Data + "\r";
            return res;
        }
        #endregion

        public static string SLMP_ExecuteCommand(TcpClient client, string cmd, int timeout = 200)
        {
            string sRX = "";
            Stream stream = client.GetStream();
            byte[] data;
            //Heartbeet:
            //TX: 500000FF03FF00 0019 000014010001M*00300000010 = M3000
            //RX: D00000FF03FF00 0004 0000
            //Debug.Print("[TX] " + cmd); //DEBUG
            data = Conv.HexToBytes(cmd);
            stream.Write(data, 0, data.Length);
            Stopwatch stw = new Stopwatch();
            stw.Restart();
            while (stw.ElapsedMilliseconds < timeout)
            {
                if (client.Available > 0)
                {
                    data = new byte[client.Available];
                    stream.Read(data, 0, data.Length);
                    sRX += Conv.BytesToStringHex(data);

                    if (sRX.Length >= 18)
                    {
                        string strlen = sRX.Substring(14, 4);
                        //int datalen = Conv.atoi32(sRX.Substring(14, 4), 0, 16);
                        int datalen = Conv.SHtoi16($"{strlen[2]}{strlen[3]}{strlen[0]}{strlen[1]}", 0, 16);
                        if (datalen + 18 == sRX.Length)
                        {
                            break;
                        }
                    }
                }
                Thread.Sleep(1);
            }
            //Debug.Print($"[RX] {sRX} [{stw.ElapsedMilliseconds}]ms"); //DEBUG
            if (sRX.Length >= 18) //Tối thiểu chứa
            {
                sRX = sRX.Substring(18);
            }
            else sRX = "";
            return sRX;
        }

        #region PLC MESSAGE 3E Q
        /*
        Device                          ASCII Frame         BINARY Frame
        Special relay ♀                     SM              91
        Special register ♀                  SD              A9
        In                                  X*              9C
        Out                                 Y*              9D
        Internal relay                      M*              90
        Latch relay                         L*              92
        Annunciator                         F*              93
        Edge Relay                          V*              94
        Link relay                          B*              A0
        Data register                       D*              A8
        Link register                       W*              B4
        Timer – contact ♀                   TS              C1
        Timer – coil ♀                      TC              C0
        Timer – current ♀                   TN              C2
        Retentive timer – contact           SS              C7
        Retentive timer – coil              SC              C6
        Retentive timer – current           SN              C8
        Counter – contact ♀                 CS              C4
        Counter – coil ♀                    CC              C3
        Counter – current ♀                 CN              C5
        Special link relay                  SB              A1
        Special link register               SW              B5
        Step relay ♀                        S*              98
        Direct input ♀                      DX              A2
        Direct output ♀                     DY              A3
        Index register ♀                    Z*              CC
        File Register                       R*              AF
                                            ZR              B0
        */

        public static string Read_DReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0000    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0000 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} A8 {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        public static string Read_RReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0000    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0000 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} AF {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        private static string Read_WReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0000    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = address.PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0000 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} B4 {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        private static string Read_ZRReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0000    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0000 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} B0 {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        public static string Read_MReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0100 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} 90 {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }

            return ret;
        }

        public static string Write_MRegSigle_Binary3E(string address, bool val)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = 1.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            string valH = val ? "10" : "00";
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0D00 0000 0114 0100 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} 90 {num[2]}{num[3]}{num[0]}{num[1]} {valH}".Replace(" ", "");
            }

            return ret;
        }
        public static string Write_MultyDRegSigle_Binary3E(string address, int numbers, byte[] datas)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = numbers.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            string valH = "";
            string leng = (datas.Length + 12).ToString("X2").PadLeft(4, '0'); ;
            for (int i = 0; i < datas.Length; i++)
            {
                valH += datas[i].ToString("X2");
            }
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 {leng[2]}{leng[3]}{leng[0]}{leng[1]} 0000 0114 0000 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} A8 {num[2]}{num[3]}{num[0]}{num[1]} {valH}".Replace(" ", "");
            }

            return ret;
        }

        private static string Read_XReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = address.PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0100 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} 9C {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        private static string Read_YReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = address.PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0100 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} 9D {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        private static string Read_LReg_Binary3E(string address, int numreg)
        {   // header  Network No   PC No   Module No     Module station no    Length   CPU timer    Cmd   SubCmd   addreg  Reg  NumReg
            //  5000        00       FF        FF03              00             0C00       0000      0104   0100    401F00   A8   0100
            string ret = "";
            string num = numreg.ToString("X2").PadLeft(4, '0');
            string addr = Conv.atoi32(address).ToString("X2").PadLeft(6, '0');
            if (num.Length == 4 && addr.Length == 6)
            {
                ret = $"5000 00 FF FF03 00 0C00 0000 0104 0100 {addr[4]}{addr[5]}{addr[2]}{addr[3]}{addr[0]}{addr[1]} 92 {num[2]}{num[3]}{num[0]}{num[1]}".Replace(" ", "");
            }
            return ret;
        }
        #endregion


        public static string SLMP_ExecuteCommandADP(TcpClient client, string cmd, int timeout = 200)
        {
            string sRX = "";
            Stream stream = client.GetStream();
            byte[] data;

            //Heartbeet:
            //TX: 500000FF03FF00 0019 000014010001M*00300000010 = M3000
            //RX: D00000FF03FF00 0004 0000
            Debug.Print("[TX] " + cmd); //DEBUG
            data = Conv.HexToBytes(cmd);
            stream.Write(data, 0, data.Length);
            Thread.Sleep(timeout);
            if (client.Available >= 2) //Tối thiểu chứa
            {
                long dataLen;
                data = new byte[client.Available];
                stream.Read(data, 0, data.Length);
                sRX = Conv.ByteArrayToHexString(data);
                //Debug.Print("[RX] " + sRX); //DEBUG               
            }
            return sRX;
        }

        #region PLC MESSAGE ADP FX
        public static string Write_MReg_BinaryADP(int addressStart, bool stt)
        {
            //"02 FF 00 00 94 11 00 00 20 4D 01 00 10".Replace(" ", "");
            string ret = "";
            string str = stt ? "10" : "00";
            string reg = (addressStart & 0xFFFF).ToString("X4").PadLeft(4, '0');
            if (reg.Length == 4)
            {
                reg = $"{reg[2]}{reg[3]}{reg[0]}{reg[1]}";
                ret = $"02FF0000{reg}0000204D0100{str}";
            }

            return ret;
        }
        public static string Read_MReg_BinaryADP(int addressStart, int numberReg)
        {
            // "00 FF 00 00 E8 03 00 00 20 4D 0A 00".Replace(" ", ""); //read M1000, 10 reg
            string ret = "";
            string num = (numberReg & 0XFF).ToString("X2");
            string reg = (addressStart & 0xFFFF).ToString("X4").PadLeft(4, '0');
            if (reg.Length == 4)
            {
                reg = $"{reg[2]}{reg[3]}{reg[0]}{reg[1]}";
                ret = $"00FF0000{reg}0000204D{num}00";
            }
            return ret;
        }
        public static string Read_XReg_BinaryADP(string addressStart, int numberReg)
        {
            // "00 FF 00 00 00 00 00 00 20 58 80 00".Replace(" ", ""); //read X0 128
            string ret = "";
            string num = (numberReg & 0XFF).ToString("X2");
            string reg = addressStart.PadLeft(4, '0');
            if (reg.Length == 4)
            {
                reg = $"{reg[2]}{reg[3]}{reg[0]}{reg[1]}";
                ret = $"00FF0000{reg}00002058{num}00";
            }

            return ret;
        }
        public static string Read_YReg_BinaryADP(string addressStart, int numberReg)
        {
            // "00 FF 00 00 50 00 00 00 20 59 60 00".Replace(" ", ""); //read Y80 96
            string ret = "";
            string num = (numberReg & 0XFF).ToString("X2");
            string reg = addressStart.PadLeft(4, '0').Substring(0, 4);
            if (reg.Length == 4)
            {
                reg = $"{reg[2]}{reg[3]}{reg[0]}{reg[1]}";
                ret = $"00FF0000{reg}00002059{num}00";
            }

            return ret;
        }
        public static string Read_DReg_BinaryADP(int addressStart, int numberReg)
        {
            // "01 FF 00 00 54 0B 00 00 20 44 3C 00".Replace(" ", ""); //read D2900 60
            string ret = "";
            string num = (numberReg & 0XFF).ToString("X2");
            string reg = (addressStart & 0xFFFF).ToString("X4").PadLeft(4, '0');
            if (reg.Length == 4)
            {
                reg = $"{reg[2]}{reg[3]}{reg[0]}{reg[1]}";
                ret = $"01FF0000{reg}00002044{num}00";
            }

            return ret;
        }
        #endregion


        public static TblBoardregistration OutputGetTblBoardregApi(string blockid, string stationID)
        {
            TblBoardregistration res = null;
            string url = @"http://10.126.5.110:8082/RestfulAPI/Boardregistration/GetByBlockID?";
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create($"{url}BlockID={blockid}&APIRequester={stationID}");
            try
            {
                WebReq.Method = "GET";
                WebReq.Timeout = 4000;
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

                if (WebResp.StatusCode == HttpStatusCode.OK)
                {
                    string jsonString = "";
                    using (Stream stream = WebResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                        jsonString = reader.ReadToEnd();
                        Global.WriteLogFile("[API - GetData] - " + "\r\n" + jsonString);
                    }
                    List<TblBoardregistration> items = JsonConvert.DeserializeObject<List<TblBoardregistration>>(jsonString);
                    if (items.Count > 0)
                    {
                        return items[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile("[API - GetData] - " + ex.Message);
            }
            return res;
        }

        public static void SaveBCRLog(string machine, string stext)
        {
            string MyPath = Global.ApplicationPath() + @"\BCRLogs\" + machine + @"\";
            try
            {
                string appendText = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] - ") + stext + "\r\n";
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += DateTime.Now.ToString("yyyyMMdd") + "[" + Process.GetCurrentProcess().ProcessName + "].log";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "Đây là LOG file event \r\n";
                    File.WriteAllText(MyPath, createText);
                }
                File.AppendAllText(MyPath, appendText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        public static void SaveReportLog(string machine, string stext)
        {
            string MyPath = Global.ApplicationPath() + @"\EventsLog\" + machine + @"\";
            try
            {
                string appendText = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] - ") + stext + "\r\n";
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                MyPath += DateTime.Now.ToString("yyyyMMdd") + "[" + Process.GetCurrentProcess().ProcessName + "].log";
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string createText = "Đây là LOG file event \r\n";
                    File.WriteAllText(MyPath, createText);
                }
                File.AppendAllText(MyPath, appendText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        public static void SaveReportLog2(string machine, string text, string text2 = "")
        {
            string MyPath = SavePath;
            string time = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                string MyPath2 = MyPath;
                MyPath += @"\" + "[" + machine + "]" + "_" + time + ".csv";
                string MyFile2 = MyPath2 + @"\" + "[" + machine + "]" + "_" + time + ".txt"; ;
                if (File.Exists(MyPath) == false)
                {
                    // Create a file to write to.
                    string sBuff = "\"NG_Type_S\",\"ItemCd\",\"BatchNo\",\"RegDateTime\",\"NG_Qty\",\"Title\",\"Factory\"\r\n";
                    sBuff += $"{text}";
                    File.WriteAllText(MyPath, sBuff);
                }
                if (File.Exists(MyFile2) == false && text2.Length > 0)
                {
                    File.WriteAllText(MyFile2, text2);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static bool ValidateBlockID(string blockID)
        {
            bool ret = false;
            if (blockID.Length >= 12 && blockID != "ERROR" && blockID != "REORR") ret = true;
            return ret;
        }

        public static bool CheckMixing(string machineID, string factory, string blockID, ItemFromPlus[] arrItem, bool enableMixing, int blockIdIndex = 0)
        {
            bool isMixlot = false;
            string[] arrBlockID = blockID.Split('-');
            string comment = "";
            string itemcode = "";
            string lot = "";
            string reportcmt = "";
            string reportcmt2 = "";



            ItemFromPlus[] arrItemTemp = new ItemFromPlus[3] { new ItemFromPlus(), new ItemFromPlus(), new ItemFromPlus() };
            try
            {
                for (int i = 0; i < arrItemTemp.Length; i++)
                {
                    arrItemTemp[i].ItemCode = arrItem[i].ItemCode;
                    arrItemTemp[i].ItemLot = arrItem[i].ItemLot;
                    arrItemTemp[i].ItemName = arrItem[i].ItemName;
                    arrItemTemp[i].ItemSeq = arrItem[i].ItemSeq;
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile(ex.Message);
            }
            if (blockID.Length >= 12 && blockID != "ERROR" && blockID != "REORR")
            {
                SQLFunc.Mixing_InsertBlockIdHist(machineID, blockID, blockIdIndex);
                //semi
                if (!STRNumber.Contains(blockID[2]))
                {
                    if (false)
                    {
                        itemcode = blockID.Substring(0, 7);
                        lot = blockID.Substring(9, 3);
                        bool isExist = false;
                        bool isNotNullPlusA = false;
                        for (int i = 0; i < arrItemTemp.Length; i++)
                        {
                            if (arrItemTemp[i].ItemCode != "" && arrItemTemp[i].ItemCode.Length >= 12)
                            {
                                isNotNullPlusA = true;
                                if (arrItemTemp[i].ItemCode.StartsWith(itemcode) && arrItemTemp[i].ItemLot.StartsWith(lot))
                                {

                                    //// neu la lot cut thi kiem tra o board registration
                                    //if (arrItemTemp[i].ItemLot.Contains("-"))
                                    //{
                                    //    TblBoardregistration tblAPI = OutputGetTblBoardregApi(blockID, "SERVER_CHECKMIX");
                                    //    if (tblAPI != null)
                                    //    {
                                    //        if ($"{arrItemTemp[i].ItemCode}-{arrItemTemp[i].ItemLot}" != tblAPI.IndicationNumber)
                                    //        {
                                    //            isMixlot = true;
                                    //            comment = $"[MIXING] BlockID: {blockID}, Item From PlusA ({i + 1})[{arrItemTemp[i].ItemCode};{arrItemTemp[i].ItemLot}], Data from API [{tblAPI.IndicationNumber}]";
                                    //            reportcmt = $"\"M100\",\"{arrItemTemp[i].ItemCode}\",\"{arrItemTemp[i].ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Lot Mixing (MachineID: {machineID})(BlockID: {blockID})\",\"{factory}\"\r\n";
                                    //            reportcmt2 = $"{machineID},{arrItemTemp[i].ItemCode},{arrItemTemp[i].ItemLot},{arrItemTemp[i].ItemSeq},{blockID},1,0,{(enableMixing ? "1" : "0")},0\r\n";
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        isMixlot = true;
                                    //        comment = $"[MIXING] BlockID: {blockID}, Item From PlusA ({i + 1})[{arrItemTemp[i].ItemCode};{arrItemTemp[i].ItemLot}], Cannot get data from API(result NULL)";
                                    //    }
                                    //}
                                    isExist = true;
                                    break;
                                }
                            }
                        }
                        if (!isExist && isNotNullPlusA)
                        {
                            for (int i = 0; i < arrItemTemp.Length; i++)
                            {
                                if (arrItemTemp[i].ItemCode != "" && arrItemTemp[i].ItemCode.Length >= 12 && arrItemTemp[i].ItemLot != "")
                                {
                                    isMixlot = true;
                                    comment = $"[MIXING] BlockID: {blockID}, {GetMessRpt(arrItemTemp)}";
                                    reportcmt = $"\"M100\",\"{arrItemTemp[i].ItemCode}\",\"{arrItemTemp[i].ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Lot Mixing (MachineID: {machineID})(BlockID: {blockID})\",\"{factory}\"\r\n";
                                    reportcmt2 = $"{machineID},{arrItemTemp[i].ItemCode},{arrItemTemp[i].ItemLot},{arrItemTemp[i].ItemSeq},{blockID},1,0,{(enableMixing ? "1" : "0")},0\r\n";
                                    break;
                                }
                            }
                        }
                    }

                }
                //mass
                else
                {
                    itemcode = blockID.Substring(0, 6);
                    lot = blockID.Substring(7, 5);
                    bool isExist = false;
                    bool isNotNullPlusA = false;
                    for (int i = 0; i < arrItemTemp.Length; i++)
                    {
                        if (arrItemTemp[i].ItemCode != "" && arrItemTemp[i].ItemCode.Length >= 12)
                        {
                            isNotNullPlusA = true;
                            if (arrItemTemp[i].ItemCode.StartsWith(itemcode) && arrItemTemp[i].ItemLot.StartsWith(lot))
                            {
                                //// neu la lot cut thi kiem tra o board registration
                                //if (arrItemTemp[i].ItemLot.Contains("-"))
                                //{
                                //    TblBoardregistration tblAPI = OutputGetTblBoardregApi(blockID, "SERVER_CHECKMIX");
                                //    if (tblAPI != null)
                                //    {
                                //        if ($"{arrItemTemp[i].ItemCode}{arrItemTemp[i].ItemLot}" != tblAPI.IndicationNumber && !$"{arrItemTemp[i].ItemCode}{arrItemTemp[i].ItemLot}".Contains(tblAPI.IndicationNumber))
                                //        {
                                //            isMixlot = true;
                                //            comment = $"[MIXING] BlockID: {blockID}, Item From PlusA ({i + 1})[{arrItemTemp[i].ItemCode};{arrItemTemp[i].ItemLot}], Data from API [{tblAPI.IndicationNumber}]";
                                //            reportcmt = $"\"M100\",\"{arrItemTemp[i].ItemCode}\",\"{arrItemTemp[i].ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Lot Mixing (MachineID: {machineID})(BlockID: {blockID})\",\"{factory}\"\r\n";
                                //            reportcmt2 = $"{machineID},{arrItemTemp[i].ItemCode},{arrItemTemp[i].ItemLot},{arrItemTemp[i].ItemSeq},{blockID},1,0,{(enableMixing ? "1" : "0")},0\r\n";
                                //        }
                                //    }
                                //    else
                                //    {
                                //        isMixlot = true;
                                //        comment = $"[MIXING] BlockID: {blockID}, Item From PlusA ({i + 1})[{arrItemTemp[i].ItemCode};{arrItemTemp[i].ItemLot}], Cannot get data from API(result NULL)";
                                //    }
                                //}
                                isExist = true;
                                break;
                            }
                        }
                    }
                    if (!isExist && isNotNullPlusA)
                    {
                        for (int i = 0; i < arrItemTemp.Length; i++)
                        {
                            if (arrItemTemp[i].ItemCode != "" && arrItemTemp[i].ItemCode.Length >= 12 && arrItemTemp[i].ItemLot != "")
                            {
                                isMixlot = true;
                                comment = $"[MIXING] BlockID: {blockID}, {GetMessRpt(arrItemTemp)}";
                                reportcmt = $"\"M100\",\"{arrItemTemp[i].ItemCode}\",\"{arrItemTemp[i].ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Lot Mixing (MachineID: {machineID})(BlockID: {blockID})\",\"{factory}\"\r\n";
                                reportcmt2 = $"{machineID},{arrItemTemp[i].ItemCode},{arrItemTemp[i].ItemLot},{arrItemTemp[i].ItemSeq},{blockID},1,0,{(enableMixing ? "1" : "0")},0\r\n";
                                break;
                            }
                        }
                    }
                }
                //if (comment != "")
                //{
                //    SaveReportLog(machineID, comment);
                //    SaveReportLog2(machineID, reportcmt, reportcmt2);
                //}
            }
            //else if (blockID.Length > 0 && blockID != "ERROR" && blockID != "REORR")
            //{
            //    for (int i = 0; i < arrItemTemp.Length; i++)
            //    {
            //        if (arrItemTemp[i].ItemCode != "" && arrItemTemp[i].ItemLot != "")
            //        {
            //            isMixlot = true;
            //            comment = $"[MIXING] BlockID: {blockID}, {GetMessRpt(arrItemTemp)}";
            //            reportcmt = $"\"M100\",\"{arrItemTemp[i].ItemCode}\",\"{arrItemTemp[i].ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Lot Mixing (MachineID: {machineID})(BlockID: {blockID})\",\"{factory}\"\r\n";
            //            reportcmt2 = $"{machineID},{arrItemTemp[i].ItemCode},{arrItemTemp[i].ItemLot},{arrItemTemp[i].ItemSeq},{blockID},1,0\r\n";
            //            break;
            //        }
            //    }
            //    if (comment != "")
            //    {
            //        SaveReportLog(machineID, comment);
            //        SaveReportLog2(machineID, reportcmt, reportcmt2);
            //    }
            //}
            return isMixlot;
        }

        public static string GetMessRpt(ItemFromPlus[] arrItem)
        {
            string res = "";
            for (int i = 0; i < arrItem.Length; i++)
            {
                if (res.Length > 0) res += ",";
                res += $"Item From PlusA ({i + 1})[{arrItem[i].ItemCode};{arrItem[i].ItemLot}]";

            }
            return res;
        }

        public static bool SaveLogFTP()
        {
            bool ret = false;
            try
            {
                var url = "ftp://ftp.example.com/remote/path/file.zip";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Credentials = new NetworkCredential("username", "password");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream fileStream = File.OpenRead(@"C:\local\path\file.zip"))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }

        private static void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
        {
            foreach (AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = FTPPass;
                }
            }
        }
        public static bool FileUploadSFTP(string filepath, string filename)
        {
            bool ret = false;
            var host = FTPHost;
            var port = 22;
            var username = FTPUser;
            var password = FTPPass;

            // path for file you want to upload
            var uploadFile = filepath;

            try
            {
                KeyboardInteractiveAuthenticationMethod keybAuth = new
                KeyboardInteractiveAuthenticationMethod(username);
                keybAuth.AuthenticationPrompt += new
                    EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);

                ConnectionInfo coninfo = new ConnectionInfo(host, port, username, keybAuth);
                using (var client = new SftpClient(coninfo))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        //Debug.WriteLine("I'm connected to the client");
                        client.ChangeDirectory(SavePathFTP);
                        using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                        {
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            client.UploadFile(fileStream, filename);
                            ret = true;
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("I couldn't connect");
                    }
                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"Copy file SFTP error: {FTPHost} {username} {FTPPass} {SavePathFTP}" + ex.Message);
            }

            return ret;
        }
        public static bool FileUploadSFTPUpdate(string filepath, string filename)
        {
            bool ret = false;
            var host = FTPHost;
            var port = 22;
            var username = FTPUser;
            var password = FTPPass;

            // path for file you want to upload
            var uploadFile = filepath;

            try
            {
                var authMethod = new PasswordAuthenticationMethod(FTPUser, FTPPass);

                ConnectionInfo coninfo = new ConnectionInfo(host, port, username, authMethod);
                using (var client = new SftpClient(coninfo))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        //Debug.WriteLine("I'm connected to the client");
                        client.ChangeDirectory(SavePathFTP);
                        using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                        {
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            client.UploadFile(fileStream, filename);
                            ret = true;
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("I couldn't connect");
                    }
                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"Copy file SFTP error: {FTPHost} {username} {FTPPass} {SavePathFTP}" + ex.Message);
            }

            return ret;
        }
    }
}
