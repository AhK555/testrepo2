using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Runtime;
using System.Collections.Generic;
using Gssoft.Gscad.Geometry;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Gssoft.Gscad.ApplicationServices;
using System.Windows.Forms;
using Application = Gssoft.Gscad.ApplicationServices.Application;
using System.Linq;

namespace NSVLibUtils
{
    public class AttributeUtils
    {
        public static void AssignAttributeToBlock(
            string blockName,
            Dictionary<string, string> attributes,
            bool forceTag = false,
            bool forceValue = false)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            using (var tr = doc.Database.TransactionManager.StartTransaction())
            {
                var blockTable = tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                AssignAttributeToBlock(blockTable[blockName], attributes, forceTag, forceValue);
                tr.Commit();
            }
        }

        public static void AssignAttributeToBlock(
            ObjectId objectId, 
            Dictionary<string, string> attributes, 
            bool forceTag=false,
            bool forceValue = false)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            using (var tr = doc.Database.TransactionManager.StartTransaction())
            {
                if (objectId == ObjectId.Null)
                    throw new System.ArgumentException("object id is null");
                if(objectId.ObjectClass == RXObject.GetClass(typeof(BlockTableRecord)))
                {
                    AssignAttributeToBlockDef(
                        objectId,
                        attributes,
                        forceTag = false);
                    
                }
                else if(objectId.ObjectClass.DxfName == "INSERT")
                {
                    AssignAttributeToBlockRefference(
                        objectId,
                        attributes,
                        forceTag,
                        forceValue);
                }
                tr.Commit();
            }
        }
        private static void AssignAttributeToBlockRefference(
            ObjectId objectId,
            Dictionary<string, string> attributes,
            bool forceTag,
            bool forceValue)
        {
            var attributesToAssign = new Dictionary<string, string>(attributes);

            var tr = objectId.Database.TransactionManager.TopTransaction;
            var blkRef = (BlockReference)tr.GetObject(objectId, OpenMode.ForWrite);
            var blockTableRecord = tr.GetObject(blkRef.BlockTableRecord, OpenMode.ForWrite) as BlockTableRecord;
            AssignAttributeToBlockDef(blkRef.BlockTableRecord, attributesToAssign, forceTag);
            
            foreach (ObjectId id in blkRef.AttributeCollection)
            {
                if (!id.IsErased)
                {
                    var attRef = (AttributeReference)tr.GetObject(id, OpenMode.ForWrite);
                    if (attributesToAssign.ContainsKey(attRef.Tag))
                    {
                        if (forceValue)
                        {
                            attRef.TextString = attributesToAssign[attRef.Tag];
                        }
                        attributesToAssign.Remove(attRef.Tag);
                    }
                }
            }
            
            foreach (ObjectId btrcontent in blockTableRecord)
            {
                var ent = (Entity)tr.GetObject(btrcontent, OpenMode.ForWrite);
                if (ent is AttributeDefinition attDef)
                {
                    if (attributesToAssign.ContainsKey(attDef.Tag))
                    {
                        var attRef = new AttributeReference
                        {
                            TextString = attributesToAssign[attDef.Tag],
                            Position = attDef.Position
                        };
                        attRef.SetAttributeFromBlock(attDef, blkRef.BlockTransform);
                        blkRef.AttributeCollection.AppendAttribute(attRef);
                        tr.AddNewlyCreatedDBObject(attRef, true);
                    }
                    //else
                    //    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                    //        "\nCould not find attribute definition in a block!");
                }
            }
        }


        private static void AssignAttributeToBlockDef(
            ObjectId blkDef, 
            Dictionary<string, string> attributes, 
            bool forceTag)
        {
            var db = blkDef.Database;
            var tr = db.TransactionManager.TopTransaction;
            var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            var blockTableRecord = tr.GetObject(blkDef, OpenMode.ForWrite) as BlockTableRecord;

            foreach (ObjectId id in blockTableRecord)
            {
                var obj = tr.GetObject(id, OpenMode.ForWrite);
                if (obj is AttributeDefinition attDef)
                {
                    if(forceTag || attributes.ContainsKey(attDef.Tag))
                        attDef.Erase();
                }
            }
            int i = -10;
            foreach (KeyValuePair<string, string> dict in attributes)
            {
                var acAttDef = new AttributeDefinition();
                acAttDef.Position = new Point3d(0, i, 0);
                i -= 2;
                acAttDef.Tag = dict.Key;
                acAttDef.TextString = dict.Value;
                acAttDef.Height = 1;
                acAttDef.Justify = AttachmentPoint.BottomCenter;
                acAttDef.Invisible = true;
                //acAttDef.Visible = true;
                blockTableRecord.AppendEntity(acAttDef);
                tr.AddNewlyCreatedDBObject(acAttDef, true);
            }
        }
        public static void UpdatePreviousAttributes(string blkName,Dictionary<string,string> attributes)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            try
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    var blockTableRecord = tr.GetObject(blockTable[blkName],OpenMode.ForRead) 
                        as BlockTableRecord;
                    var blkRefIds = blockTableRecord.GetBlockReferenceIds(true, false);
                    foreach(ObjectId blkRefid in blkRefIds)
                    {
                        AssignAttributeToBlockRefference(blkRefid, attributes, true, true);
                    }
                    tr.Commit();
                }
            }
            catch
            {
                doc.Editor.WriteMessage("probbly we fucked up");
            }
        }
        public static Dictionary<string,string> GetSimilarBlockRefAttributes(string blockName)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var result = new Dictionary<string, string>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (blockTable.Has(blockName))
                {
                    var blkDef = blockTable[blockName];
                    var btr = tr.GetObject(blkDef, OpenMode.ForRead) as BlockTableRecord;
                    var blkRefIds = btr.GetBlockReferenceIds(true, false);
                    if (blkRefIds.Count == 0)
                        return null;
                    var blkRef = tr.GetObject(blkRefIds[0], OpenMode.ForRead) as BlockReference;
                    foreach (ObjectId attId in blkRef.AttributeCollection)
                    {
                        var attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;

                        result.Add(attRef.Tag, attRef.TextString);
                    }
                    tr.Commit();
                    return result;
                }
                else
                {
                    tr.Commit();
                    throw new System.ArgumentException("Current Database dose not contain this block!");
                }
            }
        }

        public static Dictionary<string,string> GetBlockRefAttribute(ObjectId blkRefId)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var result = new Dictionary<string, string>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blkRef = tr.GetObject(blkRefId, OpenMode.ForRead) as BlockReference;
                foreach (ObjectId attId in blkRef.AttributeCollection)
                {
                    var attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                    result.Add(attRef.Tag, attRef.TextString);
                }
                tr.Commit();
                return result;
            }
        }
    }
}
