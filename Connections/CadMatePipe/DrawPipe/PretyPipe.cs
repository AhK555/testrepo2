using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using System.Threading.Tasks;
using NSVLibUtils;
using Gssoft.Gscad.ApplicationServices;
using System.Drawing;
using Region = Gssoft.Gscad.DatabaseServices.Region;

namespace PipeApp
{
    public static class PretyPipe
    {
        public static void TrimOnBreakPipes(Database db,(BlockReference,BlockReference)blockRefPair)
        {
            //must use after the current pretty pipe is added to database
            var doc = ACAD.DocumentManager.GetDocument(db);
            var ed = doc.Editor;
            var ucs = ed.CurrentUserCoordinateSystem;
            Point3d origin = new Point3d(0, 0, 0);
            Vector3d normal = new Vector3d(0, 0, 1);
            normal = normal.TransformBy(ucs);
            var plane = new Plane(origin, normal);

            var tr = db.TransactionManager.TopTransaction;

            var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            try
            {
                Polyline prettyPipe = GetCorespondingPrettyPipe(blockRefPair,db);
                var prettyPipeWidth = prettyPipe.GetStartWidthAt(0);
                if (!Check.IsPointOnPolyline(prettyPipe, blockRefPair.Item1.Position))
                    return;
                if (!Check.IsPointOnPolyline(prettyPipe, blockRefPair.Item2.Position))
                    return;

                var newHalf1 = new Polyline();
                newHalf1.Layer = prettyPipe.Layer;
                newHalf1.ColorIndex = prettyPipe.ColorIndex;
                newHalf1.AddVertexAt(0, prettyPipe.StartPoint.Convert2d(plane), 0, prettyPipeWidth, prettyPipeWidth);
                var firstBlock = prettyPipe.StartPoint.DistanceTo(blockRefPair.Item1.Position)<prettyPipe.StartPoint.DistanceTo(blockRefPair.Item2.Position)?
                    blockRefPair.Item1: blockRefPair.Item2;
                newHalf1.AddVertexAt(1, firstBlock.Position.Convert2d(plane), 0, prettyPipeWidth, prettyPipeWidth);

                currentSpace.AppendEntity(newHalf1);
                tr.AddNewlyCreatedDBObject(newHalf1, true);

                var newHalf2 = new Polyline();
                newHalf2.Layer = prettyPipe.Layer;
                newHalf2.ColorIndex = prettyPipe.ColorIndex;
                var secondBlock = firstBlock == blockRefPair.Item1 ? blockRefPair.Item2 : blockRefPair.Item1;
                newHalf2.AddVertexAt(0,secondBlock.Position.Convert2d(plane),0,prettyPipeWidth, prettyPipeWidth);
                newHalf2.AddVertexAt(1, prettyPipe.EndPoint.Convert2d(plane),0,prettyPipeWidth, prettyPipeWidth);

                currentSpace.AppendEntity(newHalf2);
                tr.AddNewlyCreatedDBObject(newHalf2, true);

                if (!prettyPipe.IsWriteEnabled)
                    prettyPipe.UpgradeOpen();
                prettyPipe.Erase();
            }
            catch (System.Exception)
            {
                doc.Editor.WriteMessage("Error while trying to break pretty pipe and add break pipe to it");
            }

        }
        


        public static Polyline GetCorespondingPrettyPipe((BlockReference,BlockReference) breakPipe,Database db)
        {
            var doc = ACAD.DocumentManager.GetDocument(db);
            var ed = doc.Editor;
            var allPrettyPipes = FilterSelection.GetPrettyPipe(ed);
            if (allPrettyPipes is null)
                throw new System.Exception("Break pipe blocks created on nothing!");
            var tr = db.TransactionManager.TopTransaction;
            foreach(ObjectId id in allPrettyPipes)
            {
                var prettyPipe = tr.GetObject(id, OpenMode.ForWrite) as Polyline;
                if (Check.IsPointOnPolyline(prettyPipe, breakPipe.Item1.Position))
                    return prettyPipe;
                else if (Check.IsPointOnPolyline(prettyPipe, breakPipe.Item2.Position))
                    return prettyPipe;
                else
                    continue;
            }
            throw new System.Exception("Could not find corresponding pretty pipe");
        }

        
    }
}
