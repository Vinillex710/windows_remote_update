using System.Management;

namespace SystemControlApp.Controllers
{
    public static class BrightnessController
    {
        public static void SetBrightness(byte brightness)
        {
            var scope = new ManagementScope(@"\\.\root\wmi");
            scope.Connect();

            using var classInstance = new ManagementClass(
                scope,
                new ManagementPath("WmiMonitorBrightnessMethods"),
                null);

            foreach (ManagementObject instance in classInstance.GetInstances())
            {
                instance.InvokeMethod(
                    "WmiSetBrightness",
                    new object[] { 1, brightness });
            }
        }
    }
}
