using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using SystemControlApp.Controllers;

namespace SystemControlApp
{
    public partial class App : System.Windows.Application
    {
        private static Mutex? _mutex;
        private WatchdogController? _watchdog;
        private bool _isShuttingDownCleanly = false;

        protected override async void OnStartup(StartupEventArgs e)
        {
            const string mutexName = @"Global\SystemControlApp_Watchdog";

            bool isNewInstance;
            _mutex = new Mutex(true, mutexName, out isNewInstance);

            if (!isNewInstance)
            {
                Shutdown();
                return;
            }

            // Catch ALL crashes
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            base.OnStartup(e);

            string folderPath = @"C:\Program Files\Wauly Signage\App";
            string targetExePath = Path.Combine(folderPath, "wauly_app.exe");
            string serverXmlUrl = "http://10.244.96.210/config.xml";

        

            //// =========================
            //// 📦 INITIAL SETUP
            //// =========================
            //await InitialSetup.EnsureAppExists(folderPath, serverXmlUrl);

            ////// =========================
            ////// 🔁 INSTALL DOWNLOADED UPDATE (if any)
            ////// =========================
            //Installer.InstallIfAvailable(folderPath);

            // =========================
            // 🚀 START WATCHDOG
            // =========================
            _watchdog = new WatchdogController(targetExePath);
            _watchdog.Start();


            // =========================
            // 🪟 UI WINDOW
            // =========================
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;

            mainWindow.Closed += (_, __) =>
            {
                if (!_isShuttingDownCleanly)
                {
                    RelaunchSelf();
                }
            };

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _isShuttingDownCleanly = true;

            _watchdog?.Stop();
            _mutex?.ReleaseMutex();

            base.OnExit(e);
        }

        // =========================
        // 🔁 SELF-RELAUNCH LOGIC
        // =========================

        private void RelaunchSelf()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule!.FileName!,
                    UseShellExecute = true
                });
            }
            catch
            {
                // fallback
            }
            finally
            {
                _isShuttingDownCleanly = true;
                Shutdown();
            }
        }

        private void OnDispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            RelaunchSelf();
        }

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            RelaunchSelf();
        }
    }
}