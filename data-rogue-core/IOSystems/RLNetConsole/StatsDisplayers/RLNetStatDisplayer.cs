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

namespace data_rogue_core.IOSystems
{

    public class RLNetStatDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "Stat";

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var statName = display.Parameters;
            console.Print(1, line, $"{statName}: {systemContainer.EventSystem.GetStat(player, statName)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
        }
    }
}
