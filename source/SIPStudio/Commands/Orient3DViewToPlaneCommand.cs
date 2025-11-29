using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace SIPStudio.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class Orient3DViewToPlaneCommand : ExternalCommand
{
    public const string Name = "Orient 3D View To Plane";

    public override void Execute()
    {
        if (ActiveView is not View3D view)
        {
            ErrorMessage = "The active view must be a 3D view before running this command.";
            Result = Result.Failed;
            return;
        }

        try
        {
            Reference selection = UiDocument.Selection.PickObject(ObjectType.Face, "Select a planar face to orient the view");
            if (Document.GetElement(selection).GetGeometryObjectFromReference(selection) is not PlanarFace selectedFace) return;

            using Transaction transaction = new(Document, Name);
            transaction.Start();
            OrientViewToFace(view, selectedFace);
            transaction.Commit();

            UiDocument.ShowElements([selection.ElementId]);
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

    private static void OrientViewToFace(View3D view, PlanarFace face)
    {
        XYZ forwardDirection = face.ComputeNormal(new UV(0, 0)).Negate();

        XYZ upDirection = (XYZ.BasisZ - forwardDirection * forwardDirection.DotProduct(XYZ.BasisZ)).IsZeroLength()
            ? XYZ.BasisX - forwardDirection * forwardDirection.DotProduct(XYZ.BasisX)
            : XYZ.BasisZ - forwardDirection * forwardDirection.DotProduct(XYZ.BasisZ);

        view.SetOrientation(new(face.Origin, upDirection.Normalize(), forwardDirection));
    }
}