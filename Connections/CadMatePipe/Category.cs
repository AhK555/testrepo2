using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NSVLibUtils
{
    public static class Category
    {
        public static List<string> GetBlockNames(Enum catName, Database db, string namedDictionary)
        {
            var result = new List<string>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var nod = db.GetNOD(tr, OpenMode.ForWrite);

                var nsvDict = nod.GetOrCreateNamedDictionary(namedDictionary, tr);
                try
                {
                    if (nsvDict.TryGetXrecordData(catName.ToString(), tr, out ResultBuffer data))
                    {
                        foreach (var tpv in data)
                        {
                            result.Add(tpv.Value.ToString());
                        }
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Error during reading category names");
                }
                tr.Commit();
            }
            return result;
        }

        public static void SetCategory(string blockName, Enum catName, Database db, string namedDictionary)
        {
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var nod = db.GetNOD(tr, OpenMode.ForWrite);
                var nsvDict = nod.GetOrCreateNamedDictionary(namedDictionary, tr);
                try
                {
                    if (nsvDict.TryGetXrecordData(catName.ToString(), tr, out ResultBuffer data))
                    {
                        foreach (var tpv in data)
                        {
                            if ((string)tpv.Value == blockName)
                            {
                                return;
                            }
                        }
                        data.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, blockName));
                        nsvDict.SetXrecordData(catName.ToString(), tr, data);
                    }
                    else
                    {
                        data = new ResultBuffer
                        {
                            new TypedValue((int)DxfCode.ExtendedDataAsciiString,blockName)
                        };
                        nsvDict.SetXrecordData(catName.ToString(), tr, data);
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Error during assigning category to newly added blocks");
                }
                tr.Commit();
            }
        }

        public static void RemoveRecord(string blockName, Enum category, Database db, string namedDictionary)
        {
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var nod = db.GetNOD(trans, OpenMode.ForWrite);
                var nsvDict = nod.GetOrCreateNamedDictionary(namedDictionary, trans);

                foreach (string categoryName in Enum.GetNames(category.GetType()))
                {
                    if (!nsvDict.TryGetXrecordData(categoryName, trans, out ResultBuffer data))
                    {
                        continue;
                    }
                    else
                    {
                        var newData = new List<TypedValue>(data.AsArray());
                        if (newData.Contains(new TypedValue((int)DxfCode.ExtendedDataAsciiString, blockName.ToLower())))
                        {
                            foreach (var tpv in newData)
                            {
                                if (tpv.Value as string == blockName.ToLower())
                                {
                                    newData.Remove(tpv);
                                    break;
                                }
                            }
                            data = new ResultBuffer(newData.ToArray());
                            nsvDict.SetXrecordData(categoryName, trans, data);
                        }
                    }
                }
                trans.Commit();
            }
        }

    }
}
