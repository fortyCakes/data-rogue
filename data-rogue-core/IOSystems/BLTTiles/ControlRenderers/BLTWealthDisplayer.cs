using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTWealthDisplayer : BLTControlRenderer
    {
        public override string DisplayType => "Wealth";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, InfoDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var wealthType = display.Parameters;
            var text = $"{wealthType}: {systemContainer.ItemSystem.CheckWealth(entity, wealthType)}";

            RenderText(x, ref y, text, display.Color);
        }
    }
}