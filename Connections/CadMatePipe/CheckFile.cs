using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSVLIBConstants;

namespace PipeApp
{
    public class CheckFile
    {
        public static void checkDWG(Address address)
        {
            try
            {
                var dwgPath = address.UserDwgAddress;
                var bmpPath = address.UserBmpAddress;

                string dwgDirectoryPath = Path.GetDirectoryName(dwgPath);
                string bmpDirectoryPath = Path.GetDirectoryName(bmpPath);

                if (!Directory.Exists(dwgDirectoryPath))
                {
                    Directory.CreateDirectory(dwgDirectoryPath);
                }

                if (!File.Exists(dwgPath))
                {
                    using (Database db = new Database(true, true))
                    {
                        db.SaveAs(dwgPath, DwgVersion.Current);
                    }
                }
            }
            catch { }
        }
    }
}
