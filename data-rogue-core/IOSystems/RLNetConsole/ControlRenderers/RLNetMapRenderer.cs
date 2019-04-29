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
    public class RLNetMapRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MapControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var mapConsole = new RLConsole(control.Position.Width, control.Position.Height);

            mapConsole.Clear();

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            var consoleWidth = mapConsole.Width;
            var consoleHeight = mapConsole.Height;

            int offsetX = consoleWidth / 2;
            int offsetY = consoleHeight / 2;

            for (int y = 0; y < consoleHeight; y++)
            {
                for (int x = 0; x < consoleWidth; x++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    MapRendererHelper.DrawCell(mapConsole, x, y, systemContainer.PositionSystem, currentMap, lookupX, lookupY, playerFov);
                }
            }

            RLConsole.Blit(mapConsole, 0, 0, mapConsole.Width, mapConsole.Height, console, control.Position.Left, control.Position.Top);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }
    }
}
