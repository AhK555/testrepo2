using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSVLibUtils
{
    public class ObjectsUtils
    {
        public static ObjectIdCollection DrawObjects(Database db, params Entity[] entities)
        {
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            var objeIDCollection = new ObjectIdCollection();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                foreach (var entity in entities)
                {
                    if (entity.ObjectId == ObjectId.Null)
                    {
                        objeIDCollection.Add(currentSpace.AppendEntity(entity));
                        tr.AddNewlyCreatedDBObject(entity, true);
                    }
                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
            return objeIDCollection;
        }
        public static ObjectIdCollection DrawObjects(params Entity[] entities)
        {
            var db = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            var objeIDCollection = new ObjectIdCollection();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                foreach (var entity in entities)
                {
                    if (entity.ObjectId == ObjectId.Null)
                    {
                        objeIDCollection.Add(currentSpace.AppendEntity(entity));
                        tr.AddNewlyCreatedDBObject(entity, true);
                    }
                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
            return objeIDCollection;
        }
        public static void MakeInvisible(Database db, ObjectIdCollection objids)
        {
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var currentSpace = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                foreach (ObjectId id in objids)
                {
                    var entity = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    entity.Visible = false;

                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
        }

        public static void RemoveObject(Database db, ObjectIdCollection objids)
        {
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in objids)
                {
                    var entity = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    entity.Erase();
                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
        }
        public static void RemoveObject(ObjectIdCollection objids)
        {
            var db = objids[0].Database;
            if (db == null)
                throw new Exception("Object not in Database");
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in objids)
                {
                    var entity = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    entity.Erase();
                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
        }
        public static void RemoveObject(params Entity[] ents)
        {
            var db = ents[0].Database;
            if (db == null)
                throw new Exception("Object not in Database");
            var ed = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.GetDocument(db).Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                foreach (var entity in ents)
                {
                    if(!entity.IsWriteEnabled)
                        tr.GetObject(entity.ObjectId, OpenMode.ForWrite);
                    entity.Erase();
                }
                tr.Commit();
            }
            db.TransactionManager.QueueForGraphicsFlush();
            ed.UpdateScreen();
        }
    }
}
