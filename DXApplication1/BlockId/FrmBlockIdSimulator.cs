using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXApplication1.Common;
using DXApplication1.MachineCls.SM_Printing;
using Newtonsoft.Json;

namespace DXApplication1.BlockId
{
    public class FrmBlockIdSimulator : DevExpress.XtraEditors.XtraForm
    {
        private System.ComponentModel.IContainer components = null;

        // UI Controls - Server Section
        private Label lblServerStatus;
        private Button btnStartServer;
        private Button btnStopServer;
        private ComboBox cbbServerBehavior;
        private NumericUpDown numServerPort;
        private DataGridView dgrReceivedEvents;

        // UI Controls - Simulation Section
        private Button btnSimulateSingle;
        private Button btnSimulateBatch10;
        private Button btnSimulateCutNetwork;
        private CheckBox chkAutoRun;
        private System.Windows.Forms.Timer timerAutoRun;
        private Label lblSimulationStats;

        // UI Controls - Connect
        private Button btnApplyUrlToConfig;

        // Internal Mock Server
        private TcpListener _mockListener;
        private bool _isServerRunning = false;
        private readonly HashSet<string> _receivedUuids = new HashSet<string>();
        private int _totalReceived = 0;
        private int _totalDuplicates = 0;
        private int _simulatedCount = 0;

        public FrmBlockIdSimulator()
        {
            InitializeComponent();
            SetupDataGrid();
            InitAutoRunTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Bộ Mô Phỏng Sản Xuất & Máy Chủ Ảo BlockId (BlockId Realtime Simulator)";
            this.Size = new Size(960, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1. GroupBox Máy chủ BlockId Ảo
            var grpServer = new GroupBox
            {
                Text = "1. Máy Chủ BlockId Ảo (Mock API Server)",
                Location = new Point(12, 10),
                Size = new Size(920, 140)
            };

            var lblPort = new Label { Text = "Port:", Location = new Point(15, 28), AutoSize = true };
            numServerPort = new NumericUpDown { Location = new Point(55, 25), Width = 75, Minimum = 1000, Maximum = 65535, Value = 18660 };

            btnStartServer = new Button { Text = "BẬT Server Ảo", Location = new Point(140, 23), Size = new Size(110, 28), BackColor = Color.LightGreen };
            btnStartServer.Click += (s, e) => StartMockServer((int)numServerPort.Value);

            btnStopServer = new Button { Text = "TẮT Server", Location = new Point(255, 23), Size = new Size(90, 28), Enabled = false };
            btnStopServer.Click += (s, e) => StopMockServer();

            btnApplyUrlToConfig = new Button { Text = "Trỏ App vào Server Ảo này", Location = new Point(355, 23), Size = new Size(180, 28) };
            btnApplyUrlToConfig.Click += BtnApplyUrlToConfig_Click;

            lblServerStatus = new Label { Text = "Server: ĐANG TẮT", Location = new Point(550, 28), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), ForeColor = Color.Red };

            var lblBehavior = new Label { Text = "Kịch bản phản hồi của Server Ảo:", Location = new Point(15, 68), AutoSize = true };
            cbbServerBehavior = new ComboBox { Location = new Point(200, 65), Width = 450, DropDownStyle = ComboBoxStyle.DropDownList };
            cbbServerBehavior.Items.Add("1. Bình thường (202 Accepted cho UUID mới, 200 Duplicate nếu gửi lại)");
            cbbServerBehavior.Items.Add("2. Mất mạng / Treo Timeout (> 10s)");
            cbbServerBehavior.Items.Add("3. Quá tải (HTTP 429 Too Many Requests)");
            cbbServerBehavior.Items.Add("4. Máy chủ lỗi (HTTP 500 Internal Server Error)");
            cbbServerBehavior.Items.Add("5. Lỗi Client (HTTP 400 Bad Request - Sai mã thông số)");
            cbbServerBehavior.Items.Add("6. Lỗi bảo mật (HTTP 401 Unauthorized - Sai API Key)");
            cbbServerBehavior.SelectedIndex = 0;

            grpServer.Controls.Add(lblPort);
            grpServer.Controls.Add(numServerPort);
            grpServer.Controls.Add(btnStartServer);
            grpServer.Controls.Add(btnStopServer);
            grpServer.Controls.Add(btnApplyUrlToConfig);
            grpServer.Controls.Add(lblServerStatus);
            grpServer.Controls.Add(lblBehavior);
            grpServer.Controls.Add(cbbServerBehavior);

            // 2. GroupBox Giả lập Dây Chuyền Sản Xuất
            var grpSimulation = new GroupBox
            {
                Text = "2. Giả Lập Chu Trình Dập In & Barcode Thực Tế (Machine Simulator)",
                Location = new Point(12, 160),
                Size = new Size(920, 95)
            };

            btnSimulateSingle = new Button { Text = "Mô phỏng dập 1 tấm (Single Shot)", Location = new Point(15, 25), Size = new Size(210, 32), Font = new Font(this.Font, FontStyle.Bold) };
            btnSimulateSingle.Click += (s, e) => SimulateProductionRun(1);

            btnSimulateBatch10 = new Button { Text = "Mô phỏng dập 10 tấm liên tục", Location = new Point(235, 25), Size = new Size(200, 32) };
            btnSimulateBatch10.Click += (s, e) => SimulateProductionRun(10);

            chkAutoRun = new CheckBox { Text = "Tự động dập mỗi 3 giây (Auto Run)", Location = new Point(450, 30), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            chkAutoRun.CheckedChanged += ChkAutoRun_CheckedChanged;

            btnSimulateCutNetwork = new Button { Text = "Rút cáp mạng (Tắt Server)", Location = new Point(710, 25), Size = new Size(190, 32), BackColor = Color.MistyRose };
            btnSimulateCutNetwork.Click += (s, e) => StopMockServer();

            lblSimulationStats = new Label { Text = "Số tấm đã giả lập: 0 | Nhận thành công: 0 | Trùng lặp: 0 | Đang chờ trong Queue: 0", Location = new Point(15, 68), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            grpSimulation.Controls.Add(btnSimulateSingle);
            grpSimulation.Controls.Add(btnSimulateBatch10);
            grpSimulation.Controls.Add(chkAutoRun);
            grpSimulation.Controls.Add(btnSimulateCutNetwork);
            grpSimulation.Controls.Add(lblSimulationStats);

            // 3. DataGrid hiển thị dữ liệu nhận được thời gian thực
            var lblGrid = new Label { Text = "3. Dữ liệu thời gian thực máy chủ BlockId nhận được:", Location = new Point(12, 265), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            dgrReceivedEvents = new DataGridView
            {
                Location = new Point(12, 285),
                Size = new Size(920, 345),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            this.Controls.Add(grpServer);
            this.Controls.Add(grpSimulation);
            this.Controls.Add(lblGrid);
            this.Controls.Add(dgrReceivedEvents);

            this.FormClosing += FrmBlockIdSimulator_FormClosing;
        }

        private void SetupDataGrid()
        {
            dgrReceivedEvents.Columns.Add("colTime", "Thời gian nhận");
            dgrReceivedEvents.Columns.Add("colId", "UUID (id)");
            dgrReceivedEvents.Columns.Add("colBarcode", "Mã Barcode");
            dgrReceivedEvents.Columns.Add("colSubject", "Mã máy");
            dgrReceivedEvents.Columns.Add("colParams", "Thông số (Values)");
            dgrReceivedEvents.Columns.Add("colStatus", "Kết quả trả về");

            dgrReceivedEvents.Columns["colTime"].Width = 130;
            dgrReceivedEvents.Columns["colId"].Width = 220;
            dgrReceivedEvents.Columns["colBarcode"].Width = 140;
            dgrReceivedEvents.Columns["colSubject"].Width = 90;
            dgrReceivedEvents.Columns["colStatus"].Width = 120;
        }

        private void InitAutoRunTimer()
        {
            timerAutoRun = new System.Windows.Forms.Timer { Interval = 3000 };
            timerAutoRun.Tick += (s, e) => SimulateProductionRun(1);
        }

        private void ChkAutoRun_CheckedChanged(object sender, EventArgs e)
        {
            timerAutoRun.Enabled = chkAutoRun.Checked;
            if (chkAutoRun.Checked)
            {
                chkAutoRun.ForeColor = Color.DarkGreen;
            }
            else
            {
                chkAutoRun.ForeColor = Color.Black;
            }
        }

        private void BtnApplyUrlToConfig_Click(object sender, EventArgs e)
        {
            int port = (int)numServerPort.Value;
            var cfg = BlockIdConfig.Instance;
            cfg.Enable = true;
            cfg.ApiUrl = $"http://127.0.0.1:{port}/ingest/v1/events";
            cfg.ApiKey = "mock-secret-key-123456";
            cfg.MachineCode = "UVPS008";
            cfg.Save();

            // Khởi động worker của ứng dụng nếu chưa chạy
            BlockIdQueueManager.StartWorker();

            MessageBox.Show($"Đã cấu hình App trỏ về Server Ảo:\nURL: {cfg.ApiUrl}\nAPI Key: {cfg.ApiKey}\nTrạng thái BlockId: ĐÃ BẬT", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region MOCK SERVER ENGINE

        private void StartMockServer(int port)
        {
            if (_isServerRunning) return;

            try
            {
                _mockListener = new TcpListener(IPAddress.Loopback, port);
                _mockListener.Start();
                _isServerRunning = true;

                btnStartServer.Enabled = false;
                btnStopServer.Enabled = true;
                lblServerStatus.Text = $"Server: ĐANG CHẠY (http://127.0.0.1:{port}/ingest/v1/events)";
                lblServerStatus.ForeColor = Color.Green;

                Task.Run(() => ServerListenerLoop());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở cổng {port}:\n{ex.Message}", "Lỗi mở Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopMockServer()
        {
            _isServerRunning = false;
            try { _mockListener?.Stop(); } catch { }

            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
            lblServerStatus.Text = "Server: ĐÃ NGẮT KẾT NỐI (Mô phỏng mất mạng)";
            lblServerStatus.ForeColor = Color.Red;
        }

        private void ServerListenerLoop()
        {
            while (_isServerRunning)
            {
                try
                {
                    var client = _mockListener.AcceptTcpClient();
                    Task.Run(() => HandleIncomingClient(client));
                }
                catch
                {
                    break;
                }
            }
        }

        private void HandleIncomingClient(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                byte[] buffer = new byte[16384];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) return;

                string rawReq = Encoding.UTF8.GetString(buffer, 0, read);
                int splitIdx = rawReq.IndexOf("\r\n\r\n");
                string body = splitIdx > 0 ? rawReq.Substring(splitIdx + 4) : "";

                // Xác định kịch bản phản hồi
                int behavior = 0;
                this.Invoke(new Action(() => behavior = cbbServerBehavior.SelectedIndex));

                if (behavior == 1) // Kịch bản Timeout > 10s
                {
                    Thread.Sleep(12000);
                    return; // Đóng stream gây timeout
                }

                int statusCode = 202;
                string respBody = "";
                string displayStatus = "202 Accepted";

                if (behavior == 2) // 429
                {
                    statusCode = 429;
                    respBody = "{\"error\":\"Too Many Requests\"}";
                    displayStatus = "429 Rate Limit";
                }
                else if (behavior == 3) // 500
                {
                    statusCode = 500;
                    respBody = "{\"error\":\"Internal Server Error\"}";
                    displayStatus = "500 Server Error";
                }
                else if (behavior == 4) // 400
                {
                    statusCode = 400;
                    respBody = "{\"error\":\"Bad Request: Invalid Parameter Code\"}";
                    displayStatus = "400 Bad Request";
                }
                else if (behavior == 5) // 401
                {
                    statusCode = 401;
                    respBody = "{\"error\":\"Unauthorized: Wrong API Key\"}";
                    displayStatus = "401 Unauthorized";
                }
                else // 0: Bình thường (202 hoặc 200 Duplicate)
                {
                    string recId = "";
                    string barcode = "";
                    string subject = "";
                    string paramSummary = "";

                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject(body);
                        recId = (string)obj.id;
                        subject = (string)obj.subject;
                        barcode = (string)obj.data.barcode;

                        var valList = new List<string>();
                        if (obj.data.values != null)
                        {
                            foreach (var v in obj.data.values)
                            {
                                valList.Add($"{v.code}={v.value}");
                            }
                        }
                        paramSummary = string.Join(", ", valList);
                    }
                    catch { }

                    lock (_receivedUuids)
                    {
                        if (!string.IsNullOrEmpty(recId) && _receivedUuids.Contains(recId))
                        {
                            // Trùng lặp
                            statusCode = 200;
                            respBody = JsonConvert.SerializeObject(new { id = recId, status = "duplicate" });
                            displayStatus = "200 Duplicate";
                            _totalDuplicates++;
                        }
                        else
                        {
                            // Mới
                            if (!string.IsNullOrEmpty(recId)) _receivedUuids.Add(recId);
                            statusCode = 202;
                            respBody = JsonConvert.SerializeObject(new { id = recId, status = "accepted" });
                            displayStatus = "202 Accepted";
                            _totalReceived++;
                        }
                    }

                    // Đưa lên DataGrid hiển thị
                    this.BeginInvoke(new Action(() =>
                    {
                        dgrReceivedEvents.Rows.Insert(0,
                            DateTime.Now.ToString("HH:mm:ss.fff"),
                            recId,
                            barcode,
                            subject,
                            paramSummary,
                            displayStatus
                        );
                        UpdateStats();
                    }));
                }

                // Gửi HTTP Response về cho App
                string statusLine = statusCode == 202 ? "HTTP/1.1 202 Accepted\r\n" :
                                    statusCode == 200 ? "HTTP/1.1 200 OK\r\n" :
                                    statusCode == 429 ? "HTTP/1.1 429 Too Many Requests\r\n" :
                                    statusCode == 400 ? "HTTP/1.1 400 Bad Request\r\n" :
                                    statusCode == 401 ? "HTTP/1.1 401 Unauthorized\r\n" :
                                    "HTTP/1.1 500 Internal Server Error\r\n";

                byte[] bBytes = Encoding.UTF8.GetBytes(respBody);
                string header = statusLine +
                                "Content-Type: application/json; charset=utf-8\r\n" +
                                $"Content-Length: {bBytes.Length}\r\n" +
                                "Connection: close\r\n\r\n";

                byte[] hBytes = Encoding.ASCII.GetBytes(header);
                stream.Write(hBytes, 0, hBytes.Length);
                stream.Write(bBytes, 0, bBytes.Length);
            }
        }

        #endregion

        #region PRODUCTION SIMULATOR

        private static readonly Random _rnd = new Random();

        private void SimulateProductionRun(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _simulatedCount++;
                string sampleBarcode = $"SHEET-2026-{_rnd.Next(100000, 999999)}";
                string sampleLot = $"LOT-A{_rnd.Next(10, 99)}";

                var mockParams = new McSMPrintingParamCls
                {
                    PrintingTime = _rnd.Next(12, 18),
                    StencilAngle = 45,
                    SqueezeSpeed = _rnd.Next(100, 150),
                    ReturnSpeed = _rnd.Next(120, 160),
                    SqueezePressure = _rnd.Next(65, 85),
                    CoaterPressure = _rnd.Next(40, 60),
                    SqueezePringtingDistanceStart = 10,
                    SqueezePringtingDistanceFinish = 250,
                    Gap = _rnd.Next(10, 20),
                    MachineAirPressure = 50
                };

                // Đưa vào hàng đợi BlockIdQueueManager như khi máy dập in thật
                BlockIdQueueManager.EnqueueRecord(sampleBarcode, sampleLot, mockParams, "UVPS008");
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            lblSimulationStats.Text = $"Đã mô phỏng: {_simulatedCount} tấm | Server nhận mới: {_totalReceived} | Trùng lặp: {_totalDuplicates} | Đang chờ trong Queue: {BlockIdQueueManager.PendingCount}";
        }

        #endregion

        private void FrmBlockIdSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerAutoRun.Stop();
            StopMockServer();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
