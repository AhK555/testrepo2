using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using NSVLibUtils.Bahrani;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Region = Gssoft.Gscad.DatabaseServices.Region;
using Gile.AutoCAD.R20.Geometry;
using Application = Gssoft.Gscad.ApplicationServices.Application;
using Gssoft.Gscad.BoundaryRepresentation;

namespace NSVLibUtils
{
    public static class Check
    {
        public static bool IsPointBetween(Point3d point, Line ln)
        {
            if (point.Equals(ln.StartPoint) || point.Equals(ln.EndPoint)) 
                return false;
            else if(IsPointOnLine(point, ln)) 
                return true;
            else 
                return false;
        }

        public static bool IsPointOnLine(Point3d point, Line ln)
        {
            using (var segment = new LineSegment3d(ln.StartPoint, ln.EndPoint))
            {
                return segment.IsOn(point);
            }
        }
        public static bool IsPointOnPolyline(Polyline pl, Point3d pt)
        {
            bool isOn = false;
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                Curve3d seg = null;
                SegmentType segType = pl.GetSegmentType(i);
                if (segType == SegmentType.Arc)
                    seg = pl.GetArcSegmentAt(i);
                else if (segType == SegmentType.Line)
                    seg = pl.GetLineSegmentAt(i);
                if (seg != null)
                {
                    isOn = seg.IsOn(pt);
                    seg.Dispose();
                    if (isOn)
                        break;
                }
            }
            return isOn;
        }
        public static bool LayerExist(string layerName)
        {
            bool check = false;
            var db = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            using (var transaction = db.TransactionManager.StartTransaction())
            {
                var layerTable = transaction.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                check = layerTable.Has(layerName);
                transaction.Commit();
            }

            return check;
        }

        public static bool IsPointInBoundingBox(BlockReference blkRef, Point3d point)
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;

            var clonedBlock = blkRef.Clone() as BlockReference;
            var extents = clonedBlock.GeometricExtents;

            using (var pl = new Polyline())
            {
                pl.AddVertexAt(0, new Point2d(extents.MinPoint.X, extents.MinPoint.Y), 0, 0, 0);
                pl.AddVertexAt(1, new Point2d(extents.MaxPoint.X, extents.MinPoint.Y), 0, 0, 0);
                pl.AddVertexAt(2, new Point2d(extents.MaxPoint.X, extents.MaxPoint.Y), 0, 0, 0);
                pl.AddVertexAt(3, new Point2d(extents.MinPoint.X, extents.MaxPoint.Y), 0, 0, 0);
                pl.Closed = true;
                clonedBlock?.Dispose();
                return IsPointInside(pl, point);
            }
        }

        public static bool IsOrthogonal(Line line)
        {
            var tolerance = 0.01;
            
            if (Math.Abs(line.StartPoint.X - line.EndPoint.X) <= tolerance
                || Math.Abs(line.StartPoint.Y - line.EndPoint.Y) <= tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPointInside(Curve curve, Point3d pt)
        {
            if (!curve.Closed) return false;
            var inside = false;
            using (var ray = new Ray())
            {
                ray.BasePoint = pt;
                var ext = curve.GeometricExtents;
                var h = ext.MaxPoint.Y - ext.MinPoint.Y;
                var w = ext.MaxPoint.X - ext.MinPoint.X;
                var l = Math.Max(h, w);
                ray.SecondPoint = new Point3d(pt.X + 2 * l, pt.Y + 2 * l, pt.Z);

                var pts = new Point3dCollection();
                curve.IntersectWith(
                    ray, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count > 0)
                {
                    inside = pts.Count == 1 ? true : pts.Count % 2 != 0;
                }
            }
            return inside;
        }


        public static bool IsPointInside(Region region, Point3d pt)
        {
            if (region.Area < 1)
                return false;

            return RegionExtension.GetPointContainment(region, pt) != PointContainment.Outside;
        }

        public static bool IsPerpendicular(Line a, Line b)
        {
            return new LineSegment3d(a.StartPoint, a.EndPoint)
               .IsPerpendicularTo(new LineSegment3d(b.StartPoint, b.EndPoint));
        }

        public static bool IsParallel(Line a, Line b)
        {
            return new LineSegment3d(a.StartPoint, a.EndPoint)
            .IsParallelTo(new LineSegment3d(b.StartPoint, b.EndPoint));
        }


        public static bool IsLineCompletelyInside(Line ln, Polyline pl)
        {
            var points = new Point3dCollection();
            ln.IntersectWith(pl, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
            try
            {
                if (!Check.IsPointInside(pl, ln.StartPoint) || !Check.IsPointInside(pl, ln.EndPoint))
                {
                    return false;
                }
                for (var i = 0; i < points.Count; i++)
                {
                    for (var j = i + 1; j < points.Count; j++)
                    {
                        using (var ln1 = new Line(points[i], points[j]))
                        {
                            if (!Check.IsPointInside(pl, ln1.GetPointAtDist(ln1.Length / 2)))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return true;
            }
        }

        public static bool IsLineCompletelyInside(Line ln, Region rg)
        {
            var points = new Point3dCollection();
            ln.IntersectWith(rg, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
            if (!Check.IsPointInside(rg, ln.StartPoint) || !Check.IsPointInside(rg, ln.EndPoint))
            {
                return false;
            }
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    using (var ln1 = new Line(points[i], points[j]))
                    {
                        if (!Check.IsPointInside(rg, ln1.GetPointAtDist(ln1.Length / 2)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool IsLineInside(Line line, Polyline polyLine)
        {
            if (IsLineCompletelyInside(line, polyLine))
                return true;
            else if (IsPointInside(polyLine, line.StartPoint))
                return true;
            else if (IsPointInside(polyLine, line.EndPoint))
                return true;
            else
                return false;
        }
        public static bool IsLineInside(Line line, Region region)
        {
            if (IsLineCompletelyInside(line, region))
                return true;
            else if (IsPointInside(region, line.StartPoint))
                return true;
            else if (IsPointInside(region, line.EndPoint))
                return true;
            else
                return false;
        }

        public static bool AreTheSameLine(Line line1, Line line2)
        {
            Tolerance tolerance = new Tolerance(0.001, 0.001);
            if (line1.StartPoint.IsEqualTo(line2.EndPoint, tolerance) && line1.EndPoint.IsEqualTo(line2.StartPoint, tolerance))
            {
                return true;
            }
            if (line1.StartPoint.IsEqualTo(line2.StartPoint) && line1.EndPoint.IsEqualTo(line2.EndPoint))
            {
                return true;
            }
            return false;
        }

        public static bool IsPointInBlockRef(BlockReference blkRef, Point3d point)
        {
            using (var region = GetOutermostBoundary(blkRef))
            {
                return IsPointInside(region, point);
            }
        }

        private static List<Region> GetTotalRegionFromBlockRef(BlockReference blkRef)
        {
            using (var entities = new DBObjectCollection())
            {
                blkRef.Explode(entities);
                using (var curves = new DBObjectCollection())
                {
                    var output = new List<Region>();
                    foreach (Entity entity in entities)
                    {
                        if (entity is Curve)
                        {
                            try
                            {

                                var doc = ACAD.DocumentManager.MdiActiveDocument;
                                var db = doc.Database;
                                var ed = doc.Editor;
                                using (var tr = db.TransactionManager.StartTransaction())
                                {
                                    curves.Add(entity);
                                    var region = Region.CreateFromCurves(curves);
                                    output.Add((region[0]) as Region);
                                }
                            }
                            catch (System.Exception)
                            {
                                continue;
                            }
                        }
                        else if (entity is BlockReference br)
                        {
                            foreach (var r in GetTotalRegionFromBlockRef(br))
                                output.Add(r);
                        }
                    }
                    return output;
                }
            }
        }
        public static Region GetOutermostBoundary(BlockReference br)
        {
            var curves = GetTotalRegionFromBlockRef(br);
            try
            {
                var region = (Region)curves[0];
                for (int i = 1; i < curves.Count; i++)
                {
                    var db = ACAD.DocumentManager.MdiActiveDocument.Database;
                    var rg = (Region)curves[i];
                    region.BooleanOperation(BooleanOperationType.BoolUnite, rg.Clone() as Region);
                    curves[i].Dispose();
                }
                return region;
            }
            catch { return null; }
        }
    }
}
