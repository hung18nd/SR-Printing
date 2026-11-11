using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXApplication1.Common;

namespace DXApplication1.Common
{
    public class AutoUpdateManager
    {
        private static string ConfigPath => Path.Combine(Global.ApplicationPath(), "AppConfig.ini");

        public static bool AutoCheckUpdate { get; set; } = false;
        public static string ServerUpdatePath { get; set; } = "";
        public static string ServerUser { get; set; } = "Administrator";
        public static string ServerPass { get; set; } = "Seev@123";

        #region Win32 Network Connection P/Invoke
        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public int Scope = 2;          // RESOURCE_GLOBALNET
            public int ResourceType = 1;   // RESOURCETYPE_DISK
            public int DisplayType = 3;    // RESOURCEDISPLAYTYPE_SHARE
            public int Usage = 1;          // RESOURCEUSAGE_CONNECTABLE
            public string LocalName = null;
            public string RemoteName;
            public string Comment = null;
            public string Provider = null;
        }

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
        #endregion

        public static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    IniFile ini = new IniFile(ConfigPath);
                    AutoCheckUpdate = ini.IniReadValue("UPDATE", "AutoCheckUpdate", "0") == "1";
                    ServerUpdatePath = ini.IniReadValue("UPDATE", "ServerUpdatePath", "");
                    
                    string u = ini.IniReadValue("UPDATE", "ServerUser", "Administrator");
                    ServerUser = string.IsNullOrWhiteSpace(u) ? "Administrator" : u;

                    string p = ini.IniReadValue("UPDATE", "ServerPass", "Seev@123");
                    ServerPass = string.IsNullOrWhiteSpace(p) ? "Seev@123" : p;
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[AutoUpdateManager.LoadConfig Error] {ex.Message}");
            }
        }

        public static void SaveConfig(bool autoCheck, string serverPath, string user = "Administrator", string pass = "Seev@123")
        {
            try
            {
                AutoCheckUpdate = autoCheck;
                ServerUpdatePath = serverPath;
                ServerUser = string.IsNullOrWhiteSpace(user) ? "Administrator" : user;
                ServerPass = string.IsNullOrWhiteSpace(pass) ? "Seev@123" : pass;

                IniFile ini = new IniFile(ConfigPath);
                ini.IniWriteValue("UPDATE", "AutoCheckUpdate", autoCheck ? "1" : "0");
                ini.IniWriteValue("UPDATE", "ServerUpdatePath", serverPath ?? "");
                ini.IniWriteValue("UPDATE", "ServerUser", ServerUser);
                ini.IniWriteValue("UPDATE", "ServerPass", ServerPass);
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[AutoUpdateManager.SaveConfig Error] {ex.Message}");
            }
        }

        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }
            return "127.0.0.1";
        }

        public static string GetHostFromPath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !path.Trim().StartsWith(@"\\")) return "";
                string[] parts = path.Trim().TrimEnd('\\').Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0) return parts[0];
            }
            catch { }
            return "";
        }

        public static bool SaveCredentialsToWindowsVault(string host, string user, string pass)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)) return false;

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmdkey.exe",
                    Arguments = $"/add:\"{host}\" /user:\"{user}\" /pass:\"{pass}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit(3000);
                    return p.ExitCode == 0;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Lay goc thu muc chia se mang UNC (Vi du: \\10.126.20.10\1.PrintingF5)
        /// </summary>
        public static string GetUncShareRoot(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !path.Trim().StartsWith(@"\\")) return "";
                string trimmed = path.Trim().TrimEnd('\\');
                string[] parts = trimmed.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    return $@"\\{parts[0]}\{parts[1]}";
                }
                else if (parts.Length == 1)
                {
                    return $@"\\{parts[0]}\IPC$";
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Dang nhap / xac thuc quyen truy cap vao thu muc mang chia se tren Server
        /// Ho tro co che Remember Credentials (ghi nho tai khoan) cho cac lan sau
        /// </summary>
        public static bool EnsureNetworkAuthenticated(string path, string user, string pass, out string authMessage)
        {
            authMessage = "";
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !path.Trim().StartsWith(@"\\"))
                {
                    // Duong dan cuc bo (D:\...), khong can xac thuc mang
                    return true;
                }

                string cleanPath = path.Trim();
                string uncRoot = GetUncShareRoot(cleanPath);
                string host = GetHostFromPath(cleanPath);

                // 1. KIEM TRA XEM WINDOWS DA CO QUYEN TRUY CAP SAN CHUA (DA GHI NHO TAI KHOAN TU TRUOC)
                if (Directory.Exists(cleanPath) || File.Exists(cleanPath))
                {
                    authMessage = $"Đã có quyền truy cập vào Server (Windows đã ghi nhớ phiên đăng nhập)";
                    return true;
                }

                string targetUser = string.IsNullOrWhiteSpace(user) ? ServerUser : user;
                string targetPass = string.IsNullOrWhiteSpace(pass) ? ServerPass : pass;

                if (string.IsNullOrWhiteSpace(targetUser)) targetUser = "Administrator";
                if (string.IsNullOrWhiteSpace(targetPass)) targetPass = "Seev@123";

                // 2. LUU TAI KHOAN VAO WINDOWS CREDENTIAL MANAGER (REMEMBER CREDENTIALS)
                if (!string.IsNullOrEmpty(host))
                {
                    SaveCredentialsToWindowsVault(host, targetUser, targetPass);
                }

                // 3. THU THUC HIEN NET USE VOI CHE DO GHI NHO /PERSISTENT:YES
                if (!string.IsNullOrEmpty(uncRoot))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c net use \"{uncRoot}\" \"{targetPass}\" /user:\"{targetUser}\" /persistent:yes",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    using (Process p = Process.Start(psi))
                    {
                        bool finished = p.WaitForExit(5000);
                        if (!finished)
                        {
                            try { p.Kill(); } catch { }
                        }
                    }
                }

                // 4. KIEM TRA LAI XEM WINDOWS DA TRUY CAP DUOC CHUA
                if (Directory.Exists(cleanPath) || File.Exists(cleanPath))
                {
                    authMessage = $"Đăng nhập thành công và đã ghi nhớ tài khoản (Remember) cho các lần sau!";
                    return true;
                }

                authMessage = $"Chưa thể truy cập vào '{cleanPath}'.\r\n" +
                              $"- Tài khoản: {targetUser}\r\n" +
                              $"- Bạn có thể bấm nút [Mở Folder (Remember)] để Windows bật hộp thoại đăng nhập và tích chọn [Remember my credentials] một lần duy nhất.";
                return false;
            }
            catch (Exception ex)
            {
                authMessage = $"Lỗi kiểm tra quyền truy cập mạng: {ex.Message}";
                Global.WriteLogFile($"[AutoUpdate.EnsureNetworkAuthenticated Error] {ex.Message}");
                return false;
            }
        }

        public static bool EnsureNetworkAuthenticated(string path, out string authMessage)
        {
            return EnsureNetworkAuthenticated(path, ServerUser, ServerPass, out authMessage);
        }

        public static bool TestConnection(string path, string user, string pass, out string message)
        {
            message = "";
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    message = "Đường dẫn cập nhật không được để trống!";
                    return false;
                }

                string currentExeName = Path.GetFileName(Application.ExecutablePath);
                string cleanPath = path.Trim();

                // 1. Thuc hien dang nhap vao Server
                bool isAuth = EnsureNetworkAuthenticated(cleanPath, user, pass, out string authMsg);
                if (!isAuth)
                {
                    message = authMsg;
                    return false;
                }

                string serverExePath;
                if (Directory.Exists(cleanPath))
                {
                    serverExePath = Path.Combine(cleanPath, currentExeName);
                }
                else if (File.Exists(cleanPath))
                {
                    string targetFileName = Path.GetFileName(cleanPath);
                    if (!targetFileName.Equals(currentExeName, StringComparison.OrdinalIgnoreCase))
                    {
                        message = $"Tên file trên server ({targetFileName}) không khớp với tên phần mềm hiện tại ({currentExeName})!\r\nFile thực thi phải có tên giống nhau để thay thế an toàn.";
                        return false;
                    }
                    serverExePath = cleanPath;
                }
                else
                {
                    serverExePath = Path.Combine(cleanPath, currentExeName);
                }

                if (!File.Exists(serverExePath))
                {
                    message = $"Xác thực: {authMsg}\r\n\r\nKhông tìm thấy file '{currentExeName}' trên server:\r\n{serverExePath}\r\nVui lòng kiểm tra lại tài khoản đăng nhập hoặc quyền chia sẻ thư mục!";
                    return false;
                }

                // Kiểm tra đọc được thông tin file
                FileInfo serverFi = new FileInfo(serverExePath);
                FileInfo localFi = new FileInfo(Application.ExecutablePath);

                bool isDifferent = Math.Abs((serverFi.LastWriteTime - localFi.LastWriteTime).TotalSeconds) >= 2;

                message = $"ĐĂNG NHẬP & KẾT NỐI SERVER THÀNH CÔNG!\r\n" +
                          $"- Trạng thái đăng nhập: {authMsg}\r\n" +
                          $"- Tài khoản sử dụng: {user}\r\n" +
                          $"- File trên Server: {serverFi.FullName}\r\n" +
                          $"- Thời gian sửa đổi Server: {serverFi.LastWriteTime:yyyy-MM-dd HH:mm:ss}\r\n" +
                          $"- Dung lượng Server: {serverFi.Length / 1024} KB\r\n\r\n" +
                          $"- File Local hiện tại: {localFi.FullName}\r\n" +
                          $"- Thời gian sửa đổi Local: {localFi.LastWriteTime:yyyy-MM-dd HH:mm:ss}\r\n" +
                          $"- Dung lượng Local: {localFi.Length / 1024} KB\r\n\r\n" +
                          $"Trạng thái: {(isDifferent ? ">> CÓ BẢN MỚI / THỜI GIAN KHÁC NHAU (Sẽ cập nhật khi có xác nhận)" : ">> BẢN HIỆN TẠI ĐÃ TRÙNG KHỚP VỚI SERVER")}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Lỗi kết nối tới đường dẫn server:\r\n{ex.Message}";
                return false;
            }
        }

        public static bool TestConnection(string path, out string message)
        {
            return TestConnection(path, ServerUser, ServerPass, out message);
        }

        /// <summary>
        /// Ghi file log ghi nhận máy client đã cập nhật bản mới lên thư mục Server
        /// </summary>
        public static void WriteServerUpdateLog(string serverDir, FileInfo serverFi, FileInfo localFi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serverDir)) return;

                if (File.Exists(serverDir))
                {
                    serverDir = Path.GetDirectoryName(serverDir);
                }

                EnsureNetworkAuthenticated(serverDir, ServerUser, ServerPass, out _);

                if (!Directory.Exists(serverDir)) return;

                string machineId = string.IsNullOrWhiteSpace(VarGlobal.McID) ? "UNKNOWN_MC" : VarGlobal.McID;
                string computerName = Environment.MachineName;
                string ip = GetLocalIPAddress();
                string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 1. Ghi file chi tiet rieng cho tung may (Update_Log_{machineId}.txt) de tranh xung dot ghi file
                string individualLog = Path.Combine(serverDir, $"Update_Log_{machineId}.txt");
                string logContent = $"================================================================================\r\n" +
                                    $"[CẬP NHẬT PHIÊN BẢN MỚI THÀNH CÔNG]\r\n" +
                                    $"Thời gian cập nhật : {timeStr}\r\n" +
                                    $"Mã máy (MachineID) : {machineId}\r\n" +
                                    $"Tên máy tính (PC)  : {computerName}\r\n" +
                                    $"Địa chỉ IP máy     : {ip}\r\n" +
                                    $"Tài khoản Server   : {ServerUser}\r\n" +
                                    $"File thực thi      : {Path.GetFileName(Application.ExecutablePath)}\r\n" +
                                    $"Thời gian bản mới  : {serverFi?.LastWriteTime:yyyy-MM-dd HH:mm:ss} ({((serverFi != null) ? serverFi.Length / 1024 : 0)} KB)\r\n" +
                                    $"Thời gian bản cũ   : {localFi?.LastWriteTime:yyyy-MM-dd HH:mm:ss} ({((localFi != null) ? localFi.Length / 1024 : 0)} KB)\r\n" +
                                    $"Trạng thái         : Người dùng đã chọn [CÓ], hệ thống đã nạp bản mới và khởi động lại.\r\n" +
                                    $"================================================================================\r\n\r\n";

                File.AppendAllText(individualLog, logContent, System.Text.Encoding.UTF8);

                // 2. Ghi vao file lich su chung (Update_Clients_History.log)
                string summaryLog = Path.Combine(serverDir, "Update_Clients_History.log");
                string summaryLine = $"[{timeStr}] MachineID: {machineId,-10} | Host: {computerName,-15} | IP: {ip,-15} | User: {ServerUser,-15} | Da cap nhat ban: {serverFi?.LastWriteTime:yyyy-MM-dd HH:mm:ss}\r\n";
                File.AppendAllText(summaryLog, summaryLine, System.Text.Encoding.UTF8);

                Global.WriteLogFile($"[AutoUpdate] Da ghi nhan nhat ky cap nhat len Server: {individualLog}");
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[AutoUpdate.WriteServerUpdateLog Error] {ex.Message}");
            }
        }

        /// <summary>
        /// Kiem tra update khi bat phan mem len. Neu co ban moi, hien hop thoai xac nhan (Co/Khong).
        /// Neu nguoi dung chon Co thi moi thuc hien cap nhat va tu dong chay lai.
        /// </summary>
        public static void CheckUpdateOnStartup(Form parentForm)
        {
            Task.Run(() =>
            {
                try
                {
                    System.Threading.Thread.Sleep(1200); // Đợi form chính hiển thị hoàn chỉnh
                    LoadConfig();
                    if (!AutoCheckUpdate || string.IsNullOrWhiteSpace(ServerUpdatePath))
                    {
                        return;
                    }

                    string currentExeName = Path.GetFileName(Application.ExecutablePath);
                    string cleanPath = ServerUpdatePath.Trim();

                    // Dang nhap vao server
                    EnsureNetworkAuthenticated(cleanPath, ServerUser, ServerPass, out _);

                    string serverExePath = Directory.Exists(cleanPath)
                        ? Path.Combine(cleanPath, currentExeName)
                        : cleanPath;

                    if (!File.Exists(serverExePath))
                    {
                        return;
                    }

                    string localExePath = Application.ExecutablePath;
                    FileInfo serverFi = new FileInfo(serverExePath);
                    FileInfo localFi = new FileInfo(localExePath);

                    TimeSpan timeDiff = serverFi.LastWriteTime - localFi.LastWriteTime;
                    if (Math.Abs(timeDiff.TotalSeconds) < 2)
                    {
                        return;
                    }

                    parentForm.BeginInvoke(new Action(() =>
                    {
                        string msg = $"Phát hiện có phiên bản mới của phần mềm trên Server!\r\n\r\n" +
                                     $"• Bản mới trên Server : {serverFi.LastWriteTime:yyyy-MM-dd HH:mm:ss} ({serverFi.Length / 1024} KB)\r\n" +
                                     $"• Bản hiện tại của máy: {localFi.LastWriteTime:yyyy-MM-dd HH:mm:ss} ({localFi.Length / 1024} KB)\r\n\r\n" +
                                     $"Bạn có muốn thực hiện cập nhật ngay bây giờ không?\r\n\r\n" 
                                    ;

                        DialogResult result = MessageBox.Show(parentForm, msg, "XÁC NHẬN CẬP NHẬT PHẦN MỀM", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Global.WriteLogFile("[AutoUpdate] Người dùng đồng ý cập nhật phiên bản mới.");
                            PerformUpdate(serverExePath, localExePath, serverFi, localFi, restart: true);
                        }
                        else
                        {
                            Global.WriteLogFile("[AutoUpdate] Người dùng chọn KHÔNG cập nhật.");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Global.WriteLogFile($"[AutoUpdate.CheckUpdateOnStartup Error] {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Kiem tra khi dong ung dung. Neu co ban moi, hoi xac nhan nguoi dung co muon cap nhat truoc khi tat khong.
        /// </summary>
        public static void CheckAndUpdateOnClose()
        {
            try
            {
                LoadConfig();
                if (!AutoCheckUpdate || string.IsNullOrWhiteSpace(ServerUpdatePath))
                {
                    return;
                }

                string currentExeName = Path.GetFileName(Application.ExecutablePath);
                string cleanPath = ServerUpdatePath.Trim();

                // Dang nhap vao server
                EnsureNetworkAuthenticated(cleanPath, ServerUser, ServerPass, out _);

                string serverExePath = Directory.Exists(cleanPath)
                    ? Path.Combine(cleanPath, currentExeName)
                    : cleanPath;

                if (!File.Exists(serverExePath))
                {
                    return;
                }

                string localExePath = Application.ExecutablePath;
                FileInfo serverFi = new FileInfo(serverExePath);
                FileInfo localFi = new FileInfo(localExePath);

                TimeSpan timeDiff = serverFi.LastWriteTime - localFi.LastWriteTime;
                if (Math.Abs(timeDiff.TotalSeconds) < 2)
                {
                    return;
                }

                string msg = $"Phát hiện có phiên bản mới trên Server!\r\n\r\n" +
                             $"• Bản mới trên Server : {serverFi.LastWriteTime:yyyy-MM-dd HH:mm:ss}\r\n" +
                             $"• Bản hiện tại của máy: {localFi.LastWriteTime:yyyy-MM-dd HH:mm:ss}\r\n\r\n" +
                             $"Bạn có muốn thực hiện cập nhật bản mới không?\r\n";

                DialogResult result = MessageBox.Show(msg, "XÁC NHẬN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    PerformUpdate(serverExePath, localExePath, serverFi, localFi, restart: true);
                }
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[AutoUpdate.CheckAndUpdateOnClose Error] {ex.Message}");
            }
        }

        /// <summary>
        /// Thuc hien ghi log len server, tao script bat de thay the exe va tu dong khoi dong lai ung dung.
        /// </summary>
        private static void PerformUpdate(string serverExePath, string localExePath, FileInfo serverFi, FileInfo localFi, bool restart)
        {
            try
            {
                string serverDir = Directory.Exists(ServerUpdatePath.Trim()) ? ServerUpdatePath.Trim() : Path.GetDirectoryName(serverExePath);
                string localDir = Path.GetDirectoryName(localExePath);
                string uncRoot = GetUncShareRoot(serverDir);

                // 1. Ghi log phan mem da cap nhat vao thu muc tren Server
                WriteServerUpdateLog(serverDir, serverFi, localFi);

                // 2. Tao updater.bat de cho tien trinh tat han roi copy replace va tu dong chay lai file exe
                string updaterBat = Path.Combine(Global.ApplicationPath(), "updater.bat");
                string updaterLog = Path.Combine(localDir, "updater.log");
                int currentPid = Process.GetCurrentProcess().Id;
                string machineId = string.IsNullOrWhiteSpace(VarGlobal.McID) ? "UNKNOWN_MC" : VarGlobal.McID;
                string serverSummaryLog = Path.Combine(serverDir, "Update_Clients_History.log");

                string authCmd = !string.IsNullOrEmpty(uncRoot)
                    ? $@"echo Dang ket noi/dang nhap vao Server ({uncRoot}) voi user {ServerUser}... >> ""{updaterLog}""
net use ""{uncRoot}"" /user:""{ServerUser}"" ""{ServerPass}"" >nul 2>&1
"
                    : "";

                string batContent = $@"@echo off
chcp 65001 >nul
cd /d ""{localDir}""
echo [%date% %time%] Bat dau qua trinh cap nhat tu server... > ""{updaterLog}""

:WAIT_PID
tasklist /fi ""PID eq {currentPid}"" 2>nul | find ""{currentPid}"" >nul
if not errorlevel 1 (
    timeout /t 1 /nobreak >nul
    goto WAIT_PID
)

:: Cho them 1 giay de Windows giai phong file lock
timeout /t 1 /nobreak >nul

{authCmd}
echo Dang copy file exe moi tu server... >> ""{updaterLog}""
copy /y ""{serverExePath}"" ""{localExePath}"" >> ""{updaterLog}"" 2>&1
if errorlevel 1 (
    echo [ERROR] Copy file exe moi that bai! >> ""{updaterLog}""
    if exist ""{serverSummaryLog}"" (
        echo [%date% %time%] May [{machineId} - %computername%] COPY EXE THAT BAI! >> ""{serverSummaryLog}""
    )
    pause
    exit /b 1
)

echo Copy file thanh cong! >> ""{updaterLog}""
if exist ""{serverSummaryLog}"" (
    echo [%date% %time%] May [{machineId} - %computername%] Da cap nhat va khoi dong lai ung dung thanh cong. >> ""{serverSummaryLog}""
)

echo Dang tu dong khoi dong lai phan mem ({localExePath})... >> ""{updaterLog}""
cd /d ""{localDir}""
start """" ""{localExePath}""
exit
";

                File.WriteAllText(updaterBat, batContent, System.Text.Encoding.GetEncoding(65001));

                // 3. Khoi chay updater.bat thong qua cmd.exe o background
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{updaterBat}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = localDir,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi);

                // 4. Dong ngay tien trinh hien tai de updater.bat khong bi vuong file lock
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Global.WriteLogFile($"[AutoUpdate.PerformUpdate Error] {ex.Message}");
                MessageBox.Show($"Lỗi thực hiện cập nhật: {ex.Message}", "LỖI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
