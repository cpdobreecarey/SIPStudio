using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SIPStudio.Configuration.Options;

public static class ApplicationOptions
{
    /// <summary>
    ///    Add global Host configuration
    /// </summary>
    public static void AddApplicationOptions(this IServiceCollection services)
    {
        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
    }
}