using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class BLTMenuActionRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MenuActionsControl);
        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuActionsControl;

            if (display.AvailableActions.Count > 1)
            {
                BLT.Font("text");
                BLTLayers.Set(BLTLayers.Text, display.ActivityIndex);

                BLT.Color(display.Color);

                var x = control.Position.X;
                var y = control.Position.Y;

                BLT.Print(x, y, "[[");
                x += BLT.Measure("[[").Width;

                foreach (var action in display.AvailableActions)
                {
                    if (action == display.SelectedAction)
                    {
                        BLT.Color(display.SelectedColor);
                    }
                    BLT.Print(x, y, action.ToString());
                    BLT.Color("");

                    x += BLT.Measure(action.ToString()).Width;
                    BLT.Print(x, y, "|");
                    x += BLT.Measure("|").Width;
                }

                x -= BLT.Measure("|").Width;
                BLT.Print(x, y, "]]");
            }
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MenuActionsControl;
            var text = $"[[{string.Join("|", display.AvailableActions.Select(a => a.ToString()))}]]";

            BLT.Font("text");
            var size = BLT.Measure(text);
            return size;
        }
    }
}