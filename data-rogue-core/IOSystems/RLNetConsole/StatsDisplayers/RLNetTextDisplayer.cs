using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.IOSystems
{

    public class RLNetTextDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "Text";

        protected override void DisplayInternal(RLConsole console, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var lines = display.Parameters.Count(c => c == '\n') + 1;

            console.Print(1, line, display.Parameters, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line += lines;
        }
    }
}
