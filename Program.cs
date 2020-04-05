using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AlphamaConverter
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


            Application.Run(new Converter());
        }

        //static void RestartApp(int pid, string applicationName)
        //{
        //    // Wait for the process to terminate
        //    Process process = null;
        //    try
        //    {
        //        process = Process.GetProcessById(pid);
        //        process.WaitForExit(1000);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        // ArgumentException to indicate that the 
        //        // process doesn't exist?   LAME!!
        //    }
        //    Process.Start(applicationName, "");
        //}


    }
}
