using System.Diagnostics;
using WixSharp.CommonTasks;

namespace Installer;

/// <summary>
///     Installer versions metadata.
/// </summary>
public sealed class Versions
{
    public required Version InstallerVersion { get; init; }
    public required Version AssemblyVersion { get; init; }
    public int RevitVersion { get; init; }
}

public static class Tools
{
    /// <summary>
    ///     Compute installer versions based on the SIPStudio.dll file.
    /// </summary>
    public static Versions ComputeVersions(string[] args)
    {
        foreach (var directory in args)
        {
            var assemblies = Directory.GetFiles(directory, "SIPStudio.dll", SearchOption.AllDirectories);
            if (assemblies.Length == 0) continue;

            var projectAssembly = FileVersionInfo.GetVersionInfo(assemblies[0]);
            var version = new Version(projectAssembly.FileVersion).ClearRevision();

            return new Versions
            {
                AssemblyVersion = version,
                RevitVersion = version.Major,
                InstallerVersion = version.Major > 255 ? new Version(version.Major % 100, version.Minor, version.Build) : version
            };
        }

        throw new Exception("SIPStudio.dll could not be found");
    }
}