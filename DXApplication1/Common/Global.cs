using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DXApplication1;

static class Global
{
    public static string PassworSetting = "seev";
    public static string PassworDefaut = "0964336162";
    public static string VersionSoftware = "1.0.0";
    static public string ApplicationFullPathName() { return System.Reflection.Assembly.GetExecutingAssembly().Location; }
    static public string ApplicationPath() { return System.IO.Path.GetDirectoryName(ApplicationFullPathName()); }

    public static void DoubleBuffered(this DataGridView dgv, bool setting)
    {
        Type dgvType = dgv.GetType();
        PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
        pi.SetValue(dgv, setting, null);
    }
    public static void InvokeAndClose(this Control seft, MethodInvoker func)
    {
        IAsyncResult result = seft.BeginInvoke(func);
        seft.EndInvoke(result);
        result.AsyncWaitHandle.Close();
    }
    public static DateTime Y2K_VALUE = new DateTime(2000, 1, 1);
    static public void ChangeDataGridViewJustify(ref DataGridView dgv)
    {
        if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.AllCells)
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
        else if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader)
            //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            //else if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.ColumnHeader)
            //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            //else if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.DisplayedCells)
            //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            //else if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader)
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //else if (dgv.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.Fill)
        //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        else
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        //dgv.AutoResizeColumns(dgv.AutoSizeColumnsMode);
    }

    static public void LoadComboboxFromDatatable(object cbo_object, DataTable data_table)
    {
        if (cbo_object is ToolStripComboBox)
        {
            ToolStripComboBox cbo = (ToolStripComboBox)cbo_object;
            cbo.Items.Clear();
            if (data_table != null)
            {
                foreach (DataRow row in data_table.Rows)
                {
                    cbo.Items.Add(row[0].ToString());
                }
            }
        }
        else if (cbo_object is ComboBox)
        {
            ComboBox cbo = (ComboBox)cbo_object;
            cbo.Items.Clear();
            if (data_table != null)
            {
                foreach (DataRow row in data_table.Rows)
                {
                    cbo.Items.Add(row[0].ToString());
                }
            }
        }

    }



    //static public void LoadComboboxFromDatatable(ComboBox cbo, DataTable data_table)
    //{
    //    cbo.Items.Clear();
    //    if (data_table != null)
    //    {
    //        //for (int i = 0; i < data_table.Rows.Count - 1; i++)
    //        //{
    //        //    cbo.Items.Add(data_table.Columns[0].);   
    //        //}
    //        foreach (DataRow row in data_table.Rows)
    //        {
    //            cbo.Items.Add(row.ToString()); 
    //        }
    //    }
    //}

    //static public void LoadComboboxFromDatatable(ToolStripComboBox cbo, DataTable data_table)
    //{
    //    cbo.Items.Clear();
    //    if (data_table != null)
    //    {
    //        //for (int i = 0; i < data_table.Rows.Count - 1; i++)
    //        //{
    //        //    cbo.Items.Add(data_table.Columns[0].);   
    //        //}
    //        foreach (DataRow row in data_table.Rows)
    //        {
    //            cbo.Items.Add(row.ToString());
    //        }
    //    }
    //}

    static public void Delay_ms(int ms_count)
    {
        for (int i = 0; i < ms_count; i++)
        {
            Thread.Sleep(1);
            Application.DoEvents();
        }
    }

    public static void ReleaseObject(object obj)
    {
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            obj = null;
        }
        catch
        {
            obj = null;
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
    public static void delay_ms(int ms_count)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds < ms_count) // && string.IsNullOrEmpty(buffReceiver))
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(1);
        }

    }

    public static string PasswordInput(string default_value = "", bool show_pass = false)
    {
        frmInputPassword frm = new frmInputPassword(default_value, show_pass);
        frm.ShowDialog();
        return frm.txtInput.Text;
    }

    private static readonly object _logLock = new object();
    private static readonly Dictionary<string, DateTime> _recentLogMap = new Dictionary<string, DateTime>();
    private static readonly Queue<DateTime> _globalLogTimestamps = new Queue<DateTime>();

    static public void WriteLogFile(string stext)
    {
        if (string.IsNullOrEmpty(stext)) return;

        lock (_logLock)
        {
            string MyPath = ApplicationPath() + @"\EventsLog\";
            try
            {
                DateTime now = DateTime.Now;

                // 1. Giới hạn tần suất tổng thể: không ghi quá 10 log/giây trên toàn bộ ứng dụng
                while (_globalLogTimestamps.Count > 0 && (now - _globalLogTimestamps.Peek()).TotalSeconds >= 1)
                {
                    _globalLogTimestamps.Dequeue();
                }
                if (_globalLogTimestamps.Count >= 10)
                {
                    return; // Vượt quá giới hạn ghi log/giây, bỏ qua để tránh tràn RAM/I/O
                }

                // 2. Tạo khóa nhận diện cho thông điệp (dòng đầu tiên hoặc 80 ký tự đầu)
                // Giúp phân biệt từng hàm/lỗi riêng biệt ngay cả khi các hàm khác nhau gọi ghi log xen kẽ
                string logKey = stext.Trim();
                int newlineIdx = logKey.IndexOfAny(new char[] { '\r', '\n' });
                if (newlineIdx > 0)
                {
                    logKey = logKey.Substring(0, newlineIdx).Trim();
                }
                if (logKey.Length > 80)
                {
                    logKey = logKey.Substring(0, 80);
                }

                // 3. Kiểm tra xem lỗi từ hàm/thông điệp này có vừa được ghi trong vòng 30 giây không
                if (_recentLogMap.TryGetValue(logKey, out DateTime lastLoggedTime))
                {
                    if ((now - lastLoggedTime).TotalSeconds < 30)
                    {
                        return; // Đã ghi thông điệp tương tự trong 30s, bỏ qua
                    }
                }

                // Cập nhật thời điểm ghi của khóa này
                _recentLogMap[logKey] = now;

                // Dọn dẹp bộ nhớ định kỳ cho _recentLogMap để tránh rò rỉ RAM
                if (_recentLogMap.Count > 100)
                {
                    List<string> expiredKeys = new List<string>();
                    foreach (var pair in _recentLogMap)
                    {
                        if ((now - pair.Value).TotalMinutes > 2)
                        {
                            expiredKeys.Add(pair.Key);
                        }
                    }
                    foreach (var k in expiredKeys)
                    {
                        _recentLogMap.Remove(k);
                    }
                }

                if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
                string logFile = MyPath + now.ToString("yyyyMMdd_HH") + "[" + Process.GetCurrentProcess().ProcessName + "].log";
                string appendText = now.ToString("[yyyy-MM-dd HH:mm:ss.fff] - ") + stext + "\r\n";

                if (File.Exists(logFile) == false)
                {
                    string createText = "Đây là LOG file event \r\n";
                    File.WriteAllText(logFile, createText);
                }

                // Giới hạn kích thước file log tối đa 20MB để bảo vệ ổ cứng
                FileInfo fi = new FileInfo(logFile);
                if (fi.Exists && fi.Length > 20 * 1024 * 1024)
                {
                    return;
                }

                File.AppendAllText(logFile, appendText);
                _globalLogTimestamps.Enqueue(now);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
    static public void WriteEventLogFile(string stext, string machine)
    {
        string MyPath = ApplicationPath() + @"\EventsLog\" + machine;
        try
        {
            string appendText = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] - ") + stext + "\r\n";
            if (!Directory.Exists(MyPath)) Directory.CreateDirectory(MyPath);
            MyPath += DateTime.Now.ToString("yyyyMMdd_HH") + "[" + Process.GetCurrentProcess().ProcessName + "].log";
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
}
