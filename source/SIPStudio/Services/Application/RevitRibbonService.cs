using Autodesk.Revit.UI;
using SIPStudio.Commands;
using Adwin = Autodesk.Windows;

namespace SIPStudio.Services.Application;

public sealed class RevitRibbonService
{
    private const string SipTabName = "SIPStudio";
    private const string HiddenPanelName = "Hidden";

    private static Adwin.RibbonPanel? _hiddenPanelAdwin;

    public void CreateRibbon()
    {
        UIControlledApplication application = Context.UiControlledApplication;

        RibbonPanel betaPanel = application.CreatePanel("Beta", SipTabName);
        //betaPanel.AddPushButton<Command>("Command")
        //    .SetImage("/SIPStudio;component/Resources/Icons/RibbonIcon16.png")
        //    .SetLargeImage("/SIPStudio;component/Resources/Icons/RibbonIcon32.png");

        RibbonPanel hiddenPanel = application.CreatePanel(HiddenPanelName, SipTabName);
        hiddenPanel.AddPushButton<Orient3DViewToPlaneCommand>("Orient to Plane").AddShortcuts("OP");

        application.ControlledApplication.ApplicationInitialized += ControlledApplication_ApplicationInitialized;
    }

    private void ControlledApplication_ApplicationInitialized(object? sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
    {
        Adwin.RibbonControl ribbon = Adwin.ComponentManager.Ribbon;
        Adwin.RibbonTab? addInTab = null;
        Adwin.RibbonTab? myTab = null;

        foreach (var tab in ribbon.Tabs)
        {
            if (tab.Title == "Add-Ins") addInTab = tab;
            if (tab.Title == SipTabName) myTab = tab;
        }
        if (addInTab is null || myTab is null) return;

        _hiddenPanelAdwin = myTab.Panels.FirstOrDefault(p => p.Source.Title == HiddenPanelName);
        if (_hiddenPanelAdwin is null) return;

        myTab.Activated += TabActivated;
        myTab.Deactivated += TabDeactivated;

        if (!addInTab.Panels.Contains(_hiddenPanelAdwin))
            addInTab.Panels.Add(_hiddenPanelAdwin);

        addInTab.Activated += TabActivated;
        addInTab.Deactivated += TabDeactivated;
    }

    private void TabDeactivated(object? sender, EventArgs e)
    {
        if (_hiddenPanelAdwin is null) return;
        if (sender is not Adwin.RibbonTab tab) return;

        if (!tab.Panels.Contains(_hiddenPanelAdwin))
            tab.Panels.Add(_hiddenPanelAdwin);
    }

    private void TabActivated(object? sender, EventArgs e)
    {
        if (_hiddenPanelAdwin is null) return;
        if (sender is not Adwin.RibbonTab tab) return;

        tab.Panels.Remove(_hiddenPanelAdwin);
    }
}