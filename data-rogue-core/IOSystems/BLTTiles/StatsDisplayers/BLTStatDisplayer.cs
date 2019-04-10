using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTStatDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "Stat";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            throw new System.NotImplementedException();
        }
    }
}