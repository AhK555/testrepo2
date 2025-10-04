using Gssoft.Gscad.DatabaseServices;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit = NSVLIBConstants.Enums.Unit;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Gssoft.Gscad.Geometry;
using PipeApp.DrawPipe;
namespace PipeApp
{
    public enum SizingMethod
    {
        Default,
        PipeScheduling,
        AssignNow
    }
    public class PipeSizer
    {
        private ObjectIdCollection _pipeIds;
        public PipeSizer(ObjectIdCollection objIds)
        {
            _pipeIds = objIds;
            if (_pipeIds.Count == 0)
                throw new ArgumentException("No pipe selected to assgin size to");
            PreviousSizeBlocks.Remove(_pipeIds);
        }
        public static void AssingSizeToNewPipes(params Line[] pipes)
        {
            var tr = ACAD.DocumentManager.MdiActiveDocument.Database.TransactionManager.TopTransaction;
            foreach (var pipe in pipes)
            {
                var pt = new DBPoint(Point3d.Origin);
                var objIds = ObjectsUtils.DrawObjects(pt);
                ObjectsUtils.RemoveObject(objIds);
                var newPipe = tr.GetObject(pipe.ObjectId, OpenMode.ForWrite) as Line;
                newPipe.Layer = InputData.IsDrawingMain ? "NSVMainPipe" : "NSVBranchPipe";
                newPipe.ColorIndex = 256;
                newPipe.LineWeight = LineWeight.ByLayer;
                var xDataDictionary = new Dictionary<string, string>
                {
                    {"type","pipe" },
                    {"ismain",InputData.IsDrawingMain.ToString() },
                    {"size",InputData.IsDrawingMain?InputData.MainSize:InputData.BranchSize },
                    {"cFactor",InputData.IsDrawingMain? InputData.MainPipeCFactor:InputData.BranchPipeCFactor },
                    {"dia",InputData.IsDrawingMain? InputData.MainPipeDiameter : InputData.BranchPipeDiameter },
                    {"material", InputData.IsDrawingMain? InputData.MainPipeMaterial.ToString(): InputData.BranchPipeMaterial.ToString() }
                };
                XDataUtil.SetXdata(tr, newPipe, "NSVPipe", xDataDictionary);
            }
        }

        public static void AssignSizeToPreviousSplitedPipes(
            Line originalPipe,
            List<Line> PreviousPipes)
        {
            var tr = ACAD.DocumentManager.MdiActiveDocument.Database.TransactionManager.TopTransaction;
            try
            {
                foreach (var l in PreviousPipes)
                {
                    l.Layer = originalPipe.Layer;
                    l.Color = originalPipe.Color;
                    l.LineWeight = LineWeight.ByLayer;
                    var existingSize = XDataUtil.ReadXData(tr, originalPipe.ObjectId, "NSVPipe", "size");
                    XDataUtil.SetXdata(tr, l, "NSVPipe", new Dictionary<string, string> { { "size", existingSize } });
                }
            }
            catch (Exception)
            {
                throw new Exception("\nError while assigning Xdata to splitted new Pipes!");
            }
        }
        public void ShowPipeSize()
        {
            var tr = _pipeIds[0].Database.TransactionManager.TopTransaction;
            if (tr is null)
                throw new Exception("Operation must be in transaction scope");

            var blkDef = BlockDefs.GetPipeSize(_pipeIds[0].Database);
            foreach (ObjectId id in _pipeIds)
            {
                var pipe = (Line)tr.GetObject(id, OpenMode.ForWrite);

                var midPoint = pipe.GetPointAtDist(pipe.Length / 2);
                var blkRef = new BlockReference(midPoint, blkDef);

                var pipeAngle = Math.Round(pipe.Angle, 4);
                if ((pipeAngle <= Math.Round(Math.PI / 2, 4) && pipeAngle >= 0) ||
                    (pipeAngle > Math.Round(3 * Math.PI / 2, 4) && pipeAngle <= Math.Round(2 * Math.PI, 4)))
                    blkRef.Rotation = pipe.Angle;
                else
                    blkRef.Rotation = pipe.Angle - Math.PI;
                ObjectsUtils.DrawObjects(blkRef);
    
                double pipewidth;
                var prettyPipe = PipeVisualizer.GetCorespondingPrettyPipe(pipe);
                if (prettyPipe == null)
                    pipewidth = 1;
                else
                {
                    pipewidth = PipeVisualizer.GetCorespondingPrettyPipe(pipe).GetStartWidthAt(0);
                }
                var pipeProperties = GetPipeProperties(pipe,tr);
                PrintOnScreen(blkRef,pipe,pipewidth, pipeProperties, pipeAngle);
            }
        }

        private List<string> GetPipeProperties(Line pipe,Transaction tr)
        {
            var pipeLength = string.Empty;
            switch (InputData.Unit)
            {
                case Unit.imperial:
                    var feetPart = (int)Math.Floor(pipe.Length / 12);
                    var inchPart = Math.Round(pipe.Length - feetPart * 12);
                    if (inchPart == 12)
                    {
                        feetPart = feetPart + 1;
                        inchPart = 0;
                    }
                    pipeLength = $"{feetPart}\'-{inchPart}\"";
                    break;
                case Unit.metric:
                    pipeLength = Math.Round(pipe.Length).ToString();
                    break;
                default:
                    throw new NotImplementedException("unit not supported yet!");
            }
            var pipeSize = InputData.PipeSize;
            if (InputData.SizingMethod != SizingMethod.AssignNow)
                pipeSize = XDataUtil.ReadXData(tr, pipe.ObjectId, "NSVPipe", "size");
            else
            {
                var xDataDictionary = new Dictionary<string, string>
                {
                    {"size",pipeSize}
                };
                XDataUtil.SetXdata(tr, pipe, "NSVPipe", xDataDictionary);
            }
            if (string.IsNullOrEmpty(pipeSize))
                throw new Exception("Size is not assigned");

            return new List<string> { pipeSize, pipeLength };
        }

        private void PrintOnScreen(
            BlockReference blkRef,
            Line pipe,
            double width,
            List<string> pipeProperties, 
            double pipeAngle)
        {
            var tr = blkRef.Database.TransactionManager.TopTransaction;
            var blkDef = (BlockTableRecord)tr.GetObject(blkRef.BlockTableRecord, OpenMode.ForWrite);

            if (!blkDef.HasAttributeDefinitions)
                throw new Exception("Size block reference does not contain attributes!");
            foreach (ObjectId objectId in blkDef)
            {
                if (tr.GetObject(objectId, OpenMode.ForRead) is AttributeDefinition attDef && !attDef.Constant)
                {
                    var attRef = new AttributeReference();
                    attRef.Position = blkRef.Position;
                    attRef.SetAttributeFromBlock(attDef, blkRef.BlockTransform);

                    attRef.TextStyleId = InputData.TextStyle;
                    attRef.Height = InputData.TextHeight;

                    var prettyPipeVector = Vector3d.ZAxis;
                    prettyPipeVector = pipe.StartPoint.GetVectorTo(pipe.EndPoint).GetNormal()
                        * (width + (attRef.Height * 0.7));
                    
                    var perpendicularVector = new Vector3d();

                    if ((pipeAngle <= Math.Round(Math.PI / 2, 4) && pipeAngle >= 0) ||
                        (pipeAngle > Math.Round(3 * Math.PI / 2, 4) && pipeAngle <= Math.Round(2 * Math.PI, 4)))
                    {
                        perpendicularVector = prettyPipeVector.TransformBy(
                            Matrix3d.Rotation(Math.PI / 2, pipe.Normal, Point3d.Origin));
                    }
                    else
                    {
                        perpendicularVector = prettyPipeVector.TransformBy(
                            Matrix3d.Rotation(-Math.PI / 2, pipe.Normal, Point3d.Origin));
                    }

                    if (attDef.Tag == "Diameter")
                    {
                        attRef.Justify = AttachmentPoint.MiddleCenter;
                        attRef.HorizontalMode = TextHorizontalMode.TextCenter;
                        attRef.VerticalMode = TextVerticalMode.TextVerticalMid;
                        attRef.TextString = pipeProperties[0];
                        attRef.AlignmentPoint = blkRef.Position.Add(perpendicularVector);
                        attRef.AdjustAlignment(blkRef.Database);
                    }
                    else if (attDef.Tag == "Lenght")
                    {
                        attRef.Justify = AttachmentPoint.MiddleCenter;
                        attRef.HorizontalMode = TextHorizontalMode.TextCenter;
                        attRef.VerticalMode = TextVerticalMode.TextVerticalMid;
                        attRef.TextString = pipeProperties[1];
                        attRef.AlignmentPoint = blkRef.Position.Add(-perpendicularVector);
                        attRef.AdjustAlignment(blkRef.Database);
                    }
                    blkRef.AttributeCollection.AppendAttribute(attRef);
                    tr.AddNewlyCreatedDBObject(attRef, true);
                }
            }
        }
    }
}
