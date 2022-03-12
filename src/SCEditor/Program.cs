using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using SCEditor.Helpers;

namespace SCEditor
{
    internal static class Program
    {
        [SupportedOSPlatform("windows")]
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();

        internal static MainForm Interface;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            #region Debug
            #if DEBUG

            AllocConsole();
            Console.SetOut(new Prefixed());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SC Editor | Development Edition");

            #endif
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(Program.Interface = new MainForm());
            }
            catch
            {
                throw;
            }
            #region Debug
            #if DEBUG

            Console.WriteLine("Debugging done");
            Console.ReadLine();

            #endif
            #endregion
        }
    }
}
