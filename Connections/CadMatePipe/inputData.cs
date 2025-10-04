using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Internal;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit = NSVLIBConstants.Enums.Unit;

namespace PipeApp
{
    public static class InputData
    {
        public static double _crossBlkWidth;
        public static double CrossSymbolDistance
        {
            get
            {
                if (MainWidth > BranchWidth)
                {
                    if (_currentPipeStatus == PipeElevation.BelowPrevious)
                        return Math.Max(_crossBlkWidth, MainWidth * 1.5);
                    else
                        return Math.Max(_crossBlkWidth, MainWidth * 1.5);
                }
                else
                {
                    if (CurrentPipeStatus == PipeElevation.BelowPrevious)
                        return Math.Max(_crossBlkWidth, MainWidth * 1.5);
                    else
                        return Math.Max(_crossBlkWidth, MainWidth * 1.5);
                }
            }
        }
        private static PipeElevation _currentPipeStatus;

        public static PipeElevation CurrentPipeStatus
        {
            get => _currentPipeStatus;
            set
            {
                _currentPipeStatus = value;
            }
        }
        public static double CrossSymbolScale 
        {
            get
            {
                return _breakPipeScale;
            }
            set 
            {
                _breakPipeScale = value;
                using (var blkRef = new BlockReference(Point3d.Origin, BlockDefs.GetBreakPipe()))
                {
                    blkRef.ScaleFactors = new Scale3d(value);
                    var extents = blkRef.GeometricExtents;
                    _crossBlkWidth = extents.MinPoint.DistanceTo(new Point3d(extents.MaxPoint.X, extents.MinPoint.Y, 0));
                }
            }
        }
        private static double _breakPipeScale;
        public static bool IncludeElevationSign;
        public static double MainWidth { get; set; }
        public static string MainSize { get; set; }
        public static int MainColor { get; set; }
        public static double BranchWidth { get; set; }
        public static string BranchSize { get; set; }
        public static int BranchColor { get; set; }
        public static List<string> SprinklerNames { get; set; } = new List<string>();
        public static bool IsDrawingMain { get; set; }
        public static Unit Unit { get; set; }
        public static bool UseMainPipeLineWeight { get; set; }
        public static LineWeight MainLineWeight { get; set; }
        public static bool UseBranchPipeLineWeight { get; set; }
        public static LineWeight BranchLineWeight { get;set; }
        public static ConnectionType ConnectionType { get; set; }
        public static double RotationAngle { get; set; }
        public static double TextHeight { get; set; }
        public static double RiserLenght { get; set; }
        public static string ZConnectionSize { get; set; }
        public static double? PressureDrop { get; set; }
        public static double? EQ_Length { get; set; }
        public static ObjectId TextStyle {  get; set; }
        public static string PipeSize { get; set; }
        public static SizingMethod SizingMethod { get; set; }
        public static MaterialEnum MainPipeMaterial { get; set; }
        public static MaterialEnum BranchPipeMaterial { get; set; }
        public static string MainPipeSubMaterial { get; set; }
        public static string BranchPipeSubeMaterial { get; set; }
        public static int cFactor { get; set; }
        public static string MainPipeDiameter { get; set; }
        public static string BranchPipeDiameter { get; set; }
        public static string MainPipeCFactor { get; set; }
        public static string BranchPipeCFactor { get; set; }
        public static Int16 OsMode { get; set; }

    }
}