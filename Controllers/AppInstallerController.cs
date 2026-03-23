using System.Diagnostics;
using System.IO;

namespace SystemControlApp.Controllers
{
    public static class AppInstallerController
    {
        private static readonly string BasePath =
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "WaulySignage");

        private static readonly string InstallDirectory =
            @"C:\Program Files\Wauly Signage";

        public static void InstallIfAvailable()
        {
            var exeFiles = Directory.GetFiles(BasePath, "downloaded_*.exe");

            if (exeFiles.Length == 0)
                return;

            string exePath = exeFiles[0];

            KillRunningApp();
            CleanInstallDirectory();

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART",
                UseShellExecute = true
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();
        }

        private static void KillRunningApp()
        {
            foreach (var proc in Process.GetProcessesByName("wauly_app"))
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
                catch { }
            }
        }

        private static void CleanInstallDirectory()
        {
            try
            {
                if (Directory.Exists(InstallDirectory))
                {
                    Directory.Delete(InstallDirectory, true);
                }
            }
            catch { }
        }
    }
}