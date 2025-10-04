using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using NSVLIBConstants;
using System;
using System.Collections.Generic;
using System.Globalization;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using System.IO;

namespace NSVLibUtils
{
    public static class DatabaseUtils
    {
        public static string AddBlockToSideDb(Enum enumCat, Address sideDBAdress, string namedDictionary, Dictionary<string, string> attribute)
        {
            var activeDoc = Application.DocumentManager.MdiActiveDocument;
            var ed = activeDoc.Editor;
            var activeDb = activeDoc.Database;
            var blockNameToAddinSideDb = SelectBlock(out string nameInActiveDb, activeDoc);
            if (blockNameToAddinSideDb == null)
            {
                return blockNameToAddinSideDb;
            }

            var blockIds = new ObjectIdCollection();
            using (var tr = activeDoc.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var activeBlockTable = tr.GetObject(activeDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                var activeBlkDef = activeBlockTable[nameInActiveDb];
                AttributeUtils.AssignAttributeToBlock(activeBlkDef, attribute, true);
                blockIds.Add(activeBlkDef);
                tr.Commit();
            }
            try
            {
                string address = sideDBAdress.UserDwgAddress;
                using (var sideDb = new Database(false, true))
                {
                    sideDb.ReadDwgFile(address, System.IO.FileShare.ReadWrite, false, null);
                    Category.SetCategory(blockNameToAddinSideDb, enumCat, sideDb, namedDictionary);
                    using (var tr = sideDb.TransactionManager.StartTransaction())
                    {
                        var mapping = new IdMapping();
                        activeDb.WblockCloneObjects(
                            blockIds, sideDb.BlockTableId, mapping, DuplicateRecordCloning.Ignore, false);

                        var sideBlockTable = (BlockTable)tr.GetObject(sideDb.BlockTableId, OpenMode.ForRead);
                        var blkDef = sideBlockTable[nameInActiveDb];
                        var blockTableRecord = tr.GetObject(blkDef, OpenMode.ForWrite) as BlockTableRecord;
                        blockTableRecord.Name = blockNameToAddinSideDb;

                        AttributeUtils.AssignAttributeToBlock(blkDef, attribute, true);
                        tr.Commit();
                    }

                    sideDb.SaveAs(address, true, DwgVersion.Current, sideDb.SecurityParameters);
                    //sideDb.CloseInput(true);
                }
                //this is commented because we removed the bitmap folder!
                //BitmapGenerator.Generate(blockNameToAddinSideDb, enumCat.ToString(), sideDBAdress.UserBmpAddress);
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.ToString());
            }
            return blockNameToAddinSideDb;
        }

        private static string SelectBlock(out string nameInActiveDatabase, Document activeDoc)
        {
            Database db = activeDoc.Database;
            Editor ed = activeDoc.Editor;
            nameInActiveDatabase = string.Empty;
            var peo = new PromptEntityOptions("select a block refrence to add to the database: ");
            peo.SetRejectMessage("this item is not block refrence!");
            peo.AddAllowedClass(typeof(BlockReference), true);
            var newItem = ed.GetEntity(peo);
            var newItemId = newItem.ObjectId;

            if (newItem.Status != PromptStatus.OK)
                return null;

            var pko = new PromptKeywordOptions("do you want to use block's name or set a custom name?");
            pko.Keywords.Add("Block Name");
            pko.Keywords.Add("Custom Name");
            pko.AllowNone = false;
            var prk = ed.GetKeywords(pko);

            if (prk.Status != PromptStatus.OK)
                return null;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var blkRef = (BlockReference)tr.GetObject(newItemId, OpenMode.ForRead);
                nameInActiveDatabase = blkRef.Name;
                tr.Commit();
            }

            string blockNameInSideDb = "";
            if (prk.StringResult.Contains("Block"))
            {
                blockNameInSideDb = nameInActiveDatabase;
            }
            else
            {
                var pso = new PromptStringOptions("set a name for this item: ");
                pso.AllowSpaces = true;
                var check = ed.GetString(pso);
                blockNameInSideDb = check.StringResult;

                if (check.Status != PromptStatus.OK) return null;
            }
            return blockNameInSideDb;
        }

        public static void RemoveBlockFromSideDb(string blockToRemove, Address SideDB, Enum categories, string namedDictionary)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            try
            {
                using (var destDB = new Database(false, true))
                {
                    var address = SideDB.UserDwgAddress;
                    destDB.ReadDwgFile(address, FileOpenMode.OpenForReadAndAllShare, false, string.Empty);
                    Category.RemoveRecord(blockToRemove, categories, destDB, namedDictionary);
                    if (RemoveBlockDefAndRefFromDestination(destDB, blockToRemove))
                    {
                        destDB.SaveAs(address, true, DwgVersion.Current, destDB.SecurityParameters);
                        //destDB.CloseInput(true);
                        //the below line is commented since the folder is removed!
                        //RemovePictureFromFolder(SideDB.UserBmpAddress, blockToRemove);
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

        //Dont remove this
        private static void RemovePictureFromFolder(string folderPath, string blockToRemove)
        {
            string filePath = folderPath + blockToRemove.ToLower() + ".bmp";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static bool RemoveBlockDefAndRefFromDestination(Database destDB, string blockToRemove)
        {
            var doesExistInSideDb = false;
            using (var tr = destDB.TransactionManager.StartTransaction())
            {
                var destBlockTable = tr.GetObject(destDB.BlockTableId, OpenMode.ForRead) as BlockTable;

                if (destBlockTable.Has(blockToRemove))
                {
                    var blockDefId = destBlockTable[blockToRemove];
                    var btr = tr.GetObject(blockDefId, OpenMode.ForWrite) as BlockTableRecord;
                    var blockRefs = btr.GetBlockReferenceIds(false, false);
                    if (blockRefs.Count > 0)
                    {
                        foreach (ObjectId objId in blockRefs)
                        {
                            var blkRef = tr.GetObject(objId, OpenMode.ForWrite) as BlockReference;
                            if (blkRef.ObjectId != ObjectId.Null)
                            {
                                blkRef.Erase();
                            }
                        }
                    }
                    btr.Erase();
                    doesExistInSideDb = true;
                }
                tr.Commit();
            }
            return doesExistInSideDb;
        }

        public static List<string> GetBlockNamesFromSideDb(Enum categoryName, Address address, string namedDictionary)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            var blockNames = new List<string>();
            if (namedDictionary == "NSV_Valves")
            {
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.NsvDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames = Category.GetBlockNames(categoryName, sideDb, namedDictionary);
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.UserDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames.AddRange(Category.GetBlockNames(categoryName, sideDb, namedDictionary));
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
            }
            else
            {
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.NsvDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames = Category.GetBlockNames(categoryName, sideDb, namedDictionary);
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.UserDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames.AddRange(Category.GetBlockNames(categoryName, sideDb, namedDictionary));
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
            }
            //if(blockNames.Count == 0)
            //    MessageBox.Show("Please report this bug");

            blockNames.Sort();
            if (namedDictionary == "NSV_Valves")
            {
                for (int i = 0; i < blockNames.Count; i++)
                {
                    blockNames[i] = ConvertToTitleCase(blockNames[i]);
                }
            }

            return blockNames;
        }

        public static List<string> GetBlockNamesFromSideDb(Enum categoryName, Address address, string namedDictionary, NSVLIBConstants.Enums.SprinklerOrientation spType)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            var blockNames = new List<string>();
            if (namedDictionary == "NSV_Valves")
            {
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.NsvDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames = Category.GetBlockNames(categoryName, sideDb, namedDictionary);
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.UserDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames.AddRange(Category.GetBlockNames(categoryName, sideDb, namedDictionary));
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
            }
            else
            {
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.NsvDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames = Category.GetBlockNames(categoryName, sideDb, namedDictionary);
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
                try
                {
                    using (var sideDb = new Database(false, true))
                    {
                        sideDb.ReadDwgFile(address.UserDwgAddress, System.IO.FileShare.Read, true, "");
                        blockNames.AddRange(Category.GetBlockNames(categoryName, sideDb, namedDictionary));
                    }
                }
                catch
                {
                    ed.WriteMessage("\nError during reading block categories from side database");
                }
            }
            //if(blockNames.Count == 0)
            //    MessageBox.Show("Please report this bug");

            blockNames.Sort();
            if (namedDictionary == "NSV_Valves")
            {
                for (int i = 0; i < blockNames.Count; i++)
                {
                    blockNames[i] = ConvertToTitleCase(blockNames[i]);
                }
            }

            var filteredBlockNames = new List<string>();

            foreach (var blockName in blockNames)
            {
                if (spType == NSVLIBConstants.Enums.SprinklerOrientation.Sidewall)
                {
                    if (blockName.ToLower().Contains("sidewall"))
                    {
                        filteredBlockNames.Add(blockName);
                    }
                }
                else //pendent
                {
                    if (blockName.ToLower().Contains("pendent") || blockName.ToLower().Contains("upright"))
                    {
                        filteredBlockNames.Add(blockName);
                    }
                }
            }

            return filteredBlockNames;
        }

        static string ConvertToTitleCase(string input)
        {
            string[] lowercaseWords = { "to", "with", "and", "a", "for", "or" };

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titleCase = textInfo.ToTitleCase(input);

            string[] words = titleCase.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (Array.Exists(lowercaseWords, word => word.Equals(words[i], StringComparison.OrdinalIgnoreCase)))
                {
                    words[i] = words[i].ToLower();
                }
            }

            return string.Join(" ", words);
        }

        public static ObjectId ImporBlockDefFromSidedb(string blkName, string sidedbAddress)
        {
            var activeDb = ACAD.DocumentManager.MdiActiveDocument.Database;
            try
            {
                var blkdef = new ObjectIdCollection();
                using (var sideDb = new Database(false, true))
                {
                    sideDb.ReadDwgFile(sidedbAddress, System.IO.FileShare.Read, true, "");

                    using (var tran = sideDb.TransactionManager.StartTransaction())
                    {
                        var bt = (BlockTable)tran.GetObject(sideDb.BlockTableId, OpenMode.ForRead);
                        if (bt.Has(blkName))
                            blkdef.Add(bt[blkName]);

                        tran.Commit();
                    }
                    if (blkdef.Count > 0)
                    {
                        var mapping = new IdMapping();
                        sideDb.WblockCloneObjects(blkdef, activeDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
                    }
                    else
                    {
                        throw new ArgumentException("Block does not exist in side database");
                    }
                }

                using (var tr = activeDb.TransactionManager.StartTransaction())
                {
                    var bt = tr.GetObject(activeDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    if (bt.Has(blkName))
                    {
                        tr.Commit();
                        return bt[blkName];
                    }
                    else
                    {
                        tr.Commit();
                        throw new Exception("Operation faied while cloning from side data base");
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        


    }
}
