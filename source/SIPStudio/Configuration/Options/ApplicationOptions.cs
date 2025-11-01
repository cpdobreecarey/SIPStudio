using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SIPStudio.Abstractions.Options;
using SIPStudio.Common.Utils;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

namespace SIPStudio.Configuration.Options;

public static class ApplicationOptions
{
    /// <summary>
    ///    Add global Host configuration
    /// </summary>
    public static void AddApplicationOptions(this IServiceCollection services)
    {
        ConfigureConsoleOptions(services);
        ConfigureAssemblyOptions(services);
    }

    /// <summary>
    ///     Configures ConsoleLifetime to hide the Generic Host’s built-in startup/shutdown status lines
    /// </summary>
    private static void ConfigureConsoleOptions(IServiceCollection services)
    {
        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
    }

    /// <summary>
    ///     Captures assembly framework and version, infers admin scope from location/write access, and binds to AssemblyOptions
    /// </summary>
    private static void ConfigureAssemblyOptions(IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string assemblyLocation = assembly.Location;
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        Version fileVersion = new(FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion!);

        TargetFrameworkAttribute targetFrameworkAttribute = assembly
            .GetCustomAttributes(typeof(TargetFrameworkAttribute), true)
            .Cast<TargetFrameworkAttribute>()
            .First();

        services.Configure<AssemblyOptions>(options =>
        {
            options.Framework = targetFrameworkAttribute.FrameworkDisplayName ?? targetFrameworkAttribute.FrameworkName;
            options.Version = new Version(fileVersion.Major, fileVersion.Minor, fileVersion.Build);
            options.HasAdminAccess = assemblyLocation.StartsWith(appDataPath) || !AccessUtils.CheckWriteAccess(assemblyLocation);
        });
    }
}