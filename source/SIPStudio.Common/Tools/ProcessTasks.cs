using System.Diagnostics;
using System.Text;

namespace SIPStudio.Common.Tools;

/// <summary>
///     Tasks for starting and managing processes. Supports different APIs for .NET Core and .NET Framework.
/// </summary>
public static class ProcessTasks
{
    /// <summary>
    ///     Start a process and redirect its output to the logger
    /// </summary>
    public static Process? StartProcess(string toolPath, string arguments = "", Action<OutputType, string>? logger = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = toolPath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        var process = Process.Start(startInfo);
        if (process == null) return null;

        RedirectProcessOutput(process, logger);
        return process;
    }

    /// <summary>
    ///     Start a shell process
    /// </summary>
    public static Process? StartShell(string toolPath, string arguments = "")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = toolPath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = true
        };

        return Process.Start(startInfo);
    }

    /// <summary>
    ///     Redirect the process output to the logger
    /// </summary>
    private static void RedirectProcessOutput(Process process, Action<OutputType, string>? logger)
    {
        logger ??= DefaultLogger;
        process.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data)) return;
            logger.Invoke(OutputType.Standard, args.Data);
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data)) return;
            logger.Invoke(OutputType.Error, args.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    /// <summary>
    ///     Default logger for the process output
    /// </summary>
    private static void DefaultLogger(OutputType type, string output)
    {
        Console.WriteLine(output);
    }
}

/// <summary>
///     Process output type
/// </summary>
public enum OutputType
{
    Standard,
    Error
}