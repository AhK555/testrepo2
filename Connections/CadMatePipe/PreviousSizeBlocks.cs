using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
namespace PipeApp
{
    public class PreviousSizeBlocks
    {
        public static void Remove( ObjectIdCollection pipeIds)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var filter = new SelectionFilter(new TypedValue[]
                {
                    new TypedValue((int)DxfCode.Start ,"INSERT"),
                    new TypedValue((int)DxfCode.BlockName , "PipeProperties")
                });

                var selectedAttBlocks = ed.SelectAll(filter);


                if (selectedAttBlocks.Value == null)
                    return;

                foreach (ObjectId pipeID in pipeIds)
                {
                    var pipe = tr.GetObject(pipeID, OpenMode.ForWrite) as Line;

                    foreach (SelectedObject attBlockObj in selectedAttBlocks.Value)
                    {
                        var attBlkRef = tr.GetObject(attBlockObj.ObjectId, OpenMode.ForWrite) as BlockReference;
                        if (attBlkRef == null)
                            continue;

                        if (Check.IsPointOnLine(attBlkRef.Position, pipe))
                        {
                            attBlkRef.Erase();
                        }
                    }
                }

                tr.Commit();
            }
        }
    }
}
