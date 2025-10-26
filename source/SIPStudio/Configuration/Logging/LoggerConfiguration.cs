using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SIPStudio.Configuration.Logging;

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
            .WriteTo.Debug(LogEventLevel.Debug, LogTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Exception exception = (Exception)args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        logger.LogCritical(exception, "Domain unhandled exception");
    }
}