using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace SIPStudio.Configurations.Logging;

/// <summary>
///     Application logging configuration
/// </summary>
/// <example>
/// <code lang="csharp">
/// public class Class(ILogger&lt;Class&gt; logger)
/// {
///     private void Execute()
///     {
///         logger.LogInformation("Message");
///     }
/// }
/// </code>
/// </example>
public static class LoggerConfiguration
{
    private static readonly string LogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        .AppendPath("SIPStudio")
        .AppendPath(Context.Application.VersionNumber)
        .AppendPath("LogSIPStudio_.txt");

    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static void AddSerilogConfiguration(this ILoggingBuilder builder)
    {
        Logger logger = CreateDefaultLogger();
        builder.AddSerilog(logger);

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }

    private static Logger CreateDefaultLogger()
    {
        return new Serilog.LoggerConfiguration()
            //.WriteTo.Debug(LogEventLevel.Debug, LogTemplate)
            //.MinimumLevel.Debug()
            .WriteTo.File(LogPath,
                outputTemplate: LogTemplate,
                fileSizeLimitBytes: 10000000,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2)
            .CreateLogger();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Exception exception = (Exception)args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        logger.LogCritical(exception, "Domain unhandled exception");
    }
}