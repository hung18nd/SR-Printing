using System;
using System.IO;
using DXApplication1.Common;

namespace DXApplication1.BlockId
{
    public class BlockIdConfig
    {
        private static string ConfigPath => Path.Combine(Global.ApplicationPath(), "BlockIdConfig.ini");

        public bool Enable { get; set; } = false;
        public bool EnablePlcKV { get; set; } = true;
        public string ApiUrl { get; set; } = "http://10.126.1.245:660/ingest/v1/events";
        public string ApiKey { get; set; } = "";
        public string MachineCode { get; set; } = "";
        public string Source { get; set; } = "urn:blockid:client:sr-baking";
        public int SendMode { get; set; } = 1; // 1: Từng Barcode, 2: Theo Khay

        // Mapping mã thông số do BlockId cấp
        public string CodePrintingTime { get; set; } = "PRINT_TIME";
        public string CodeStencilAngle { get; set; } = "STENCIL_ANGLE";
        public string CodeSqueezeSpeed { get; set; } = "SQUEEZE_SPEED";
        public string CodeReturnSpeed { get; set; } = "RETURN_SPEED";
        public string CodeSqueezePressure { get; set; } = "SQUEEZE_PRESS";
        public string CodeCoaterPressure { get; set; } = "COATER_PRESS";
        public string CodeSqueezeDistStart { get; set; } = "SQ_DIST_START";
        public string CodeSqueezeDistFinish { get; set; } = "SQ_DIST_FINISH";
        public string CodeGap { get; set; } = "GAP";
        public string CodeMachineAirPressure { get; set; } = "AIR_PRESS";

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
                    cfg.ApiUrl = ini.IniReadValue("BLOCKID", "ApiUrl", "http://10.126.1.245:660/ingest/v1/events");
                    cfg.ApiKey = ini.IniReadValue("BLOCKID", "ApiKey", "");
                    cfg.MachineCode = ini.IniReadValue("BLOCKID", "MachineCode", "");
                    cfg.Source = ini.IniReadValue("BLOCKID", "Source", "urn:blockid:client:sr-baking");
                    cfg.SendMode = int.TryParse(ini.IniReadValue("BLOCKID", "SendMode", "1"), out int mode) ? mode : 1;

                    cfg.CodePrintingTime = ini.IniReadValue("PARAM_CODES", "PrintingTime", "PRINT_TIME");
                    cfg.CodeStencilAngle = ini.IniReadValue("PARAM_CODES", "StencilAngle", "STENCIL_ANGLE");
                    cfg.CodeSqueezeSpeed = ini.IniReadValue("PARAM_CODES", "SqueezeSpeed", "SQUEEZE_SPEED");
                    cfg.CodeReturnSpeed = ini.IniReadValue("PARAM_CODES", "ReturnSpeed", "RETURN_SPEED");
                    cfg.CodeSqueezePressure = ini.IniReadValue("PARAM_CODES", "SqueezePressure", "SQUEEZE_PRESS");
                    cfg.CodeCoaterPressure = ini.IniReadValue("PARAM_CODES", "CoaterPressure", "COATER_PRESS");
                    cfg.CodeSqueezeDistStart = ini.IniReadValue("PARAM_CODES", "SqueezeDistStart", "SQ_DIST_START");
                    cfg.CodeSqueezeDistFinish = ini.IniReadValue("PARAM_CODES", "SqueezeDistFinish", "SQ_DIST_FINISH");
                    cfg.CodeGap = ini.IniReadValue("PARAM_CODES", "Gap", "GAP");
                    cfg.CodeMachineAirPressure = ini.IniReadValue("PARAM_CODES", "MachineAirPressure", "AIR_PRESS");
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
                ini.IniWriteValue("BLOCKID", "Enable", Enable.ToString().ToLower());
                ini.IniWriteValue("BLOCKID", "EnablePlcKV", EnablePlcKV.ToString().ToLower());
                ini.IniWriteValue("BLOCKID", "ApiUrl", ApiUrl ?? "");
                ini.IniWriteValue("BLOCKID", "ApiKey", ApiKey ?? "");
                ini.IniWriteValue("BLOCKID", "MachineCode", MachineCode ?? "");
                ini.IniWriteValue("BLOCKID", "Source", Source ?? "");
                ini.IniWriteValue("BLOCKID", "SendMode", SendMode.ToString());

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
