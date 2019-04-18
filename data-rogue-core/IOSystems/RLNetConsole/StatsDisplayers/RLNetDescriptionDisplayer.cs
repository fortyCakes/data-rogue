using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{
    public class RLNetDescriptionDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(DescriptionControl);

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            console.Print(1, line, player.Get<Description>().Detail, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
        }
    }
}
