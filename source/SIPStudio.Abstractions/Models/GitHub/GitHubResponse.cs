using System.Text.Json.Serialization;

namespace SIPStudio.Abstractions.Models.GitHub;

/// <summary>
///     Represents a GitHub API response.
/// </summary>
[Serializable]
public sealed class GitHubResponse
{
    [JsonPropertyName("html_url")] public string? Url { get; set; }
    [JsonPropertyName("tag_name")] public string? TagName { get; set; }
    [JsonPropertyName("draft")] public bool Draft { get; set; }
    [JsonPropertyName("prerelease")] public bool PreRelease { get; set; }
    [JsonPropertyName("published_at")] public DateTimeOffset PublishedDate { get; set; }
    [JsonPropertyName("assets")] public List<GutHubResponseAsset>? Assets { get; set; }
}