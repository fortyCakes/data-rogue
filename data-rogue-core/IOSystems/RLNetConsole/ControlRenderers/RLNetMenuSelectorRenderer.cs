using System;
using System.Collections.Generic;
using System.Drawing;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetMenuSelectorRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MenuSelectorControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuSelectorControl;

            if (display.Direction == TileDirections.Left)
            {
                console.Print(display.Position.X, display.Position.Y, ">", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            }
            else if (display.Direction == TileDirections.Right)
            {
                console.Print(display.Position.X, display.Position.Y, "<", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(1,1);
        }
    }
}