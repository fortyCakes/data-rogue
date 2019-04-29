using System;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Forms;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_core.IOSystems
{
    public class RLNetTextFormDataRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(TextFormData);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var formData = control as TextFormData;
            console.Print(control.Position.X, control.Position.Y, ((string)formData.Value).PadRight(30, '_'), formData.IsFocused ? RLColor.Cyan : RLColor.White);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(30, 1);
        }
    }
    public class RLNetTargetingOverlayRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(TargetingOverlayControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var targetingOverlayControl = control as TargetingOverlayControl;
            var targetingActivityData = targetingOverlayControl.TargetingActivityData;

            var mapConsole = new RLConsole(control.Position.Width, control.Position.Height);

            RLConsole.Blit(console, 0, 0, mapConsole.Width, mapConsole.Height, mapConsole, 0, 0);

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var targetableCells = targetingActivityData.TargetingData.TargetableCellsFrom(playerPosition);

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

                    var currentCell = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);

                    var targetable = targetableCells.Any(c => c == currentCell);
                    var isTarget = targetingActivityData.CurrentTarget == currentCell;

                    var cellTargeting = CellTargeting.None;

                    if (targetable) cellTargeting |= CellTargeting.Targetable;
                    if (isTarget) cellTargeting |= CellTargeting.CurrentTarget;

                    if (cellTargeting != CellTargeting.None)
                    {
                        MapRendererHelper.DrawCell(mapConsole, x, y, systemContainer.PositionSystem, currentMap, lookupX, lookupY, playerFov, cellTargeting);
                    }
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
