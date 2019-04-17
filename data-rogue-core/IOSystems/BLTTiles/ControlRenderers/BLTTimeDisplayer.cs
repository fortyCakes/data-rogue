using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTimeDisplayer : BLTControlRenderer
    {
        public override string DisplayType => "Time";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, InfoDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var text = $"Time: {systemContainer.TimeSystem.TimeString}";
            RenderText(x, ref y, text, display.Color);
        }
    }
}