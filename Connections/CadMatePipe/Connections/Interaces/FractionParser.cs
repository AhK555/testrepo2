using System;
using System.Globalization;

namespace CadMatePipe.Connections
{
    internal class FractionParser
    {
        internal static double ParseMixedNumber(string input)
        {
            //TODO: move to libUtils
            input = input.Trim();

            // Handle mixed number (e.g., "2 3/4")
            if (input.Contains(" ") && input.Contains("/"))
            {
                string[] parts = input.Split(' ');
                double whole = double.Parse(parts[0], CultureInfo.InvariantCulture);

                string[] fractionParts = parts[1].Split('/');
                double numerator = double.Parse(fractionParts[0], CultureInfo.InvariantCulture);
                double denominator = double.Parse(fractionParts[1], CultureInfo.InvariantCulture);

                return whole + (numerator / denominator);
            }
            // Handle pure fraction (e.g., "3/4")
            else if (input.Contains("/"))
            {
                string[] fractionParts = input.Split('/');
                double numerator = double.Parse(fractionParts[0], CultureInfo.InvariantCulture);
                double denominator = double.Parse(fractionParts[1], CultureInfo.InvariantCulture);

                return numerator / denominator;
            }
            // Handle whole number (e.g., "1")
            else
            {
                return double.Parse(input, CultureInfo.InvariantCulture);
            }
        }
    }
}