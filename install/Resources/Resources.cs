namespace Installer.Resources;

public static class Resources
{
    private static readonly string BaseDirectory = Path.Combine(AppContext.BaseDirectory, "Resources");

    private static readonly string RevitDirectory = Path.Combine(BaseDirectory, "Revit");

    public static readonly string ShellIcon = Path.Combine(BaseDirectory, "Icons", "ShellIcon.ico");

    public static readonly string TemplatesDirectory = Path.Combine(RevitDirectory, "Templates");

    public static readonly string TexturesDirectory = Path.Combine(RevitDirectory, "Textures");
}