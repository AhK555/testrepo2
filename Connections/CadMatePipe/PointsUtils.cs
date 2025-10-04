using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSVLibUtils
{
    public static class PointUtils
    {
        public static bool IsTheSamePoint(Point3d p1, Point3d p2, double tolerance=0.0001)
        {
            var Tolerance = new Tolerance(tolerance, tolerance);
            var dist = p1.DistanceTo(p2);
            return dist <= Tolerance.EqualPoint;
        }

        public static Point2d ProjectTo2d(Point3d pt1, Editor ed)
        {
            var currentUserCoordinate = ed.CurrentUserCoordinateSystem;
            var projectionPlane = new Plane(currentUserCoordinate.CoordinateSystem3d.Origin, currentUserCoordinate.CoordinateSystem3d.Zaxis);
            return pt1.Convert2d(projectionPlane);
        }

        public static Point2d GetMedianByAxis(List<Point2d> points, Vector3d axis)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Point list is empty.");

            List<Point2d> sorted;
            if (axis == Vector3d.XAxis)
                sorted = points.OrderBy(p => p.X).ToList();
            else 
                sorted = points.OrderBy(p => p.Y).ToList();

            int mid = sorted.Count / 2;
            return sorted[mid]; // Note: returns the upper median if count is even
        }

        
    }
}
