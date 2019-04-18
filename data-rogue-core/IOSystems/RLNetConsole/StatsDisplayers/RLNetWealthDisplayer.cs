using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{

    public class RLNetWealthDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(WealthControl);

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var wealthType = display.Parameters;
            console.Print(1, line, $"{wealthType}: {systemContainer.ItemSystem.CheckWealth(player, wealthType)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
        }
    }
}
