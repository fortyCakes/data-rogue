using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace data_rogue_core.IOSystems
{

    public class RLNetLocationDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(LocationControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var mapname = systemContainer.PositionSystem.CoordinateOf(entity).Key.Key;
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            console.Print(control.Position.X, control.Position.Y, "Location:", display.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(control.Position.X+1, control.Position.Y+1, mapname, display.Color.ToRLColor(), display.BackColor.ToRLColor());
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(25, 2);
        }
    }
}
