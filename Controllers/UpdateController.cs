using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SystemControlApp.Controllers
{
    public static class UpdateController
    {
        private static readonly string BasePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WaulySignage");

        private static readonly string XmlPath = Path.Combine(BasePath, "update.xml");

        public static async Task<bool> CheckAndDownload()
        {
            Directory.CreateDirectory(BasePath);

            string xmlUrl = "http://192.168.0.106:8080/update.xml";
            string tempXml = Path.Combine(BasePath, "temp_update.xml");


            using (HttpClient client = new HttpClient())
            {
                var xmlData = await client.GetByteArrayAsync(xmlUrl);
                await File.WriteAllBytesAsync(tempXml, xmlData);
            }

            var newDoc = XDocument.Load(tempXml);
            string newVersion = newDoc.Root.Element("Version")?.Value;

            string currentVersion = GetLocalVersion();

            if (currentVersion != null &&
                Version.Parse(newVersion) <= Version.Parse(currentVersion))
            {
                return false;
            }

            string exeUrl = newDoc.Root.Element("ExeUrl")?.Value;
            string fileName = newDoc.Root.Element("FileName")?.Value;

            CleanupOldDownloads();

            string newExePath = Path.Combine(BasePath, "downloaded_" + fileName);

            using (HttpClient client = new HttpClient())
            {
                var exeData = await client.GetByteArrayAsync(exeUrl);
                await File.WriteAllBytesAsync(newExePath, exeData);
            }

            File.Copy(tempXml, XmlPath, true);

            return true;
        }

        public static bool IsFirstTimeInstall()
        {
            return GetLocalVersion() == null;
        }

        private static void CleanupOldDownloads()
        {
            var files = Directory.GetFiles(BasePath, "downloaded_*.exe");

            foreach (var file in files)
            {
                try { File.Delete(file); } catch { }
            }
        }

        private static string GetLocalVersion()
        {
            if (!File.Exists(XmlPath))
                return null;

            var doc = XDocument.Load(XmlPath);
            return doc.Root.Element("Version")?.Value;
        }
    }
}