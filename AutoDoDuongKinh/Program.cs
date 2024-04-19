using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
namespace AutoDoDuongKinh
{
    static class Program
    {
       
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //  Application.Run(new frmMain());

            Form frm = new frmMain();
            SingleInstanceApplication.Run(frm, NewInstanceHandler);
        }
        public static void NewInstanceHandler(object sender,StartupNextInstanceEventArgs e)
        {
            // Kích hoạt cửa sổ của instance đang chạy
            // Bạn có thể thay thế bằng 1 hành động khác
            frmMain frm = new frmMain();
            frm.Activate();
        }
        /// <summary>
        /// Lớp này dùng để gọi chạy form dạng single instance
        /// </summary>
        public class SingleInstanceApplication : WindowsFormsApplicationBase
        {
            private SingleInstanceApplication()
            {
                base.IsSingleInstance = true;
            }
            public static void Run(Form f,
            StartupNextInstanceEventHandler startupHandler)
            {
                SingleInstanceApplication app = new SingleInstanceApplication();
                app.MainForm = f;
                app.StartupNextInstance += startupHandler;
                app.Run(Environment.GetCommandLineArgs());
            }
        }
    }
}
