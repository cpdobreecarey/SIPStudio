namespace SIPStudio.Abstractions.Options;

/// <summary>
///     Runtime dynamic information about the Application assembly
/// </summary>
public sealed class AssemblyOptions
{
    public required string Framework { get; set; }
    public required Version Version { get; set; }
    public required bool HasAdminAccess { get; set; }
}