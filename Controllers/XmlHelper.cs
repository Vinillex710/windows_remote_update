using System.IO;
using System.Xml.Serialization;

public static class XmlHelper
{
    public static AppConfig Load(string path)
    {
        if (!File.Exists(path))
            return null;

        var serializer = new XmlSerializer(typeof(AppConfig));
        using var stream = File.OpenRead(path);
        return (AppConfig)serializer.Deserialize(stream);
    }

    public static void Save(string path, AppConfig config)
    {
        var serializer = new XmlSerializer(typeof(AppConfig));
        using var stream = File.Create(path);
        serializer.Serialize(stream, config);
    }
}