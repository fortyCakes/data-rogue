using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleTargetingRenderer : BaseConsoleRenderer, ITargetingRenderer
    {
        private Dictionary<IRendereringConfiguration, RLConsole> Consoles = new Dictionary<IRendereringConfiguration, RLConsole>();
        
        public IOSystemConfiguration IOSystemConfiguration { get; }

        public ConsoleTargetingRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration) : base(console)
        {
            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

            List<IRendereringConfiguration> allConfigs = new List<IRendereringConfiguration>();
            allConfigs.AddRange(ioSystemConfiguration.MessageConfigurations);
            allConfigs.AddRange(ioSystemConfiguration.StatsConfigurations);
            allConfigs.AddRange(ioSystemConfiguration.MapConfigurations);

            foreach (IRendereringConfiguration config in allConfigs)
            {
                Consoles.Add(config, new RLConsole(config.Position.Width, config.Position.Height));
            }

            IOSystemConfiguration = ioSystemConfiguration;
        }

        public void Render(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            if (ReferenceEquals(systemContainer.PositionSystem, null))
            {
                return;
            }

            foreach (var mapConfiguration in IOSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer, targetingActivityData);
            }
        }

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            var mapConsole = Consoles[mapConfiguration];

            mapConsole.Clear();

            RLConsole.Blit(Console, 0, 0, mapConsole.Width, mapConsole.Height, mapConsole, 0, 0);

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);
            var playerFov = currentMap.FovFrom(playerPosition, 9);

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



            RLConsole.Blit(mapConsole, 0, 0, mapConsole.Width, mapConsole.Height, Console, mapConfiguration.Position.Left, mapConfiguration.Position.Top);
        }
    }
}
