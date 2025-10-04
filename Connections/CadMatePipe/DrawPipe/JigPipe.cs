using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Polyline = Gssoft.Gscad.DatabaseServices.Polyline;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using NSVLibUtils;
using Application = Gssoft.Gscad.ApplicationServices.Application;
using Pipe;

namespace PipeApp.DrawPipe
{
    public class JigPipe : DrawJig
    {
        
        public Line Pipe { get; private set; } = new Line()
        {
            ColorIndex = 256,
            LineWeight = LineWeight.ByLayer,
        };
        private Point3d? _startPoint = null;
        private Point3d _movingPoint;
        private Document _doc;
        private string _elevationKeyword;
        private string _elevationShortcut;
        private string _isMainKeyword;
        private string _isMainShortcut;
        public JigPipe (Document doc, Point3d? startPoint)
        {
            this._doc = doc;
            if (startPoint.HasValue)
            {
                _startPoint = startPoint.Value;
            }
        }

        public PromptResult Move()
        {
            PromptResult res;
            if (InputData.IsDrawingMain)
                Pipe.Layer = NSVLIBConstants.Enums.NSVLayers.NSVMainPipe.ToString();
            else
                Pipe.Layer = NSVLIBConstants.Enums.NSVLayers.NSVBranchPipe.ToString();
            if(_startPoint.HasValue)
                Pipe.StartPoint = _startPoint.Value;

            do
            {
                if (InputData.IsDrawingMain)
                {
                    _isMainKeyword = "Branch";
                    _isMainShortcut = "B";
                }
                else
                {
                    _isMainKeyword = "Main";
                    _isMainShortcut = "M";
                }
                if(InputData.CurrentPipeStatus == PipeElevation.AbovePrevious)
                {
                    _elevationKeyword = "current-beLow";
                    _elevationShortcut = "L";
                }
                else
                {
                    _elevationKeyword = "current-Above";
                    _elevationShortcut = "A";
                }
                using(new SprinklerSnap())
                {
                    res = _doc.Editor.Drag(this);
                }
                if (res.Status == PromptStatus.Keyword)
                {
                    switch (res.StringResult)
                    {
                        case "B":
                            InputData.IsDrawingMain = false;
                            break;
                        case "M":
                            InputData.IsDrawingMain = true;
                            break;
                        case "P":
                            var newSize = _doc.Editor.GetString("\nEnter new pipe size:");
                            if (newSize.Status == PromptStatus.OK)
                            {
                                if (InputData.IsDrawingMain)
                                    InputData.MainSize = newSize.StringResult;
                                else
                                    InputData.BranchSize = newSize.StringResult;
                            }
                            break;
                        case "C":
                            CrossPipeSymbol.SetDistance();
                            break;
                        case "L":
                            InputData.CurrentPipeStatus = PipeElevation.BelowPrevious;
                            break;
                        case "A":
                            InputData.CurrentPipeStatus = PipeElevation.AbovePrevious;
                            break;
                        case "F":
                            return res;
                    }
                }
                else if (res.Status == PromptStatus.OK)
                {
                    _movingPoint = GetCorrectedPoint(_movingPoint);
                    if(!_startPoint.HasValue)
                    {
                        Pipe.StartPoint = _movingPoint;
                        _startPoint = _movingPoint;
                        continue;
                    }
                    Pipe.EndPoint = _movingPoint;
                    return res;
                }

                else
                    return res;

            } while (true);
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var options = new JigPromptPointOptions("\nSepecify next point or ");
            var keyWord = $"[{_isMainKeyword}/{_elevationKeyword}/Form/Cross-size/Pipe-size]";
            var shortCuts = $"{_isMainShortcut} {_elevationShortcut} F C P";
            //MessageBox.Show(keyWord.ToString());
            //MessageBox.Show(shortCuts.ToString());

            //// Define a consistent set of keywords first
            //var localKeywords = "Main/Elevation/InputForm/CrossSize/PipeSize";
            //var globalKeywords = "Main Elevation InputForm CrossSize PipeSize";

            // Now use these consistent strings to build your prompt
            options.SetMessageAndKeywords($"\nSpecify next point or [{keyWord}]:", shortCuts);
            bool isShiftPressed = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            if (_startPoint.HasValue)
            {
                options.UserInputControls = 
                    UserInputControls.GovernedByOrthoMode|
                    UserInputControls.NullResponseAccepted;
                ACAD.SetSystemVariable("ORTHOMODE", isShiftPressed? 0: 1);
                options.BasePoint = Pipe.StartPoint;
                options.UseBasePoint = true;
            }

            var result = prompts.AcquirePoint(options);

            if(_movingPoint.Equals(result.Value))
                return SamplerStatus.NoChange;
            _movingPoint = result.Value;

            return SamplerStatus.OK;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            if (_startPoint.HasValue)
            {
                // Temporary line, not your DB object
                var tmpLine = new Line(_startPoint.Value, _movingPoint)
                {
                    ColorIndex = 256,
                    LineWeight = LineWeight.ByLayer,
                    Layer = InputData.IsDrawingMain ? "NSVMainPipe" : "NSVBranchPipe"
                };
                draw.Geometry.Draw(tmpLine);
                tmpLine.Dispose();
            }
            return true;
        }


        private Point3d GetCorrectedPoint(Point3d clickedPoint)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            var tr = db.TransactionManager.TopTransaction;

            var pipeLayers = new List<string>
            {
                NSVLIBConstants.Enums.NSVLayers.NSVMainPipe.ToString(),
                NSVLIBConstants.Enums.NSVLayers.NSVBranchPipe.ToString()
            };
            var layers = string.Join(",", pipeLayers);

            var filter = new SelectionFilter(new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "LINE"),
                new TypedValue((int)DxfCode.LayerName, layers),
                new TypedValue((int)DxfCode.ExtendedDataRegAppName,"NSVPipe")
            });

            var h = (double)ACAD.GetSystemVariable("viewsize")/100;
            var minPoint = clickedPoint - new Vector3d(h, h, 0);
            var maxPoint = clickedPoint + new Vector3d(h, h, 0);
            var selRes = ed.SelectCrossingWindow(minPoint, maxPoint, filter);
            if (selRes.Status != PromptStatus.OK)
                return clickedPoint;
            var minDist = double.MaxValue;
            Line closestLine = null;
            foreach (var id in selRes.Value.GetObjectIds())
            {
                var line = tr.GetObject(id, OpenMode.ForRead) as Line;
                if (IsPointInBOund(minPoint, maxPoint, line.StartPoint))
                    return line.StartPoint;
                if (IsPointInBOund(minPoint, maxPoint, line.EndPoint))
                    return line.EndPoint;

                var dist = line.GetClosestPointTo(clickedPoint, true).DistanceTo(clickedPoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestLine = line;
                }
            }

            var point = closestLine.GetClosestPointTo(clickedPoint,true);
            return point;
        }

        private bool IsPointInBOund(Point3d minPoint, Point3d maxPoint, Point3d startPoint)
        {
            return minPoint.X <= startPoint.X && minPoint.Y <= startPoint.Y &&
                maxPoint.X >= startPoint.X && maxPoint.Y >= startPoint.Y;
        }
    }
}