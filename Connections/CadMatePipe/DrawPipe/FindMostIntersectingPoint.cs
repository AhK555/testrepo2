using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSVLibUtils;
using Gssoft.Gscad.ApplicationServices;
using Gile.AutoCAD.R20.Geometry;
namespace PipeApp.DrawPipe
{
    public static class FindMostIntersectingPoint
    {
        public static List<Point3d> WithBlockReference(Curve curve,BlockReference blk)
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            var intersectionPoints = new List<Point3d>();

            using (var ents = new DBObjectCollection())
            {
                blk.Explode(ents);
                foreach (DBObject o in ents)
                {
                    var ent = o as Entity;
                    if (ent.GetType() != typeof(Hatch) && ent.GetType() != typeof(AttributeDefinition))// && ent.GetType() != typeof(MText) && ent.GetType() != typeof(DBText))
                    {
                        if (ent is Curve crv)
                            ent = crv.Flatten();

                        try
                        {
                            var pts = new Point3dCollection();
                            curve.IntersectWith(ent, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                            foreach (Point3d p in pts)
                            {
                                intersectionPoints.Add(p);
                            }
                            o.Dispose();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            if (intersectionPoints.Count == 0)
                throw new Exception("Block has no Intersection with curve!");
            var output = new List<Point3d>();
            Point3d? pointInside = null;
            if (Check.IsPointInBoundingBox(blk, curve.StartPoint.Flatten()))
            {
                pointInside = curve.StartPoint;
                var pt = (from p in intersectionPoints orderby p.DistanceTo(curve.EndPoint) select p).First();
                output.Add(pt);
            }
            if (Check.IsPointInBoundingBox(blk, curve.EndPoint.Flatten()))
            {
                if(pointInside.HasValue)
                    throw new Exception("curve is completely inside block reference!");
                else
                {
                    pointInside = curve.EndPoint;
                    var pt = (from p in intersectionPoints orderby p.DistanceTo(curve.StartPoint) select p).First();
                    output.Add(pt);
                }
            }
            if (!pointInside.HasValue)
            {
                output.Add((from p in intersectionPoints orderby p.DistanceTo(curve.StartPoint) select p).First());
                output.Add((from p in intersectionPoints orderby p.DistanceTo(curve.EndPoint) select p).First());
            }
            return output;
        }
    }
}
