using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMenuSelectorControlRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MenuSelectorControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuSelectorControl;
            var baseX = control.Position.Left;
            var y = control.Position.Top;
            var sprite = spriteManager.Get(display.Direction == TileDirections.Left ? "selector_left" : "selector_right");

            BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
            BLT.Font("");
            BLT.PutExt(baseX, y, 0, -1, sprite.Tile(display.Direction));
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(2, 4);
        }
    }
}