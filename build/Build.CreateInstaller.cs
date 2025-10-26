using Serilog.Events;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    /// <summary>
    ///     Create the .msi installers.
    /// </summary>
    Target CreateInstaller => _ => _
        .DependsOn(CreateBundle)
        .Executes(() =>
        {
            const string configuration = "Release";

            foreach (var (wixInstaller, wixTarget) in InstallersMap)
            {
                Log.Information("Project: {Name}", wixTarget.Name);

                DotNetBuild(settings => settings
                    .SetProjectFile(wixInstaller)
                    .SetConfiguration(configuration)
                    .SetVersion(ReleaseVersionNumber)
                    .SetVerbosity(DotNetVerbosity.minimal));

                var builderFile = Directory
                    .EnumerateFiles(wixInstaller.Directory / "bin" / configuration, $"{wixInstaller.Name}.exe")
                    .FirstOrDefault()
                    .NotNull($"No installer builder was found for the project: {wixInstaller.Name}");

                // Use the bundle directory created by CreateBundle instead of Release directories
                var bundleName = $"{wixTarget.Name}.bundle";
                var bundlePath = ArtifactsDirectory / bundleName / bundleName;

                // Check if bundle directory exists, if not recreate it from zip
                if (!bundlePath.DirectoryExists())
                {
                    var zipPath = $"{ArtifactsDirectory / bundleName}.zip";
                    Assert.True(File.Exists(zipPath), $"Bundle zip not found: {zipPath}");

                    Log.Information("Extracting bundle from zip: {ZipPath}", zipPath);
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, ArtifactsDirectory / bundleName);
                }

                var arguments = bundlePath.ToString().DoubleQuoteIfNeeded();
                var process = ProcessTasks.StartProcess(builderFile, arguments, logInvocation: false, logger: InstallerLogger);
                process.AssertZeroExitCode();
            }
        });

    /// <summary>
    ///     Logs the output of the installer process.
    /// </summary>
    void InstallerLogger(OutputType outputType, string output)
    {
        if (outputType == OutputType.Err)
        {
            Log.Error(output);
            return;
        }

        var arguments = ArgumentsRegex.Matches(output);
        var logLevel = arguments.Count switch
        {
            0 => LogEventLevel.Debug,
            > 0 when output.Contains("error", StringComparison.OrdinalIgnoreCase) => LogEventLevel.Error,
            _ => LogEventLevel.Information
        };

        if (arguments.Count == 0)
        {
            Log.Write(logLevel, output);
            return;
        }

        var properties = arguments
            .Select(match => match.Value[1..^1])
            .Cast<object>()
            .ToArray();

        var messageTemplate = ArgumentsRegex.Replace(output, match => $"{{Property{match.Index}}}");
        Log.Write(logLevel, messageTemplate, properties);
    }
}