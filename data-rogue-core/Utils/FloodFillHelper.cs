using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Utils
{
    public static class FloodFillHelper
    {
        public static IEnumerable<MapCoordinate> FloodFill(MapCoordinate mapCoordinate, Func<MapCoordinate, bool> canFillInto, int maxCheckCount = 1000)
        {
            var targetedCoordinates = new List<MapCoordinate>();

            var coordinatesToCheck = new Queue<MapCoordinate>();
            coordinatesToCheck.Enqueue(mapCoordinate);

            var alreadyChecked = new List<MapCoordinate> { };

            while (coordinatesToCheck.Any())
            {
                var coordinate = coordinatesToCheck.Dequeue();

                if (canFillInto(coordinate))
                {
                    targetedCoordinates.Add(coordinate);

                    foreach (var adjacentVector in Vector.GetAdjacentCellVectors())
                    {
                        var newCoordinate = coordinate + adjacentVector;
                        if (!alreadyChecked.Contains(newCoordinate) && !coordinatesToCheck.Contains(newCoordinate))
                        {
                            coordinatesToCheck.Enqueue(newCoordinate);
                        }
                    }
                }

                alreadyChecked.Add(coordinate);

                var vector = mapCoordinate - coordinate;

                if (vector.Length > 1000 || alreadyChecked.Count > maxCheckCount)
                {
                    // The flood fill has almost certainly escaped.
                    return new List<MapCoordinate>();
                }
            }

            return targetedCoordinates;
        }
    }
}
