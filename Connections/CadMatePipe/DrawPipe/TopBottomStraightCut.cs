using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PipeApp
{
    internal class TopBottomStraightCut
    {
        private Document _doc = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        private Line _pipeLine;

        public TopBottomStraightCut(Line pipeLine)
        {
            _pipeLine = pipeLine;
        }
        public IEnumerable<Line> Init()
        {
            var db = _doc.Database;
            var res = new List<Line>();
            if(!IsApplicable())
            {
                res.Add(_pipeLine);
                return res;
            }
            var topLines = GenerateTop();
            var bottomLines = GenerateBottom();
            var alllines = new List<Line>();
            alllines.AddRange(bottomLines);
            alllines.AddRange(topLines);
            var tempObjectsIds = ObjectsUtils.DrawObjects(alllines.ToArray());
            var prompttbsopts = new PromptKeywordOptions("[Top/Bottom/Straight]", "Top Bottom Straight");
            var promptttbs = _doc.Editor.GetKeywords(prompttbsopts);

            if (promptttbs.Status != PromptStatus.OK)
            {
                res.Add(_pipeLine);
                ObjectsUtils.RemoveObject(db, tempObjectsIds);
                return res;
            }

            switch (promptttbs.StringResult) 
            {
                case "Top":
                    res.AddRange(topLines.Select(item => item.Clone() as Line));
                    break;
                case "Bottom":
                    res.AddRange(bottomLines.Select(item => item.Clone() as Line));
                    break;
                case "Straight":
                    res.Add(_pipeLine);
                    break;
            }

            ObjectsUtils.RemoveObject(db, tempObjectsIds);
            res.ForEach((line) => line.Layer = "NSVMainPipe");
            return res;
        }

        private IEnumerable<Line> GenerateTop()
        {
            var midPoint = new Point3d();
            if (_pipeLine.StartPoint.Y > _pipeLine.EndPoint.Y)
                midPoint = new Point3d(_pipeLine.EndPoint.X, _pipeLine.StartPoint.Y, 0);
            else
                midPoint = new Point3d(_pipeLine.StartPoint.X, _pipeLine.EndPoint.Y, 0);

            return GenerateLines(_pipeLine, midPoint);
        }

        private IEnumerable<Line> GenerateBottom()
        {
            var midPoint = new Point3d(_pipeLine.EndPoint.X, _pipeLine.StartPoint.Y, 0);
            if (_pipeLine.StartPoint.Y > _pipeLine.EndPoint.Y)
                midPoint = new Point3d(_pipeLine.StartPoint.X, _pipeLine.EndPoint.Y, 0);
            else
                midPoint = new Point3d(_pipeLine.EndPoint.X, _pipeLine.StartPoint.Y, 0);
            return GenerateLines(_pipeLine, midPoint);
        }

        private IEnumerable<Line> GenerateLines(Line pipeLine, Point3d midPoint) 
        {
            var res = new List<Line>();
            var line1 = new Line(pipeLine.StartPoint, midPoint);
            var line2 = new Line(midPoint, pipeLine.EndPoint);
            line1.Transparency = new Gssoft.Gscad.Colors.Transparency((byte)50);
            line2.Transparency = new Gssoft.Gscad.Colors.Transparency((byte)50);
            res.Add(line1); res.Add(line2);

            return res;
        }

        public bool IsApplicable()
        {
            double tolerance = 0;
            if (InputData.Unit == NSVLIBConstants.Enums.Unit.metric)
                tolerance = 50;
            else
                tolerance = 3;

            if (Math.Abs(_pipeLine.Delta.X) < tolerance || Math.Abs(_pipeLine.Delta.Y) < tolerance)
                return false;

            //if (!IsInSprinkler())
            //    return false;

            return true;
        }

        private bool IsInSprinkler()
        {
            // don't delete This method we might choose returning back to the old way of when pipes endPoint is inside a sprinkler or so
            var db = _doc.Database;
            var sprinklerObjIds = FilterSelection.GetSprinklers(_doc.Editor, _pipeLine);
            if (sprinklerObjIds == null)
                return false;

            List<BlockReference> sprinklers = new List<BlockReference>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId sprinklerObjId in sprinklerObjIds)
                {
                    var blk = tr.GetObject(sprinklerObjId, OpenMode.ForWrite) as BlockReference;
                    sprinklers.Add(blk);
                }

                tr.Commit();
            }

            foreach (var sprinkler in sprinklers)
            {
                if (Check.IsPointInBlockRef(sprinkler, _pipeLine.EndPoint))
                {
                    return true;
                }
            }

            foreach (var sprinkler in sprinklers)
            {
                sprinkler?.Dispose();
            }

            return false;
        }
    }
}
