using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.GraphicsInterface;
using System.Collections.Generic;
using System.Linq;
using Polyline = Gssoft.Gscad.DatabaseServices.Polyline;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using System;
using System.Runtime.ExceptionServices;
using Gssoft.Gscad.Internal;
using System.Windows.Forms.VisualStyles;
using NSVLibUtils;
using Gile.AutoCAD.R20.Geometry;

namespace PipeApp.DrawPipe
{
    public class PipeVisualizer
    {
        private List<Line> _newPipes = new List<Line>();
        private List<Polyline> _graphicalPipes = new List<Polyline>();
        public PipeVisualizer(List<Line> newPipes)
        {
            _newPipes = newPipes;
        }

        public static List<Polyline> GetTotalPrettyPipes(Database db)
        {
            var tr = db.TransactionManager.TopTransaction;
            if (tr == null)
                throw new System.Exception("Operation not in a transaction scope");
            var ed = ACAD.DocumentManager.GetDocument(db).Editor;

            var prettyPipeSSet = FilterSelection.GetPrettyPipe(ed);

            if (prettyPipeSSet is null)
                return new List<Polyline>();

            var prettyPipes = new List<Polyline>();

            foreach (ObjectId id in prettyPipeSSet)
            {
                var prettyPipe = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                if (prettyPipe != null)
                    prettyPipes.Add(prettyPipe);
            }
            return prettyPipes;
        }
        public static Polyline GetCorespondingPrettyPipe(Line ln)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var allPrettyPipes = FilterSelection.GetPrettyPipe(doc.Editor);
            if (allPrettyPipes is null)
            {
                return null;
            }
            var tr = doc.Database.TransactionManager.TopTransaction;
            foreach (ObjectId id in allPrettyPipes)
            {
                var prettyPipe = tr.GetObject(id, OpenMode.ForWrite) as Polyline;
                var prettyPipeMidPoint = prettyPipe.GetLineSegmentAt(0).MidPoint.TransformBy(doc.Editor.WCS2UCS()).Flatten();
                if (NSVLibUtils.Check.IsPointOnLine(prettyPipeMidPoint, ln) && prettyPipe.Length != 0)
                    return prettyPipe;
            }
            return null;
        }
        public void AddGraphicalPipes()
        {
            _graphicalPipes.Clear();
            foreach (var pipe in _newPipes)
            {
                var db = ACAD.DocumentManager.MdiActiveDocument.Database;
                var colorIndex = 256;
                var globalWidth = 0.0;
                if (InputData.IsDrawingMain)
                {
                    globalWidth = InputData.MainWidth;
                    colorIndex = InputData.MainColor;
                }
                else
                {
                    globalWidth = InputData.BranchWidth;
                    colorIndex = InputData.BranchColor;
                }
                var prettyPipe = new Polyline();
                prettyPipe.Layer = "NSVPrettyPipe";
                prettyPipe.ColorIndex = colorIndex;
                prettyPipe.AddVertexAt(0, new Point2d(pipe.StartPoint.X,pipe.StartPoint.Y), 0, globalWidth, globalWidth);
                prettyPipe.AddVertexAt(1, new Point2d(pipe.EndPoint.X,pipe.EndPoint.Y), 0, globalWidth, globalWidth);
                TrimInsideSprinkler(prettyPipe);
                ObjectsUtils.DrawObjects(db, prettyPipe);
                _graphicalPipes.Add(prettyPipe);
            } 
        }
        public void EnhanceElevationOnBreakPipe(List<CrossSymbol> pairs)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var tr = doc.Database.TransactionManager.TopTransaction;
            foreach (var symbol in pairs)
            {
                var prettyPipe = GetCorespondingPrettyPipe(symbol.CorrespondingPipe);
                var offset = prettyPipe.Clone() as Polyline;
                offset.TransformBy(Matrix3d.Scaling(0.9, prettyPipe.GetPointAtDist(prettyPipe.Length / 2)));
                if (!Check.IsPointOnPolyline(prettyPipe, symbol.Pair.blk1.Position))
                    symbol.Pair.blk1.Position = offset.GetClosestPointTo(symbol.Pair.blk1.Position, false);
                if (!Check.IsPointOnPolyline(prettyPipe, symbol.Pair.blk2.Position))
                    symbol.Pair.blk2.Position = offset.GetClosestPointTo(symbol.Pair.blk2.Position, false);

//#if(DEBUG)
//                var p1 = symbol.Pair.Item1.Clone() as BlockReference;
//                p1.ColorIndex = 1;
//                var p2 = symbol.Pair.Item2.Clone() as BlockReference;
//                p2.ColorIndex = 2;
//                doc.Database.TransactionManager.QueueForGraphicsFlush();
//                ObjectsUtils.DrawObjects(p1, p2);
//#endif

                if (prettyPipe is null)
                {
                    doc.Editor.WriteMessage("Cant Place Cross symbols. Try a smaller scale!");
                    return;
                }
                UpdateVertecies(symbol.Pair, prettyPipe);
                prettyPipe.UpgradeOpen();
            }
        }

        private void UpdateVertecies(
            (BlockReference, BlockReference) pair,
            Polyline prettyPipe)
        {
            var param1 = prettyPipe.GetParameterAtPoint(pair.Item1.Position);
            var param2 = prettyPipe.GetParameterAtPoint(pair.Item2.Position);
            var width = prettyPipe.GetStartWidthAt(0);
            if (Math.Floor(param1) != Math.Floor(param2))
                throw new Exception("Cross symbols not on the same segment!");
            if (param1 < param2)
            {
                var index = (int)Math.Ceiling(param1);
                prettyPipe.AddVertexAt(index,new Point2d(pair.Item1.Position.X, pair.Item1.Position.Y),
                    0, 0, 0);
                prettyPipe.AddVertexAt(index+1,
                    new Point2d(pair.Item2.Position.X, pair.Item2.Position.Y),
                    0, width,width);
            }
            else
            {
                var index = (int)Math.Ceiling(param1);
                prettyPipe.AddVertexAt(index,
                    new Point2d(pair.Item2.Position.X, pair.Item2.Position.Y),    
                    0, 0, 0);
                prettyPipe.AddVertexAt(index + 1,
                    new Point2d(pair.Item1.Position.X, pair.Item1.Position.Y),
                    0, width, width);
            }
        }

        private void TrimInsideSprinkler(Polyline prettyPipe)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var sprinklers = FilterSelection.GetSprinklers(prettyPipe, doc.Editor);
            
            if (sprinklers is null)
                return;

            for (int i = 0; i < sprinklers.Count(); i++)
            {
                for (int j = 0; j < sprinklers.Count(); j++)
                {
                    if (i == j)
                        continue;
                    if (sprinklers[i].Position.IsEqualTo(sprinklers[j].Position, new Tolerance(0.001, 0.001)))
                    {
                        Application.ShowAlertDialog($"Duplicate sprinklers found at:\nX: {sprinklers[i].Position.X}, Y: {sprinklers[i].Position.Y}");
                        // Optionally, return or break if one message is enough
                        throw new Exception("Duplicated position for sprinklers");
                    }
                }
            }
            try
            {
                var tr = doc.Database.TransactionManager.TopTransaction;
                if(prettyPipe.ObjectId != ObjectId.Null)
                    tr.GetObject(prettyPipe.Id, OpenMode.ForWrite);
                if (sprinklers.Count == 1)
                {
                    if (Check.IsPointInBoundingBox(sprinklers[0], prettyPipe.StartPoint))
                        EditVertex(prettyPipe, sprinklers[0], prettyPipe.StartPoint);
                    else if (Check.IsPointInBoundingBox(sprinklers[0], prettyPipe.EndPoint))
                        EditVertex(prettyPipe, sprinklers[0], prettyPipe.EndPoint);
                    else
                        SplitPolyline(prettyPipe,sprinklers);
                }
                else if (sprinklers.Count == 2)
                {
                    EditVertex(prettyPipe, sprinklers[0], prettyPipe.StartPoint);
                    EditVertex(prettyPipe, sprinklers[1], prettyPipe.EndPoint);
                }
                else
                {
                    SplitPolyline(prettyPipe, sprinklers);
                }
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage("\n" + ex.Message);
            }
        }
        private static void SplitPolyline(Polyline prettyPipe, List<BlockReference> sprinklers)
        {
            var intersectionPoints = new Point3dCollection();
            foreach (var blkRef in sprinklers)
            {
                var points = FindMostIntersectingPoint.WithBlockReference(prettyPipe, blkRef);
                if (points != null)
                {
                    for (int i = 0; i < points.Count; i++)
                        intersectionPoints.Add(points[i]);
                }
            }
            var curves = prettyPipe.GetSplitCurves(intersectionPoints);
            var partials = curves.Cast<Polyline>().ToList();
            foreach (Polyline l in partials)
            {
                var toBeRemoved = false;
                foreach (var blkRef in sprinklers)
                {
                    if (Check.IsPointInBoundingBox(blkRef, l.GetPointAtDist(l.Length / 2)))
                    {
                        toBeRemoved = true;
                        break;
                    }
                }
                if (toBeRemoved)
                    continue;
                var partialPrettyPipe = new Polyline();
                partialPrettyPipe.Layer = prettyPipe.Layer;
                partialPrettyPipe.ColorIndex = prettyPipe.ColorIndex;
                var globalWidth = prettyPipe.ConstantWidth;
                partialPrettyPipe.AddVertexAt(0, l.StartPoint.Convert2d(), 0, globalWidth, globalWidth);
                partialPrettyPipe.AddVertexAt(1, l.EndPoint.Convert2d(), 0, globalWidth, globalWidth);
                ObjectsUtils.DrawObjects(partialPrettyPipe);
            }
            prettyPipe.Erase();
        }
        private static void EditVertex(Polyline pl, BlockReference blkRef,Point3d point)
        {
            var intersections = FindMostIntersectingPoint.WithBlockReference(pl, blkRef);
            if (intersections.Count > 1)
                throw new System.Exception("polyline must end at sprinkler");

            if (pl.GetPoint3dAt(0).Equals(point))
                pl.SetPointAt(0, intersections[0].Convert2d());
            else if (pl.GetPoint3dAt(1).Equals(point))
                pl.SetPointAt(1, intersections[0].Convert2d());
        }

        public static void SplitPreviousGraphicalPipes(Line previousPipe, Point3d breakPoint)
        {
            var prettyPipe = GetCorespondingPrettyPipe(previousPipe);
            if (prettyPipe is null)
                return;

            var intersectionVertex = Point3d.Origin;
            var param = prettyPipe.GetParameterAtPoint(breakPoint);
            var segment1 = new Polyline();

            for (var i = 0; i < param; i++)
            {
                var point = prettyPipe.GetPoint2dAt(i);
                segment1.AddVertexAt(i,
                    point,
                    0,
                    prettyPipe.GetStartWidthAt(i),
                    prettyPipe.GetEndWidthAt(i));
            }

            segment1.AddVertexAt(segment1.NumberOfVertices,
                prettyPipe.GetPointAtParameter(param).Convert2d(),
                0, 0, 0);

            var segment2 = new Polyline();
            segment2.AddVertexAt(0,
                prettyPipe.GetPointAtParameter(param).Convert2d(),
                0,
                prettyPipe.GetStartWidthAt((int)Math.Floor(param)),
                prettyPipe.GetEndWidthAt((int)Math.Floor(param)));

            for (var i = (int)Math.Ceiling(param); i < prettyPipe.NumberOfVertices; i++)
            {
                var point = prettyPipe.GetPoint2dAt(i);
                segment2.AddVertexAt(segment2.NumberOfVertices,
                point,
                0,
                prettyPipe.GetStartWidthAt(i),
                prettyPipe.GetEndWidthAt(i));
            }
            segment1.Layer = segment2.Layer = prettyPipe.Layer;
            segment1.ColorIndex = segment2.ColorIndex = prettyPipe.ColorIndex;
            ObjectsUtils.DrawObjects(segment1, segment2);
            ObjectsUtils.RemoveObject(new ObjectIdCollection { prettyPipe.ObjectId});
        }

        internal void DrawSymbols(List<CrossSymbol> orderedSymbols)
        {
            var db = ACAD.DocumentManager.MdiActiveDocument.Database;
            var allSymbols = orderedSymbols.SelectMany((item) => new[] { item.Pair.Item1, item.Pair.Item2}).ToList();
            ObjectsUtils.DrawObjects(db, allSymbols.ToArray());
        }
    }
}
