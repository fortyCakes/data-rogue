using System.Collections.Generic;
using BearLib;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{

    internal class BLTLargeTextDisplayer : BLTControlRenderer
    {
        public override string DisplayType => "LargeText";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, InfoDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var text = display.Parameters;

            BLT.Layer(BLTLayers.Text);
            BLT.Font("textLarge");
            BLT.Color(display.Color);
            BLT.Print(x, y, text);
                
            var size = BLT.Measure(text);
            y += size.Height + 1;

            BLT.Color("");
        }
    }
}