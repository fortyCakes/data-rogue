using System;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using data_rogue_core.Controls;
using data_rogue_core.Activities;
using System.Drawing;

namespace data_rogue_core.IOSystems
{

    public class RLNetWealthDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(WealthControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var wealthType = display.Parameters;
            console.Print(display.Position.X, display.Position.Y, $"{wealthType}: {systemContainer.ItemSystem.CheckWealth(entity, wealthType)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(20, 1);
        }
    }
}
