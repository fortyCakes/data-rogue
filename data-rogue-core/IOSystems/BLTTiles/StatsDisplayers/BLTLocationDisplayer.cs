using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTLocationDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "Location";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int y)
        {
            var mapName = systemContainer.PositionSystem.CoordinateOf(player).Key.Key;
            if (mapName.StartsWith("Branch:"))
            {
                mapName = mapName.Substring(7);
            }

            RenderText(x, ref y, $"Location: {mapName}", display.Color);
        }
    }
}