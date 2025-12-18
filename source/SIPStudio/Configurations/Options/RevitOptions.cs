using Microsoft.Extensions.DependencyInjection;
using SIPStudio.Abstractions.Options;
using System.IO;

namespace SIPStudio.Configurations.Options;

public static class RevitOptions
{
    public static void AddRevitEnvironmentOptions(this IServiceCollection services)
    {
        services.Configure<RevitEnvironmentOptions>(options =>
        {
            options.UserConfigurationFilePath = Path.Combine(Context.Application.CurrentUsersDataFolderPath, "Revit.ini");
        });
    }
}