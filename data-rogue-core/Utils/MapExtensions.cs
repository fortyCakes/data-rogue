using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Linq;

namespace data_rogue_core.Utils
{
    public static class MapExtensions
    {
        public static MapCoordinate GetEmptyPosition(this Map map, IPrototypeSystem prototypeSystem, IPositionSystem positionSystem, IRandom random)
        {
            var emptyCell = prototypeSystem.Get("Cell:Empty");

            var emptyPositions = map.Cells
                .Where(c => c.Value == emptyCell)
                .Where(c => !positionSystem.Any(c.Key))
                .ToList();
            return random.PickOne(emptyPositions).Key;
        }

        public static MapCoordinate GetQuickEmptyPosition(this Map map, IPrototypeSystem prototypeSystem, IPositionSystem positionSystem, IRandom random, int tries = 5)
        {
            for (int i = 0; i < tries; i++)
            {
                var emptyCell = prototypeSystem.Get("Cell:Empty");

                var emptyPositions = map.Cells
                    .ToList();
                var cell = random.PickOne(emptyPositions);

                if (cell.Value == emptyCell && !positionSystem.Any(cell.Key))
                {
                    return cell.Key;
                }
            }

            return null;
        }
    }
}
