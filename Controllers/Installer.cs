using System.IO;

public static class Installer
{
    public static void InstallIfAvailable(string folderPath)
    {
        string exePath = Path.Combine(folderPath, "wauly_app.exe");
        string newExePath = Path.Combine(folderPath, "wauly_app_new.exe");

        string xmlPath = Path.Combine(folderPath, "config.xml");
        string newXmlPath = Path.Combine(folderPath, "config_new.xml");

        if (File.Exists(newExePath))
        {
            if (File.Exists(exePath))
                File.Delete(exePath);

            File.Move(newExePath, exePath);

            if (File.Exists(newXmlPath))
            {
                if (File.Exists(xmlPath))
                    File.Delete(xmlPath);

                File.Move(newXmlPath, xmlPath);
            }
        }
    }
}