using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleTargetingRenderer : BaseConsoleRenderer, ITargetingRenderer
    {
        private readonly int STATS_WIDTH;
        private readonly int MESSAGE_HEIGHT;

        private RLConsole MapConsole { get; set; }
        private RLConsole StatsConsole { get; set; }

        public ConsoleTargetingRenderer(RLConsole console) : base(console)
        {
            STATS_WIDTH = ConsoleGameplayRenderer.STATS_WIDTH;
            MESSAGE_HEIGHT = ConsoleGameplayRenderer.MESSAGE_HEIGHT;

            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

            MapConsole = new RLConsole(consoleWidth - STATS_WIDTH - 1, consoleHeight - MESSAGE_HEIGHT - 1);
            StatsConsole = new RLConsole(STATS_WIDTH, consoleHeight - MESSAGE_HEIGHT - 1);

        }

        public void Render(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            if (ReferenceEquals(systemContainer.PositionSystem, null))
            {
                return;
            }

            RenderMap(systemContainer, targetingActivityData);

            RenderStats(systemContainer, targetingActivityData);

        }

        private void RenderStats(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            StatsConsole.Clear();

            RLConsole.Blit(Console, Console.Width - 22, 0, StatsConsole.Width, StatsConsole.Height, StatsConsole, 0, 0);

            if (targetingActivityData.CurrentTarget == null)
            {
                StatsConsole.Print(1, StatsConsole.Height - 15, $"Target: -", RLColor.White);
            }
            else
            {
                StatsConsole.Print(1, StatsConsole.Height - 15, $"Target: {targetingActivityData.CurrentTarget.X}, {targetingActivityData.CurrentTarget.Y}", RLColor.White);
            }

            RLConsole.Blit(StatsConsole, 0, 0, StatsConsole.Width, StatsConsole.Height, Console, Console.Width - 22, 0);
        }

        private void RenderMap(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            MapConsole.Clear();

            RLConsole.Blit(Console, 0, 0, MapConsole.Width, MapConsole.Height, MapConsole, 0, 0);

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);
            var playerFov = currentMap.FovFrom(playerPosition, 9);

            var targetableCells = targetingActivityData.TargetingData.TargetableCellsFrom(playerPosition);

            var consoleWidth = MapConsole.Width;
            var consoleHeight = MapConsole.Height;

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
                        MapRendererHelper.DrawCell(MapConsole, x, y, systemContainer.PositionSystem, currentMap, lookupX, lookupY, playerFov, cellTargeting);
                    }
                }
            }



            RLConsole.Blit(MapConsole, 0, 0, MapConsole.Width, MapConsole.Height, Console, 0, 0);
        }
    }
}
