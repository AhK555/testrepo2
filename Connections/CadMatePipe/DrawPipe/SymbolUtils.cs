using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.GraphicsInterface;
using NSVLibUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PipeApp.DrawPipe
{
    internal class SymbolUtils
    {
        internal static IEnumerable<CrossSymbol> GroupSymbols(IEnumerable<CrossSymbol> symbols, 
            WorldDraw draw = null)
        {
            symbols = SortSymbols(symbols);
            var res = new List<CrossSymbol> ();
            if (symbols.Count() == 0)
                return res;
            var firstSymbolPoint = symbols.First().Pair.Item1.Position;
            foreach (var symbol in symbols)
            {
                if (res.Count == 0)
                {
                    res.Add(symbol);
                    continue;
                }
                var lastSymbol = res.Last();
                if (draw != null)
                {
                    var line1 = new Line(lastSymbol.Pair.Item1.Position, lastSymbol.Pair.Item2.Position);
                    line1.ColorIndex = 2;
                    line1.LineWeight = LineWeight.LineWeight090;
                    draw.Geometry.Draw(line1);
                    var line2 = new Line(symbol.Pair.Item1.Position, symbol.Pair.Item2.Position);
                    line2.ColorIndex = 3;
                    line2.LineWeight = LineWeight.LineWeight090;
                    draw.Geometry.Draw(line2);
                }
                if (!DoOverlap(symbol, lastSymbol, firstSymbolPoint))
                {
                    res.Add(symbol);
                    continue;
                }

                if (firstSymbolPoint.DistanceTo(lastSymbol.Pair.Item2.Position) > firstSymbolPoint.DistanceTo(symbol.Pair.Item2.Position))
                    continue;

                res.RemoveAt(res.Count - 1);
                res.Add(new CrossSymbol((lastSymbol.Pair.Item1, symbol.Pair.Item2), lastSymbol.CorrespondingPipe));
            }

            return res;
        }

        private static bool DoOverlap(CrossSymbol symbol, CrossSymbol lastSymbol, Point3d firstSymbolPoint)
        {
            var upperBounded = firstSymbolPoint.DistanceTo(lastSymbol.Pair.Item2.Position) > firstSymbolPoint.DistanceTo(symbol.Pair.Item1.Position);
            var lowerBounded = firstSymbolPoint.DistanceTo(lastSymbol.Pair.Item1.Position) < firstSymbolPoint.DistanceTo(symbol.Pair.Item1.Position);
            return (upperBounded && lowerBounded);
        }

        private static IEnumerable<CrossSymbol> SortSymbols(IEnumerable<CrossSymbol> symbols)
        {
            var symbolList = symbols.ToList();
            if (symbolList.Count < 2)
                return symbolList;

            var allSymbols = symbolList.SelectMany((item) => new[] { item.Pair.Item1, item.Pair.Item2 });
            var anchorSymbol = allSymbols.OrderBy(s => s.Position.X)
                                         .ThenBy(s => s.Position.Y)
                                         .First();

            var anchorPoint = anchorSymbol.Position;
            var fixedSymbolList = new List<CrossSymbol>();
            foreach (var symbol in symbolList)
            {
                if (symbol.Pair.Item1.Position.DistanceTo(anchorPoint) < symbol.Pair.Item2.Position.DistanceTo(anchorPoint))
                {
                    fixedSymbolList.Add(symbol);
                    continue;
                }

                fixedSymbolList.Add(symbol.Flip());
            }

            return fixedSymbolList.OrderBy((item) => item.Pair.Item1.Position.DistanceTo(anchorPoint)).ToList();
        }

        internal static 
            (IEnumerable<CrossSymbol> symbolsOnPipe, 
            IEnumerable<CrossSymbol> SymbolsOutSidePipe)  
            SeperateSymbols(IEnumerable<CrossSymbol> symbols, Line pipe)
        {
            var symbolsOnPipe = new List<CrossSymbol>();
            var symbolsOutOfPipe = new List<CrossSymbol>();
            if (pipe.Length == 0)
                return (symbolsOnPipe, symbolsOutOfPipe);

            var pipeTolerantRegion = LocalUtils.CreateLineTolerantRegion(pipe);

            foreach (var symbol in symbols)
            {
                if (
                    Check.IsPointInside(
                        pipeTolerantRegion,symbol.Pair.Item1.Position) ||
                    Check.IsPointInside(
                        pipeTolerantRegion,symbol.Pair.Item2.Position))
                {
                    symbolsOnPipe.Add(symbol);
                }
                else
                    symbolsOutOfPipe.Add(symbol);
            }

            return (symbolsOnPipe, symbolsOutOfPipe);
        }
    }
}