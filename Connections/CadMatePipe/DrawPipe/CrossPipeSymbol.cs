using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Internal;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Properties=CadMatePipe.Properties;

namespace PipeApp
{
    public enum PipeElevation
    {
        BelowPrevious,
        AbovePrevious
    }
    public class CrossPipeSymbol
    {
        public static void SetDistance()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var pdo = new PromptDoubleOptions("\nEnter cross symbol scale :");
            pdo.DefaultValue = InputData.CrossSymbolScale;
            pdo.AllowNegative = false;
            pdo.AllowNone = false;
            pdo.AllowZero = false;
            var pdr = doc.Editor.GetDouble(pdo);
            var scale = 0.0;
            if (pdr.Status == PromptStatus.OK)
            {
                SetScaleFactor(pdr.Value);
                Properties.Settings.Default.pipeScaleImperial =InputData._crossBlkWidth;
                Properties.Settings.Default.Save();
            }
        }

        private static void SetScaleFactor(double value)
        {
            double scale = 1;
            if (value == 0)
                value = 1;
            InputData.CrossSymbolScale = value;
            if (InputData.IsDrawingMain)
            {
                if (InputData._crossBlkWidth < InputData.MainWidth)
                {
                    scale = InputData.MainWidth / InputData._crossBlkWidth;
                }
            }
            else
            {
                if (InputData._crossBlkWidth < InputData.BranchWidth)
                {
                    scale = InputData.MainWidth / InputData._crossBlkWidth;
                }
            }
            if (scale != 0.0)
            {
                InputData.CrossSymbolScale = value * scale;
            }
            else
                InputData.CrossSymbolScale = value;
        }

        public static List<CrossSymbol> GetPairSymbol(Line currentPipe)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var tr = db.TransactionManager.TopTransaction;

            var result = new List<CrossSymbol>();
            var previousPipeIds = FilterSelection.GetPipes(doc.Editor);
            if (previousPipeIds is null)
                return result;
            foreach (ObjectId pipeId in previousPipeIds)
            {
                var previous = tr.GetObject(pipeId, OpenMode.ForRead) as Line;
                var intersections = new Point3dCollection();

                currentPipe.IntersectWith(
                    previous,
                    Intersect.OnBothOperands,
                    intersections,
                    IntPtr.Zero, 
                    IntPtr.Zero);

                if (intersections.Count == 1)
                {
                    if (intersections[0] == currentPipe.StartPoint || intersections[0] == currentPipe.EndPoint)
                        continue;
                    else
                    {
                        if (InputData.CurrentPipeStatus == PipeElevation.BelowPrevious)
                            result.Add(new CrossSymbol(GetCrossSymbolPair(intersections[0], currentPipe), currentPipe));
                        else
                            result.Add(new CrossSymbol(GetCrossSymbolPair(intersections[0], previous), previous));
                    }
                }
            }
            result.OrderBy((symbol) =>
            symbol.Pair.Item1.Position.DistanceTo(currentPipe.StartPoint)).ToList();
            return result;
        }
        
        private static (BlockReference, BlockReference) GetCrossSymbolPair(
            Point3d intersectionPoint,
            Curve pipeToShowSymbolOn)
        {
            var pipeNormalVector = pipeToShowSymbolOn.StartPoint.GetVectorTo(pipeToShowSymbolOn.EndPoint).GetNormal();
            double angle;
            if (pipeToShowSymbolOn is Polyline pl)
                angle = new Line(pl.StartPoint, pl.EndPoint).Angle;
            else
                angle = (pipeToShowSymbolOn as Line).Angle;

            var pt1 = intersectionPoint.Add(-pipeNormalVector * InputData.CrossSymbolDistance );
            var crossSymbol1 = new BlockReference(pt1, BlockDefs.GetBreakPipe());
            crossSymbol1.Rotation = angle;

            SetScaleFactor(InputData.CrossSymbolScale);

            crossSymbol1.ScaleFactors = new Scale3d(InputData.CrossSymbolScale);

            var pt2 = intersectionPoint.Add(pipeNormalVector * InputData.CrossSymbolDistance );
            var crossSymbol2 = new BlockReference(pt2, BlockDefs.GetBreakPipe());
            crossSymbol2.Rotation = angle;
            crossSymbol2.ScaleFactors = new Scale3d(InputData.CrossSymbolScale);

            return (crossSymbol1, crossSymbol2);
        }
    }
}
