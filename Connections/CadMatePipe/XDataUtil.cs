using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;

namespace NSVLibUtils
{
    public static class XDataUtil
    {
        public static TypedValue[] ReadXData(ObjectId id,string appName)
        {
            if (id == ObjectId.Null)
                throw new NullReferenceException("object id is null. cant read XDATA");

            using (var tr = ACAD.DocumentManager.MdiActiveDocument.Database.TransactionManager.
                StartOpenCloseTransaction())
            {
                var entity = tr.GetObject(id, OpenMode.ForRead) as Entity;
                var resultbuffer = entity.GetXDataForApplication(appName);
                if (resultbuffer == null)
                    throw new NullReferenceException("App name is not registered on entity");
                else 
                    return resultbuffer.AsArray();
            }
        }

        public static string ReadXData(Transaction tr,ObjectId id, string appName, string value )
        {
            if (id == ObjectId.Null)
                throw new NullReferenceException("object id is null. cant read XDATA");

            var entity = tr.GetObject(id, OpenMode.ForRead) as Entity;
            var resultbuffer = entity.GetXDataForApplication(appName);
            if (resultbuffer == null)
                throw new NullReferenceException("App name is not registered on entity");
            else
            {
                TypedValue[] data = resultbuffer.AsArray();
                for (int i = 0; i < data.Length; i++)
                {
                    TypedValue tv = data[i];
                    if (tv.TypeCode == 1000 && tv.Value.ToString().Contains(value))
                    {
                        var val = tv.Value.ToString();
                        return val.Substring(val.IndexOf(":")+1);
                    }
                }
            }
            throw new NullReferenceException("value does not exist in XData");
        }

        public static void SetXdata(
            Transaction tr,
            Entity entity,
            string appName, 
            Dictionary<string, string> xData)
        {
            var resultbuffer = entity.GetXDataForApplication(appName);

            if (resultbuffer == null)
            {
                resultbuffer = AddXData(tr,appName,xData);
            }
            else
            {
                try
                {
                    resultbuffer = EditXData(resultbuffer, xData);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            entity.XData = resultbuffer;
        }

        private static ResultBuffer AddXData(
            Transaction tr,
            string appName, 
            Dictionary<string,string> xData)
        {
            var registeredAppTable = (RegAppTable)tr.GetObject(ACAD.DocumentManager.MdiActiveDocument
                .Database.RegAppTableId, OpenMode.ForRead);
            if (!registeredAppTable.Has(appName))
            {
                using (RegAppTableRecord regAppRecord = new RegAppTableRecord())
                {
                    regAppRecord.Name = appName;
                    registeredAppTable.UpgradeOpen();
                    registeredAppTable.Add(regAppRecord);
                    registeredAppTable.DowngradeOpen();
                    tr.AddNewlyCreatedDBObject(regAppRecord, true);
                }
            }

            var data = new TypedValue[xData.Count + 1 ];

            int i =0;
            data[i] = new TypedValue(1001, appName);
            foreach(var xDataKVP in xData)
            {
                i++;
                data[i] = new TypedValue(1000, $"{xDataKVP.Key}:{xDataKVP.Value}");
            }

            return new ResultBuffer(data);
        }

        private static ResultBuffer EditXData(
    ResultBuffer data,
    Dictionary<string, string> newData)
        {
            // Work with a List for easier add/replace
            var currentXData = data.AsArray().ToList();

            foreach (var kvp in newData)
            {
                bool found = false;

                for (int i = 0; i < currentXData.Count; i++)
                {
                    TypedValue tv = currentXData[i];

                    if (tv.TypeCode == 1000 && (tv.Value as string).StartsWith(kvp.Key + ":"))
                    {
                        // Replace existing entry
                        currentXData[i] = new TypedValue(1000, $"{kvp.Key}:{kvp.Value}");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // Add new entry
                    currentXData.Add(new TypedValue(1000, $"{kvp.Key}:{kvp.Value}"));
                }
            }

            return new ResultBuffer(currentXData.ToArray());
        }
    }
}
