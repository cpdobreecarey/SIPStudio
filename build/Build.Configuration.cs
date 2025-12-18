using Nuke.Common.Git;
using Nuke.Common.ProjectModel;

sealed partial class Build
{
    /// <summary>
    ///     Patterns of solution configurations for compiling.
    /// </summary>
    readonly string[] Configurations =
    [
        //"Release*"
        "Release R26"
    ];

    /// <summary>
    ///     Mapping configurations and their assembly versions.
    /// </summary>
    readonly Dictionary<string, string> AssemblyVersionsMap = new()
    {
        { "Release", "1.0.0" },
        { "Release R21", "2021.0.0" },
        { "Release R22", "2022.0.0" },
        { "Release R23", "2023.0.0" },
        { "Release R24", "2024.0.0" },
        { "Release R25", "2025.0.0" },
        { "Release R26", "2026.0.0" },
    };

    /// <summary>
    ///     Projects packed in the Autodesk Bundle.
    /// </summary>
    Project[] Bundles =>
    [
        Solution.Revit.SIPStudio
    ];

    /// <summary>
    ///     Mapping between used installer project and the project containing the installation files.
    /// </summary>
    Dictionary<Project, Project> InstallersMap => new()
    {
        {Solution.Automation.Installer, Solution.Revit.SIPStudio}
    };

    /// <summary>
    ///     Path to build output.
    /// </summary>
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";

    /// <summary>
    ///     Releases changelog path.
    /// </summary>
    readonly AbsolutePath ChangelogPath = RootDirectory / "Changelog.md";

    /// <summary>
    ///     Add-in release version, includes version number and release stage.
    /// </summary>
    /// <remarks>Supported version format: <c>version-environment.n.date</c>.</remarks>
    /// <example>
    ///     1.0.0-alpha.1.250101 <br/>
    ///     1.0.0-beta.2.250101 <br/>
    ///     1.0.0
    /// </example>
    [Parameter] string ReleaseVersion;

    /// <summary>
    ///     Numeric release version without a stage.
    /// </summary>
    string ReleaseVersionNumber => ReleaseVersion?.Split('-')[0];

    /// <summary>
    ///     Determines whether the Revit add-ins release is preview.
    /// </summary>
    bool IsPrerelease
    {
        get
        {
            string env = Environment.GetEnvironmentVariable("IS_PRERELEASE");

            if (bool.TryParse(env, out bool fromEnvironment))
                return fromEnvironment;

            return ReleaseVersion != ReleaseVersionNumber;
        }
    }

    /// <summary>
    ///     Git repository metadata.
    /// </summary>
    [GitRepository] readonly GitRepository GitRepository;
    
    /// <summary>
    ///     Solution structure metadata.
    /// </summary>
    [Solution(GenerateProjects = true)] Solution Solution;

    /// <summary>
    ///     Set not-defined properties.
    /// </summary>
    protected override void OnBuildInitialized()
    {
        ReleaseVersion ??= GitRepository.Tags.SingleOrDefault();
    }
}