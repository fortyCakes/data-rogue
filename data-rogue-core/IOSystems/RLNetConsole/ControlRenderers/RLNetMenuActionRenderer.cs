using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetMenuActionRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MenuActionsControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuActionsControl;

            if (display.AvailableActions.Count > 1)
            {
                var x = control.Position.X;
                var y = control.Position.Y;

                console.Print(x++, y, "[", control.Color.ToRLColor(), control.BackColor.ToRLColor());

                foreach (var action in display.AvailableActions)
                {
                    var foreColor = action == display.SelectedAction ? display.SelectedColor : control.Color;

                    console.Print(x, y, action.ToString(), foreColor.ToRLColor(), control.BackColor.ToRLColor());
                    x += action.ToString().Length;
                    console.Print(x, y, "|", control.Color.ToRLColor(), control.BackColor.ToRLColor());
                    x++;
                }

                x--;
                console.Print(x, y, "]", control.Color.ToRLColor(), control.BackColor.ToRLColor());
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuActionsControl;

            if (display.AvailableActions.Count > 1)
            {
                var width = GetWidth(display);

                return new Size(width, 1);
            }
            else
            {
                return new Size(0,0);
            }
        }

        private int GetWidth(MenuActionsControl display)
        {
            var totalActionWidth = display.AvailableActions.Sum(a => a.ToString().Length + 1) - 1;
            return totalActionWidth + 2;
        }
    }
}