using Gile.AutoCAD.R20.Geometry;
using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using NSVLIBConstants;
using NSVLibUtils.Bahrani;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Application = Gssoft.Gscad.ApplicationServices.Application;

namespace PipeApp
{
    public static class LocalUtils
    {
        private static Matrix3d _UCS;

        public static bool IsOnEnd(Point3d pt, Line ln) // Move to LibUtils.Check
        {
            return NSVLibUtils.PointUtils.IsTheSamePoint(pt, ln.EndPoint) ||
                NSVLibUtils.PointUtils.IsTheSamePoint(pt, ln.StartPoint);
        }

        public static Extents3d GetGeometricExtents(BlockReference br) //Move to LibUtils later
        {
            var db = Gssoft.Gscad.ApplicationServices.Application.
                DocumentManager.MdiActiveDocument.Database;
            var extents = new Extents3d();
            using (var entitySet = new DBObjectCollection())
            {
                br.Explode(entitySet);
                foreach (Entity entity in entitySet)
                {
                    if (entity.Visible)
                    {
                        var bounds = entity.Bounds;
                        if (bounds.HasValue)
                        {
                            extents.AddExtents(bounds.Value);
                        }
                    }
                    entity.Dispose();
                }
            }
            return extents;
        }

        internal static Region CreateLineTolerantRegion(Line line, double tolerance=1e-2)
        {
            var sp = new Point2d(line.StartPoint.X, line.StartPoint.Y);
            var ep = new Point2d (line.EndPoint.X, line.EndPoint.Y);
            var dir = ep - sp;
            var normal = new Vector2d(-dir.Y, dir.X).GetNormal() * tolerance;

            var p1 = sp + normal;
            var p2 = ep + normal;
            var p3 = ep - normal;
            var p4 = sp - normal;

            var regionPl = new Polyline();
            regionPl.AddVertexAt(0, p1, 0, 0, 0);
            regionPl.AddVertexAt(1, p2, 0, 0, 0);
            regionPl.AddVertexAt(2, p3, 0, 0, 0);
            regionPl.AddVertexAt(3, p4, 0, 0, 0);
            regionPl.Closed = true;

            var reg = regionPl.ToRegion();
            regionPl.Dispose();

            return reg;
        }

        public static Curve Flatten(this Curve curve)
        {
            //TODO: move ZRemove to NSVLIBUtils fuck this life
            if (curve is Polyline pl)
            {
                var pline = pl.Clone() as Polyline;
                if (pline.Elevation != 0.0)
                {
                    for (int i = 0; i < pline.NumberOfVertices; i++)
                    {
                        var point = pline.GetPoint3dAt(i);
                        pline.SetPointAt(i, new Point2d(point.X, point.Y));
                    }
                    pline.Elevation = 0;
                    pline.Normal = Vector3d.ZAxis;
                }
                return pline;
            }
            else if (curve is Line line)
            {
                return new Line(line.StartPoint.Flatten(), line.EndPoint.Flatten());
            }
            else if (curve is Circle circle)
            {
                var newCircle = circle.Clone() as Circle;
                var transFormValue = -1 * circle.Center.Z;
                newCircle.TransformBy(Matrix3d.Displacement(Vector3d.ZAxis * transFormValue));

                return newCircle;
            }
            else
                throw new NotImplementedException($"Flatten Method not implemented for {curve.GetType()}"); 
        }

        internal static void ResetUCSToWorld()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            _UCS = ed.CurrentUserCoordinateSystem;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var vtr = tr.GetObject(db.CurrentViewportTableRecordId, OpenMode.ForWrite) as ViewportTableRecord;

                vtr.SetUcsToWorld();
                tr.Commit();
            }
        }

        internal static void ResetUCSToUser()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var vtr = tr.GetObject(db.CurrentViewportTableRecordId, OpenMode.ForWrite) as ViewportTableRecord;

                vtr.SetUcs(_UCS.CoordinateSystem3d.Origin, _UCS.CoordinateSystem3d.Xaxis, _UCS.CoordinateSystem3d.Yaxis);
                tr.Commit();
            }
        }
    }
}
