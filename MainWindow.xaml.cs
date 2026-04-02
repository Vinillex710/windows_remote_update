using System.Diagnostics;
using System.Windows;
using SystemControlApp.Controllers;


namespace SystemControlApp
{
    public partial class MainWindow : Window
    {
        private bool _usbEnabled;

        

        public MainWindow()
        {
            InitializeComponent();

            _usbEnabled = UsbController.IsUsbEnabled();

            // 🔒 Keep watchdog in background, never steal focus
            Loaded += (_, __) =>
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;

                
            };

            AppLauncherController.LaunchApp();


            CheckInternetStatus();
            LoadRecentCrashes();
        }

        private void VolumeSlider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeController.SetVolume((float)(e.NewValue / 100));
        }

        private void BrightnessSlider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            BrightnessController.SetBrightness((byte)e.NewValue);
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /t 0",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/r /t 0",
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }

        private void Sleep_Click(object sender, RoutedEventArgs e)
        {
            PowerController.Sleep();
        }

        private void RotateScreen_Click(object sender, RoutedEventArgs e)
        {
            DisplayController.RotateClockwise();
        }

        private void CaptureScreen_Click(object sender, RoutedEventArgs e)
        {
            string filePath = ScreenCaptureController.CaptureScreen();
            MessageBox.Show($"Screenshot saved to:\n{filePath}",
                "Screen Captured",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async void CheckInternetStatus()
        {
            InternetStatusText.Text = "Internet: Checking...";
            InternetStatusText.Foreground = System.Windows.Media.Brushes.Gray;

            bool connected = await InternetConnectionController.IsConnectedToGoogleAsync();

            if (connected)
            {
                InternetStatusText.Text = "Internet: Connected";
                InternetStatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                InternetStatusText.Text = "Internet: Not Connected";
                InternetStatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void CheckInternet_Click(object sender, RoutedEventArgs e)
        {
            CheckInternetStatus();
        }

        private void LoadRecentCrashes()
        {
            CrashList.Items.Clear();

            var crashes = CrashDetectionController.GetRecentCrashes(15);

            if (crashes.Count == 0)
            {
                CrashList.Items.Add("No recent application crashes detected.");
                return;
            }

            foreach (var crash in crashes)
            {
                CrashList.Items.Add(
                    $"{crash.Time:G} | {crash.ApplicationName} | " +
                    $"Module: {crash.FaultingModule} | Code: {crash.ExceptionCode}"
                );
            }
        }

        private void LaunchApp_Click(object sender, RoutedEventArgs e)
        {
            string exePath = @"C:\Program Files\Wauly Signage\wauly_app.exe";

            bool launched = AppLauncherController.LaunchApp(exePath);

            if (!launched)
            {
                MessageBox.Show(
                    "Failed to launch application.",
                    "Launch Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ToggleUSB_Click(object sender, RoutedEventArgs e)
        {
            if (_usbEnabled)
            {
                UsbController.DisableUSB();
                MessageBox.Show("USB Disabled");
            }
            else
            {
                UsbController.EnableUSB();
                MessageBox.Show("USB Enabled");
            }

            _usbEnabled = !_usbEnabled;
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = @"C:\Program Files\Wauly Signage\App";
            string serverXmlUrl = "https://yourserver.com/config.xml";

            var updater = new UpdateController(folderPath);
            await updater.CheckAndDownloadUpdate(serverXmlUrl);

            MessageBox.Show("Update downloaded (if available). Will install on next launch.");
        }

    }
}


