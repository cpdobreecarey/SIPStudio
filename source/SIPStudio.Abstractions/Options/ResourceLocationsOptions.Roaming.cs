namespace SIPStudio.Abstractions.Options;

/// <summary>
///     Defines storage locations for application data that roams with the user across multiple devices.
/// </summary>
/// <remarks>
///     Used for storing user-specific settings, configurations, or small data files  
///     that need to be available on all devices in a domain environment (e.g., Active Directory).  
///     Typical usage includes user preferences, UI settings, or lightweight configuration files.
/// </remarks>
public sealed partial class ResourceLocationsOptions
{
    public required string LibrariesDirectory { get; set; }
    public required string DefaultTemplateFileName { get; set; }
    public required string DefaultTemplatePath { get; set; }

    //public required string ConfigurationsDirectory { get; set; }
    //public required string ConfigurationFilePath { get; set; }
    //public required string MaterialTexturesDirectory { get; set; }
    //public required string ImperialFamiliesDirectory { get; set; }
    //public required string MetricFamiliesDirectory { get; set; }
}