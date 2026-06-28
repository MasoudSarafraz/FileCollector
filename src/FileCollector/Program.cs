using System;
using System.Windows.Forms;
using FileCollector.Core;
using FileCollector.Forms;

namespace FileCollector
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                // Initialize logger first
                LogManager.Initialize();

                // Initialize local SQLite database
                DatabaseManager.InitializeLocalDatabase();

                LogManager.Info("Application started.");

                Application.Run(new MainForm());

                LogManager.Info("Application exiting.");
                LogManager.Shutdown();
            }
            catch (Exception ex)
            {
                LogManager.Error("Fatal error in Main", ex);
                MessageBox.Show(
                    "یک خطای بحرانی رخ داد:\n\n" + ex.Message + "\n\n" + ex.StackTrace,
                    "خطای برنامه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }
    }
}
