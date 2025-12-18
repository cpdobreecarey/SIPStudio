using Nice3point.Revit.Toolkit.External;
using SIPStudio.Services.Application;

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
        Host.Stop();
    }
}