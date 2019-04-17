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

    public class RLNetComponentCounterDisplayer : RLNetStatsRendererHelper
    {
        public override string DisplayType => "ComponentCounter";

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var componentCounterSplits = display.Parameters.Split(',');
            var componentName = componentCounterSplits[0];
            var counterName = componentCounterSplits[1];

            var component = player.Get(componentName);
            FieldInfo[] fields = component.GetType().GetFields();
            var field = fields.Single(f => f.Name == counterName);
            var counter = (Counter)field.GetValue(component);

            ConsoleRendererHelper.PrintBar(console, 1, line, console.Width - 2, counterName, counter, display.BackColor.ToRLColor());

            line++;
        }
    }
}
