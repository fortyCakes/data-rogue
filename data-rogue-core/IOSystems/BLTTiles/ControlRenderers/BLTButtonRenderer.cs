using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTButtonRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(ButtonControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as ButtonControl;
            var x = control.Position.X;
            var y = control.Position.Y;
            var pressed = control.IsPressed;
            var focused = control.IsFocused;
            var spriteSheet = focused ? spriteManager.Get("button_pressed") : spriteManager.Get("button_unpressed");

            BLT.Font("text");

            var textSize = BLT.Measure(display.Text).Width;
            var buttonSize = textSize + 4;
            if (buttonSize % 8 != 0)
            {
                buttonSize = (buttonSize / 8 + 1) * 8;
            }

            var numberOfTiles = buttonSize / 8;

            BLT.Font("");
            BLTLayers.Set((int)BLTLayers.UIElements, control.ActivityIndex);
            for (int i = 0; i < numberOfTiles; i++)
            {
                TileDirections direction = TileDirections.None;
                if (i != 0)
                {
                    direction |= TileDirections.Left;
                }

                if (i != numberOfTiles - 1)
                {
                    direction |= TileDirections.Right;
                }

                BLT.Put(x + 8 * i, y, spriteSheet.Tile(direction));
            }

            BLT.Font("text");
            BLTLayers.Set((int)BLTLayers.Text, control.ActivityIndex);
            BLT.Print(x + buttonSize / 2 - textSize / 2, y + (focused ? 3 : 2), display.Text);
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as ButtonControl;

            BLT.Font("text");

            var textSize = BLT.Measure(display.Text).Width;
            var buttonSize = textSize + 4;
            if (buttonSize % 8 != 0)
            {
                buttonSize = (buttonSize / 8 + 1) * 8;
            }

            var numberOfTiles = buttonSize / 8;

            return new Size(8 * numberOfTiles, 8);
        }
    }
}