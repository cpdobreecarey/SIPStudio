using System.Text.Json.Serialization;

namespace SIPStudio.Abstractions.Models.GitHub;

/// <summary>
///     Represents a GitHub response asset.
/// </summary>
[Serializable]
public sealed class GutHubResponseAsset
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("browser_download_url")] public string? DownloadUrl { get; set; }
}