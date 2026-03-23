using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SystemControlApp.Controllers
{
    public static class ScreenCaptureController
    {
        public static string CaptureScreen()
        {
            var screen = Screen.PrimaryScreen
                ?? throw new InvalidOperationException("No primary screen detected.");

            var bounds = screen.Bounds;

            using Bitmap bitmap = new Bitmap(
                bounds.Width,
                bounds.Height,
                PixelFormat.Format32bppArgb);

            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(
                bounds.Left,
                bounds.Top,
                0,
                0,
                bounds.Size,
                CopyPixelOperation.SourceCopy);

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"ScreenCapture_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            bitmap.Save(filePath, ImageFormat.Png);

            return filePath;
        }
    }
}
