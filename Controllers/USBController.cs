using Microsoft.Win32;

namespace SystemControlApp.Controllers
{
    public static class UsbController
    {
        private const string RegistryPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";

        public static void EnableUSB()
        {
            SetUsbState(3);
        }

        public static void DisableUSB()
        {
            SetUsbState(4);
        }

        public static bool IsUsbEnabled()
        {
            using var key = Registry.LocalMachine.OpenSubKey(RegistryPath, false);
            var value = key?.GetValue("Start");

            return value != null && (int)value == 3;
        }

        private static void SetUsbState(int value)
        {
            using var key = Registry.LocalMachine.OpenSubKey(RegistryPath, true);
            key?.SetValue("Start", value, RegistryValueKind.DWord);
        }
    }
}