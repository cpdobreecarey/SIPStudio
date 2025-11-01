namespace SIPStudio.Abstractions.Services.Application;

/// <summary>
///     Software update provider.
/// </summary>
public interface ISoftwareUpdateService
{
    /// <summary>
    ///     A new available version to download.
    /// </summary>
    public string? NewVersion { get; }

    /// <summary>
    ///     The URL to the release notes of the new version.
    /// </summary>
    public string? ReleaseNotesUrl { get; }

    /// <summary>
    ///     The local file path to the downloaded update.
    /// </summary>
    public string? LocalFilePath { get; }

    /// <summary>
    ///     The date of the latest check for updates.
    /// </summary>
    public DateTime? LatestCheckDate { get; }

    /// <summary>
    ///     Check for updates on the server.
    /// </summary>
    Task<bool> CheckUpdatesAsync();

    /// <summary>
    ///     Download the update from the server.
    /// </summary>
    Task DownloadUpdate();
}