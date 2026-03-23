using System.Diagnostics;
using System.IO;

namespace SystemControlApp.Controllers
{
    public static class AppLauncherController
    {
        private static readonly string InstalledPath =
            @"C:\Program Files\Wauly Signage\wauly_app.exe";

        public static bool IsInstalled()
        {
            return File.Exists(InstalledPath);
        }

        public static bool LaunchApp(string? arguments = null)
        {
            try
            {
                if (!IsInstalled())
                    return false;

                var startInfo = new ProcessStartInfo
                {
                    FileName = InstalledPath,
                    Arguments = arguments ?? "",
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(InstalledPath)
                };

                Process.Start(startInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}