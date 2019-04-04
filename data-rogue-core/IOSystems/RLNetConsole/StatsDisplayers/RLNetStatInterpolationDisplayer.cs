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

    public class RLNetStatInterpolationDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "StatInterpolation";

        protected override void DisplayInternal(RLConsole console, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var interpolationSplits = display.Parameters.Split(',');
            var format = interpolationSplits[0];

            var statValues = interpolationSplits.Skip(1).Select(s => systemContainer.EventSystem.GetStat(player, s).ToString()).ToArray();

            var interpolated = string.Format(format, statValues);
            console.Print(1, line, interpolated, display.Color.ToRLColor(), display.BackColor.ToRLColor());

            line++;
        }
    }
}
