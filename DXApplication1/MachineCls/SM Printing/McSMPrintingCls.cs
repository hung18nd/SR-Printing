using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.MachineCls.SM_Printing
{
    public class McSMPrintingCls : MachineInfo
    {
        [Category("4.Datas")]
        public string[] ParamHeaders { get; set; } = new string[] { "sp", "spa", "t1", "t2", "t3", "t4", "t5", "t6", "t7", "t8", "t9", "t10", "t11", "t12", "t13", "t14", "t15", "t16", "t17", "t18", "t19", "t20", "t21", "t22", "c1", "c2" };
        [Category("4.Datas")]
        public string[] ParamValues { get; set; } = new string[1000];
        [Category("4.Datas")]
        [TypeConverter(typeof(ArrayConverter))]
        public BlockIDDatas[] BlockIDs { get; set; } = new BlockIDDatas[8] { new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas(), new BlockIDDatas() };
        [Category("4.Datas")]
        public int LastBcrIndex { get; set; } = 0;
        [Category("4.Datas")]
        [TypeConverter(typeof(ArrayConverter))]
        public ItemFromPlus[] arrItem { get; set; } = new ItemFromPlus[3] { new ItemFromPlus(), new ItemFromPlus(), new ItemFromPlus() };
        public bool IsNewBcr { get; set; } = false;

        public bool IsTrigger { get; set; } = false;

        public bool IsSwStart { get; set; } = false;

        public bool IsMcReadyStart { get; set; } = false;

        public bool IsMcStart { get; set; } = false;
        public bool IsLastMcStart { get; set; } = false;

        public bool IsOKSignal { get; set; } = false;
        public bool LastOKSignal { get; set; } = false;

        public bool IsPassSignal { get; set; } = false;
        public bool LastPassSignal { get; set; } = false;

        public int Counter { get; set; } = 0;

        public void Read_Config()
        {
            string iniFile = Global.ApplicationPath() + @"\AppConfig" + $@"\{MachineID}.ini";
            IniFile iniFile1 = new IniFile(iniFile);
            string temp = iniFile1.IniReadValue("PARAM", "IsEnable", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IsEnable", Enable ? "1" : "0");
            Enable = iniFile1.IniReadValue("PARAM", "IsEnable", "") == "1";

            temp = iniFile1.IniReadValue("PARAM", "IpPLCMachine", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IpPLCMachine", PlcIp);
            PlcIp = iniFile1.IniReadValue("PARAM", "IpPLCMachine", "");

            temp = iniFile1.IniReadValue("PARAM", "PortPLCMachine", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "PortPLCMachine", PlcPort.ToString());
            PlcPort = Conv.atoi32(iniFile1.IniReadValue("PARAM", "PortPLCMachine", ""));

            temp = iniFile1.IniReadValue("PARAM", "IpPLCBlock", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IpPLCBlock", PlcIpKV);
            PlcIpKV = iniFile1.IniReadValue("PARAM", "IpPLCBlock", "");

            temp = iniFile1.IniReadValue("PARAM", "PortPLCBlock", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "PortPLCBlock", PlcPortKV.ToString());
            PlcPortKV = Conv.atoi32(iniFile1.IniReadValue("PARAM", "PortPLCBlock", ""));

            temp = iniFile1.IniReadValue("PARAM", "IsEnablePlcKV", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IsEnablePlcKV", EnablePlcKV ? "1" : "0");
            EnablePlcKV = iniFile1.IniReadValue("PARAM", "IsEnablePlcKV", "") == "1";

            temp = iniFile1.IniReadValue("PARAM", "IsEnablePlcKV", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IsEnablePlcKV", EnablePlcKV ? "1" : "0");
            EnablePlcKV = iniFile1.IniReadValue("PARAM", "IsEnablePlcKV", "") == "1";

            temp = iniFile1.IniReadValue("PARAM", "IsEnableMixing", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IsEnableMixing", EnableMixing ? "1" : "0");
            EnableMixing = iniFile1.IniReadValue("PARAM", "IsEnableMixing", "") == "1";

            temp = iniFile1.IniReadValue("PARAM", "IsEnableReinput", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "IsEnableReinput", EnableReinput ? "1" : "0");
            EnableReinput = iniFile1.IniReadValue("PARAM", "IsEnableReinput", "") == "1";


            temp = iniFile1.IniReadValue("PARAM", "BcrSrx300Ip", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrSrx300Ip", BcrSrx300Ip);
            BcrSrx300Ip = iniFile1.IniReadValue("PARAM", "BcrSrx300Ip", "");
            temp = iniFile1.IniReadValue("PARAM", "BcrSrx300Port", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrSrx300Port", BcrSrx300Port.ToString());
            BcrSrx300Port = Conv.atoi32(iniFile1.IniReadValue("PARAM", "BcrSrx300Port", ""));

            temp = iniFile1.IniReadValue("PARAM", "BcrCognexIp", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrCognexIp", BcrCognexIp);
            BcrCognexIp = iniFile1.IniReadValue("PARAM", "BcrCognexIp", "");
            temp = iniFile1.IniReadValue("PARAM", "BcrCognexPort", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrCognexPort", BcrCognexPort.ToString());
            BcrCognexPort = Conv.atoi32(iniFile1.IniReadValue("PARAM", "BcrCognexPort", ""));


            temp = iniFile1.IniReadValue("PARAM", "NumBlockPerPrint", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "NumBlockPerPrint", "2");
            BlockNumPerPrints = Conv.atoi32(iniFile1.IniReadValue("PARAM", "NumBlockPerPrint", "2"));
            temp = iniFile1.IniReadValue("PARAM", "BcrMode", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrMode", "1");
            BcrMode = Conv.atoi32(iniFile1.IniReadValue("PARAM", "BcrMode", "1"));
            temp = iniFile1.IniReadValue("PARAM", "BcrSelect", "");
            if (temp == "") iniFile1.IniWriteValue("PARAM", "BcrSelect", "0");
            BcrSelect = Conv.atoi32(iniFile1.IniReadValue("PARAM", "BcrSelect", "0"));


            //------------------------------------------------------------------------
            temp = iniFile1.IniReadValue("REG", "PrintingTime", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "PrintingTime", "EM18006");
            RegPrintingTime = iniFile1.IniReadValue("REG", "PrintingTime", "");

            temp = iniFile1.IniReadValue("REG", "StencilAngle", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "StencilAngle", "EM17990");
            RegStencilAngle = iniFile1.IniReadValue("REG", "StencilAngle", "");

            temp = iniFile1.IniReadValue("REG", "SqueezeSpeed", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "SqueezeSpeed", "EM18000");
            RegSqueezeSpeed = iniFile1.IniReadValue("REG", "SqueezeSpeed", "");

            temp = iniFile1.IniReadValue("REG", "SqueezePressure", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "SqueezePressure", "EM18001");
            RegSqueezePressure = iniFile1.IniReadValue("REG", "SqueezePressure", "");

            temp = iniFile1.IniReadValue("REG", "CoaterPressure", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "CoaterPressure", "EM18002");
            RegCoaterPressure = iniFile1.IniReadValue("REG", "CoaterPressure", "");

            temp = iniFile1.IniReadValue("REG", "SqueezePringtingDistanceStart", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "SqueezePringtingDistanceStart", "EM18003");
            RegSqueezePringtingDistanceStart = iniFile1.IniReadValue("REG", "SqueezePringtingDistanceStart", "");

            temp = iniFile1.IniReadValue("REG", "SqueezePringtingDistanceFinish", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "SqueezePringtingDistanceFinish", "EM17984");
            RegSqueezePringtingDistanceFinish = iniFile1.IniReadValue("REG", "SqueezePringtingDistanceFinish", "");

            temp = iniFile1.IniReadValue("REG", "Gap", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "Gap", "EM17985");
            RegGap = iniFile1.IniReadValue("REG", "Gap", "");

            temp = iniFile1.IniReadValue("REG", "MachineAirPressure", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "MachineAirPressure", "EM18100");
            RegMachineAirPressure = iniFile1.IniReadValue("REG", "MachineAirPressure", "");

            temp = iniFile1.IniReadValue("REG", "RegReturnSpeed", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "RegReturnSpeed", "EM18100");
            RegReturnSpeed = iniFile1.IniReadValue("REG", "RegReturnSpeed", "");

            temp = iniFile1.IniReadValue("REG", "BlockID", "");
            if (temp == "") iniFile1.IniWriteValue("REG", "BlockID", "W2130");
            RegBlockID = iniFile1.IniReadValue("REG", "BlockID", "");

        }
        public void Write_Config()
        {
            string iniFile = Global.ApplicationPath() + @"\AppConfig" + $@"\{MachineID}.ini";
            IniFile iniFile1 = new IniFile(iniFile);
            iniFile1.IniWriteValue("PARAM", "IsEnable", Enable ? "1" : "0");
            iniFile1.IniWriteValue("PARAM", "IpPLCMachine", PlcIp);
            iniFile1.IniWriteValue("PARAM", "PortPLCMachine", PlcPort.ToString());
            iniFile1.IniWriteValue("PARAM", "IpPLCBlock", PlcIpKV);
            iniFile1.IniWriteValue("PARAM", "PortPLCBlock", PlcPortKV.ToString());
            iniFile1.IniWriteValue("PARAM", "IsEnablePlcKV", EnablePlcKV ? "1" : "0");
            iniFile1.IniWriteValue("PARAM", "IsEnableMixing", EnableMixing ? "1" : "0");
            iniFile1.IniWriteValue("PARAM", "IsEnableReinput", EnableReinput ? "1" : "0");
        }


        public void Read_LastIdxBcr()
        {
            IniFile iniFile = new IniFile(Global.ApplicationPath() + @"\HistoryData" + $@"\{MachineID}.ini");
            LastBcrIndex = Conv.atoi32(iniFile.IniReadValue("HISTORY", $"LastBcrIndex", ""));
        }
        public void Read_LastBlockIDs()
        {
            IniFile iniFile = new IniFile(Global.ApplicationPath() + @"\HistoryData" + $@"\{MachineID}.ini");
            for (int i = 0; i < BlockIDs.Length; i++)
            {
                BlockIDs[i].LastBlockID = iniFile.IniReadValue("BLOCKID_DATA_1", $"BlockID{i}", "");
                BlockIDs[i].LastBlockIDIndex = Conv.atoi32(iniFile.IniReadValue("BLOCKID_DATA_1", $"BlockIDIndex{i}", ""));
                BlockIDs[i].BlockIDIndex = BlockIDs[i].LastBlockIDIndex;
            }
        }
        public void Save_LastBlockIDs()
        {
            string MyPath = Global.ApplicationPath() + @"\HistoryData";
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                IniFile iniFile = new IniFile(MyPath + @"\" + MachineID + ".ini");
                for (int i = 0; i < BlockIDs.Length; i++)
                {
                    iniFile.IniWriteValue("BLOCKID_DATA_1", $"BlockID{i}", BlockIDs[i].LastBlockID);
                    iniFile.IniWriteValue("BLOCKID_DATA_1", $"BlockIDIndex{i}", BlockIDs[i].LastBlockIDIndex.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Save_LastIdxBcr()
        {
            string MyPath = Global.ApplicationPath() + @"\HistoryData";
            try
            {
                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                IniFile iniFile = new IniFile(MyPath + @"\" + MachineID + ".ini");
                iniFile.IniWriteValue("HISTORY", "LastBcrIndex", LastBcrIndex.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        public bool CheckReInput(string blockID, ItemFromPlus itemFromPlus, string lastBlockID = "")
        {
            bool ret = false;
            bool isNeedCheck = false;
            if (VarGlobal.ValidateBlockID(blockID))
            {
                isNeedCheck = true;
                if (isNeedCheck)
                {
                    string strQuery = $"SELECT [blockId],[printing_time] FROM [dbo].[Tbl_Printing_Baking_Mapping] WHERE [blockId] = '{blockID}' AND [printing_seq] = '{itemFromPlus.ItemSeq}'";
                    DataTable dtab = SQLFunc.ExecuteDataTable(strQuery);
                    if (dtab.Columns.Count > 0)
                    {
                        if (dtab.Rows.Count > 0)
                        {
                            return true;
                            //DateTime dateTime = Conv.atodt(dtab.Rows[0]["printing_time"]);
                            //if (dateTime.AddSeconds(10) < DateTime.Now)
                            //{
                            //    //string reportcmt = $"\"M100\",\"{itemFromPlus.ItemCode}\",\"{itemFromPlus.ItemLot}\",\"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\",1,\"FPC Re-Input (MachineID: {MachineID})(BlockID: {blockID})\",\"{Factory}\"\r\n";
                            //    //string reportcmt2 = $"{MachineID},{itemFromPlus.ItemCode},{itemFromPlus.ItemLot},{itemFromPlus.ItemSeq},{blockID},0,1,,{(EnableReinput ? "1" : "0")}\r\n";
                            //    //VarGlobal.SaveReportLog2(MachineID, reportcmt, reportcmt2);
                                
                            //}
                        }
                    }
                }
            }

            return ret;
        }
    }
}
