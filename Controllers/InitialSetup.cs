using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public static class InitialSetup
{
    public static async Task EnsureAppExists(string folderPath, string serverXmlUrl)
    {
        Directory.CreateDirectory(folderPath);

        string exePath = Path.Combine(folderPath, "wauly_app.exe");
        string xmlPath = Path.Combine(folderPath, "config.xml");

        if (File.Exists(exePath) && File.Exists(xmlPath))
            return;

        using var http = new HttpClient();

        var xmlString = await http.GetStringAsync(serverXmlUrl);

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AppConfig));
        AppConfig config;

        using (var reader = new StringReader(xmlString))
        {
            config = (AppConfig)serializer.Deserialize(reader);
        }

        var exeBytes = await http.GetByteArrayAsync(config.DownloadUrl);
        await File.WriteAllBytesAsync(exePath, exeBytes);

        XmlHelper.Save(xmlPath, config);
    }
}