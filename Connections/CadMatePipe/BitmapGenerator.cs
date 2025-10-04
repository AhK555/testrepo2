using System.Collections.Generic;
using System.IO;
using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Runtime;
using Gssoft.Gscad.Windows;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using NSVLIBConstants;
using System;
using System.Drawing;

namespace NSVLibUtils
{
    public static class BitmapGenerator
    {
        public static void Generate(string blkName,string addFile, string address)
        {
            //try
            //{
            //    if (!Directory.Exists(address))
            //        throw new ArgumentException("Address does not exist");

            //    var destDb = new Database();
            //    destDb.ReadDwgFile(address, FileOpenMode.OpenForReadAndAllShare, false, null);
            //    GenerateCurrentBlockPreviews(blkName,destDb, address);
            //}
            //catch (System.Exception ex)
            //{
            //    throw new System.Exception("Could not generate bitmap"+ex.Message);
            //}
        }
        private static void GenerateCurrentBlockPreviews(string blkName,Database db, string address)
        {
            //try
            //{
            //    using (var tr = db.TransactionManager.StartTransaction())
            //    {
            //        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

            //        if (bt.Has(blkName))
            //        {
            //            var blkId = bt[blkName];
            //            var btr = (BlockTableRecord)tr.GetObject(blkId, OpenMode.ForRead);
            //            var imgsrc = CMLContentSearchPreviews.GetBlockTRThumbnail(btr);
            //            var bmp = ImageSourceToGDI(imgsrc as System.Windows.Media.Imaging.BitmapSource);
            //            var fname = address + btr.Name.ToLower() + ".bmp";

            //            if (File.Exists(fname))
            //                File.Delete(fname);
            //            bmp.Save(fname);
            //        }

            //        tr.Commit();
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    throw new System.Exception("Failed during generating block preview"+ex.Message);
            //}
        }
        private static System.Drawing.Image ImageSourceToGDI(System.Windows.Media.Imaging.BitmapSource src)
        {
            var ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(src));

            encoder.Save(ms);

            ms.Flush();

            return System.Drawing.Image.FromStream(ms);
        }
    }

}