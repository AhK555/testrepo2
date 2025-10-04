using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gssoft.Gscad.Geometry;
using NSVLibUtils;
using Gile.AutoCAD.R20.Geometry;


namespace PipeApp
{
    public static class FilterSelection
    {
        public static ObjectIdCollection GetPipes(Editor ed)
        {
            var sset = ed.SelectAll(
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"LINE"),
                        new TypedValue((int)DxfCode.LayerName,"NSVMainPipe,NSVBranchPipe")
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }

        public static ObjectIdCollection GetPipes(Editor ed,Curve pipe) 
        {
            var sset = ed.SelectFence(
                new Point3dCollection
                {
                    pipe.StartPoint.TransformBy(ed.WCS2UCS()),
                    pipe.EndPoint.TransformBy(ed.WCS2UCS())
                },
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"LINE"),
                        new TypedValue((int)DxfCode.LayerName,"NSVMainPipe,NSVBranchPipe")
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }

        public static ObjectIdCollection GetPipeInWindow(Editor ed, Tuple<Point3d, Point3d> window)
        {
            var (pt1, pt2) = window;
            pt1.TransformBy(ed.WCS2UCS());
            pt2.TransformBy(ed.WCS2UCS());
            var sset = ed.SelectCrossingWindow(pt1, pt2
                , new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"LINE"),
                        new TypedValue((int)DxfCode.LayerName,"NSVMainPipe,NSVBranchPipe")
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }
        public static ObjectIdCollection GetPrettyPipe(Editor ed, Line pipe)
        {
            var sset = ed.SelectFence(
                new Point3dCollection
                {
                    pipe.StartPoint.TransformBy(ed.WCS2UCS()),
                    pipe.EndPoint.TransformBy(ed.WCS2UCS())
                },
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"LWPOLYLINE"),
                        new TypedValue((int)DxfCode.LayerName,"NSVPrettyPipe")
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }
        public static ObjectIdCollection GetPrettyPipe(Editor ed)
        {
            var sset = ed.SelectAll(
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"LWPOLYLINE"),
                        new TypedValue((int)DxfCode.LayerName,"NSVPrettyPipe")
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }
        public static ObjectIdCollection GetSprinklers(Editor ed, Curve pipe)
        {
            string blockNames = string.Join(",", InputData.SprinklerNames);
            var fencePoints = new Point3dCollection();
            if(pipe is Polyline polyline)
            {
                for(int i =0; i<polyline.NumberOfVertices;i++)
                    fencePoints.Add(polyline.GetPoint3dAt(i).TransformBy(ed.WCS2UCS()).Flatten());
            }
            else if(pipe is Line line)
            {
                fencePoints.Add(line.StartPoint.TransformBy(ed.WCS2UCS()));
                fencePoints.Add(line.EndPoint.TransformBy(ed.WCS2UCS()));
                //fencePoints.Add(line.StartPoint);
                //fencePoints.Add(line.EndPoint);
            }
            var sset = ed.SelectFence(
                fencePoints,
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"INSERT"),
                        new TypedValue((int)DxfCode.BlockName,blockNames)
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()): null);
        }

        public static ObjectIdCollection GetSprinklers(Editor ed)
        {
            var blockNames = string.Join(",", InputData.SprinklerNames);
            var sset = ed.SelectAll(
                new SelectionFilter(
                    new TypedValue[]
                    {
                        new TypedValue((int)DxfCode.Start,"INSERT"),
                        new TypedValue((int)DxfCode.BlockName,blockNames)
                    }));
            return (sset.Status == PromptStatus.OK ? new ObjectIdCollection(sset.Value.GetObjectIds()) : null);
        }

        public static List<BlockReference> GetSprinklers(Curve pipe,Editor ed)
        {
            var objectIds = new ObjectIdCollection();
            objectIds = GetSprinklers(ed, pipe);
            if(objectIds is null)
                return null;
            var blocks = GetBlockList(objectIds, ed);

            return blocks.OrderBy(blk => blk.Position.DistanceTo(pipe.StartPoint)).ToList();
        }
        private static List<BlockReference> GetBlockList(
            ObjectIdCollection intersectingBlockRefIds,Editor ed)
        {
            using (var tr = ed.Document.Database.TransactionManager.StartTransaction())
            {
                var result = new List<BlockReference>();
                foreach (ObjectId id in intersectingBlockRefIds)
                    result.Add(tr.GetObject(id, OpenMode.ForRead) as BlockReference);
                tr.Commit();
                
                return result;
            }
        }
    }
}
