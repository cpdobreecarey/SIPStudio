using WixSharp;
using WixSharp.CommonTasks;

namespace Installer;

public static class ResourceGenerator
{
    public static Dir GenerateAppDataDirectory(int version)
    {
        string revitResourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Revit");
        //string imperialFamiliesPath = Path.Combine(revitResourcesPath, "Families", "Imperial");
        //string metricFamiliesPath = Path.Combine(revitResourcesPath, "Families", "Metric");
        string templatesPath = Path.Combine(revitResourcesPath, "Templates");
        string texturesPath = Path.Combine(revitResourcesPath, "Textures");

        //WixEntity[] imperialfamilyEntities = GenerateWixFileEntities(imperialFamiliesPath);
        //WixEntity[] metricfamilyEntities = GenerateWixFileEntities(metricFamiliesPath);
        WixEntity[] templateEntities = GenerateWixFileEntities(templatesPath);
        WixEntity[] textureEntities = GenerateWixFileEntities(texturesPath);

        Dir librariesDir = new("Libraries");
        librariesDir.AddFiles([.. templateEntities.OfType<WixSharp.File>()]);
        librariesDir.AddDir(new Dir("Textures", textureEntities));

        return new Dir(new Id($"SIPSTUDIO_APPDATA_{version}"), $@"SIPStudio\{version}", librariesDir);
    }

    private static WixEntity[] GenerateWixFileEntities(string path) => [
        .. Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                    .Select(p => new WixSharp.File(p))
    ];
}