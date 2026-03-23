using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SystemControlApp.Controllers
{
    public class WatchdogController
    {
        private readonly string _exePath;
        private readonly string _exeName;
        private Process? _process;
        private CancellationTokenSource? _cts;

        public WatchdogController(string exePath)
        {
            _exePath = exePath;
            _exeName = Path.GetFileNameWithoutExtension(exePath);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => MonitorLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_process == null || _process.HasExited)
                    {
                        LaunchOrAttach();
                    }
                }
                catch
                {
                    // swallow to keep watchdog alive
                }

                await Task.Delay(2000, token);
            }
        }

        private void LaunchOrAttach()
        {
            var existing = Process.GetProcessesByName(_exeName).FirstOrDefault();
            if (existing != null)
            {
                _process = existing;
                BringToFront(existing);
                HookExit(existing);
                return;
            }

            _process = Process.Start(new ProcessStartInfo
            {
                FileName = _exePath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(_exePath)
            });

            if (_process != null)
            {
                HookExit(_process);
                Task.Delay(1000).ContinueWith(_ => BringToFront(_process));
            }
        }

        private void HookExit(Process process)
        {
            process.EnableRaisingEvents = true;
            process.Exited += (_, __) => _process = null;
        }

        private void BringToFront(Process process)
        {
            if (process.MainWindowHandle != IntPtr.Zero)
            {
                SetForegroundWindow(process.MainWindowHandle);
            }
        }

    }
}
