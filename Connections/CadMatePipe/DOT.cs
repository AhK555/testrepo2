using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace PipeApp
{
    public class DOT
    {
        public static void BringPipesToTop()
        {
            var ed = ACAD.DocumentManager.MdiActiveDocument.Editor;
            var pipeIds = FilterSelection.GetPipes(ed);

            if (pipeIds is null)
                return;

            var db = pipeIds[0].Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                var dot = tr.GetObject(currentSpace.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;
                dot.MoveToTop(pipeIds);
                tr.Commit();
            }
        }
        public static void BringPipesToBottom()
        {
            var ed = ACAD.DocumentManager.MdiActiveDocument.Editor;
            var pipeIds = FilterSelection.GetPipes(ed);

            if (pipeIds is null)
                return;

            var db = pipeIds[0].Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                var dot = tr.GetObject(currentSpace.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;
                dot.MoveToBottom(pipeIds);
                tr.Commit();
            }
        }
    }
}
