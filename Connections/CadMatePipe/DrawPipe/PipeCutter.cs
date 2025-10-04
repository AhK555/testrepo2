using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.Colors;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
namespace PipeApp.DrawPipe
{
    public class PipeCutter
    {
        private Line _jiggingLine;
        private Document _doc = ACAD.DocumentManager.MdiActiveDocument;
        public List<Line> PreviousCuttedPipes = new List<Line>();
        public List<Line> NewPipes = new List<Line>();
        private Point3d? _nextPoint = null;
        public Point3d NextStartPoint
        {
            get
            {
                if(_nextPoint is null)
                    return _jiggingLine.EndPoint;
                else
                    return _nextPoint.Value;
            }
        }
        public PipeCutter(Line jiggingLine)
        {
            _jiggingLine = jiggingLine;
        }

        public void BreakCurrentPipeAtSprinklerIntersections()
        {
            var db = _doc.Database;
            var ed = _doc.Editor;
            var tr = db.TransactionManager.TopTransaction;

            var splittedPipes = new List<Line>();
            var intersectingBlockRefIds = FilterSelection.GetSprinklers(ed, _jiggingLine);
            if (intersectingBlockRefIds == null)
            {
                splittedPipes = new List<Line> { _jiggingLine };
            }
            else
            {
                var sprinklers = FilterSelection.GetSprinklers(_jiggingLine, ed);
                var orderedBlockRefs = new List<BlockReference>();
                foreach (var blkRef in sprinklers)
                {
                    orderedBlockRefs.Add(blkRef);
                    if (Check.IsPointInBlockRef(blkRef, _jiggingLine.StartPoint))
                        orderedBlockRefs.Remove(blkRef);
                    else if (Check.IsPointInBlockRef(blkRef, _jiggingLine.EndPoint))
                        orderedBlockRefs.Remove(blkRef);
                }

                var previousPoint = _jiggingLine.StartPoint;
                foreach (var blkRef in orderedBlockRefs)
                {
                    var partialPipe = new Line(_jiggingLine.GetClosestPointTo(previousPoint, false),
                                                _jiggingLine.GetClosestPointTo(blkRef.Position, false));
                    partialPipe.Layer = _jiggingLine.Layer;
                    partialPipe.LineWeight = LineWeight.ByLayer;
                    partialPipe.ColorIndex = 256;
                    if (partialPipe.Length > 1e-3)
                        splittedPipes.Add(partialPipe);
                    previousPoint = partialPipe.EndPoint;
                }
                if (previousPoint.DistanceTo(_jiggingLine.EndPoint) > 1e-3)
                    splittedPipes.Add(new Line(previousPoint, _jiggingLine.EndPoint));
            }
            foreach (Line l in splittedPipes)
            {
                var currentLienIds = ObjectsUtils.DrawObjects(l);
                NewPipes.Add(l);
                _nextPoint = l.EndPoint;
            }
        }
        public void BreakPreviousPipesAtPipeIntersections()
        {
            var tr = _doc.Database.TransactionManager.TopTransaction;
            var intersectingPipes = FilterSelection.GetPipes(_doc.Editor, _jiggingLine);
            var crossedLines = new List<Line>();
            if (intersectingPipes != null)
            {
                foreach (ObjectId id in intersectingPipes)
                {
                    var previousLine = tr.GetObject(id, OpenMode.ForRead) as Line;
                    crossedLines.Add(previousLine);
                    if(previousLine.StartPoint.Equals(_jiggingLine.StartPoint) ||
                        previousLine.StartPoint.Equals(_jiggingLine.EndPoint) ||
                        previousLine.EndPoint.Equals(_jiggingLine.EndPoint) ||
                        previousLine.EndPoint.Equals(_jiggingLine.StartPoint))
                        crossedLines.Remove(previousLine);
                }
                if (intersectingPipes.Count == 0)
                    return;
                foreach (Line previousPipe in crossedLines)
                {
                    Point3d? intersectionPoint = null;
                    if (Check.IsPointOnLine(_jiggingLine.StartPoint, previousPipe))
                        intersectionPoint = _jiggingLine.StartPoint;
                    else if (Check.IsPointOnLine(_jiggingLine.EndPoint, previousPipe))
                        intersectionPoint = _jiggingLine.EndPoint;
                    if (!intersectionPoint.HasValue)
                        continue;
                    var parameter = new DoubleCollection { previousPipe.GetParameterAtPoint(intersectionPoint.Value) };
                    var previousDbObjects = previousPipe.GetSplitCurves(parameter);
                    var cuttedPipes = previousDbObjects.Cast<Line>().ToList();
                    PipeSizer.AssignSizeToPreviousSplitedPipes(previousPipe, cuttedPipes);
                    PipeVisualizer.SplitPreviousGraphicalPipes(previousPipe, cuttedPipes[0].EndPoint);
                    previousPipe.UpgradeOpen();
                    previousPipe.Erase();
                    var previousPipeIds = ObjectsUtils.DrawObjects(cuttedPipes.ToArray());
                    foreach (ObjectId pipeId in previousPipeIds)
                        PreviousCuttedPipes.Add(tr.GetObject(pipeId, OpenMode.ForRead) as Line);
                }
            }
        }
    }
}
