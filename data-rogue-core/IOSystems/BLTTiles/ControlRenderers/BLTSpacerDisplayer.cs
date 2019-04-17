using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTSpacerDisplayer : BLTControlRenderer
    {
        public override string DisplayType => "Spacer";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, InfoDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            y += 4;
        }
    }
}