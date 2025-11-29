using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace SIPStudio.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class DWGLinkCommand : ExternalCommand
{
    public const string Name = "Link DWG";

    public override void Execute()
    {
        try
        {
            string? file = GetDWGPath();
            if (file is null) return;

            DWGImportOptions options = CreateDefaultImportOptions();
            OverrideGraphicSettings graphics = CreateDefaultOverrideGraphics();

            using Transaction transaction = new(Document, Name);
            transaction.Start();
            Document.Link(file, options, ActiveView, out ElementId linkedInstanceId);
            ActiveView.SetElementOverrides(linkedInstanceId, graphics);
            SetDefaultDrawLayer(linkedInstanceId);
            transaction.Commit();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            Result = Result.Failed;
            return;
        }
    }

    private static string? GetDWGPath()
    {
        const string filter = "DWG Files (*.dwg)|*.dwg";

        FileOpenDialog fileDialog = new(filter);
        fileDialog.Show();

        ModelPath modelPath = fileDialog.GetSelectedModelPath();
        if (modelPath is null) return null;

        return ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
    }

    private static DWGImportOptions CreateDefaultImportOptions()
    {
        return new DWGImportOptions
        {
            AutoCorrectAlmostVHLines = true,
            ColorMode = ImportColorMode.BlackAndWhite,
            //CustomScale = 1.0,
            //OrientToView = false,
            Placement = ImportPlacement.Origin,
            //ReferencePoint = new XYZ(0, 0, 0),
            ThisViewOnly = true,
            Unit = ImportUnit.Millimeter,
            VisibleLayersOnly = true
        };
    }

    private static OverrideGraphicSettings CreateDefaultOverrideGraphics()
    {
        OverrideGraphicSettings settings = new();

        Color colour = new(64, 128, 128);
        settings.SetProjectionLineColor(colour);
        settings.SetSurfaceTransparency(100);

        return settings;
    }

    private void SetDefaultDrawLayer(ElementId elementId)
    {
        if (Document.GetElement(elementId) is not ImportInstance importInstance) return;
        importInstance.get_Parameter(BuiltInParameter.IMPORT_BACKGROUND).Set(0);
    }
}