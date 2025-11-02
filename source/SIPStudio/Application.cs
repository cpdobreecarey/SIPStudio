using Nice3point.Revit.Toolkit.External;
using SIPStudio.Commands;

namespace SIPStudio;

[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        Host.Start();
        CreateRibbon();
    }

    public override void OnShutdown()
    {
        Host.Stop();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "SIPStudio");

        panel.AddPushButton<StartupCommand>("Execute")
            .AddShortcuts("EC")
            .SetImage("/SIPStudio;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/SIPStudio;component/Resources/Icons/RibbonIcon32.png");
    }
}