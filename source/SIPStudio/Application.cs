using Nice3point.Revit.Toolkit.External;
using SIPStudio.Services.Application;
using SIPStudio.Services.Configuration;
using System.IO;

namespace SIPStudio;

[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        Host.Start();
        
        RevitRibbonService ribbonService = Host.GetService<RevitRibbonService>();
        ribbonService.CreateRibbon();
    }

    public override void OnShutdown()
    {
        RevitTemplateService templateService = Host.GetService<RevitTemplateService>();
        string revitIniPath = Path.Combine(Context.Application.CurrentUsersDataFolderPath, "Revit.ini");
        templateService.RegisterDefaultTemplate(revitIniPath);
        
        Host.Stop();
    }
}