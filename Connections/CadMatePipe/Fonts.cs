using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeApp
{
    public class Fonts
    {
        public static ObjectId GetTextStyleId(Database db, string FontStyle)
        {
            ObjectId styleId = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tst = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (tst.Has(FontStyle))
                {
                    styleId = tst[FontStyle];
                }
                else
                {
                    TextStyleTableRecord tsr = new TextStyleTableRecord();
                    tsr.Name = FontStyle;
                    tsr.FileName = $"{FontStyle}.ttf"; // Change this to the desired font file
                    tsr.Font = new Gssoft.Gscad.GraphicsInterface.FontDescriptor(FontStyle, false, false, 0, 0); // Change this to the desired font properties

                    tst.UpgradeOpen();
                    styleId = tst.Add(tsr);
                    tr.AddNewlyCreatedDBObject(tsr, true);
                }
                tr.Commit();
            }

            return styleId;
        }
    }
}
