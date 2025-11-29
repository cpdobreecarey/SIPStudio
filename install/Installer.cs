using Installer;
using WixSharp;

const string outputName = "SIPStudio";
const string bundleName = $"{outputName}.bundle";

Dictionary<int, string> guidMap = new()
{
    { 2021, "83AAEE7B-F5E8-4752-9BEB-B2BF4F4BA65C" },
    { 2022, "ABA98BC0-DEDE-45C2-8126-643CACD1FC50" },
    { 2023, "9DCEE810-4932-4FF2-BAA8-CB749A059C79" },
    { 2024, "B6D1FEF8-1C9A-4247-96B7-2A3F0EA7FD85" },
    { 2025, "E0C07A84-2DEF-4765-BB14-E63067FC1965" },
    { 2026, "CEDBDBED-57F4-4CC6-9ABB-8AEE40EB842A" },
};

Versions versions = Tools.ComputeVersions(args);
if (!guidMap.TryGetValue(versions.RevitVersion, out var guid))
{
    throw new Exception($"Version GUID mapping missing for the specified version: '{versions.RevitVersion}'");
}

Project project = new()
{
    OutDir = "output",
    Name = outputName,
    GUID = new Guid(guid),
    Platform = Platform.x64,
    UI = WUI.WixUI_ProgressOnly,
    MajorUpgrade = MajorUpgrade.Default,
    Version = versions.InstallerVersion,
    ControlPanelInfo =
    {
        Manufacturer = outputName,
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args, versions.AssemblyVersion);

//
var baseDir = AppContext.BaseDirectory;
var osbTexturePath = Path.Combine(baseDir, "Resources", "Textures", "OSB.jpg");
//

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{versions.AssemblyVersion}-SingleUser";
    project.Dirs =
    [
        //C:\Users\User\AppData\Roaming\Autodesk\ApplicationPlugins
        new InstallDir($@"%AppDataFolder%\Autodesk\ApplicationPlugins", new Dir(bundleName, wixEntities))
    ];
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{versions.AssemblyVersion}-MultiUser";

    //
    Dir texturesDir = new(
        new Id("SIPSTUDIO_APPDATA_" + versions.RevitVersion), versions.RevitVersion.ToString(),
        new Dir("Libraries", new Dir("Textures", new WixSharp.File(osbTexturePath)))
    );
    //

    project.Dirs =
    [
        //C:\ProgramData\Autodesk\ApplicationPlugins
        new InstallDir($@"%CommonAppDataFolder%\Autodesk\ApplicationPlugins", new Dir(bundleName, wixEntities)),
        //new Dir($@"%AppDataFolder%\SIPStudio\{versions.RevitVersion}\Libraries\Textures", new WixSharp.File(osbTexturePath))
        new Dir(@"%AppDataFolder%\SIPStudio", texturesDir)
    ];
    project.BuildMsi();
}