using NSVLIBConstants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeApp
{
    public class PipeDiameterTable
    {
        public static string GetPipeDiameter(string pipeSize, string PipeMaterial, string pipeSubMaterial)
        {
            pipeSize = pipeSize.Replace("\"", "");
            string pipeDiameter = string.Empty;
            if (PipeMaterial == "Black_Steel" || PipeMaterial == "Galvanized_Steel")
            {
                switch (pipeSubMaterial)
                {
                    case "10":
                        pipeDiameter = PipeDiameterTable.Schedule10[pipeSize];
                        break;
                    case "30":
                        pipeDiameter = PipeDiameterTable.Schedule30[pipeSize];
                        break;
                    case "40":
                        pipeDiameter = PipeDiameterTable.Schedule40[pipeSize];
                        break;
                }
            }
            else if (PipeMaterial == "Stainless_Steel")
            {
                switch (pipeSubMaterial)
                {
                    case "5ُ":
                        pipeDiameter = PipeDiameterTable.StainlessSteel5S[pipeSize];
                        break;
                    case "10ُS":
                        pipeDiameter = PipeDiameterTable.StainlessSteel10S[pipeSize];
                        break;
                    case "40S":
                        pipeDiameter = PipeDiameterTable.StainlessSteel40S[pipeSize];
                        break;
                    case "80S":
                        pipeDiameter = PipeDiameterTable.StainlessSteel80S[pipeSize];
                        break;
                }
            }
            else if (PipeMaterial == "Copper")
            {
                switch (pipeSubMaterial)
                {
                    case "Type K":
                        pipeDiameter = PipeDiameterTable.TypeK[pipeSize];
                        break;
                    case "Type L":
                        pipeDiameter = PipeDiameterTable.TypeL[pipeSize];
                        break;
                    case "Type M":
                        pipeDiameter = PipeDiameterTable.TypeM[pipeSize];
                        break;
                }
            }
            else if (PipeMaterial == "Brass")
            {
                switch (pipeSubMaterial)
                {
                    case "BrassRegular":
                        pipeDiameter = PipeDiameterTable.BrassRegular[pipeSize];
                        break;
                    case "BrassExtraStrong":
                        pipeDiameter = PipeDiameterTable.BrassExtraStrong[pipeSize];
                        break;
                }
            }
            else if (PipeMaterial == "CPVC")
            {
                switch (pipeSubMaterial)
                {
                    case "CPVCSDR11":
                        pipeDiameter = PipeDiameterTable.CPVCSDR11[pipeSize];
                        break;
                    case "CPVCSDR13.5":
                        pipeDiameter = PipeDiameterTable.CPVCSDR135[pipeSize];
                        break;
                    case "CPVCSDR17":
                        pipeDiameter = PipeDiameterTable.CPVCSDR17[pipeSize];
                        break;
                    case "CPVCSDR21":
                        pipeDiameter = PipeDiameterTable.CPVCSDR21[pipeSize];
                        break;
                    case "CPVCSDR26":
                        pipeDiameter = PipeDiameterTable.CPVCSDR26[pipeSize];
                        break;
                }
            }
            else
            {
                throw new ArgumentException("Pipe Diameter was not determined");
            }
            return pipeDiameter;
        }

        public static Dictionary<string, string> Schedule40 = new Dictionary<string, string>
        {
            {"1/2","0.622" },
            {"3/4","0.824" },
            {"1","1.049" },
            {"1-1/4","1.380" },
            {"1-1/2","1.610" },
            {"2","2.067" },
            {"2-1/2","2.469" },
            {"3","3.068" },
            {"3-1/2","3.548" },
            {"4","4.026" },
            {"5","5.047" },
            {"6","6.065" },
            {"8","7.981" },
            {"10","10.020" },
            {"12","11.938" },
            {"14","13.124" },
            {"16","15" },
            {"18","16.876" },
            {"20","18.812" },
            {"24","22.624" },
        };
        public static Dictionary<string, string> Schedule10 = new Dictionary<string, string>
        {
            {"1/2","0.674" },
            {"3/4","0.884" },
            {"1","1.097" },
            {"1-1/4","1.442" },
            {"1-1/2","1.682" },
            {"2","2.157" },
            {"2-1/2","2.635" },
            {"3","3.260" },
            {"3-1/2","3.760" },
            {"4","4.260" },
            {"5","5.295" },
            {"6","6.357" },
            {"8","8.249" },
            {"10","10.370" },
            
        };
        public static Dictionary<string, string> Schedule30 = new Dictionary<string, string>
        {
            {"8","8.071" },
            {"10","10.140" },
            {"12","12.090" },
        };
        public static Dictionary<string, string> TypeK = new Dictionary<string, string>
        {
            {"3/4","0.745" },
            {"1","0.995" },
            {"1-1/4","1.245" },
            {"1-1/2","1.481" },
            {"2","1.959" },
            {"2-1/2","2.435" },
            {"3","2.907" },
            {"3-1/2","3.385" },
            {"4","3.857" },
            {"5","4.805" },
            {"6","5.741" },
            {"8","7.583" },
            {"10","9.449" },
        };
        public static Dictionary<string, string> TypeL = new Dictionary<string, string>
        {
            {"3/4","0.785" },
            {"1","1.025" },
            {"1-1/4","1.265" },
            {"1-1/2","1.505" },
            {"2","1.985" },
            {"2-1/2","2.465" },
            {"3","2.947" },
            {"3-1/2","3.425" },
            {"4","3.905" },
            {"5","4.875" },
            {"6","5.484" },
            {"8","7.725" },
            {"10","9.625" },
        };
        public static Dictionary<string, string> TypeM = new Dictionary<string, string>
        {
            {"3/4","0.811" },
            {"1","1.055" },
            {"1-1/4","1.291" },
            {"1-1/2","1.527" },
            {"2","2.009" },
            {"2-1/2","2.495" },
            {"3","2.981" },
            {"3-1/2","3.459" },
            {"4","3.935" },
            {"5","4.907" },
            {"6","5.881" },
            {"8","7.785" },
            {"10","9.701" },
        };
        public static Dictionary<string, string> BrassRegular = new Dictionary<string, string>
        {
            {"1/2","0.622" },
            {"3/4","0.824" },
            {"1","1.049" },
            {"1-1/4","1.380" },
            {"1-1/2","1.610" },
            {"2","2.067" },
            {"2-1/2","2.469" },
            {"3","3.068" },
            {"3-1/2","3.548" },
            {"4","4.026" },
            {"5","5.047" },
            {"6","6.065" },
            {"8","7.981" },
            {"10","10.020" },
            {"12","12" },
        };
        public static Dictionary<string, string> BrassExtraStrong = new Dictionary<string, string>
        {
            {"1/2","0.546" },
            {"3/4","0.742" },
            {"1","0.957" },
            {"1-1/4","1.278" },
            {"1-1/2","1.500" },
            {"2","1.939" },
            {"2-1/2","2.323" },
            {"3","2.900" },
            {"3-1/2","3.364" },
            {"4","3.826" },
            {"5","4.813" },
            {"6","5.761" },
            {"8","7.625" },
            {"10","9.750" },
        };
        public static Dictionary<string, string> StainlessSteel5S = new Dictionary<string, string>
        {
            {"1/2","0.710" },
            {"3/4","0.920" },
            {"1","1.185" },
            {"1-1/4","1.530" },
            {"1-1/2","1.770" },
            {"2","2.245" },
            {"2-1/2","2.659" },
            {"3","3.334" },
            {"3-1/2","3.834" },
            {"4","4.334" },
            {"5","5.345" },
            {"6","6.407" },
            {"8","8.407" },
            {"10","10.482" },
            {"12","12.438" },
            {"14","13.688" },
            {"16","15.670" },
            {"18","17.671" },
            {"20","19.624" },
            {"22","21.624" },
        };
        public static Dictionary<string, string> StainlessSteel10S = new Dictionary<string, string>
        {
            {"1/8","0.307" },
            {"1/4","0.410" },
            {"3/8","0.545" },
            {"1/2","0.674" },
            {"3/4","0.884" },
            {"1","1.097" },
            {"1-1/4","1.442" },
            {"1-1/2","1.682" },
            {"2","2.157" },
            {"2-1/2","2.635" },
            {"3","3.260" },
            {"3-1/2","3.760" },
            {"4","4.260" },
            {"5","5.295" },
            {"6","6.357" },
            {"8","8.329" },
            {"10","10.420" },
            {"12","12.390" },
            {"14","13.624" },
            {"16","15.624" },
            {"18","17.624" },
            {"20","19.564" },
            {"22","21.564" },
            {"24","23.500" },
        };
        public static Dictionary<string, string> StainlessSteel40S = new Dictionary<string, string>
        {
            {"1/8","0.269" },
            {"1/4","0.364" },
            {"3/8","0.493" },
            {"1/2","0.622" },
            {"3/4","0.824" },
            {"1","1.049" },
            {"1-1/4","1.380" },
            {"1-1/2","1.610" },
            {"2","2.067" },
            {"2-1/2","2.469" },
            {"3","3.068" },
            {"3-1/2","3.548" },
            {"4","4.026" },
            {"5","5.047" },
            {"6","6.065" },
            {"8","7.981" },
            {"10","10.020" },
            {"12","12.000" },
            {"14","13.250" },
            {"16","15.250" },
            {"18","17.250" },
            {"20","19.250" },
            {"22","21.250" },
            {"24","23.250" },
        };

        public static Dictionary<string, string> StainlessSteel80S = new Dictionary<string, string>
        {
            {"1/8","0.215" },
            {"1/4","0.302" },
            {"3/8","0.423" },
            {"1/2","0.546" },
            {"3/4","0.742" },
            {"1","0.957" },
            {"1-1/4","1.278" },
            {"1-1/2","1.500" },
            {"2","1.939" },
            {"2-1/2","2.323" },
            {"3","2.900" },
            {"3-1/2","3.364" },
            {"4","3.826" },
            {"5","4.813" },
            {"6","5.761" },
            {"8","7.625" },
            {"10","9.750" },
            {"12","11.750" },
        };
        public static Dictionary<string, string> CPVCSDR325 = new Dictionary<string, string>
        {
            {"1/8", "0.408"},
            {"1/4", "0.666"},
            {"3/8", "0.868"},
            {"1/2", "1.108"},
            {"3/4", "1.348"},
            {"1", "1.658"},
            {"1 1/4", "2.098"},
            {"1 1/2", "2.348"},
            {"2", "2.998"},
            {"2 1/2", "3.498"},
            {"3", "4.098"},
            {"4", "5.398"},
            {"5", "6.598"},
            {"6", "7.898"},
            {"8", "10.298"},
            {"10", "12.698"},
            {"12", "15.098"}
        };

        public static Dictionary<string, string> CPVCSDR26 = new Dictionary<string, string>
        {
            {"1/8", "0.408"},
            {"1/4", "0.666"},
            {"3/8", "0.868"},
            {"1/2", "1.108"},
            {"3/4", "1.348"},
            {"1", "1.658"},
            {"1 1/4", "2.098"},
            {"1 1/2", "2.348"},
            {"2", "2.998"},
            {"2 1/2", "3.498"},
            {"3", "4.098"},
            {"4", "5.398"},
            {"5", "6.598"},
            {"6", "7.898"},
            {"8", "10.298"},
            {"10", "12.698"},
            {"12", "15.098"}
        };

        public static Dictionary<string, string> CPVCSDR21 = new Dictionary<string, string>
        {
            {"1/8", "0.408"},
            {"1/4", "0.666"},
            {"3/8", "0.868"},
            {"1/2", "1.109"},
            {"3/4", "1.350"},
            {"1", "1.660"},
            {"1 1/4", "2.100"},
            {"1 1/2", "2.350"},
            {"2", "3.000"},
            {"2 1/2", "3.500"},
            {"3", "4.100"},
            {"4", "5.400"},
            {"5", "6.600"},
            {"6", "7.900"},
            {"8", "10.300"},
            {"10", "12.700"},
            {"12", "15.100"}
        };

        public static Dictionary<string, string> CPVCSDR17 = new Dictionary<string, string>
        {
            {"1/8", "0.409"},
            {"1/4", "0.667"},
            {"3/8", "0.870"},
            {"1/2", "1.112"},
            {"3/4", "1.354"},
            {"1", "1.664"},
            {"1 1/4", "2.104"},
            {"1 1/2", "2.354"},
            {"2", "3.004"},
            {"2 1/2", "3.504"},
            {"3", "4.104"},
            {"4", "5.404"},
            {"5", "6.604"},
            {"6", "7.904"},
            {"8", "10.304"},
            {"10", "12.704"},
            {"12", "15.104"}
        };

        public static Dictionary<string, string> CPVCSDR135 = new Dictionary<string, string>
        {
            {"1/8", "0.410"},
            {"1/4", "0.669"},
            {"3/8", "0.872"},
            {"1/2", "1.116"},
            {"3/4", "1.358"},
            {"1", "1.668"},
            {"1 1/4", "2.108"},
            {"1 1/2", "2.358"},
            {"2", "3.008"},
            {"2 1/2", "3.508"},
            {"3", "4.108"},
            {"4", "5.408"},
            {"5", "6.608"},
            {"6", "7.908"},
            {"8", "10.308"},
            {"10", "12.708"},
            {"12", "15.108"}
        };

        public static Dictionary<string, string> CPVCSDR11 = new Dictionary<string, string>
        {
            {"1/8", "0.412"},
            {"1/4", "0.672"},
            {"3/8", "0.876"},
            {"1/2", "1.122"},
            {"3/4", "1.366"},
            {"1", "1.678"},
            {"1 1/4", "2.120"},
            {"1 1/2", "2.372"},
            {"2", "3.024"},
            {"2 1/2", "3.528"},
            {"3", "4.128"},
            {"4", "5.432"},
            {"5", "6.640"},
            {"6", "7.944"},
            {"8", "10.352"},
            {"10", "12.760"},
            {"12", "15.168"}
        };

        public static List<string> GetPipeSize (string subMaterial, NSVLIBConstants.Enums.Unit unit)
        {
            var sizes = new List<string>();
            switch (subMaterial)
            {
                case "10":
                    sizes = PipeDiameterTable.Schedule10.Keys.ToList();
                    break;
                case "30":
                    sizes = PipeDiameterTable.Schedule30.Keys.ToList();
                    break;
                case "40":
                    sizes = PipeDiameterTable.Schedule40.Keys.ToList();
                    break;
                case "5S":
                    sizes = PipeDiameterTable.StainlessSteel5S.Keys.ToList();
                    break;
                case "10S":
                    sizes = PipeDiameterTable.StainlessSteel10S.Keys.ToList();
                    break;
                case "40S":
                    sizes = PipeDiameterTable.StainlessSteel40S.Keys.ToList();
                    break;
                case "80S":
                    sizes = PipeDiameterTable.StainlessSteel80S.Keys.ToList();
                    break;
                case "Type K":
                    sizes = PipeDiameterTable.TypeK.Keys.ToList();
                    break;
                case "Type L":
                    sizes = PipeDiameterTable.TypeL.Keys.ToList();
                    break;
                case "Type M":
                    sizes = PipeDiameterTable.TypeM.Keys.ToList();
                    break;
                case "Regular":
                    sizes = PipeDiameterTable.BrassRegular.Keys.ToList();
                    break;
                case "Extra Strong":
                    sizes = PipeDiameterTable.BrassExtraStrong.Keys.ToList();
                    break;
                case "SDR32.5":
                    sizes = PipeDiameterTable.CPVCSDR325.Keys.ToList();
                    break;
                case "SDS26":
                    sizes = PipeDiameterTable.CPVCSDR26.Keys.ToList();
                    break;
                case "SDR21":
                    sizes = PipeDiameterTable.CPVCSDR21.Keys.ToList();
                    break;
                case "SDR17":
                    sizes = PipeDiameterTable.CPVCSDR17.Keys.ToList();
                    break;
                case "SDS135":
                    sizes = PipeDiameterTable.CPVCSDR135.Keys.ToList();
                    break;
                case "SDR11":
                    sizes = PipeDiameterTable.CPVCSDR11.Keys.ToList();
                    break;
                default:
                    throw new ArgumentException("sub material not found in list");
            }

            if(unit == NSVLIBConstants.Enums.Unit.metric)
            {
                var metricSizeData = new List<string>();

                foreach (var data in sizes)
                {
                    metricSizeData.Add((ParseFraction(data.ToString()) * 25).ToString());
                }

                return metricSizeData;
            }
            return sizes;
        }

        private static double ParseFraction(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Trim();

            if (input.Contains("-"))
            {
                var parts = input.Split('-');
                double whole = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double frac = ParseFraction(parts[1]);
                return whole + frac;
            }

            if (input.Contains("/"))
            {
                var parts = input.Split('/');
                double numerator = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double denominator = double.Parse(parts[1], CultureInfo.InvariantCulture);
                return numerator / denominator;
            }

            return double.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}
