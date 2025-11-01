using Microsoft.Extensions.DependencyInjection;
using SIPStudio.Abstractions.Options;

namespace SIPStudio.Configuration.Options;

public static class ResourcesOptions
{
    /// <summary>
    ///     Add add-in folders and file paths configuration
    /// </summary>
    public static void AddResourceLocationsOptions(this IServiceCollection services)
    {
        services.Configure<ResourceLocationsOptions>(options =>
        {
            //options.ApplicationDataDirectory = Environment
            //    .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            //    .AppendPath("SIPStudio")
            //    .AppendPath(Context.Application.VersionNumber);

            options.LocalApplicationDataDirectory = Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AppendPath("SIPStudio")
                .AppendPath(Context.Application.VersionNumber);

            //Local directories
            options.DownloadsFolder = options.LocalApplicationDataDirectory.AppendPath("Downloads");
            //options.LogsFolder = options.LocalApplicationDataDirectory.AppendPath("Logs");

            //Local files
            //options.LogPath = options.LogsFolder.AppendPath("SIPStudio-log.txt");

            //Roaming directories
            //options.SettingsDirectory = options.ApplicationDataDirectory.AppendPath("Settings");

            //Roaming files
            //options.ApplicationSettingsPath = options.SettingsDirectory.AppendPath("Application.json");
        });
    }
}