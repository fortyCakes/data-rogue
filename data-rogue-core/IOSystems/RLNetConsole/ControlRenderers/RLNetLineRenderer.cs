using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.IOSystems
{
    public class RLNetLineRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(LineControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as LineControl;

            for (int i = 0; i < control.Position.Width; i++)
            {
                console.Set(control.Position.Left + i, control.Position.Top, control.Color.ToRLColor(), control.BackColor.ToRLColor(), 196);
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(control.Position.Width, 2);
        }
    }
}