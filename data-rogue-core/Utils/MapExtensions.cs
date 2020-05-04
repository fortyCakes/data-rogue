using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Utils
{
    public static class MapExtensions
    {
        public static MapCoordinate GetEmptyPosition(this Map map, IPositionSystem positionSystem, IRandom random)
        {
            var emptyPositions = map.Cells
                .Where(c => c.Value.Get<Physical>().Passable && c.Value.Get<Physical>().Transparent)
                .Where(c => !positionSystem.Any(c.Key))
                .ToList();
            return random.PickOne(emptyPositions).Key;
        }

        public static MapCoordinate GetQuickEmptyPosition(this Map map, IPositionSystem positionSystem, IRandom random, int tries = 5)
        {
            for (int i = 0; i < tries; i++)
            {

                var emptyPositions = map.Cells
                    .ToList();
                var cell = random.PickOne(emptyPositions);

                if (cell.Value.Get<Physical>().Passable && cell.Value.Get<Physical>().Transparent && !positionSystem.Any(cell.Key))
                {
                    return cell.Key;
                }
            }

            return null;
        }
    }
}
