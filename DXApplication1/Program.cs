using DXApplication1.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool testDebug = false;
            string process_name = Process.GetCurrentProcess().ProcessName;
            if ((Process.GetProcessesByName(process_name).Length > 1))
            {
                MessageBox.Show("Chương trình '" + process_name + "' đã được chạy.\r\nKích vào biểu tượng tại khay hệ thống để mở lai chương trình.");
                Application.Exit();
                //Thoat khoi chuong trinh
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //UInt16[] test = new UInt16[10] { 19767, 13360, 18996, 12333, 12336, 13364, 12845, 13113, 12845, 2573 };
                //string text = Conv.UInt2StringCharRevert(test);
                //int[] arrTx = Conv.StringChar2Int("720123-00123-001-1", 9);
                //string test = Conv.Int2StringChar(arrTx);
                IniFile iniFile = new IniFile(Global.ApplicationPath() + @"\AppConfig.ini");
                VarGlobal.McID = iniFile.IniReadValue("COMMON", "MachineID", "");
                //VarGlobal.SavePath = iniFile.IniReadValue("COMMON", "SavePath", "");
                //if (!Directory.Exists(VarGlobal.SavePath))
                //{
                //    MessageBox.Show("Đường dẫn lưu logfile không tồn tại, dữ liệu sẽ được lưu ở đường dẫn mặc định!", "MESSAGE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    VarGlobal.SavePath = Global.ApplicationPath() + @"\ReportLog";
                //}

                SQLFunc.InitGlobalVarial();

                VarGlobal.InitAllThreadPollingPLC();
            
                Application.Run(new FrmMain());

                VarGlobal.StopAllThreadPollingPLC();
            }
        }
    }
}
