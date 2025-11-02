using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace SIPStudio.Configuration.Http;

public static class HttpClientConfiguration
{
    public static void AddHttpApiClients(this IServiceCollection services)
    {
        services.AddHttpClient("GitHubSource", client => client.BaseAddress = new Uri("https://api.github.com/repos/cpdobreecarey/SIPStudio/"));

        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.RemoveAllLoggers();
        });

        services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
    }
}