using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class UpdateController
{
    private readonly string _folderPath;
    private readonly string _localXmlPath;
    private readonly string _newExePath;
    private readonly string _newXmlPath;

    public UpdateController(string folderPath)
    {
        _folderPath = folderPath;

        _localXmlPath = Path.Combine(folderPath, "config.xml");
        _newExePath = Path.Combine(folderPath, "wauly_app_new.exe");
        _newXmlPath = Path.Combine(folderPath, "config_new.xml");
    }

    public async Task CheckAndDownloadUpdate(string serverXmlUrl)
    {
        using var http = new HttpClient();

        var xmlString = await http.GetStringAsync(serverXmlUrl);

        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AppConfig));
        AppConfig serverConfig;

        using (var reader = new StringReader(xmlString))
        {
            serverConfig = (AppConfig)serializer.Deserialize(reader);
        }

        var localConfig = XmlHelper.Load(_localXmlPath);

        if (localConfig == null || serverConfig.Version != localConfig.Version)
        {
            var exeBytes = await http.GetByteArrayAsync(serverConfig.DownloadUrl);
            await File.WriteAllBytesAsync(_newExePath, exeBytes);

            XmlHelper.Save(_newXmlPath, serverConfig);
        }
    }
}