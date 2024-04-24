using System;
using System.IO;
using System.Windows.Forms;

namespace TDown
{
    static class Program
    {
        public static System.Diagnostics.Process Pro = System.Diagnostics.Process.GetCurrentProcess();
        public static string ExePath = Pro.MainModule.FileName;
        public static string BasePath = new FileInfo(ExePath).DirectoryName + Path.DirectorySeparatorChar;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
