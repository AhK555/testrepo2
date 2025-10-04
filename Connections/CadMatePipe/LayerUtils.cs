using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.Colors;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace NSVLibUtils
{
    public class LayerUtils
    {

        public HashSet<string> LockedLayers = new HashSet<string>();
        public HashSet<string> FrozenLayers = new HashSet<string>();
        public HashSet<string> OffLayers = new HashSet<string>();
        private Document _doc = ACAD.DocumentManager.MdiActiveDocument;
        private Database _db = ACAD.DocumentManager.MdiActiveDocument.Database;
        private Editor _ed = ACAD.DocumentManager.MdiActiveDocument.Editor;
        public HashSet<string> TotalActiveLayers = new HashSet<string>();
        public LayerUtils()
        {
            AssignClassFields();
        }

        public void OffAllLayersExcetpWall()
        {
            using (Transaction tr = _db.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                if(_doc.Database.Clayer != layerTable[NSVLIBConstants.Enums.NSVLayers.NSVWall.ToString()])
                {
                    var nsvwall = tr.GetObject(layerTable[NSVLIBConstants.Enums.NSVLayers.NSVWall.ToString()], OpenMode.ForWrite) as LayerTableRecord;
                    nsvwall.IsLocked = false;
                    nsvwall.IsOff = false;
                    nsvwall.IsFrozen = false;
                    _doc.Database.Clayer = layerTable[NSVLIBConstants.Enums.NSVLayers.NSVWall.ToString()];
                }
                
                foreach (var layer in TotalActiveLayers)
                {
                    LayerTableRecord ltr = tr.GetObject(layerTable[layer], OpenMode.ForWrite) as LayerTableRecord;
                    if(layer == NSVLIBConstants.Enums.NSVLayers.NSVWall.ToString())
                        continue;
                    ltr.IsOff = true;
                }
                tr.Commit();
            }
        }
        public void ResetToDefault()
        {
            using (Transaction tr = _db.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (var layer in TotalActiveLayers)
                {
                    try
                    {
                        LayerTableRecord ltr = tr.GetObject(layerTable[layer], OpenMode.ForWrite) as LayerTableRecord;
                        ltr.IsLocked = false;
                        ltr.IsFrozen = false;
                        ltr.IsOff = false;
                    }
                    catch (Exception)
                    {
                        _doc.Editor.WriteMessage("Catch in setting active layers: " + layer);
                        continue;
                    }
                }
                foreach (var layer in FrozenLayers)
                {
                    try 
                    { 
                        LayerTableRecord ltr = tr.GetObject(layerTable[layer], OpenMode.ForWrite) as LayerTableRecord;
                        ltr.IsFrozen = true;
                    }
                    catch (Exception)
                    {
                        _doc.Editor.WriteMessage("Catch in setting frozen layers: " + layer);
                        continue;
                    }
                }
                foreach (var layer in OffLayers)
                {
                    try
                    {
                        LayerTableRecord ltr = tr.GetObject(layerTable[layer], OpenMode.ForWrite) as LayerTableRecord;
                        ltr.IsOff = true;
                    }
                    catch (Exception)
                    {
                        _doc.Editor.WriteMessage("Catch in setting off layers: " + layer);
                        continue;
                    }
                }
                foreach (var layer in LockedLayers)
                {
                    try
                    {
                        LayerTableRecord ltr = tr.GetObject(layerTable[layer], OpenMode.ForWrite) as LayerTableRecord;
                        ltr.IsLocked = true;
                    }
                    catch (Exception)
                    {
                        _doc.Editor.WriteMessage("Catch in setting locked layers: " + layer);
                        continue;
                    }
                }
                _doc.Editor.Regen();
                tr.Commit();
            }
        }

        private void AssignClassFields()
        {
            using (Transaction tr = _db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (ObjectId id in layerTable)
                {
                    LayerTableRecord layer = tr.GetObject(id, OpenMode.ForRead) as LayerTableRecord;
                    TotalActiveLayers.Add(layer.Name);
                    if (layer.IsLocked)
                        LockedLayers.Add(layer.Name);
                    if (layer.IsFrozen)
                    {
                        FrozenLayers.Add(layer.Name);
                        TotalActiveLayers.Remove(layer.Name);
                    }
                    if (layer.IsOff)
                    {
                        TotalActiveLayers.Remove(layer.Name) ;
                        OffLayers.Add(layer.Name);
                    }
                }
                tr.Commit();
            }
        }
        public void FreezeAndOffNSVLayers<T>()where T: Enum
        {
            using (Transaction tr = _db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (T value in Enum.GetValues(typeof(T)))
                {
                    try
                    {
                        var layerId = layerTable[value.ToString()];
                        if (layerId != ObjectId.Null)
                        {
                            var layerTableRec = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                            layerTableRec.IsFrozen = true;
                            FrozenLayers.Add(value.ToString());
                            layerTableRec.IsOff = true;
                            OffLayers.Add(value.ToString());
                        }
                    }
                    catch (System.Exception)
                    {
                        _ed.WriteMessage($"\nLayer {value} does not exist in current database");
                    }
                }
                tr.Commit();
            }
        }
        public void UnFreezeAndTurnOnNSVLayers<T>() where T : Enum
        {
            using (Transaction tr = _db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (T value in Enum.GetValues(typeof(T)))
                {
                    try
                    {
                        var layerId = layerTable[value.ToString()];
                        if (layerId != ObjectId.Null)
                        {
                            var layerTableRec = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                            layerTableRec.IsFrozen = false;
                            layerTableRec.IsOff = false;
                        }
                    }
                    catch (Exception)
                    {
                        _ed.WriteMessage($"\nLayer {value} does not exist in current database!");
                        continue;
                    }
                }
                tr.Commit();
            }
        }

        public void UnlockLayers()
        {
            using (var tr = _db.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (var layerName in LockedLayers)
                {
                    try
                    {
                        var layerID = layerTable[layerName];
                        var layer = tr.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                        if (!layer.IsLocked)
                        {
                            _ed.WriteMessage($"\nLayer : {layerName} is no longer locked!");
                            continue;
                        }
                        layer.IsLocked = false;
                    }
                    catch (Exception)
                    {
                        _ed.WriteMessage($"\nLayer : {layerName} is removed from database!");
                        continue;
                    }
                }
                tr.Commit();
            }
        }

        public void LockLayers()
        {
            using (var tr = _db.TransactionManager.StartTransaction())
            {
                var layerTable = (LayerTable)tr.GetObject(_db.LayerTableId, OpenMode.ForRead);
                foreach (var layerName in LockedLayers)
                {
                    try
                    {
                        var layerID = layerTable[layerName];
                        if (layerID == ObjectId.Null)
                        {
                            continue;
                        }
                        var layer = tr.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                        if (layer.IsLocked)
                        {
                            _ed.WriteMessage($"\nLayer : {layerName} is already locked!");
                            continue;
                        }
                        layer.IsLocked = true;
                    }
                    catch (Exception)
                    {
                        _ed.WriteMessage($"\nLayer : {layerName} is removed from database!");
                        continue ;
                    }
                }
                tr.Commit();
            }
        }

        public static void ChangeLineWeight(string layerName, LineWeight lineWeight)
        {
            var db = ACAD.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                var layerRecord = trans.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                layerRecord.LineWeight = lineWeight;
                trans.Commit();
            }

        }

        public static void ChangeColour(string layerName, short colorIndex)
        {
            var db = ACAD.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                var layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                var layerRecord = trans.GetObject(layerTable[layerName], OpenMode.ForWrite) as LayerTableRecord;
                layerRecord.Color = Color.FromColorIndex(ColorMethod.ByLayer, colorIndex);
                trans.Commit();
            }
        }
    }
}
