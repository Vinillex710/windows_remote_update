using System.Runtime.InteropServices;

namespace SystemControlApp.Controllers
{
    public static class PowerController
    {
        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern bool SetSuspendState(
            bool hibernate,
            bool forceCritical,
            bool disableWakeEvent);

        public static void Sleep()
        {
            // false = sleep (not hibernate)
            // false = allow apps to veto
            // false = allow wake timers
            SetSuspendState(false, false, false);
        }
    }
}
