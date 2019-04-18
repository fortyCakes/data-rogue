using System;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems
{

    public class RLNetTextDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(TextControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;

            console.Print(control.Position.X, control.Position.Y, display.Parameters, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var lines = display.Parameters.Count(c => c == '\n') + 1;
            var longestLine = display.Parameters.Split('\n').Max(l => l.Length);
            return new Size(longestLine, lines);
        }
    }
}
