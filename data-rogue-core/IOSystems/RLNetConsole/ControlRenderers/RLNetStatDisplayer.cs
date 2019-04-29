using System;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{

    public class RLNetStatDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(StatControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;

            var x = display.Position.X;
            var y = display.Position.Y;
            var entity = display.Entity;

            var statName = display.Parameters;
            console.Print(x, y, $"{statName}: {systemContainer.EventSystem.GetStat(entity, statName)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(console.Width, 1);
        }
    }
}
