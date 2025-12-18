using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIPStudio.Abstractions.Services.Application;
using SIPStudio.Configurations.Http;
using SIPStudio.Configurations.Logging;
using SIPStudio.Configurations.Options;
using SIPStudio.Services.Application;
using SIPStudio.Services.Configuration;
using System.IO;
using System.Reflection;

namespace SIPStudio;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IHost? _host;

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static void Start()
    {
        HostApplicationBuilder builder = new(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true
        });

        //Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilogConfiguration();

        //Options
        builder.Services.AddApplicationOptions();
        builder.Services.AddResourceLocationsOptions();
        builder.Services.AddRevitEnvironmentOptions();

        //Application services
        builder.Services.AddHttpApiClients();
        builder.Services.AddSingleton<ISoftwareUpdateService, SoftwareUpdateService>();
        builder.Services.AddSingleton<RevitRibbonService>();
        builder.Services.AddHostedService<HostBackgroundService>();

        //Configuration services
        builder.Services.AddSingleton<RevitTemplateService>();

        _host = builder.Build();
        _host.Start();
    }

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static void Stop()
    {
        _host!.StopAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
        return _host!.Services.GetRequiredService<T>();
    }
}