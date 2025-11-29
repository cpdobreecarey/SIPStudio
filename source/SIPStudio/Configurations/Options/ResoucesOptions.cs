using Microsoft.Extensions.DependencyInjection;
using SIPStudio.Abstractions.Options;

namespace SIPStudio.Configurations.Options;

public static class ResourcesOptions
{
    /// <summary>
    ///     Add add-in folders and file paths configuration
    /// </summary>
    public static void AddResourceLocationsOptions(this IServiceCollection services)
    {
        services.Configure<ResourceLocationsOptions>(options =>
        {
            options.ApplicationDataDirectory = Environment
                .GetFolderPath(Environment.SpecialFolder.ApplicationData)
                .AppendPath("SIPStudio")
                .AppendPath(Context.Application.VersionNumber);

            options.LocalApplicationDataDirectory = Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AppendPath("SIPStudio")
                .AppendPath(Context.Application.VersionNumber);

            //Local directories
            options.DownloadsDirectory = options.LocalApplicationDataDirectory.AppendPath("Downloads");

            //Roaming directories
            options.LibrariesDirectory = options.ApplicationDataDirectory.AppendPath("Libraries");
            options.MaterialTexturesDirectory = options.LibrariesDirectory.AppendPath("Textures");
            options.ImperialFamiliesDirectory = options.LibrariesDirectory.AppendPath("Imperial");
            options.MetricFamiliesDirectory = options.LibrariesDirectory.AppendPath("Metric");
            //options.ConfigurationsDirectory = options.ApplicationDataDirectory.AppendPath("Configs");

            //Roaming files
            //options.ApplicationConfigurationsPath = options.ConfigurationsDirectory.AppendPath("Application.json");
        });
    }
}