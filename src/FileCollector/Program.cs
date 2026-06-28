using System;
using System.Threading;
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
            // Catch unhandled exceptions on UI thread
            Application.ThreadException += OnThreadException;

            // Catch unhandled exceptions on non-UI threads
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            try
            {
                // Initialize logger first so subsequent init steps can log
                LogManager.Initialize();
                LogManager.Info("=== Application starting ===");

                // Initialize local SQLite database (history/dedup tables)
                DatabaseManager.InitializeLocalDatabase();

                // Start watching config.json for external edits (hot reload)
                ConfigManager.StartWatching();

                LogManager.Info("All subsystems initialized. Launching main window.");

                Application.Run(new MainForm());

                LogManager.Info("=== Application exiting normally ===");
                ConfigManager.StopWatching();
                LogManager.Shutdown();
            }
            catch (Exception ex)
            {
                LogManager.Fatal("Fatal error in Main", ex);
                ShowFatalError(ex);
            }
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                LogManager.Error("Unhandled UI thread exception", e.Exception);
            }
            catch { }

            ShowFatalError(e.Exception);
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                {
                    LogManager.Fatal("Unhandled non-UI thread exception (isTerminating=" + e.IsTerminating + ")", ex);
                }
                else
                {
                    LogManager.Error("Unhandled non-UI thread exception: " + (e.ExceptionObject?.ToString() ?? "null"));
                }
            }
            catch { }
        }

        private static void ShowFatalError(Exception ex)
        {
            try
            {
                MessageBox.Show(
                    "یک خطای بحرانی رخ داد:\n\n" + ex.Message + "\n\n" + ex.StackTrace,
                    "خطای برنامه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            catch
            {
                // If we can't even show a message box, just give up.
            }
        }
    }
}
