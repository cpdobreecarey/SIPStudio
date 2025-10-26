using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    /// <summary>
    ///     Compile all solution configurations.
    /// </summary>
    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            foreach (var configuration in GlobBuildConfigurations())
            {
                AssemblyVersionsMap.TryGetValue(configuration, out var version);

                DotNetBuild(settings => settings
                    .SetProjectFile(Solution)
                    .SetConfiguration(configuration)
                    .SetVersion(version)
                    .SetVerbosity(DotNetVerbosity.minimal));
            }
        });
}