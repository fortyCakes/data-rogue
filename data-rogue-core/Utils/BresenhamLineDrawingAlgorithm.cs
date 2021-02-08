using System;
using System.Collections.Generic;
using data_rogue_core.Maps;

namespace data_rogue_core.Utils
{
    public class BresenhamLineDrawingAlgorithm : ILineDrawingAlgorithm
    {
        public IEnumerable<MapCoordinate> DrawLine(MapCoordinate origin, MapCoordinate destination)
        {
            double deltaX = origin.X - destination.X;
            double deltaY = origin.Y - destination.Y;

            if (Math.Abs(deltaY) < Math.Abs(deltaX))
            {
                if (origin.X < destination.X)
                {
                    return PlotLineLow(destination, origin);
                }
                else
                {
                    return PlotLineLow(origin, destination);
                }
            }
            else
            {
                if (origin.Y < destination.Y)
                {
                    return PlotLineHigh(destination, origin);
                }
                else
                {
                    return PlotLineHigh(origin, destination);
                }
            }
        }

        private IEnumerable<MapCoordinate> PlotLineHigh(MapCoordinate destination, MapCoordinate origin)
        {
            return PlotLineAbstract(
                origin.Y, 
                destination.Y, 
                origin.X, 
                destination.X, 
                (y, x) => new MapCoordinate(origin.Key, x, y));
        }

        private IEnumerable<MapCoordinate> PlotLineLow(MapCoordinate destination, MapCoordinate origin)
        {
            return PlotLineAbstract(
                origin.X, 
                destination.X, 
                origin.Y, 
                destination.Y, 
                (x, y) => new MapCoordinate(origin.Key, x, y));
        }

        private IEnumerable<MapCoordinate> PlotLineAbstract(int x0, int x1, int y0, int y1, Func<int, int, MapCoordinate> plot)
        {
            double deltaX = x1 - x0;
            double deltaY = y1 - y0;
            int yi = 1;

            if (deltaY < 0)
            {
                yi = -1;
                deltaY *= -1;
            }

            var D = (2 * deltaY) - deltaX;
            var y = y0;

            for (int x = x0; x <= x1; x++)
            {
                yield return plot(x, y);

                if (D > 0)
                {
                    y += yi;
                    D += 2 * (deltaY - deltaX);
                }
                else
                {
                    D += 2 * deltaY;
                }
            }
        }
    }
}