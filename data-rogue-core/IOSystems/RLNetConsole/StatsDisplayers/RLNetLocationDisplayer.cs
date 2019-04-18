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

    public class RLNetLocationDisplayer : RLNetControlRenderer
    {
        public override string DisplayType => "Location";

        protected override void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            var mapname = systemContainer.PositionSystem.CoordinateOf(player).Key.Key;
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            console.Print(1, line, "Location:", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
            console.Print(1, line, mapname, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
        }
    }
}
