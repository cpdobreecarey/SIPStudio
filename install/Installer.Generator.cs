using WixSharp;
using WixSharp.CommonTasks;

namespace Installer;

public static class Generator
{
    /// <summary>
    ///     Generates Wix entities for the installer from existing bundle structure.
    /// </summary>
    public static WixEntity[] GenerateWixEntities(IEnumerable<string> args, Version version)
    {
        List<WixEntity> entities = [];
        foreach (string directory in args)
        {
            Console.WriteLine($"Bundle installer files for version '{version}':");
            GenerateRootEntities(directory, entities);
        }

        return [.. entities];
    }

    /// <summary>
    ///     Generates root entities.
    /// </summary>
    private static void GenerateRootEntities(string directory, ICollection<WixEntity> entities)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            if (!FilterEntities(file)) continue;

            Console.WriteLine($"'{file}'");
            entities.Add(new WixSharp.File(file));
        }

        foreach (var folder in Directory.GetDirectories(directory))
        {
            var folderName = Path.GetFileName(folder);
            var entity = new Dir(folderName);
            entities.Add(entity);

            GenerateSubEntities(folder, entity);
        }
    }

    /// <summary>
    ///     Generates nested entities recursively.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="parent"></param>
    private static void GenerateSubEntities(string directory, Dir parent)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            if (!FilterEntities(file)) continue;

            Console.WriteLine($"'{file}'");
            parent.AddFile(new WixSharp.File(file));
        }

        foreach (var subfolder in Directory.GetDirectories(directory))
        {
            var folderName = Path.GetFileName(subfolder);
            var entity = new Dir(folderName);
            parent.AddDir(entity);

            GenerateSubEntities(subfolder, entity);
        }
    }

    /// <summary>
    ///     Filter installer files and exclude from output. 
    /// </summary>
    private static bool FilterEntities(string file)
    {
        return !file.EndsWith(".pdb");
    }
}