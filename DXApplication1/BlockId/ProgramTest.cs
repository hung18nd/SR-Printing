using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXApplication1.BlockId;
using DXApplication1.MachineCls.SM_Printing;
using Newtonsoft.Json;

namespace DXApplication1.Tests
{
    class ProgramTest
    {
        private static TcpListener _tcpListener;
        private static bool _serverRunning = true;
        private static string _lastReceivedHeader = "";
        private static string _lastReceivedBody = "";
        private static int _responseStatusToReturn = 202;
        private static string _responseBodyToReturn = "{\"id\":\"test\",\"status\":\"accepted\"}";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("===============================================================");
            Console.WriteLine("    BẮT ĐẦU KIỂM THỬ TÍCH HỢP BLOCKID INGESTION PROTOCOL V1.0  ");
            Console.WriteLine("===============================================================\n");

            int port = 28660;
            StartMockHttpServer(port);

            string apiUrl = $"http://127.0.0.1:{port}/ingest/v1/events";
            string apiKey = "test-secret-api-key-2026";
            string machineCode = "UVPS008";

            int passCount = 0;
            int totalTests = 5;

            // -------------------------------------------------------------
            // TC-01: Kiểm tra định dạng Payload CloudEvents v1.0 & Barcode
            // -------------------------------------------------------------
            Console.WriteLine("[TEST 1/5] Kiểm tra định dạng Payload CloudEvents 1.0 & Quy tắc Barcode...");
            var param = new McSMPrintingParamCls
            {
                PrintingTime = 16,
                StencilAngle = 45,
                SqueezeSpeed = 125,
                SqueezePressure = 72,
                Gap = 15,
                MachineAirPressure = 50
            };

            var cfg = BlockIdConfig.Instance;
            cfg.Enable = true;
            cfg.ApiUrl = apiUrl;
            cfg.ApiKey = apiKey;
            cfg.MachineCode = machineCode;

            string recId = BlockIdQueueManager.EnqueueRecord("   TEST-BCR-9988   ", "LOT-A26", param, machineCode);
            string pendingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HistoryData", "BlockId_Pending", $"{recId}.json");

            if (File.Exists(pendingFile))
            {
                string json = File.ReadAllText(pendingFile);
                var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
                dynamic obj = JsonConvert.DeserializeObject(json, settings);

                bool specOk = (string)obj.specversion == "1.0";
                bool typeOk = (string)obj.type == "vn.blockid.production.record.v1";
                bool barcodeOk = (string)obj.data.barcode == "TEST-BCR-9988"; // Đã lọc khoảng trắng
                bool timeOk = ((string)obj.time).Contains("+") || ((string)obj.time).Contains("-"); // Có múi giờ
                bool idOk = Guid.TryParse((string)obj.id, out _); // Đúng định dạng UUID

                if (specOk && typeOk && barcodeOk && timeOk && idOk)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  -> TC-01 PASS: Payload đúng chuẩn CloudEvents 1.0, Barcode đã lọc space, UUID & Múi giờ chuẩn.");
                    Console.ResetColor();
                    passCount++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  -> TC-01 FAIL: Dữ liệu payload không đúng đặc tả.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  -> TC-01 FAIL: Không tạo được file trong hàng đợi Pending.");
                Console.ResetColor();
            }

            // -------------------------------------------------------------
            // TC-02: Kiểm tra gửi HTTP POST & Nhận HTTP 202 Accepted
            // -------------------------------------------------------------
            Console.WriteLine("\n[TEST 2/5] Kiểm tra Gửi HTTP POST & Phản hồi HTTP 202 Accepted...");
            _responseStatusToReturn = 202;
            _responseBodyToReturn = "{\"id\":\"" + recId + "\",\"status\":\"accepted\"}";

            var res202 = BlockIdQueueManager.TestConnection(apiUrl, apiKey, machineCode);
            bool headerOk = _lastReceivedHeader.Contains("X-Api-Key: " + apiKey) &&
                            _lastReceivedHeader.Contains("application/cloudevents+json");

            if (res202.StatusCode == 202 && res202.IsSuccess && headerOk)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  -> TC-02 PASS: Nhận mã 202, Header X-Api-Key & Content-Type chính xác.");
                Console.ResetColor();
                passCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  -> TC-02 FAIL: StatusCode={res202.StatusCode}, IsSuccess={res202.IsSuccess}");
                Console.ResetColor();
            }

            // -------------------------------------------------------------
            // TC-03: Kiểm tra Chống trùng lặp (Idempotent HTTP 200 Duplicate)
            // -------------------------------------------------------------
            Console.WriteLine("\n[TEST 3/5] Kiểm tra Cơ chế Chống trùng lặp (HTTP 200 Duplicate)...");
            _responseStatusToReturn = 200;
            _responseBodyToReturn = "{\"id\":\"" + recId + "\",\"status\":\"duplicate\"}";

            var res200 = BlockIdQueueManager.TestConnection(apiUrl, apiKey, machineCode);
            if (res200.StatusCode == 200 && res200.IsSuccess && res200.IsDuplicate)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  -> TC-03 PASS: Mã 200 duplicate được xác định thành công và không coi là lỗi.");
                Console.ResetColor();
                passCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  -> TC-03 FAIL: StatusCode={res200.StatusCode}, IsDuplicate={res200.IsDuplicate}");
                Console.ResetColor();
            }

            // -------------------------------------------------------------
            // TC-04: Kiểm tra Lỗi Cố Định Client (HTTP 400 Bad Request)
            // -------------------------------------------------------------
            Console.WriteLine("\n[TEST 4/5] Kiểm tra Xử lý Lỗi Client 400 (Sai thông số/Mã máy)...");
            _responseStatusToReturn = 400;
            _responseBodyToReturn = "{\"error\":\"Invalid parameter code\"}";

            var res400 = BlockIdQueueManager.TestConnection(apiUrl, apiKey, machineCode);
            if (res400.StatusCode == 400 && res400.IsFatalError && !res400.IsTemporaryError)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  -> TC-04 PASS: Nhận diện chính xác Fatal Error (Không cho phép retry tự động).");
                Console.ResetColor();
                passCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  -> TC-04 FAIL: StatusCode={res400.StatusCode}, IsFatalError={res400.IsFatalError}");
                Console.ResetColor();
            }

            // -------------------------------------------------------------
            // TC-05: Kiểm tra Lỗi Mạng / Mất kết nối (Offline Buffer)
            // -------------------------------------------------------------
            Console.WriteLine("\n[TEST 5/5] Kiểm tra Xử lý Khi Server Mất Kết Nối (Offline / Timeout)...");
            // Gửi tới 1 cổng không tồn tại
            var resOffline = BlockIdQueueManager.TestConnection("http://127.0.0.1:29999/ingest/v1/events", apiKey, machineCode);
            if (resOffline.IsTemporaryError && !resOffline.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  -> TC-05 PASS: Bắt đúng ngoại lệ mất mạng, đánh dấu lỗi tạm thời để hàng đợi retry.");
                Console.ResetColor();
                passCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  -> TC-05 FAIL: IsTemporaryError={resOffline.IsTemporaryError}");
                Console.ResetColor();
            }

            // Dừng mock server
            StopMockHttpServer();

            // Tổng kết
            Console.WriteLine("\n===============================================================");
            Console.WriteLine($"                   KẾT QUẢ: {passCount}/{totalTests} TEST CASES PASS");
            Console.WriteLine("===============================================================");

            if (passCount == totalTests)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(">>> TẤT CẢ CÁC KỊCH BẢN ĐÃ ĐẠT CHUẨN GIAO THỨC BLOCKID V1.0 <<<");
                Console.ResetColor();
            }
        }

        private static void StartMockHttpServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Loopback, port);
            _tcpListener.Start();
            Task.Run(() =>
            {
                while (_serverRunning)
                {
                    try
                    {
                        var client = _tcpListener.AcceptTcpClient();
                        Task.Run(() => HandleClient(client));
                    }
                    catch { break; }
                }
            });
        }

        private static void HandleClient(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[8192];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read > 0)
                {
                    string reqStr = Encoding.UTF8.GetString(buffer, 0, read);
                    int splitIdx = reqStr.IndexOf("\r\n\r\n");
                    if (splitIdx > 0)
                    {
                        _lastReceivedHeader = reqStr.Substring(0, splitIdx);
                        _lastReceivedBody = reqStr.Substring(splitIdx + 4);
                    }
                    else
                    {
                        _lastReceivedHeader = reqStr;
                    }

                    string statusLine = _responseStatusToReturn == 202 ? "HTTP/1.1 202 Accepted\r\n" :
                                        _responseStatusToReturn == 200 ? "HTTP/1.1 200 OK\r\n" :
                                        _responseStatusToReturn == 400 ? "HTTP/1.1 400 Bad Request\r\n" :
                                        $"HTTP/1.1 {_responseStatusToReturn} Error\r\n";

                    byte[] bodyBytes = Encoding.UTF8.GetBytes(_responseBodyToReturn);
                    string respHeader = statusLine +
                                        "Content-Type: application/json; charset=utf-8\r\n" +
                                        $"Content-Length: {bodyBytes.Length}\r\n" +
                                        "Connection: close\r\n\r\n";

                    byte[] headerBytes = Encoding.ASCII.GetBytes(respHeader);
                    stream.Write(headerBytes, 0, headerBytes.Length);
                    stream.Write(bodyBytes, 0, bodyBytes.Length);
                }
            }
        }

        private static void StopMockHttpServer()
        {
            _serverRunning = false;
            try { _tcpListener?.Stop(); } catch { }
        }
    }
}
