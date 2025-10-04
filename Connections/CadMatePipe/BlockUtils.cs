using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using NSVLIBConstants;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace NSVLibUtils
{
    public class BlockUtils
    {
        public static void DeleteNestedBlock(string blockToDelete)
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            doc.LockDocument();
            var filter = new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") });
            var per = doc.Editor.SelectAll(filter);
            if (per.Status != PromptStatus.OK)
                return;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in per.Value.GetObjectIds())
                {
                    var blkRef = tr.GetObject(id, OpenMode.ForWrite) as BlockReference;
                    if (blkRef.TrueName() == blockToDelete)
                        blkRef.Erase();
                    else
                        EraseNestedBlock(blkRef, blockToDelete, tr);
                }
                tr.Commit();
            }
        }
        private static void EraseNestedBlock(BlockReference block, string blkName, Transaction tr)
        {
            try
            {
                var brDef = (BlockTableRecord)tr.GetObject(block.BlockTableRecord, OpenMode.ForRead);
                foreach (ObjectId objectId in brDef)
                {
                    var ent = (Entity)tr.GetObject(objectId, OpenMode.ForWrite);
                    switch (ent)
                    {
                        case BlockReference blockReference:
                            switch (blockReference.TrueName() == blkName)
                            {
                                case true:
                                    blockReference.Erase();
                                    break;
                                case false:
                                    EraseNestedBlock(blockReference, blkName,tr);
                                    break;
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public static List<ObjectId> SelectBlockAndGetParents()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var pne = new PromptNestedEntityOptions("\nSelect block object");
            var pner = ed.GetNestedEntity(pne);
            var objIds = new List<ObjectId>();
            if(pner.Status == PromptStatus.OK) 
            {
                objIds = pner.GetContainers().ToList();
                objIds.Reverse();
            }
            return objIds;
        }

        public static (List<ObjectId> containersObjectIds, Matrix3d NestedBlockTransform) SelectBlockAndGetParentsWithTransform()
        {
            var doc = ACAD.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var pne = new PromptNestedEntityOptions("\nSelect block object");
            var pner = ed.GetNestedEntity(pne);
            return SelectBlockAndGetParentsWithTransform(pner);
        }

        public static (List<ObjectId> containersObjectIds, Matrix3d NestedBlockTransform)SelectBlockAndGetParentsWithTransform(PromptNestedEntityResult pner)
        {
            var containerObjIds = new List<ObjectId>();
            Matrix3d blockTransform;
            if (pner.Status == PromptStatus.OK)
            {
                blockTransform = pner.Transform;
                containerObjIds = pner.GetContainers().ToList();
                containerObjIds.Reverse();
            }
            else
                blockTransform = Matrix3d.Identity;

            return (containerObjIds, blockTransform);
        }


        public static System.Drawing.Image GetBlockDefImage(string blockName, Address address = null)
        {
            return null;
            var source = GetImageSource(blockName, address);
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)source));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Position = 0;
                var btmp = new Bitmap(ms);
                return btmp;
            }
        }
        private static ImageSource GetImageSource(string blockName, Address address = null)
        {
            return null;
            //var NSVDb = new Database(false, true);
            //try
            //{
            //    using (var UserDb = new Database(false, true))
            //    {
            //        if (address == null)
            //        {
            //            NSVDb = ACAD.DocumentManager.MdiActiveDocument.Database;
            //        }
            //        else
            //        {
            //            NSVDb.ReadDwgFile(address.NsvDwgAddress, FileOpenMode.OpenForReadAndAllShare, true, "");
            //            UserDb.ReadDwgFile(address.UserDwgAddress, FileOpenMode.OpenForReadAndAllShare, true, "");
            //        }
            //        using (var tr = NSVDb.TransactionManager.StartTransaction())
            //        {
            //            try
            //            {
            //                var bt = (BlockTable)tr.GetObject(NSVDb.BlockTableId, OpenMode.ForRead);
            //                if (bt.Has(blockName))
            //                {
            //                    var blkDef = (BlockTableRecord)tr.GetObject(bt[blockName], OpenMode.ForRead);
            //                    return CMLContentSearchPreviews.GetBlockTRThumbnail(blkDef);
            //                }
            //            }
            //            finally
            //            {
            //                tr.Commit();
            //            }
            //        }

            //        if (address != null)
            //        {
            //            using (var tr = UserDb.TransactionManager.StartTransaction())
            //            {
            //                try
            //                {
            //                    var bt = (BlockTable)tr.GetObject(UserDb.BlockTableId, OpenMode.ForRead);
            //                    if (bt.Has(blockName))
            //                    {
            //                        var blkDef = (BlockTableRecord)tr.GetObject(bt[blockName], OpenMode.ForRead);
            //                        return CMLContentSearchPreviews.GetBlockTRThumbnail(blkDef);
            //                    }
            //                }
            //                finally
            //                {
            //                    tr.Commit();
            //                }
            //            }
            //        }
            //        return null;
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    throw new System.Exception("Error while attempting to get blk image : " + ex.Message);
            //}
            //finally
            //{
            //    NSVDb.Dispose();
            //}
        }

    }
}
