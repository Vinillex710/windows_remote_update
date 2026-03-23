using System;
using System.Runtime.InteropServices;

namespace SystemControlApp.Controllers
{
    public static class DisplayController
    {
        private const int ENUM_CURRENT_SETTINGS = -1;

        private const int DM_DISPLAYORIENTATION = 0x80;
        private const int DM_PELSWIDTH = 0x80000;
        private const int DM_PELSHEIGHT = 0x100000;

        private const int DISP_CHANGE_SUCCESSFUL = 0;

        private enum DisplayOrientation : int
        {
            Default = 0,
            Rotate90 = 1,
            Rotate180 = 2,
            Rotate270 = 3
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;

            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public DisplayOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;

            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettings(
            string? deviceName,
            int modeNum,
            ref DEVMODE devMode);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int ChangeDisplaySettings(
            ref DEVMODE devMode,
            int flags);

        public static void RotateClockwise()
        {
            var dm = new DEVMODE
            {
                dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE))
            };

            if (!EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
                return;

            var next = (DisplayOrientation)(((int)dm.dmDisplayOrientation + 1) % 4);

            bool swapDimensions =
                (dm.dmDisplayOrientation == DisplayOrientation.Default ||
                 dm.dmDisplayOrientation == DisplayOrientation.Rotate180) &&
                (next == DisplayOrientation.Rotate90 ||
                 next == DisplayOrientation.Rotate270);

            dm.dmDisplayOrientation = next;
            dm.dmFields = DM_DISPLAYORIENTATION;

            if (swapDimensions)
            {
                uint temp = dm.dmPelsWidth;
                dm.dmPelsWidth = dm.dmPelsHeight;
                dm.dmPelsHeight = temp;
                dm.dmFields |= DM_PELSWIDTH | DM_PELSHEIGHT;
            }

            ChangeDisplaySettings(ref dm, 0);
        }
    }
}
