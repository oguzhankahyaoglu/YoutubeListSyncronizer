using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace YoutubeListSyncronizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Swagger.YTAPI.Client.Configuration.DefaultApiClient.BasePath = "http://ytdownloader-api.herokuapp.com/";
            Swagger.YTAPI.Client.Configuration.DefaultApiClient.RestClient.BaseUrl = new Uri("http://ytdownloader-api.herokuapp.com/");
            Application.Run(new Form1());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Logger.Log(ex);
            if (!System.Diagnostics.Debugger.IsAttached)
                Environment.Exit(System.Runtime.InteropServices.Marshal.GetHRForException(ex));
        }
    }
}
