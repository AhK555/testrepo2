using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace PipeApp
{
    public class CustomBlockSelector
    {
        public ObjectIdCollection BlkobjectIds { get; set; } = new ObjectIdCollection();
        private Document _doc;
        private Database _db;
        private Editor _ed;
        public List<string> SelectedBlockNames { get; set; } = new List<string>();
        public CustomBlockSelector(Document doc)
        {
            _doc = doc;
            _db = doc.Database;
            _ed = doc.Editor;
            BlkobjectIds = CreateBlkObjIDCollection();
        }

        private ObjectIdCollection CreateBlkObjIDCollection()
        {
            var ids = new ObjectIdCollection();
            do
            {
                var tempSSet = CreateSelectionSet();

                if (tempSSet is null)
                {
                    Show(ids, true);
                    break;
                }
                else if (tempSSet.Count == 0)
                    break;
                else
                {
                    var tempObjIDColl = new ObjectIdCollection(tempSSet.GetObjectIds());
                    Show(tempObjIDColl, false);

                    foreach (ObjectId id in tempObjIDColl)
                        ids.Add(id);
                }
            } while (true);

            Show(ids, true);
            _db.TransactionManager.QueueForGraphicsFlush();
            _ed.UpdateScreen();
            _ed.Regen();
            return ids;

        }
        private SelectionSet CreateSelectionSet()
        {
            var peo = new PromptEntityOptions("\nSelect Objects");
            peo.SetRejectMessage("\nNot a valid object type");
            peo.AddAllowedClass(typeof(BlockReference), true);
            peo.AllowNone = true;
            TypedValue[] filterValue;
            var per = _ed.GetEntity(peo);

            if (per.Status == PromptStatus.Cancel)
                return null;
            else if (per.Status == PromptStatus.None)
            {
                var ids = new ObjectId[0];
                return SelectionSet.FromObjectIds(ids);
            }

            using (var tr = _db.TransactionManager.StartOpenCloseTransaction())
            {
                string blkName;
                string blkLayer;
                try
                {
                    var blkRef = tr.GetObject(per.ObjectId, OpenMode.ForRead) as BlockReference;
                    blkName = blkRef.Name;
                    blkLayer = blkRef.Layer;
                }
                catch (Gssoft.Gscad.Runtime.Exception ex)
                {
                    _ed.WriteMessage("Casting Issue " + ex.Message);
                    return null;
                }

                filterValue = new TypedValue[2];
                filterValue.SetValue(new TypedValue((int)DxfCode.Start, "insert"), 0);
                filterValue.SetValue(new TypedValue((int)DxfCode.BlockName, blkName), 1);
                
                if(!SelectedBlockNames.Contains(blkName))
                    SelectedBlockNames.Add(blkName);
                var selectionFilter = new SelectionFilter(filterValue);
                var pts = NSVLibUtils.ScreenSizeUtils.GetScreenCoordinate(_doc);

                dynamic cadApp = ACAD.AcadApplication;
                var firstCorner = pts[0];
                var secondCorner = pts[1];

                var point1 = new[] { firstCorner.X, firstCorner.Y, 0.0 };
                var point2 = new[] { secondCorner.X, secondCorner.Y, 0 };
                cadApp.ZoomWindow(point1, point2);
                var result = _ed.SelectAll(selectionFilter);
                cadApp.ZoomWindow(point1, point2);
                tr.Commit();
                return result.Value;
            }

        }
        private void Show(ObjectIdCollection objIDs, bool condition)
        {
            using (var tr = _db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId s in objIDs)
                {
                    var ent = (Entity)tr.GetObject(s, OpenMode.ForWrite);
                    ent.Visible = condition;
                }
                _db.TransactionManager.QueueForGraphicsFlush();
                tr.Commit();
            }
        }
    }
}
