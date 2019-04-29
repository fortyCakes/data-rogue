using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetButtonRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(ButtonControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as ButtonControl;
            var selected = display.IsFocused;
            var x = display.Position.X;
            var y = display.Position.Y;
            var text = display.Text;

            var foreColor = selected ? RLColor.Cyan : RLColor.White;

            console.Set(x, y, foreColor, RLColor.Black, 201);
            console.Set(x, y+1, foreColor, RLColor.Black, 186);
            console.Set(x, y+2, foreColor, RLColor.Black, 200);

            console.Set(x + text.Length + 1, y, foreColor, RLColor.Black, 187);
            console.Set(x + text.Length + 1, y + 1, foreColor, RLColor.Black, 186);
            console.Set(x + text.Length + 1, y + 2, foreColor, RLColor.Black, 188);

            for (int i = 0; i < text.Length; i++)
            {
                console.Set(x + i + 1, y, foreColor, RLColor.Black, 205);
                console.Set(x + i + 1, y + 2, foreColor, RLColor.Black, 205);
            }

            console.Print(x + 1, y + 1, text, foreColor, RLColor.Black);

            x += text.Length + 3;
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size((control as ButtonControl).Text.Length + 2, 3);
        }
    }
}