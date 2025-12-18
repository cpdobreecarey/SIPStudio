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

        #region Documentation Panel
        RibbonPanel documentationPanel = application.CreatePanel("Smart Documentation", SipTabName);
        documentationPanel.AddPushButton<DWGLinkCommand>(DWGLinkCommand.Name)
            .SetImage("/SIPStudio;component/Resources/Icons/RibbonIcon16.png")
            .SetLargeImage("/SIPStudio;component/Resources/Icons/RibbonIcon32.png");
        #endregion

        #region Hidden Panel
        RibbonPanel hiddenPanel = application.CreatePanel(HiddenPanelName, SipTabName);
        hiddenPanel.AddPushButton<Orient3DViewToPlaneCommand>(Orient3DViewToPlaneCommand.Name).AddShortcuts("OP");
        #endregion

        application.ControlledApplication.ApplicationInitialized += ApplicationInitialized;
    }

    private void ApplicationInitialized(object? sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
    {
        Adwin.RibbonControl ribbon = Adwin.ComponentManager.Ribbon;
        Adwin.RibbonTab? addInTab = null;
        Adwin.RibbonTab? sipTab = null;

        foreach (var tab in ribbon.Tabs)
        {
            if (tab.Title == "Add-Ins") addInTab = tab;
            else if (tab.Title == SipTabName) sipTab = tab;
        }
        if (addInTab is null || sipTab is null) return;

        _hiddenPanelAdwin = sipTab.Panels.FirstOrDefault(p => p.Source.Title == HiddenPanelName);
        if (_hiddenPanelAdwin is null) return;

        sipTab.Activated += TabActivated;
        sipTab.Deactivated += TabDeactivated;

        if (!addInTab.Panels.Contains(_hiddenPanelAdwin))
            addInTab.Panels.Add(_hiddenPanelAdwin);

        addInTab.Activated += TabActivated;
        addInTab.Deactivated += TabDeactivated;
    }

    private void TabActivated(object? sender, EventArgs e)
    {
        if (_hiddenPanelAdwin is null) return;
        if (sender is not Adwin.RibbonTab tab) return;

        tab.Panels.Remove(_hiddenPanelAdwin);
    }

    private void TabDeactivated(object? sender, EventArgs e)
    {
        if (_hiddenPanelAdwin is null) return;
        if (sender is not Adwin.RibbonTab tab) return;

        if (!tab.Panels.Contains(_hiddenPanelAdwin))
            tab.Panels.Add(_hiddenPanelAdwin);
    }
}