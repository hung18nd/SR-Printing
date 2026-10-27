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
using Newtonsoft.Json;

namespace BlockIdSimulatorApp
{
    public class FrmSimulator : Form
    {
        private System.ComponentModel.IContainer components = null;

        // UI Controls - Server Section
        private Label lblServerStatus;
        private Button btnStartServer;
        private Button btnStopServer;
        private ComboBox cbbServerBehavior;
        private NumericUpDown numServerPort;
        private DataGridView dgrReceivedEvents;

        // UI Controls - Generator Section
        private Button btnSendSampleSingle;
        private Button btnSendSampleBatch10;
        private Button btnClearGrid;
        private Button btnExportCsv;
        private Label lblServerStats;

        // Internal Mock Server
        private TcpListener _mockListener;
        private bool _isServerRunning = false;
        private readonly HashSet<string> _receivedUuids = new HashSet<string>();
        private int _totalReceived = 0;
        private int _totalDuplicates = 0;

        public FrmSimulator()
        {
            InitializeComponent();
            SetupDataGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "BlockId Server Simulator v1.0 (Máy Chủ Ảo Test Giao Thức BlockId Ingestion)";
            this.Size = new Size(1000, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            // 1. GroupBox Máy chủ BlockId Ảo
            var grpServer = new GroupBox
            {
                Text = "1. Cấu hình Máy Chủ BlockId Ảo (Mock Ingestion Server)",
                Location = new Point(12, 10),
                Size = new Size(960, 150)
            };

            var lblPort = new Label { Text = "Port:", Location = new Point(15, 30), AutoSize = true };
            numServerPort = new NumericUpDown { Location = new Point(55, 27), Width = 80, Minimum = 80, Maximum = 65535, Value = 18660 };

            btnStartServer = new Button { Text = "BẬT Server Ảo", Location = new Point(145, 25), Size = new Size(120, 30), BackColor = Color.LightGreen, Font = new Font(this.Font, FontStyle.Bold) };
            btnStartServer.Click += (s, e) => StartMockServer((int)numServerPort.Value);

            btnStopServer = new Button { Text = "TẮT Server", Location = new Point(275, 25), Size = new Size(100, 30), Enabled = false, BackColor = Color.MistyRose };
            btnStopServer.Click += (s, e) => StopMockServer();

            lblServerStatus = new Label { Text = "Server: ĐANG TẮT", Location = new Point(390, 31), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), ForeColor = Color.Red };

            var lblBehavior = new Label { Text = "Kịch bản phản hồi của Server Ảo:", Location = new Point(15, 75), AutoSize = true };
            cbbServerBehavior = new ComboBox { Location = new Point(220, 72), Width = 520, DropDownStyle = ComboBoxStyle.DropDownList };
            cbbServerBehavior.Items.Add("1. Bình thường (202 Accepted cho UUID mới, 200 Duplicate nếu gửi lại)");
            cbbServerBehavior.Items.Add("2. Mất mạng / Treo Timeout (> 10s)");
            cbbServerBehavior.Items.Add("3. Quá tải (HTTP 429 Too Many Requests)");
            cbbServerBehavior.Items.Add("4. Máy chủ lỗi (HTTP 500 Internal Server Error)");
            cbbServerBehavior.Items.Add("5. Lỗi Client (HTTP 400 Bad Request - Sai mã thông số)");
            cbbServerBehavior.Items.Add("6. Lỗi bảo mật (HTTP 401 Unauthorized - Sai API Key)");
            cbbServerBehavior.SelectedIndex = 0;

            lblServerStats = new Label
            {
                Text = "Địa chỉ Endpoint: http://127.0.0.1:18660/ingest/v1/events | Đã nhận: 0 | Mới: 0 | Trùng lặp: 0",
                Location = new Point(15, 115),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            grpServer.Controls.Add(lblPort);
            grpServer.Controls.Add(numServerPort);
            grpServer.Controls.Add(btnStartServer);
            grpServer.Controls.Add(btnStopServer);
            grpServer.Controls.Add(lblServerStatus);
            grpServer.Controls.Add(lblBehavior);
            grpServer.Controls.Add(cbbServerBehavior);
            grpServer.Controls.Add(lblServerStats);

            // 2. GroupBox Công cụ phát sinh dữ liệu test nội bộ
            var grpTools = new GroupBox
            {
                Text = "2. Công Cụ Test Server Độc Lập (Tự Bắn Dữ Liệu Mẫu Kiểm Tra Server)",
                Location = new Point(12, 170),
                Size = new Size(960, 75)
            };

            btnSendSampleSingle = new Button { Text = "Bắn 1 gói mẫu (Test 1)", Location = new Point(15, 25), Size = new Size(180, 32) };
            btnSendSampleSingle.Click += (s, e) => SendSampleEvents(1);

            btnSendSampleBatch10 = new Button { Text = "Bắn 10 gói mẫu liên tục", Location = new Point(205, 25), Size = new Size(180, 32) };
            btnSendSampleBatch10.Click += (s, e) => SendSampleEvents(10);

            btnClearGrid = new Button { Text = "Xóa lịch sử bảng", Location = new Point(640, 25), Size = new Size(140, 32) };
            btnClearGrid.Click += (s, e) =>
            {
                dgrReceivedEvents.Rows.Clear();
                _receivedUuids.Clear();
                _totalReceived = 0;
                _totalDuplicates = 0;
                UpdateStats();
            };

            btnExportCsv = new Button { Text = "Xuất ra file CSV", Location = new Point(790, 25), Size = new Size(150, 32) };
            btnExportCsv.Click += BtnExportCsv_Click;

            grpTools.Controls.Add(btnSendSampleSingle);
            grpTools.Controls.Add(btnSendSampleBatch10);
            grpTools.Controls.Add(btnClearGrid);
            grpTools.Controls.Add(btnExportCsv);

            // 3. DataGrid hiển thị dữ liệu thời gian thực
            var lblGrid = new Label { Text = "3. Danh sách các bản ghi nhận được thời gian thực từ Phần Mềm Chính:", Location = new Point(12, 255), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            dgrReceivedEvents = new DataGridView
            {
                Location = new Point(12, 280),
                Size = new Size(960, 385),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            this.Controls.Add(grpServer);
            this.Controls.Add(grpTools);
            this.Controls.Add(lblGrid);
            this.Controls.Add(dgrReceivedEvents);

            this.FormClosing += (s, e) => StopMockServer();
        }

        private void SetupDataGrid()
        {
            dgrReceivedEvents.Columns.Add("colTime", "Thời gian nhận");
            dgrReceivedEvents.Columns.Add("colId", "UUID (id)");
            dgrReceivedEvents.Columns.Add("colBarcode", "Mã Barcode");
            dgrReceivedEvents.Columns.Add("colSubject", "Mã máy");
            dgrReceivedEvents.Columns.Add("colParams", "Thông số kỹ thuật (Values)");
            dgrReceivedEvents.Columns.Add("colStatus", "Kết quả trả về");

            dgrReceivedEvents.Columns["colTime"].Width = 120;
            dgrReceivedEvents.Columns["colId"].Width = 220;
            dgrReceivedEvents.Columns["colBarcode"].Width = 140;
            dgrReceivedEvents.Columns["colSubject"].Width = 90;
            dgrReceivedEvents.Columns["colStatus"].Width = 120;
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
                numServerPort.Enabled = false;

                lblServerStatus.Text = $"Server: ĐANG CHẠY (http://127.0.0.1:{port}/ingest/v1/events)";
                lblServerStatus.ForeColor = Color.Green;
                UpdateStats();

                Task.Run(() => ServerListenerLoop());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở server cổng {port}:\n{ex.Message}", "Lỗi khởi động Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopMockServer()
        {
            _isServerRunning = false;
            try { _mockListener?.Stop(); } catch { }

            btnStartServer.Enabled = true;
            btnStopServer.Enabled = false;
            numServerPort.Enabled = true;

            lblServerStatus.Text = "Server: ĐÃ DỪNG (Mô phỏng mất mạng)";
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
                byte[] buffer = new byte[32768];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) return;

                string rawReq = Encoding.UTF8.GetString(buffer, 0, read);
                int splitIdx = rawReq.IndexOf("\r\n\r\n");
                string body = splitIdx > 0 ? rawReq.Substring(splitIdx + 4) : "";

                int behavior = 0;
                this.Invoke(new Action(() => behavior = cbbServerBehavior.SelectedIndex));

                if (behavior == 1) // Kịch bản Timeout > 10s
                {
                    Thread.Sleep(12000);
                    return;
                }

                int statusCode = 202;
                string respBody = "";
                string displayStatus = "202 Accepted";

                if (behavior == 2) // 429 Rate Limit
                {
                    statusCode = 429;
                    respBody = "{\"error\":\"Too Many Requests\"}";
                    displayStatus = "429 Rate Limit";
                }
                else if (behavior == 3) // 500 Server Error
                {
                    statusCode = 500;
                    respBody = "{\"error\":\"Internal Server Error\"}";
                    displayStatus = "500 Server Error";
                }
                else if (behavior == 4) // 400 Bad Request
                {
                    statusCode = 400;
                    respBody = "{\"error\":\"Bad Request: Invalid Parameter Code\"}";
                    displayStatus = "400 Bad Request";
                }
                else if (behavior == 5) // 401 Unauthorized
                {
                    statusCode = 401;
                    respBody = "{\"error\":\"Unauthorized: Wrong API Key\"}";
                    displayStatus = "401 Unauthorized";
                }
                else // 0: Normal (202 hoặc 200 Duplicate)
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
                            statusCode = 200;
                            respBody = JsonConvert.SerializeObject(new { id = recId, status = "duplicate" });
                            displayStatus = "200 Duplicate";
                            _totalDuplicates++;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(recId)) _receivedUuids.Add(recId);
                            statusCode = 202;
                            respBody = JsonConvert.SerializeObject(new { id = recId, status = "accepted" });
                            displayStatus = "202 Accepted";
                            _totalReceived++;
                        }
                    }

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

        private void UpdateStats()
        {
            int port = (int)numServerPort.Value;
            lblServerStats.Text = $"Địa chỉ Endpoint: http://127.0.0.1:{port}/ingest/v1/events | Tổng nhận: {_totalReceived + _totalDuplicates} | Mới (202): {_totalReceived} | Trùng lặp (200): {_totalDuplicates}";
        }

        #endregion

        #region INTERNAL TEST CLIENT

        private static readonly Random _rnd = new Random();

        private void SendSampleEvents(int count)
        {
            if (!_isServerRunning)
            {
                MessageBox.Show("Vui lòng BẬT Server ảo trước khi bắn dữ liệu mẫu!", "Chưa bật server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int port = (int)numServerPort.Value;
            string url = $"http://127.0.0.1:{port}/ingest/v1/events";

            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    string uuid = Guid.NewGuid().ToString();
                    var sample = new
                    {
                        specversion = "1.0",
                        type = "vn.blockid.production.record.v1",
                        id = uuid,
                        source = "urn:blockid:client:sr-baking",
                        subject = "UVPS008",
                        time = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        data = new
                        {
                            barcode = $"TEST-BCR-{_rnd.Next(100000, 999999)}",
                            lot = "LOT-2026",
                            values = new object[]
                            {
                                new { code = "PRINT_TIME", value = _rnd.Next(14, 18) },
                                new { code = "SQUEEZE_SPEED", value = _rnd.Next(110, 140) },
                                new { code = "SQUEEZE_PRESS", value = _rnd.Next(65, 85) }
                            }
                        }
                    };

                    string json = JsonConvert.SerializeObject(sample);

                    try
                    {
                        var req = (HttpWebRequest)WebRequest.Create(url);
                        req.Method = "POST";
                        req.ContentType = "application/cloudevents+json; charset=utf-8";
                        req.Headers.Add("X-Api-Key", "test-secret-key");
                        req.Timeout = 5000;

                        byte[] bytes = Encoding.UTF8.GetBytes(json);
                        req.ContentLength = bytes.Length;
                        using (var os = req.GetRequestStream())
                        {
                            os.Write(bytes, 0, bytes.Length);
                        }
                        using (var resp = req.GetResponse()) { }
                    }
                    catch { }

                    if (count > 1) Thread.Sleep(200);
                }
            });
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            if (dgrReceivedEvents.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu nào để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = $"BlockId_Received_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("ThoiGian,UUID,Barcode,MaMay,ThongSo,KetQua");
                        foreach (DataGridViewRow row in dgrReceivedEvents.Rows)
                        {
                            sb.AppendLine($"\"{row.Cells[0].Value}\",\"{row.Cells[1].Value}\",\"{row.Cells[2].Value}\",\"{row.Cells[3].Value}\",\"{row.Cells[4].Value}\",\"{row.Cells[5].Value}\"");
                        }
                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

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
