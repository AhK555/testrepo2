using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.Colors;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ACADException = Gssoft.Gscad.Runtime.Exception;
using SysException = System.Exception;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace NSVLibUtils.Bahrani
{
    public static class Utilities
    {
        public static long StringHandleToLong(string strHandle)
        {
            return Convert.ToInt64(strHandle, 16);
        }
        public static ObjectId GetObjectId(Document doc, Handle handle)
        {
            return doc.Database.GetObjectId(false, handle, 0);
        }
        public static ObjectId GetObjectId(this Handle handle, Document doc)
        {
            return doc.Database.GetObjectId(false, handle, 0);
        }
        public static string GetXData(this ResultBuffer resBuf, string tag)
        {
            return resBuf.AsArray().Where(x => x.Value.ToString().StartsWith($"{tag}:"))
                .Select(x => x.Value.ToString().Replace($"{tag}:", "")).FirstOrDefault(); ;
        }

        public static DBObjectCollection ToPolyline(this Region reg)
        {
            // We will return a collection of entities
            // (should include closed Polylines and other
            // closed curves, such as Circles)
            DBObjectCollection res = new DBObjectCollection();
            // Explode Region -> collection of Curves / Regions
            DBObjectCollection cvs = new DBObjectCollection();
            if (reg.Area == 0) return cvs;
            reg.Explode(cvs);
            // Create a plane to convert 3D coords
            // into Region coord system
            Plane pl = new Plane(new Point3d(0, 0, 0), reg.Normal);
            using (pl)
            {
                bool finished = false;
                while (!finished && cvs.Count > 0)
                {
                    // Count the Curves and the non-Curves, and find
                    // the index of the first Curve in the collection
                    int cvCnt = 0, nonCvCnt = 0, fstCvIdx = -1;
                    for (int i = 0; i < cvs.Count; i++)
                    {
                        Curve tmpCv = cvs[i] as Curve;
                        if (tmpCv == null) nonCvCnt++;
                        else
                        {
                            // Closed curves can go straight into the
                            // results collection, and aren't added
                            // to the Curve count
                            if (tmpCv.Closed)
                            {
                                res.Add(tmpCv);
                                cvs.Remove(tmpCv);
                                // Decrement, so we don't miss an item
                                i--;
                            }
                            else
                            {
                                cvCnt++;
                                if (fstCvIdx == -1) fstCvIdx = i;
                            }
                        }
                    }

                    if (fstCvIdx >= 0)
                    {
                        // For the initial segment take the first
                        // Curve in the collection
                        Curve fstCv = (Curve)cvs[fstCvIdx];
                        // The resulting Polyline
                        Polyline p = new Polyline();
                        // Set common entity properties from the Region
                        p.SetPropertiesFrom(reg);
                        // Add the first two vertices, but only set the
                        // bulge on the first (the second will be set
                        // retroactively from the second segment)
                        // We also assume the first segment is counter-
                        // clockwise (the default for arcs), as we're
                        // not swapping the order of the vertices to
                        // make them fit the Polyline's order
                        p.AddVertexAt(p.NumberOfVertices, fstCv.StartPoint.Convert2d(pl), BulgeFromCurve(fstCv, false),
                            0, 0);
                        p.AddVertexAt(p.NumberOfVertices, fstCv.EndPoint.Convert2d(pl), 0, 0, 0);
                        cvs.Remove(fstCv);
                        // The next point to look for
                        Point3d nextPt = fstCv.EndPoint;
                        // We no longer need the curve
                        fstCv.Dispose();
                        // Find the line that is connected to
                        // the next point
                        // If for some reason the lines returned were not
                        // connected, we could loop endlessly.
                        // So we store the previous curve count and assume
                        // that if this count has not been decreased by
                        // looping completely through the segments once,
                        // then we should not continue to loop.
                        // Hopefully this will never happen, as the curves
                        // should form a closed loop, but anyway...
                        // Set the previous count as artificially high,
                        // so that we loop once, at least.
                        int prevCnt = cvs.Count + 1;
                        while (cvs.Count > nonCvCnt && cvs.Count < prevCnt)
                        {
                            prevCnt = cvs.Count;
                            foreach (DBObject obj in cvs)
                            {
                                Curve cv = obj as Curve;
                                if (cv != null)
                                {
                                    // If one end of the curve connects with the
                                    // point we're looking for...
                                    if (cv.StartPoint == nextPt || cv.EndPoint == nextPt)
                                    {
                                        // Calculate the bulge for the curve and
                                        // set it on the previous vertex
                                        double bulge = BulgeFromCurve(cv, cv.EndPoint == nextPt);
                                        if (bulge != 0.0) p.SetBulgeAt(p.NumberOfVertices - 1, bulge);
                                        // Reverse the points, if needed
                                        if (cv.StartPoint == nextPt) nextPt = cv.EndPoint;
                                        else
                                            // cv.EndPoint == nextPt
                                            nextPt = cv.StartPoint;
                                        // Add out new vertex (bulge will be set next
                                        // time through, as needed)
                                        p.AddVertexAt(p.NumberOfVertices, nextPt.Convert2d(pl), 0, 0, 0);
                                        // Remove our curve from the list, which
                                        // decrements the count, of course
                                        cvs.Remove(cv);
                                        cv.Dispose();
                                        break;
                                    }
                                }
                            }
                        }

                        // Once we have added all the Polyline's vertices,
                        // transform it to the original region's plane
                        p.TransformBy(Matrix3d.PlaneToWorld(pl));
                        res.Add(p);
                        if (cvs.Count == nonCvCnt) finished = true;
                    }

                    // If there are any Regions in the collection,
                    // recurse to explode and add their geometry
                    if (nonCvCnt > 0 && cvs.Count > 0)
                    {
                        foreach (DBObject obj in cvs)
                        {
                            Region subReg = obj as Region;
                            if (subReg != null)
                            {
                                DBObjectCollection subRes = ToPolyline(subReg);
                                foreach (DBObject o in subRes) res.Add(o);
                                cvs.Remove(subReg);
                                subReg.Dispose();
                            }
                        }
                    }

                    if (cvs.Count == 0) finished = true;
                }
            }

            return res;
        }

        // Helper function to calculate the bulge for arcs
        private static double BulgeFromCurve(Curve cv, bool clockwise)
        {
            double bulge = 0.0;
            Arc a = cv as Arc;
            if (a != null)
            {
                double newStart;
                // The start angle is usually greater than the end,
                // as arcs are all counter-clockwise.
                // (If it isn't it's because the arc crosses the
                // 0-degree line, and we can subtract 2PI from the
                // start angle.)
                if (a.StartAngle > a.EndAngle) newStart = a.StartAngle - 8 * Math.Atan(1);
                else newStart = a.StartAngle;
                // Bulge is defined as the tan of
                // one fourth of the included angle
                bulge = Math.Tan((a.EndAngle - newStart) / 4);
                // If the curve is clockwise, we negate the bulge
                if (clockwise) bulge = -bulge;
            }

            return bulge;
        }

        public static bool IsInside(this Region region, Point2d pnt)
        {
            Extents3d ext = region.GeometricExtents;

            if (pnt.X > ext.MinPoint.X &&
                pnt.X < ext.MaxPoint.X &&
                pnt.Y > ext.MinPoint.Y &&
                pnt.Y < ext.MaxPoint.Y) return true;

            return false;
        }
        public static Hatch SetHatchByObjectId(ObjectIdCollection objectIdsCollection, bool isGreen)
        {
            Hatch hatch = new Hatch();
            hatch.SetHatchPattern(HatchPatternType.PreDefined, "Solid");
            hatch.ColorIndex = isGreen ? 3 : 1;

            hatch.Associative = true;
            hatch.AppendLoop(HatchLoopTypes.Default, objectIdsCollection);
            hatch.EvaluateHatch(true);

            return hatch;
        }
        public static Point3d Convert3d(this Point2d pt2d, double z = 0)
        {
            return new Point3d(pt2d.X, pt2d.Y, z);
        }
        public static bool isCrossPoly(this Line line, Polyline poly)
        {
            Point3dCollection pntColls = new Point3dCollection();
            poly.IntersectWith(line, Intersect.OnBothOperands, new Plane(), pntColls, IntPtr.Zero, IntPtr.Zero);
            return pntColls.Count > 2;
        }
        public static Polyline[] GetTriangles(Polyline polygon)
        {
            List<Polyline> retPolies = new List<Polyline>();
            int j = 0;
            bool isJPlus = false;
            for (int i = 0; i + 2 < polygon.NumberOfVertices; i++)
            {
                Line tmp3rdLine = new Line(polygon.GetPoint3dAt(i + 2), polygon.GetPoint3dAt(j));
                Point3d midPnt = tmp3rdLine.GetPointAtParameter(tmp3rdLine.EndParam / 2);

                if (isCrossPoly(tmp3rdLine, polygon))
                {
                    tmp3rdLine = new Line(polygon.GetPoint3dAt(i + 2), polygon.GetPoint3dAt(j));
                    j = i + 1;
                    i++;
                    isJPlus = true;
                }

                try
                {
                    Polyline tmpPoly = new Polyline();
                    tmpPoly.AddVertexAt(0, polygon.GetPoint2dAt(j), 0, 0, 0);
                    tmpPoly.AddVertexAt(1, polygon.GetPoint2dAt(i + 1), 0, 0, 0);
                    tmpPoly.AddVertexAt(2, polygon.GetPoint2dAt(i + 2), 0, 0, 0);
                    tmpPoly.Closed = true;
                    retPolies.Add(tmpPoly);
                }
                catch (ACADException)
                {
                }
                catch (SysException)
                {
                }

            }

            if (isJPlus)
            {
                Polyline tmpPoly = new Polyline();
                tmpPoly.AddVertexAt(0, polygon.GetPoint2dAt(0), 0, 0, 0);
                tmpPoly.AddVertexAt(1, polygon.GetPoint2dAt(j), 0, 0, 0);
                tmpPoly.AddVertexAt(2, polygon.GetPoint2dAt(polygon.NumberOfVertices - 1), 0, 0, 0);
                tmpPoly.Closed = true;
                retPolies.Add(tmpPoly);
            }

            return retPolies.ToArray();
        }
        public static bool IsIntersect(this Entity orginalEntity, Entity secondEntity)
        {
            Point3dCollection pntColl = new Point3dCollection();

            orginalEntity.IntersectWith(secondEntity, Intersect.OnBothOperands, pntColl, IntPtr.Zero, IntPtr.Zero);

            return pntColl.Count > 0;
        }
        
        public static Region ToRegion(this DBObject polyline)
        {
            if(polyline is Polyline pl)
            {
                if (pl.Area < 0.01)
                    return new Region();
            }
            try
            {
                using (DBObjectCollection acDBObjColl = new DBObjectCollection())
                {
                    acDBObjColl.Add(polyline);
                    var regs = Region.CreateFromCurves(acDBObjColl);
                    var region = regs[0] as Region;
                    regs.Dispose();
                    return region;
                }
            }
            catch (Gssoft.Gscad.Runtime.Exception ex)
            {
                Debug.Print("*****  ACAD Exception in to region   *****");
                throw new ArgumentException("\nACAD exception in polylien to region: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Debug.Print("******   System Exception in to region    ******");
                throw new ArgumentException("\nsystem exception in polylien to region: " + ex.Message);
            }
        }
        
       
        public static Region ToRegion(this Hatch hatch)
        {
            if (hatch.NumberOfLoops == 0) return new Region();
            Region region = new Region();
            DBObjectCollection dbColl = new DBObjectCollection();

            for (int i = 0; i < hatch.NumberOfLoops; i++)
            {
                HatchLoop hatchLoop = hatch.GetLoopAt(i);
                Polyline tmpPoly = new Polyline();

                for (int j = 0; j < hatchLoop.Polyline.Count; j++)
                {
                    tmpPoly.AddVertexAt(j, hatchLoop.Polyline[j].Vertex, hatchLoop.Polyline[j].Bulge, 0, 0);
                }

                dbColl.Add(tmpPoly);
            }

            DBObjectCollection s = Region.CreateFromCurves(dbColl);
            foreach (Region reg in s)
            {
                region.BooleanOperation(BooleanOperationType.BoolUnite, reg);
            }
            region.XData = hatch.XData;
            return region;
        }


        public static ObjectId TextStyleByFont(this Database database, string fontName)
        {
            using (Transaction trans = database.TransactionManager.StartTransaction())
            using (TextStyleTable txtStlTbl = trans.GetObject(database.TextStyleTableId, OpenMode.ForRead) as TextStyleTable)
            using (TextStyleTableRecord txtStlRec = new TextStyleTableRecord())
            {
                foreach (ObjectId txtStlId in txtStlTbl)
                {
                    if ((trans.GetObject(txtStlId, OpenMode.ForRead) as TextStyleTableRecord).FileName == fontName)
                        return txtStlId;
                }
                txtStlRec.FileName = fontName;
                string tmpName = fontName.ToLower().Replace(".shx", "").Replace("ttf", "");
                if (!txtStlTbl.Has(tmpName)) txtStlRec.Name = tmpName;
                else txtStlRec.Name = $"_{tmpName}";

                txtStlTbl.UpgradeOpen();
                ObjectId txtStlyId = txtStlTbl.Add(txtStlRec);
                trans.AddNewlyCreatedDBObject(txtStlRec, true);

                trans.Commit();
                return txtStlyId;
            }
        }
        public static ObjectId TextStyleByFont(this TextStyleTable textStyleTable, string fontName)
        {
            foreach (ObjectId txtStyleId in textStyleTable)
            {
                if ((txtStyleId.GetObject(OpenMode.ForRead) as TextStyleTableRecord).FileName == fontName)
                    return txtStyleId;
            }

            using (Transaction trans = textStyleTable.Database.TransactionManager.StartTransaction())
            using (TextStyleTableRecord txtStlRec = new TextStyleTableRecord())
            {
                txtStlRec.FileName = fontName;
                string tmpName = fontName.ToLower().Replace(".shx", "").Replace("ttf", "");
                if (!textStyleTable.Has(tmpName)) txtStlRec.Name = tmpName;
                else txtStlRec.Name = $"_{tmpName}";

                textStyleTable.UpgradeOpen();
                ObjectId txtStlyId = textStyleTable.Add(txtStlRec);
                trans.AddNewlyCreatedDBObject(txtStlRec, true);

                trans.Commit();
                return txtStlyId;
            }
            // return ObjectId.Null;
        }

    }
}

