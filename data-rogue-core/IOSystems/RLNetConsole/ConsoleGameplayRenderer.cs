using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleGameplayRenderer : BaseConsoleRenderer, IGameplayRenderer
    {
        private Dictionary<IRendereringConfiguration, RLConsole> Consoles = new Dictionary<IRendereringConfiguration, RLConsole>();
        public IOSystemConfiguration IOSystemConfiguration { get; }

        private List<IStatsRendererHelper> statsDisplayers;

        public ConsoleGameplayRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration) : base(console)
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

            statsDisplayers = RLNetStatsRendererHelper.DefaultStatsDisplayers.OfType<IStatsRendererHelper>().ToList();

            statsDisplayers.AddRange(ioSystemConfiguration.AdditionalStatsDisplayers);
        }

        public void Render(ISystemContainer systemContainer)
        {
            Console.Clear(176, RLColor.Black, RLColor.White);

            if (ReferenceEquals(systemContainer?.PlayerSystem?.Player, null))
            {
                return;
            }

            var playerFov = CalculatePlayerFov(systemContainer);

            foreach (var mapConfiguration in IOSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer, playerFov);
            }

            foreach(var statsConfiguration in IOSystemConfiguration.StatsConfigurations)
            {
                RenderStats(statsConfiguration, systemContainer, playerFov);
            }

            foreach (var messageConfiguration in IOSystemConfiguration.MessageConfigurations)
            {
                RenderMessages(messageConfiguration, systemContainer);
            }

            RenderLines();
        }

        private List<MapCoordinate> CalculatePlayerFov(ISystemContainer systemContainer)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var playerFov = currentMap.FovFrom(playerPosition, 9);

            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

            return playerFov;
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            foreach (MapConfiguration map in IOSystemConfiguration.MapConfigurations)
            {
                if (IsOnMap(map, x, y))
                {
                    var lookupX = cameraPosition.X - map.Position.Width / 2 + x;
                    var lookupY = cameraPosition.Y - map.Position.Height / 2 + y;

                    return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
                }
            }

            return null;
        }

        private bool IsOnMap(MapConfiguration map, int x, int y)
        {
            var mapWidth = map.Position.Width;
            var mapHeight = map.Position.Height;
            return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
        }

        private void RenderLines()
        {
            var foreColor = RLColor.White;
            var backColor = RLColor.Black;

            // Work out outline renderering later
        }

        private void RenderMessages(MessageConfiguration messageConfiguration, ISystemContainer systemContainer)
        {
            var MessageConsole = Consoles[messageConfiguration];

            MessageConsole.Clear();

            var messages = systemContainer.MessageSystem.RecentMessages(15);
            messages.Reverse();

            int y = 14;
            foreach (Message message in messages)
            {
                MessageConsole.Print(0, y--, 1, message.Text, message.Color.ToRLColor(), null, MessageConsole.Width);
            }

            RLConsole.Blit(MessageConsole, 0, 0, MessageConsole.Width, MessageConsole.Height, Console, messageConfiguration.Position.Left, messageConfiguration.Position.Top);
        }

        private void RenderStats(StatsConfiguration statsConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var statsConsole = Consoles[statsConfiguration];

            statsConsole.Clear();

            var player = systemContainer.PlayerSystem.Player;
            int line = 1;

            foreach(StatsDisplay display in statsConfiguration.Displays)
            {
                var statsDisplayer = statsDisplayers.Single(s => s.DisplayType == display.DisplayType);
                statsDisplayer.Display(statsConsole, display, systemContainer, player, playerFov, ref line);
            }

            RLConsole.Blit(statsConsole, 0, 0, statsConsole.Width, statsConsole.Height, Console, statsConfiguration.Position.Left, statsConfiguration.Position.Top);
        }

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var mapConsole = Consoles[mapConfiguration];
                
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

            RLConsole.Blit(mapConsole, 0, 0, mapConsole.Width, mapConsole.Height, Console, mapConfiguration.Position.Left, mapConfiguration.Position.Top);
        }
    }
}
