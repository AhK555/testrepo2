using Gssoft.Gscad.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace NSVLibUtils
{
    public class BuldgeUtils
    {
        public static double GetRadius(double cordLength, double buldge)
        {
            // using formula
            // r = ((c/2)^2 + s^2) / 2 * s
            double halfCordLength = cordLength / 2;
            double s = halfCordLength * buldge;
            double radius = ((s * s) + (halfCordLength * halfCordLength)) / (2 * s);
            return radius;
        }

        public static double GetBuldge(double radius, double cordLength)
        {
            // using cosine law to get the angle between the end points of the arc
            // then returning the buldge as tangent of that angle / 4
            var cosine = ((2 * radius * radius) - (cordLength * cordLength)) / (2 * radius * radius);
            var angle = Math.Acos(cosine);
            return Math.Abs(Math.Tan(angle / 4));
        }

        public static double BulgeFromCurve(Curve cv, bool clockwise)
        {
            double bulge = 0.0;
            Arc a = cv as Arc;
            if (a != null)
            {
                double newStart;
                // The start angle is usually greater than the end,
                // as arcs are all counter-clockwise.
                // (If it isn't it's because the arc crosses the
                // 0-degree line, and we can subtract 2PI from the
                // start angle.)
                if (a.StartAngle > a.EndAngle) newStart = a.StartAngle - 8 * Math.Atan(1);
                else newStart = a.StartAngle;
                // Bulge is defined as the tan of
                // one fourth of the included angle
                bulge = Math.Tan((a.EndAngle - newStart) / 4);
                // If the curve is clockwise, we negate the bulge
                if (clockwise) bulge = -bulge;
            }

            return bulge;
        }
    }
}
