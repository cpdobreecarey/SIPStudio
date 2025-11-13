using Autodesk.Revit.UI;
using SIPStudio.Commands;

namespace SIPStudio.Services.Application;

public sealed class RevitRibbonService
{
    private const string _tabName = "SIPStudio";

    public void CreateRibbon()
    {
        UIControlledApplication application = Context.UiControlledApplication;
        RibbonPanel betaPanel = application.CreatePanel("Beta", _tabName);

        betaPanel.AddPushButton<Orient3DViewToPlaneCommand>("Orient 3D View\n to Plane")
            .AddShortcuts("OP")
            .SetImage("/SIPStudio;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/SIPStudio;component/Resources/Icons/RibbonIcon32.png");
    }
}