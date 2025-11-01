using Microsoft.Extensions.Options;
using SIPStudio.Abstractions.Models.GitHub;
using SIPStudio.Abstractions.Options;
using SIPStudio.Abstractions.Services.Application;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SIPStudio.Services.Application;

public sealed class SoftwareUpdateService(
    IHttpClientFactory httpFactory,
    IOptions<AssemblyOptions> assemblyOptions,
    IOptions<ResourceLocationsOptions> foldersOptions)
    : ISoftwareUpdateService
{
    private string? _downloadUrl;
    private readonly AssemblyOptions _assemblyOptions = assemblyOptions.Value;
    private readonly ResourceLocationsOptions _folderOptions = foldersOptions.Value;
    private readonly Regex _versionRegex = new(@"(\d+\.)+\d+", RegexOptions.Compiled);

    public string? NewVersion { get; private set; }
    public string? ReleaseNotesUrl { get; private set; }
    public string? LocalFilePath { get; private set; }
    public DateTime? LatestCheckDate { get; private set; }

    public async Task<bool> CheckUpdatesAsync()
    {
        LatestCheckDate = DateTime.Now;
        if (CheckExistingInstaller()) return true;

        var releases = await FetchGithubRepositoryAsync();
        if (releases.Count == 0) return false;

        var latestRelease = releases
            .Where(response => !response.Draft)
            .Where(response => !response.PreRelease)
#if NETCOREAPP
            .MaxBy(release => release.PublishedDate);
#else
            .OrderByDescending(release => release.PublishedDate)
            .FirstOrDefault();
#endif

        if (latestRelease is null) return false;
        ReleaseNotesUrl = latestRelease.Url;

        var newVersionTag = FindNewServerVersion(latestRelease);
        if (newVersionTag is null) return false;
        if (newVersionTag <= _assemblyOptions.Version) return false;

        NewVersion = newVersionTag.ToString(3);

        string newVersionFileName = Path.GetFileName(_downloadUrl!);
        string newVersionPath = Path.Combine(_folderOptions.DownloadsFolder, newVersionFileName);
        if (File.Exists(newVersionPath))
        {
            LocalFilePath = newVersionPath;
        }

        return true;
    }

    public async Task DownloadUpdate()
    {
        Directory.CreateDirectory(_folderOptions.DownloadsFolder);
        string fileName = Path.Combine(_folderOptions.DownloadsFolder, Path.GetFileName(_downloadUrl)!);

        HttpClient httpClient = httpFactory.CreateClient();
        Stream response = await httpClient.GetStreamAsync(_downloadUrl);

#if NETCOREAPP
        await using var fileStream = new FileStream(fileName, FileMode.Create);
#else
        using var fileStream = new FileStream(fileName, FileMode.Create);
#endif
        await response.CopyToAsync(fileStream);

        LocalFilePath = fileName;
    }

    private Version? FindNewServerVersion(GitHubResponse latestRelease)
    {
        if (latestRelease.Assets is null) return null;

        Version? newVersionTag = null;
        foreach (var asset in latestRelease.Assets)
        {
            if (asset.Name is null) continue;

            Match match = _versionRegex.Match(asset.Name);
            if (!match.Success) continue;
            if (!match.Value.StartsWith(_assemblyOptions.Version.Major.ToString())) continue;
            if (!_assemblyOptions.HasAdminAccess && asset.Name.Contains("MultiUser")) continue;

            newVersionTag = new Version(match.Value);
            _downloadUrl = asset.DownloadUrl;
            break;
        }

        return newVersionTag;
    }

    private bool CheckExistingInstaller()
    {
        if (string.IsNullOrEmpty(LocalFilePath)) return false;
        if (!File.Exists(LocalFilePath)) return false;

        string fileName = Path.GetFileName(LocalFilePath)!;
        if (NewVersion is null) return false;
        if (!fileName.Contains(NewVersion)) return false;

        return true;
    }

    private async Task<List<GitHubResponse>> FetchGithubRepositoryAsync()
    {
        HttpClient httpClient = httpFactory.CreateClient("GitHubSource");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "SIPStudio");

        string releasesJson = await httpClient.GetStringAsync("releases");
        var responses = JsonSerializer.Deserialize<List<GitHubResponse>>(releasesJson);
        return responses ?? [];
    }
}