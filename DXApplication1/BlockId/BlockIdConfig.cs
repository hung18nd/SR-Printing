using System;
using System.IO;
using DXApplication1.Common;

namespace DXApplication1.BlockId
{
    public class BlockIdConfig
    {
        private static string ConfigPath => Path.Combine(Global.ApplicationPath(), "AppConfig.ini");

        public bool Enable { get; set; } = false;
        public bool EnablePlcKV { get; set; } = true;
        public string ApiUrl { get; set; } = "http://10.126.16.78:660/ingest/v1/events";
        public string ApiKey { get; set; } = "bik_-hpr1fdj4c2bUR2sHy4S_O7efErPebI1YAFkhItSigo";
        public string Source { get; set; } = "urn:blockid:client:sr-baking";
        public int SendMode { get; set; } = 1; // 1: Từng Barcode, 2: Theo Khay
        public bool SendEventTime { get; set; } = false; 

        // Mapping mã thông số 
        public string CodePrintingTime { get; set; } = "prtm";
        public string CodeStencilAngle { get; set; } = "sa";
        public string CodeSqueezeSpeed { get; set; } = "sqsp";
        public string CodeReturnSpeed { get; set; } = "resp";
        public string CodeSqueezePressure { get; set; } = "sqpr";
        public string CodeCoaterPressure { get; set; } = "cp";
        public string CodeSqueezeDistStart { get; set; } = "spd";
        public string CodeSqueezeDistFinish { get; set; } = "spdf";
        public string CodeGap { get; set; } = "gap";
        public string CodeMachineAirPressure { get; set; } = "ma";

        private static BlockIdConfig _instance;
        public static BlockIdConfig Instance => _instance ?? (_instance = Load());

        public static BlockIdConfig Load()
        {
            var cfg = new BlockIdConfig();
            try
            {
                if (File.Exists(ConfigPath))
                {
                    IniFile ini = new IniFile(ConfigPath);
                    cfg.Enable = ini.IniReadValue("BLOCKID", "Enable", "false").ToLower() == "true";
                    cfg.EnablePlcKV = ini.IniReadValue("BLOCKID", "EnablePlcKV", "true").ToLower() == "true";
                    cfg.ApiUrl = ini.IniReadValue("BLOCKID", "ApiUrl", "http://10.126.16.78:660/ingest/v1/events");
                    cfg.ApiKey = ini.IniReadValue("BLOCKID", "ApiKey", "bik_-hpr1fdj4c2bUR2sHy4S_O7efErPebI1YAFkhItSigo");
                    cfg.Source = ini.IniReadValue("BLOCKID", "Source", "urn:blockid:client:sr-baking");
                    cfg.SendMode = int.TryParse(ini.IniReadValue("BLOCKID", "SendMode", "1"), out int mode) ? mode : 1;
                    cfg.SendEventTime = ini.IniReadValue("BLOCKID", "SendEventTime", "false").ToLower() == "true";

                    cfg.CodePrintingTime = ini.IniReadValue("PARAM_CODES", "PrintingTime", "prtm");
                    cfg.CodeStencilAngle = ini.IniReadValue("PARAM_CODES", "StencilAngle", "sa");
                    cfg.CodeSqueezeSpeed = ini.IniReadValue("PARAM_CODES", "SqueezeSpeed", "sqsp");
                    cfg.CodeReturnSpeed = ini.IniReadValue("PARAM_CODES", "ReturnSpeed", "resp");
                    cfg.CodeSqueezePressure = ini.IniReadValue("PARAM_CODES", "SqueezePressure", "sqpr");
                    cfg.CodeCoaterPressure = ini.IniReadValue("PARAM_CODES", "CoaterPressure", "cp");
                    cfg.CodeSqueezeDistStart = ini.IniReadValue("PARAM_CODES", "SqueezeDistStart", "spd");
                    cfg.CodeSqueezeDistFinish = ini.IniReadValue("PARAM_CODES", "SqueezeDistFinish", "spdf");
                    cfg.CodeGap = ini.IniReadValue("PARAM_CODES", "Gap", "gap");
                    cfg.CodeMachineAirPressure = ini.IniReadValue("PARAM_CODES", "MachineAirPressure", "ma");
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[BlockIdConfig.Load Error] {ex.Message}");
            }
            return cfg;
        }

        public void Save()
        {
            try
            {
                IniFile ini = new IniFile(ConfigPath);
                ini.IniWriteValue("COMMON", "MachineID", VarGlobal.McID ?? "");

                ini.IniWriteValue("BLOCKID", "Enable", Enable.ToString().ToLower());
                ini.IniWriteValue("BLOCKID", "EnablePlcKV", EnablePlcKV.ToString().ToLower());
                ini.IniWriteValue("BLOCKID", "ApiUrl", ApiUrl ?? "");
                ini.IniWriteValue("BLOCKID", "ApiKey", ApiKey ?? "");
                ini.IniWriteValue("BLOCKID", "Source", Source ?? "");
                ini.IniWriteValue("BLOCKID", "SendMode", SendMode.ToString());
                ini.IniWriteValue("BLOCKID", "SendEventTime", SendEventTime.ToString().ToLower());

                ini.IniWriteValue("PARAM_CODES", "PrintingTime", CodePrintingTime ?? "");
                ini.IniWriteValue("PARAM_CODES", "StencilAngle", CodeStencilAngle ?? "");
                ini.IniWriteValue("PARAM_CODES", "SqueezeSpeed", CodeSqueezeSpeed ?? "");
                ini.IniWriteValue("PARAM_CODES", "ReturnSpeed", CodeReturnSpeed ?? "");
                ini.IniWriteValue("PARAM_CODES", "SqueezePressure", CodeSqueezePressure ?? "");
                ini.IniWriteValue("PARAM_CODES", "CoaterPressure", CodeCoaterPressure ?? "");
                ini.IniWriteValue("PARAM_CODES", "SqueezeDistStart", CodeSqueezeDistStart ?? "");
                ini.IniWriteValue("PARAM_CODES", "SqueezeDistFinish", CodeSqueezeDistFinish ?? "");
                ini.IniWriteValue("PARAM_CODES", "Gap", CodeGap ?? "");
                ini.IniWriteValue("PARAM_CODES", "MachineAirPressure", CodeMachineAirPressure ?? "");
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[BlockIdConfig.Save Error] {ex.Message}");
            }
        }
    }
}
