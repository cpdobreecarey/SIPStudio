using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIPStudio.Abstractions.Options;
using SIPStudio.Abstractions.Services.Application;
using SIPStudio.Common.Tools;
using System.IO;
using WixToolset.Dtf.WindowsInstaller;

namespace SIPStudio.Services.Application;

/// <summary>
///     Provides life cycle processes for the application
/// </summary>
public sealed class HostBackgroundService(
    IOptions<AssemblyOptions> assemblyOptions,
    IOptions<ResourceLocationsOptions> foldersOptions,
    ISoftwareUpdateService updateService,
    ILogger<HostBackgroundService> logger)
    : IHostedService
{
    private readonly string _downloadsDirectory = foldersOptions.Value.DownloadsDirectory;
    private readonly Version _currentVersion = assemblyOptions.Value.Version;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Current Version: {Version}", _currentVersion);

        ClearDownloadCache();
        _ = CheckUpdatesAsync();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        UpdateSoftware();
        return Task.CompletedTask;
    }

    private async Task CheckUpdatesAsync()
    {
        try
        {
            bool hasUpdates = await updateService.CheckUpdatesAsync();
            if (!hasUpdates) return;

            logger.LogInformation("SIPStudio {Version} is available to download", updateService.NewVersion);

            await DownloadUpdateAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Update service error");
        }
    }

    private async Task DownloadUpdateAsync()
    {
        try
        {
            await updateService.DownloadUpdate();
            logger.LogInformation("Download complete: {Path}", updateService.LocalFilePath);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Downloading update failed");
        }
    }

    private void UpdateSoftware()
    {
        if (!File.Exists(updateService.LocalFilePath)) return;

        logger.LogInformation("Installing SIPStudio {Version} version", updateService.NewVersion);
        ProcessTasks.StartShell(updateService.LocalFilePath!);
    }

    public void ClearDownloadCache()
    {
        if (!Directory.Exists(_downloadsDirectory)) return;

        foreach (string msi in Directory.EnumerateFiles(_downloadsDirectory, "*.msi", SearchOption.TopDirectoryOnly))
        {
            Version? msiVersion = null;
            using (Database dataBase = new(msi))
            {
                if (dataBase.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductVersion'")
                    is not string version) continue;

                msiVersion = Version.Parse(version);
            }

            if (msiVersion is null || msiVersion > _currentVersion) continue;

            File.Delete(msi);
            logger.LogInformation("{Msi} installer removed from Downloads", Path.GetFileName(msi));
        }
    }
}