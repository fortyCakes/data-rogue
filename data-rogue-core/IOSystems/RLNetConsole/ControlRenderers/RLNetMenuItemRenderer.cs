using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetMenuItemRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MenuItem);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuItem;

            console.Print(control.Position.X, control.Position.Y, display.Text, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuItem;
            return new Size(display.Text.Length, 1);
        }
    }
}