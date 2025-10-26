using System.Xml.Linq;
using Autodesk.PackageBuilder;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities;

sealed partial class Build
{
    /// <summary>
    ///     Create the Autodesk .bundle package.
    /// </summary>
    Target CreateBundle => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                Log.Information("Project: {Name}", project.Name);

                var targetDirectories = Directory.GetDirectories(project.Directory, "* Release *", SearchOption.AllDirectories);
                Assert.NotEmpty(targetDirectories, "No files were found to create a bundle");

                var bundleName = $"{project.Name}.bundle";
                var bundleRoot = ArtifactsDirectory / bundleName;
                var bundlePath = bundleRoot / bundleName;
                var manifestPath = bundlePath / "PackageContents.xml";
                var contentsDirectory = bundlePath / "Contents";
                foreach (var contentDirectory in targetDirectories)
                {
                    var version = YearRegex.Match(contentDirectory).Value;

                    Log.Information("Bundle files for version {Version}:", version);
                    CopyAssemblies(contentDirectory, contentsDirectory / version);
                }

                GenerateManifest(project, targetDirectories, manifestPath);
                CompressFolder(bundleRoot);
            }
        });

    /// <summary>
    ///     Generate the Autodesk manifest for the bundle.
    /// </summary>
    void GenerateManifest(Project project, string[] directories, AbsolutePath manifestDirectory)
    {
        BuilderUtils.Build<PackageContentsBuilder>(builder =>
        {
            var company = GetConfigurationValue(project, config => config.Name == "VendorId");
            var email = GetConfigurationValue(project, config => config.Name == "VendorEmail");
            var versions = directories
                .Select(path => YearRegex.Match(path).Value)
                .Select(int.Parse);

            builder.ApplicationPackage.Create()
                .ProductType(ProductTypes.Application)
                .AutodeskProduct(AutodeskProducts.Revit)
                .Name(Solution.Name)
                .AppVersion(ReleaseVersionNumber);

            builder.CompanyDetails.Create(company)
                .Email(email);

            foreach (var version in versions)
            {
                builder.Components.CreateEntry($"Revit {version}")
                    .RevitPlatform(version)
                    .AppName(project.Name)
                    .ModuleName($"./Contents/{version}/{project.Name}.addin");
            }
        }, manifestDirectory);
    }

    /// <summary>
    ///     Read the configuration from the .addin file.
    /// </summary>
    string GetConfigurationValue(Project project, Func<XElement, bool> filter)
    {
        var defaultValue = string.Empty;
        var configPath = project.Directory.GetFiles("*.addin").FirstOrDefault();

        if (configPath is null) return defaultValue;

        var configDocument = configPath.ReadXml();
        if (configDocument.Root is null) return defaultValue;

        var sectionElement = configDocument.Root.Elements().FirstOrDefault();
        if (sectionElement is null) return defaultValue;

        var configElement = sectionElement.Elements().FirstOrDefault(filter);
        if (configElement is null) return defaultValue;

        return configElement.Value;
    }

    /// <summary>
    ///     Compress the bundle into a Zip archive.
    /// </summary>
    static void CompressFolder(AbsolutePath bundleRoot)
    {
        var zipPath = $"{bundleRoot}.zip";
        bundleRoot.CompressTo(zipPath);
        bundleRoot.DeleteDirectory();

        Log.Information("Compressing into a Zip: {Name}", zipPath);
    }

    /// <summary>
    ///     Copy assemblies from the source to the target directory, flattening main files but preserving subdirectories.
    /// </summary>
    static void CopyAssemblies(string sourcePath, string targetPath)
    {
        Directory.CreateDirectory(targetPath);
        CopyDirectory(sourcePath, targetPath, flatten: true);
    }

    /// <summary>
    ///     Recursively copy directory with optional flattening for project-named folders.
    /// </summary>
    static void CopyDirectory(string sourcePath, string targetPath, bool flatten = false)
    {
        // Copy all files in current directory
        foreach (string filePath in Directory.GetFiles(sourcePath))
        {
            string fileName = Path.GetFileName(filePath);
            string targetFilePath = Path.Combine(targetPath, fileName);

            Log.Information("{Assembly}", filePath);
            File.Copy(filePath, targetFilePath, true);
        }

        // Handle subdirectories
        foreach (string dirPath in Directory.GetDirectories(sourcePath))
        {
            string dirName = Path.GetFileName(dirPath);

            // If flattening is enabled and this is a project-named directory, flatten its contents
            if (flatten && dirName.Equals("SIPStudio", StringComparison.OrdinalIgnoreCase))
            {
                CopyDirectory(dirPath, targetPath, flatten: false);
            }
            else
            {
                // Preserve subdirectory structure (like runtimes, etc.)
                string targetDirPath = Path.Combine(targetPath, dirName);
                Directory.CreateDirectory(targetDirPath);
                CopyDirectory(dirPath, targetDirPath, flatten: false);
            }
        }
    }
}