using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.Geometry;

namespace NSVLibUtils
{
    public static class Extensions
    {
        public static string TrueName(this BlockReference blkRef)
        {
            string retName = string.Empty;
            var doc = Application.DocumentManager.MdiActiveDocument;
            using (var trans = doc.Database.TransactionManager.StartTransaction())
            {
                retName = blkRef.IsDynamicBlock ? ((BlockTableRecord)blkRef.DynamicBlockTableRecord.GetObject(OpenMode.ForRead)).Name : blkRef.Name;
                trans.Commit();
            }

            return retName;
        }
        //public static Point2d Convert2d(this Point3d pt)
        //{
        //    return new Point2d(pt.X,pt.Y);
        //}
    }
}
