using System.Xml.Serialization;

[XmlRoot("AppConfig")]
public class AppConfig
{
    public string Version { get; set; } = "";
    public string DownloadUrl { get; set; } = "";
}