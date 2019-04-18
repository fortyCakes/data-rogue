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
using System.Drawing;
using System.Linq;
using System.Reflection;
using data_rogue_core.Activities;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{

    public class RLNetComponentCounterDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(ComponentCounter);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var control = display as IDataRogueInfoControl;
            var y = control.Position.Y;

            var componentCounterSplits = control.Parameters.Split(',');
            var componentName = componentCounterSplits[0];
            var counterName = componentCounterSplits[1];

            var component = control.Entity.Get(componentName);
            FieldInfo[] fields = component.GetType().GetFields();
            var field = fields.Single(f => f.Name == counterName);
            var counter = (Counter)field.GetValue(component);

            ConsoleRendererHelper.PrintBar(console, 1, y, console.Width - 2, counterName, counter, display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(console.Width - 2, 1);
        }
    }
}
