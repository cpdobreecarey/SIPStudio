using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIPStudio.Abstractions.Options;
using System.IO;
using System.Text;

namespace SIPStudio.Services.Configuration;

public sealed class RevitTemplateService(IOptions<ResourceLocationsOptions> resourceOptions, ILogger<RevitTemplateService> logger)
{
    private const string DefaultTemplateKey = "DefaultTemplate=";
    private const char SectionPrefix = '[';
    private const char SectionSuffix = ']';
    private const string SipTemplateKey = "SIP Template=";

    public void RegisterDefaultTemplate(string revitConfigFile)
    {
        if (!File.Exists(revitConfigFile)) return;

        string[] lines = File.ReadAllLines(revitConfigFile, Encoding.Unicode);

        string? langCode = GetSelectedLanguageCode(lines);
        if (langCode is null)
        {
            logger.LogWarning("Could not determine Revit language selection from [Language] section.");
            return;
        }

        string templatePath = resourceOptions.Value.DefaultTemplatePath;
        if (string.IsNullOrWhiteSpace(templatePath))
        {
            logger.LogWarning("DefaultTemplatePath is not configured; skipping Revit.ini update.");
            return;
        }

        string schemaEntry = SipTemplateKey + templatePath;
        bool modified = TryUpdateDefaultTemplateLine(lines, langCode, schemaEntry);

        if (!modified) return;
        SaveConfigFile(revitConfigFile, lines, langCode);
    }

    private static string? GetSelectedLanguageCode(string[] lines)
    {
        const string languageSection = "Language";
        const string selectKey = "Select=";

        string? currentSection = null;

        foreach (string raw in lines)
        {
            string line = raw.Trim();

            if (line.StartsWith(SectionPrefix) && line.EndsWith(SectionSuffix))
            {
                currentSection = line[1..^1];
                continue;
            }

            if (!string.Equals(currentSection, languageSection, StringComparison.OrdinalIgnoreCase)) continue;

            if (line.StartsWith(selectKey, StringComparison.OrdinalIgnoreCase))
            {
                string code = line[selectKey.Length..].Trim();
                return string.IsNullOrWhiteSpace(code) ? null : code;
            }
        }

        return null;
    }

    private static bool TryUpdateDefaultTemplateLine(string[] lines, string langCode, string schemaEntry)
    {
        bool modified = false;
        string targetSection = $"Directories{langCode}";
        string? currentSection = null;

        for (int i = 0; i < lines.Length; i++)
        {
            string raw = lines[i];
            string trimmed = raw.TrimStart();

            if (trimmed.StartsWith(SectionPrefix) && trimmed.EndsWith(SectionSuffix))
            {
                currentSection = trimmed[1..^1];
                continue;
            }

            if (!string.Equals(currentSection, targetSection, StringComparison.OrdinalIgnoreCase)) continue;
            if (!trimmed.StartsWith(DefaultTemplateKey, StringComparison.OrdinalIgnoreCase)) continue;

            string newLine = BuildDefaultTemplateLine(trimmed, schemaEntry);

            if (!string.Equals(trimmed, newLine, StringComparison.Ordinal))
            {
                int leadingSpaces = raw.Length - raw.TrimStart().Length;
                lines[i] = new string(' ', leadingSpaces) + newLine;
                modified = true;
            }

            break;
        }

        return modified;
    }

    private static string BuildDefaultTemplateLine(string existingLine, string schemaEntry)
    {
        string value = existingLine[DefaultTemplateKey.Length..];
        List<string> tokens = [.. value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim())];
        List<string> keptEntries = [];
        bool hasSipEntry = false;

        foreach (string token in tokens)
        {
            if (!token.StartsWith(SipTemplateKey, StringComparison.OrdinalIgnoreCase))
            {
                keptEntries.Add(token);
                continue;
            }

            if (string.Equals(token, schemaEntry, StringComparison.OrdinalIgnoreCase))
            {
                keptEntries.Add(token);
                hasSipEntry = true;
            }
        }

        if (!hasSipEntry)
        {
            keptEntries.Insert(0, schemaEntry);
        }

        string newValue = string.Join(", ", keptEntries);
        return DefaultTemplateKey + newValue;
    }

    private void SaveConfigFile(string revitConfigFile, string[] lines, string langCode)
    {
        string tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, lines, Encoding.Unicode);

        try
        {
            File.Replace(tempFile, revitConfigFile, null);
            logger.LogInformation("Revit.ini successfully updated to {TemplatePath} for language {Language}", resourceOptions.Value.DefaultTemplatePath, langCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Revit.ini update failed with message: {ErrorMessage}", ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}