using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTNameDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "Name";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var text = $"Name: {entity.DescriptionName}";

            RenderText(x, ref y, text, display.Color);
        }
    }
}