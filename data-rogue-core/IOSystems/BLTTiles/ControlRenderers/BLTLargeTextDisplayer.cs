using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{

    internal class BLTLargeTextDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(LargeTextControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var text = display.Parameters;
            BLT.Font("textLarge");

            BLTLayers.Set(BLTLayers.Text, display.ActivityIndex);
            BLT.Color(display.Color);
            BLT.Print(display.Position.X, display.Position.Y, text);

            BLT.Color("");
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var text = display.Parameters;
            BLT.Font("textLarge");

            var size = BLT.Measure(text);
            size.Height += 1;

            return size;
        }
    }
}