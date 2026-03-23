using System;
using System.Diagnostics;

namespace SystemControlApp.Controllers
{
    public static class HdmiController
    {
        /// <summary>
        /// Disables all external display outputs (HDMI / DP / VGA)
        /// Requires Administrator privileges
        /// </summary>
        public static void DisableExternalDisplays()
        {
            RunDevconCommand("disable \"DISPLAY\\*\"");
        }

        /// <summary>
        /// Re-enables external displays
        /// </summary>
        public static void EnableExternalDisplays()
        {
            RunDevconCommand("enable \"DISPLAY\\*\"");
        }

        private static void RunDevconCommand(string arguments)
        {
            string devconPath = @"C:\Windows\System32\devcon.exe";

            if (!System.IO.File.Exists(devconPath))
                throw new InvalidOperationException("devcon.exe not found");

            var psi = new ProcessStartInfo
            {
                FileName = devconPath,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas", // ADMIN
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
    }
}
