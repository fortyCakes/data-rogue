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
    public class RLNetSpacerDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(Spacer);
        protected override void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(1, 1);
        }
    }
}
