using System;
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

    public class RLNetTitleDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(TitleControl);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            console.Print(display.Position.X, display.Position.Y, " the Untitled", display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(20,1);
        }
    }
}
