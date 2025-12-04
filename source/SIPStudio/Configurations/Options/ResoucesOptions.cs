using Microsoft.Extensions.DependencyInjection;
using SIPStudio.Abstractions.Options;

namespace SIPStudio.Configurations.Options;

public static class ResourcesOptions
{
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

            string appVersionSuffix = Context.Application.VersionNumber[^2..];

            //Local directories
            options.DownloadsDirectory = options.LocalApplicationDataDirectory.AppendPath("Downloads");

            //Roaming directories
            options.LibrariesDirectory = options.ApplicationDataDirectory.AppendPath("Libraries");

            //Roaming files
            options.DefaultTemplateFileName = $"R{appVersionSuffix} Base Template.rte";
            options.DefaultTemplatePath = options.LibrariesDirectory.AppendPath(options.DefaultTemplateFileName);
        });
    }
}