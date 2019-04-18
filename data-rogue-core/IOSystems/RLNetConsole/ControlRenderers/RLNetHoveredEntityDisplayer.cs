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

    public class RLNetHoveredEntityDisplayer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(HoveredEntityDisplayBox);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;

            var hoveredCoordinate = systemContainer.ControlSystem.HoveredCoordinate;

            if (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                PrintEntityDetails(display, hoveredEntity, console);
            }
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(console.Width, 3);
        }
    }
}
