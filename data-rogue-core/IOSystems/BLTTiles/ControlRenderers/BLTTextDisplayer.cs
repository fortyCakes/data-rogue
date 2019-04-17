using System.Collections.Generic;
using BearLib;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTextDisplayer : BLTControlRenderer
    {
        public override string DisplayType => "Text";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, InfoDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            RenderText(x, ref y, display.Parameters, display.Color);
        }
    }
}