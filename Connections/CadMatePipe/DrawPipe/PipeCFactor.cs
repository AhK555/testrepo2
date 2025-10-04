using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PipeApp.DrawPipe
{
    internal class PipeCFactor
    {
        public static string Get(string material, bool isDry, string systemType)
        {
            if (material == "Black_Steel")
            {
                if (isDry)
                {
                    switch (systemType)
                    {
                        case "Preaction":
                            return "100";
                        case "Preaction using Nitrogen":
                        case "Preaction using Vacuum Pressure":
                        case "Preaction using Vapor Corrosion Inhibitor":
                            return "120";
                    }
                }
                else 
                {
                    return "120";
                }
            }
            else if (material == "Galvanized_Steel")
            {
                if (isDry)
                {
                    switch (systemType)
                    {
                        case "Preaction":
                            return "100";
                        case "Preaction using Nitrogen":
                        case "Preaction using Vacuum Pressure":
                        case "Preaction using Vapor Corrosion Inhibitor":
                            return "120";
                    }
                }
                else 
                {
                    return "120";
                }
            }
            else if (material == "Stainless_Steel" || material == "Brass" || material == "Copper")
            {
                return "150";
            }
            else if (material == "CPVC")
            {
                return "150";
            }

            throw new ArgumentException("Material or system type does not exist");
        }
    }
}
