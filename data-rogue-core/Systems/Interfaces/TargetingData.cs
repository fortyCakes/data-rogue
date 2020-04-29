using data_rogue_core.Maps;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Systems.Interfaces
{

    public class TargetingData
    {
        public bool Friendly = false;
        public bool MoveToCell = false;
        public bool Rotatable = false;
        public bool TargetOrigin = true;
        public bool PathToTarget = false;

        public int? Range = null;

        public List<Vector> CellsHit = new List<Vector> { new Vector(0, 0) };
        public List<Vector> ValidVectors = new List<Vector>();

        public HashSet<MapCoordinate> TargetableCellsFrom(MapCoordinate playerPosition)
        {
            var targetableCells = new HashSet<MapCoordinate>();

            if (ValidVectors?.Any() == true)
            {
                foreach (var vector in ValidVectors)
                {
                    targetableCells.Add(playerPosition + vector);
                }
            }
            else if (Range.HasValue)
            {
                var range = Range.Value;

                if (range == 0)
                {
                   foreach (var vector in GetAdjacentCellVectors())
                   {
                       targetableCells.Add(playerPosition + vector);
                   }
                }

                if (range == 1)
                {
                    foreach (var vector in GetAdjacentAndDiagonalCellVectors())
                    {
                        targetableCells.Add(playerPosition + vector);
                    }
                }

                if (range > 1)
                {
                    for (int x = -range; x <= range; x++)
                    for (int y = -range; y <= range; y++)
                    {
                        if (x * x + y * y <= range * range && (TargetOrigin || x != 0 || y != 0))
                        {
                            targetableCells.Add(playerPosition + new Vector(x, y));
                        }
                    }
                }
            }

            return targetableCells;
        }

        private static Vector[] GetAdjacentCellVectors()
        {
            return new[]
            {
                new Vector(-1, 0),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, 0)
            };
        }

        private static Vector[] GetAdjacentAndDiagonalCellVectors()
        {
            return new[]
            {
                new Vector(-1, -1), new Vector(-1, 0), new Vector(-1, 1),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, -1), new Vector(1, 0), new Vector(1, 1)
            };
        }
    }
}
