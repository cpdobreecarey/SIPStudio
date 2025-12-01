using WixSharp;
using WixSharp.CommonTasks;

namespace Installer;

public static class ResourceGenerator
{
    public static Dir GenerateAppDataDirectory(int version)
    {
        return new Dir(new Id($"SIPSTUDIO_APPDATA_{version}"), $@"%AppDataFolder%\SIPStudio\{version}", 
            CreateLibrariesDirectory()
        );
    }

    private static Dir CreateLibrariesDirectory()
    {
        Dir librariesDir = new("Libraries");

        WixEntity[] templateEntities = GenerateWixFileEntities(Resources.Resources.TemplatesDirectory);
        WixEntity[] textureEntities = GenerateWixFileEntities(Resources.Resources.TexturesDirectory);

        librariesDir.AddDir(new Dir("Families", new Dir("Imperial"), new Dir("Metric")));
        librariesDir.AddFiles([.. templateEntities.OfType<WixSharp.File>()]);
        librariesDir.AddDir(new Dir("Textures", textureEntities));

        return librariesDir;
    }

    private static WixEntity[] GenerateWixFileEntities(string path, string searchPatern = "*.*") => [
        .. Directory.GetFiles(path, searchPatern, SearchOption.TopDirectoryOnly)
                    .Select(p => new WixSharp.File(p))
    ];
}