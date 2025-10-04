using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using NSVLIBConstants;
using System.IO;
using Gssoft.Gscad.Runtime;

namespace NSVLibUtils
{
    public class BlockDefs
    {
        public static ObjectId GetPipeSize(Database db)
        {
            var blkDef = ObjectId.Null;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForWrite);
                if (blockTable.Has("PipeProperties"))
                    blkDef = blockTable["PipeProperties"];
                else
                    blkDef = CreatePipeSizeDefinition("PipeProperties", blockTable, db);
                tr.Commit();
            }
            return blkDef;
        }
        private static ObjectId CreatePipeSizeDefinition(string blkName, BlockTable blockTable, Database db)
        {
            var blkDefId = ObjectId.Null;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blkTableRecord = new BlockTableRecord();
                blkTableRecord.Name = blkName;
                blkDefId = blockTable.Add(blkTableRecord);
                tr.AddNewlyCreatedDBObject(blkTableRecord, true);
                tr.Commit();
            }
            AssignAttributeDefinitionToBlkDef(db);
            return blkDefId;
        }
        private static void AssignAttributeDefinitionToBlkDef(Database db)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var objectId = bt["PipeProperties"];

                var blkDef = (BlockTableRecord)tr.GetObject(objectId, OpenMode.ForWrite);
                Line line = new Line(new Point3d(0, 0, 0), new Point3d(215, 0, 0));
                blkDef.Origin = Point3d.Origin;
                var attDef1 = new AttributeDefinition
                {
                    Position = new Point3d(0, 1, 0),
                    Prompt = "Enter Pipe Diameter: ",
                    Tag = "Diameter",
                    TextString = "Diameter",
                    Height = 1,
                    Justify = AttachmentPoint.MiddleCenter,
                };
                blkDef.AppendEntity(attDef1);
                tr.AddNewlyCreatedDBObject(attDef1, true);

                var attDef2 = new AttributeDefinition
                {
                    Position = new Point3d(0, -1, 0),
                    Prompt = "Pipe Length: ",
                    Tag = "Lenght",
                    TextString = "Lenght",
                    Height = 1,
                    Justify = AttachmentPoint.MiddleCenter,
                };
                blkDef.AppendEntity(attDef2);
                tr.AddNewlyCreatedDBObject(attDef2, true);
                tr.Commit();
            }
        }
        public static ObjectId GetBreakPipe()
        {
            Database db = ACAD.DocumentManager.MdiActiveDocument.Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blkTable = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                var btr = ObjectId.Null;

                if (blkTable.Has("break Pipe"))
                {
                    var blkRecord = blkTable["Break Pipe"];
                    tr.Commit();
                    return blkRecord;
                }
                else
                {
                    var pts = new List<Point2d>()
                    {
                        new Point2d(0,6),
                        new Point2d(-3,4.7574),
                        new Point2d(-3,3),
                        new Point2d(0,0),
                        new Point2d(3,-3),
                        new Point2d(3,-4.7574),
                        new Point2d(0,-6),
                    };
                    var pl = new Polyline();
                    double cordLenght = pts[0].GetDistanceTo(pts[1]);
                    var buldge = BuldgeUtils.GetBuldge(4.2426, cordLenght);
                    pl.AddVertexAt(0, pts[0], buldge, 0, 0);

                    cordLenght = pts[1].GetDistanceTo(pts[2]);
                    buldge = BuldgeUtils.GetBuldge(1.2426, cordLenght);
                    pl.AddVertexAt(1, pts[1], buldge, 0, 0);

                    pl.AddVertexAt(2, pts[2], 0, 0, 0);
                    pl.AddVertexAt(3, pts[3], 0, 0, 0);

                    cordLenght = pts[4].GetDistanceTo(pts[5]);
                    buldge = BuldgeUtils.GetBuldge(1.2426, cordLenght);
                    pl.AddVertexAt(4, pts[4], -buldge, 0, 0);

                    cordLenght = pts[5].GetDistanceTo(pts[6]);
                    buldge = BuldgeUtils.GetBuldge(4.2426, cordLenght);
                    pl.AddVertexAt(5, pts[5], -buldge, 0, 0);
                    pl.AddVertexAt(6, pts[6], 0, 0, 0);
                    pl.ConstantWidth = 1;

                    var brkPipeBlk = new BlockTableRecord();
                    brkPipeBlk.Name = "Break Pipe";
                    brkPipeBlk.AppendEntity(pl);
                    btr = blkTable.Add(brkPipeBlk);
                    tr.AddNewlyCreatedDBObject(brkPipeBlk, true);
                    tr.Commit();
                    return btr;
                }

            }
        }
        public static ObjectId GetPendent()
        {
            var db = Gssoft.Gscad.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            var tempSprinklerID = ObjectId.Null;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                try
                {
                    if (blockTable.Has("NSVPENDENTTEMPSPRINKLER"))
                    {
                        tempSprinklerID = blockTable["NSVPENDENTTEMPSPRINKLER"];
                    }
                    else
                    {
                        tempSprinklerID = CreatePendentDefinition(tr, db);
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tr.Commit();
                }
                return tempSprinklerID;
            }
        }
        private static ObjectId CreatePendentDefinition(Transaction tr, Database db)
        {
            var tempSprinklerDefID = ObjectId.Null;
            var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
            var blkDef = new BlockTableRecord();
            var circle = new Circle();

            blkDef.Name = "NSVPENDENTTEMPSPRINKLER";
            blkDef.Origin = new Point3d(0, 0, 0);
            circle.Center = Point3d.Origin;

            circle.Radius = 150;

            circle.Layer = "0";
            circle.ColorIndex = 1;
            blkDef.AppendEntity(circle);

            tempSprinklerDefID = blockTable.Add(blkDef);
            tr.AddNewlyCreatedDBObject(blkDef, true);
            return tempSprinklerDefID;
        }
        public static ObjectId GetSidewall()
        {
            var tempSprinklerID = ObjectId.Null;
            Database db = ACAD.DocumentManager.MdiActiveDocument.Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                try
                {
                    if (blockTable.Has("NSVSIDETEMPSPRINKLER"))
                        tempSprinklerID = blockTable["NSVSIDETEMPSPRINKLER"];
                    else
                        tempSprinklerID = CreateSidewallDefinition(tr, db);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally { tr.Commit(); }
                return tempSprinklerID;
            }
        }
        private static ObjectId CreateSidewallDefinition(Transaction tr, Database db)
        {
            var tempSprinklerDefID = ObjectId.Null;
            var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
            var blkDef = new BlockTableRecord();
            var lines = new List<Line>();

            blkDef.Name = "NSVSIDETEMPSPRINKLER";
            lines.Add(new Line(new Point3d(0, 0, 0), new Point3d(-150, 300, 0)));
            lines.Add(new Line(new Point3d(0, 0, 0), new Point3d(+150, 300, 0)));
            lines.Add(new Line(new Point3d(-150, 300, 0), new Point3d(+150, 300, 0)));

            foreach (var line in lines)
            {
                line.Layer = "0";
                line.ColorIndex = 1;
                blkDef.AppendEntity(line);
            }

            tempSprinklerDefID = blockTable.Add(blkDef);
            tr.AddNewlyCreatedDBObject(blkDef, true);
            return tempSprinklerDefID;
        }
        public static List<string> GetNSVSprinklerBlocks()
        {
            var output = new List<string>();
            using (var sideDb = new Database(false, true))
            {
                sideDb.ReadDwgFile(new HeadsAddress().NsvDwgAddress, System.IO.FileShare.Read, true, "");
                using (var tr = sideDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (ObjectId btrId in bt)
                    {
                        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!(btr.IsLayout || btr.IsDependent || btr.IsFromExternalReference || btr.IsFromOverlayReference))
                        {
                            if(!output.Contains(btr.Name))
                                output.Add(btr.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            return output;
        }

        public static List<string> GetNSVSprinklerBlocks(Enum category)
        {
            var output = new List<string>();
            using (var sideDb = new Database(false, true))
            {
                sideDb.ReadDwgFile(new HeadsAddress().NsvDwgAddress, System.IO.FileShare.Read, true, "");
                using (var tr = sideDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (ObjectId btrId in bt)
                    {
                        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!(btr.IsLayout || btr.IsDependent || btr.IsFromExternalReference || btr.IsFromOverlayReference))
                        {
                            if (!output.Contains(btr.Name))
                                output.Add(btr.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            return output;
        }

        public static System.Drawing.Image GetSprinklerThumbnail(string blockName)
        {
            string text = blockName.ToUpper();
            try
            {
                if (File.Exists(new HeadsAddress().UserBmpAddress + text + ".bmp"))
                    return System.Drawing.Image.FromFile(new HeadsAddress().UserBmpAddress + text + ".bmp");
                else if (File.Exists(new HeadsAddress().NSVBmpAddress + text + ".bmp"))
                    return System.Drawing.Image.FromFile(new HeadsAddress().NSVBmpAddress + text + ".bmp");
                else
                    throw new ArgumentException("Block thumbnail does not exist");
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error while getting block thumbnail" + ex.Message);
            }
        }

        public static ObjectId GetNSVBlock(string blkName)
        {
            var dwg = ACAD.DocumentManager.MdiActiveDocument;
            var ed = dwg.Editor;

            using (var tr = dwg.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    var blkTable = (BlockTable)tr.GetObject(dwg.Database.BlockTableId, OpenMode.ForRead);
                    var blockDef = ObjectId.Null;

                    if (blkTable.Has(blkName))
                        return blkTable[blkName];

                    var addreses = new List<string>
                    {
                        new HeadsAddress().NsvDwgAddress,
                        new HeadsAddress().UserDwgAddress,
                        new ValvesAddress().NsvDwgAddress,
                        new ValvesAddress().UserDwgAddress,
                        new NodesAddress().NsvDwgAddress,
                    };

                    foreach (var address in addreses)
                    {
                        try
                        {
                            return DatabaseUtils.ImporBlockDefFromSidedb(blkName, address);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    throw new ArgumentException("Block does not exist side databases");
                }
                finally
                {
                    tr.Commit();
                }
            }
        }

        public static bool TryGetBlock(string blkName, out ObjectId blkObjectID) 
        {
            var dwg = ACAD.DocumentManager.MdiActiveDocument;
            var ed = dwg.Editor;

            using (var tr = dwg.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    var blkTable = (BlockTable)tr.GetObject(dwg.Database.BlockTableId, OpenMode.ForRead);
                    var blockDef = ObjectId.Null;

                    if (blkTable.Has(blkName))
                    {
                        blkObjectID = blkTable[blkName];
                        return true;
                    }

                    var addreses = new List<string>
                    {
                        new HeadsAddress().NsvDwgAddress,
                        new HeadsAddress().UserDwgAddress,
                        new ValvesAddress().NsvDwgAddress,
                        new ValvesAddress().UserDwgAddress,
                        new NodesAddress().NsvDwgAddress,
                    };

                    foreach (var address in addreses)
                    {
                        try
                        {
                            blkObjectID = DatabaseUtils.ImporBlockDefFromSidedb(blkName, address);
                            return true;
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    blkObjectID = ObjectId.Null;
                    return false;
                }
                finally
                {
                    tr.Commit();
                }
            }
        }

        public static List<string> GetUserSprinklerBlocks()
        {
            var output = new List<string>();
            using (var sideDb = new Database(false, true))
            {
                sideDb.ReadDwgFile(new HeadsAddress().UserDwgAddress, System.IO.FileShare.Read, true, "");
                using (var tr = sideDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (ObjectId btrId in bt)
                    {
                        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!(btr.IsLayout || btr.IsDependent || btr.IsFromExternalReference || btr.IsFromOverlayReference))
                        {
                            if (!output.Contains(btr.Name))
                                output.Add(btr.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            return output;
        }
        public static List<string> GetNSVValveBlocks()
        {
            var output = new List<string>();
            using (var sideDb = new Database(false, true))
            {
                sideDb.ReadDwgFile(new ValvesAddress().NsvDwgAddress, System.IO.FileShare.Read, true, "");
                using (var tr = sideDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (ObjectId btrId in bt)
                    {
                        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!(btr.IsLayout || btr.IsDependent || btr.IsFromExternalReference || btr.IsFromOverlayReference))
                        {
                            if (!output.Contains(btr.Name))
                                output.Add(btr.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            return output;
        }
        public static List<string> GetUserValveBlocks()
        {
            var output = new List<string>();
            using (var sideDb = new Database(false, true))
            {
                sideDb.ReadDwgFile(new ValvesAddress().UserDwgAddress, System.IO.FileShare.Read, true, "");
                using (var tr = sideDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (ObjectId btrId in bt)
                    {
                        var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                        if (!(btr.IsLayout || btr.IsDependent || btr.IsFromExternalReference || btr.IsFromOverlayReference))
                        {
                            if (!output.Contains(btr.Name))
                                output.Add(btr.Name);
                        }
                    }
                    tr.Commit();
                }
            }
            return output;
        }
     
    }
}
