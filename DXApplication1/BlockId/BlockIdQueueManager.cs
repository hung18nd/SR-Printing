using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DXApplication1.Common;
using DXApplication1.MachineCls.SM_Printing;
using Newtonsoft.Json;

namespace DXApplication1.BlockId
{
    public class BlockIdQueueManager
    {
        private static readonly object _lockObj = new object();
        private static Thread _workerThread;
        private static bool _isRunning = false;

        public static int PendingCount { get; private set; } = 0;
        public static string LastStatus { get; private set; } = "IDLE";
        public static string LastError { get; private set; } = "";
        public static DateTime LastSendTime { get; private set; } = DateTime.MinValue;

        public static event Action<string, string> OnBlockIdAlert; // Message, Severity

        private static string PendingFolder => Path.Combine(Global.ApplicationPath(), "HistoryData", "BlockId_Pending");
        private static string SuccessFolder => Path.Combine(Global.ApplicationPath(), "HistoryData", "BlockId_Success");
        private static string ErrorFolder => Path.Combine(Global.ApplicationPath(), "HistoryData", "BlockId_Error");

        public static void StartWorker()
        {
            if (_isRunning) return;
            _isRunning = true;
            _workerThread = new Thread(WorkerProcess)
            {
                IsBackground = true,
                Name = "BlockIdWorkerThread"
            };
            _workerThread.Start();
        }

        public static void StopWorker()
        {
            _isRunning = false;
        }

        private static void EnsureDirectories()
        {
            try
            {
                if (!Directory.Exists(PendingFolder)) Directory.CreateDirectory(PendingFolder);
                if (!Directory.Exists(SuccessFolder)) Directory.CreateDirectory(SuccessFolder);
                if (!Directory.Exists(ErrorFolder)) Directory.CreateDirectory(ErrorFolder);
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[BlockIdQueueManager.EnsureDirectories] {ex.Message}");
            }
        }

        #region ENQUEUE METHODS

        /// <summary>
        /// Tạo và lưu bản ghi từng Barcode (Cách 1) vào hàng đợi đệm
        /// </summary>
        public static string EnqueueRecord(string barcode, string lot, McSMPrintingParamCls param, string machineId = null, DateTimeOffset? prodTime = null)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;

            EnsureDirectories();
            var config = BlockIdConfig.Instance;
            if (VarGlobal.McSMPrinting.EnablePlcKV || !config.Enable) return null;

            string cleanBarcode = barcode.Trim().Replace(" ", "");
            if (cleanBarcode.Length > 128) cleanBarcode = cleanBarcode.Substring(0, 128);

            string recordId = Guid.NewGuid().ToString();
            DateTimeOffset time = prodTime ?? DateTimeOffset.Now;

            string subject = !string.IsNullOrEmpty(config.MachineCode) ? config.MachineCode : (machineId ?? "LINE01");

            var cloudEvent = new BlockIdCloudEvent<BlockIdRecordData>
            {
                SpecVersion = "1.0",
                Type = "vn.blockid.production.record.v1",
                Id = recordId,
                Source = config.Source,
                Subject = subject,
                Time = time.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
                Data = new BlockIdRecordData
                {
                    Barcode = cleanBarcode,
                    Lot = string.IsNullOrEmpty(lot) ? null : (lot.Length > 64 ? lot.Substring(0, 64) : lot),
                    Values = BuildParamValues(param, config)
                }
            };

            string json = JsonConvert.SerializeObject(cloudEvent, Formatting.Indented);
            string filePath = Path.Combine(PendingFolder, $"{recordId}.json");

            lock (_lockObj)
            {
                File.WriteAllText(filePath, json);
            }

            return recordId;
        }

        /// <summary>
        /// Tạo và lưu bản ghi theo Khay/Xe (Cách 2) vào hàng đợi đệm
        /// </summary>
        public static string EnqueueTray(string trayId, string lot, List<string> barcodes, McSMPrintingParamCls param, string machineId = null, DateTimeOffset? prodTime = null)
        {
            if (barcodes == null || barcodes.Count == 0) return null;

            EnsureDirectories();
            var config = BlockIdConfig.Instance;
            if (VarGlobal.McSMPrinting.EnablePlcKV || !config.Enable) return null;

            string recordId = Guid.NewGuid().ToString();
            DateTimeOffset time = prodTime ?? DateTimeOffset.Now;

            var cleanBarcodes = barcodes
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b.Trim().Replace(" ", ""))
                .Take(1000)
                .ToList();

            if (cleanBarcodes.Count == 0) return null;

            string subject = !string.IsNullOrEmpty(config.MachineCode) ? config.MachineCode : (machineId ?? "LINE01");

            var cloudEvent = new BlockIdCloudEvent<BlockIdTrayData>
            {
                SpecVersion = "1.0",
                Type = "vn.blockid.production.tray.v1",
                Id = recordId,
                Source = config.Source,
                Subject = subject,
                Time = time.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
                Data = new BlockIdTrayData
                {
                    TrayId = string.IsNullOrEmpty(trayId) ? null : trayId.Trim(),
                    Lot = string.IsNullOrEmpty(lot) ? null : lot.Trim(),
                    Barcodes = cleanBarcodes,
                    Values = BuildParamValues(param, config)
                }
            };

            string json = JsonConvert.SerializeObject(cloudEvent, Formatting.Indented);
            string filePath = Path.Combine(PendingFolder, $"{recordId}.json");

            lock (_lockObj)
            {
                File.WriteAllText(filePath, json);
            }

            return recordId;
        }

        private static List<BlockIdParamValue> BuildParamValues(McSMPrintingParamCls param, BlockIdConfig config)
        {
            var list = new List<BlockIdParamValue>();
            if (param == null) return list;

            if (!string.IsNullOrEmpty(config.CodePrintingTime))
                list.Add(new BlockIdParamValue(config.CodePrintingTime, param.PrintingTime));

            if (!string.IsNullOrEmpty(config.CodeStencilAngle))
                list.Add(new BlockIdParamValue(config.CodeStencilAngle, param.StencilAngle));

            if (!string.IsNullOrEmpty(config.CodeSqueezeSpeed))
                list.Add(new BlockIdParamValue(config.CodeSqueezeSpeed, param.SqueezeSpeed));

            if (!string.IsNullOrEmpty(config.CodeReturnSpeed))
                list.Add(new BlockIdParamValue(config.CodeReturnSpeed, param.ReturnSpeed));

            if (!string.IsNullOrEmpty(config.CodeSqueezePressure))
                list.Add(new BlockIdParamValue(config.CodeSqueezePressure, param.SqueezePressure));

            if (!string.IsNullOrEmpty(config.CodeCoaterPressure))
                list.Add(new BlockIdParamValue(config.CodeCoaterPressure, param.CoaterPressure));

            if (!string.IsNullOrEmpty(config.CodeSqueezeDistStart))
                list.Add(new BlockIdParamValue(config.CodeSqueezeDistStart, param.SqueezePringtingDistanceStart));

            if (!string.IsNullOrEmpty(config.CodeSqueezeDistFinish))
                list.Add(new BlockIdParamValue(config.CodeSqueezeDistFinish, param.SqueezePringtingDistanceFinish));

            if (!string.IsNullOrEmpty(config.CodeGap))
                list.Add(new BlockIdParamValue(config.CodeGap, param.Gap));

            if (!string.IsNullOrEmpty(config.CodeMachineAirPressure))
                list.Add(new BlockIdParamValue(config.CodeMachineAirPressure, param.MachineAirPressure));

            return list;
        }

        #endregion

        #region BACKGROUND WORKER

        private static void WorkerProcess()
        {
            int retryDelaySec = 1;

            while (_isRunning)
            {
                try
                {
                    EnsureDirectories();
                    var config = BlockIdConfig.Instance;
                    if (VarGlobal.McSMPrinting.EnablePlcKV || !config.Enable || string.IsNullOrEmpty(config.ApiUrl))
                    {
                        LastStatus = VarGlobal.McSMPrinting.EnablePlcKV ? "KV ACTIVE" : "NOT USED";
                        Thread.Sleep(1000);
                        continue;
                    }

                    DirectoryInfo dir = new DirectoryInfo(PendingFolder);
                    FileInfo[] files = dir.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                                          .OrderBy(f => f.CreationTime)
                                          .ToArray();

                    PendingCount = files.Length;

                    if (files.Length == 0)
                    {
                        LastStatus = "IDLE";
                        retryDelaySec = 1;
                        Thread.Sleep(500);
                        continue;
                    }

                    foreach (var file in files)
                    {
                        if (!_isRunning) break;

                        string jsonPayload = "";
                        lock (_lockObj)
                        {
                            if (File.Exists(file.FullName))
                            {
                                jsonPayload = File.ReadAllText(file.FullName);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(jsonPayload))
                        {
                            file.Delete();
                            continue;
                        }

                        // Gửi HTTP POST lên BlockId
                        LastStatus = "SENDING";
                        var result = BlockIdApiClient.SendEvent(config.ApiUrl, config.ApiKey, jsonPayload);

                        if (result.IsSuccess)
                        {
                            // Thành công (202 hoặc 200 duplicate) -> Xóa khỏi pending, chuyển sang success
                            retryDelaySec = 1; // Reset backoff
                            LastStatus = result.IsDuplicate ? "SUCCESS (DUPLICATE)" : "SUCCESS (202)";
                            LastSendTime = DateTime.Now;
                            LastError = "";

                            string dateFolder = Path.Combine(SuccessFolder, DateTime.Now.ToString("yyyy-MM-dd"));
                            if (!Directory.Exists(dateFolder)) Directory.CreateDirectory(dateFolder);
                            string destFile = Path.Combine(dateFolder, file.Name);

                            lock (_lockObj)
                            {
                                if (File.Exists(destFile)) File.Delete(destFile);
                                File.Move(file.FullName, destFile);
                            }

                            PendingCount = Math.Max(0, PendingCount - 1);
                        }
                        else if (result.IsFatalError)
                        {
                            // Lỗi 400 - 404: Dừng retry tự động cho file này, chuyển sang thư mục Error
                            LastStatus = $"FATAL ERROR ({result.StatusCode})";
                            LastError = result.ErrorMessage;
                            Global.WriteLogFile($"[BlockId Fatal Error] File: {file.Name} - Code: {result.StatusCode} - {result.ErrorMessage}");

                            string errFile = Path.Combine(ErrorFolder, file.Name);
                            lock (_lockObj)
                            {
                                if (File.Exists(errFile)) File.Delete(errFile);
                                File.Move(file.FullName, errFile);
                            }

                            PendingCount = Math.Max(0, PendingCount - 1);

                            // Alert UI
                            OnBlockIdAlert?.Invoke($"BlockId API returned error {result.StatusCode}: {result.ErrorMessage}. Please verify configuration!", "ERROR");
                        }
                        else
                        {
                            // Lỗi tạm thời (429, 5xx, mất mạng, timeout > 10s):
                            // Tuân thủ quy tắc 8.3: Chờ rồi gửi lại, thời gian chờ tăng dần 1, 2, 4, 8, 16...
                            LastStatus = $"RETRYING in {retryDelaySec}s ({result.StatusCode})";
                            LastError = result.ErrorMessage;

                            Thread.Sleep(retryDelaySec * 1000);
                            retryDelaySec = Math.Min(retryDelaySec * 2, 60); // Tăng gấp đôi, tối đa 60s
                            break; // Dừng vòng lặp file hiện tại để thử lại từ đầu hàng đợi
                        }
                    }
                }
                catch (Exception ex)
                {
                    LastStatus = "ERROR";
                    LastError = ex.Message;
                    Global.WriteLogFile($"[BlockId WorkerProcess Exception] {ex.Message}");
                    Thread.Sleep(2000);
                }
            }
        }

        #endregion

        #region TEST METHOD

        /// <summary>
        /// Gửi ngay một bản ghi test không qua pending queue để kiểm tra kết nối API
        /// </summary>
        public static BlockIdApiResult TestConnection(string url, string apiKey, string machineCode)
        {
            var config = BlockIdConfig.Instance;
            string testRecordId = Guid.NewGuid().ToString();

            var sampleEvent = new BlockIdCloudEvent<BlockIdRecordData>
            {
                SpecVersion = "1.0",
                Type = "vn.blockid.production.record.v1",
                Id = testRecordId,
                Source = config.Source,
                Subject = !string.IsNullOrEmpty(machineCode) ? machineCode : "TEST_LINE01",
                Time = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture),
                Data = new BlockIdRecordData
                {
                    Barcode = "TEST_SAMPLE_001",
                    Lot = "LOT_TEST",
                    Values = new List<BlockIdParamValue>
                    {
                        new BlockIdParamValue(config.CodePrintingTime, 15),
                        new BlockIdParamValue(config.CodeSqueezeSpeed, 120),
                        new BlockIdParamValue(config.CodeSqueezePressure, 0.45)
                    }
                }
            };

            string json = JsonConvert.SerializeObject(sampleEvent, Formatting.Indented);
            return BlockIdApiClient.SendEvent(url, apiKey, json);
        }

        #endregion
    }
}
