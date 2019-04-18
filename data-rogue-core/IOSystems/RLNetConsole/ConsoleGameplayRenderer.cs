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
        private RLConsole backgroundConsole;
        public IOSystemConfiguration IOSystemConfiguration { get; }

        private List<IDataRogueControlRenderer> _controlRenderers;
        private char?[,] _lineChars;

        public ConsoleGameplayRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration, List<IDataRogueControlRenderer> controlRenderers) : base(console)
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

            backgroundConsole = new RLConsole(Console.Width, Console.Height);

           
            IOSystemConfiguration = ioSystemConfiguration;

            CalculateLines();

            _controlRenderers = controlRenderers;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Console.Clear();
            
            RenderLines();

            if (ReferenceEquals(systemContainer?.PlayerSystem?.Player, null))
            {
                return;
            }

            var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

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
            return x >= map.Position.Left && x <= map.Position.Right && y >= map.Position.Top && y < map.Position.Bottom;
        }

        private void RenderLines()
        {
            var foreColor = RLColor.White;
            var backColor = RLColor.Black;

            for (int x = 0; x < Console.Width; x++)
            {
                for (int y = 0; y < Console.Height; y++)
                {
                    if (_lineChars[x,y].HasValue)
                    {
                        backgroundConsole.Print(x, y, _lineChars[x, y].ToString(), foreColor, backColor);
                    }
                }
            }


            RLConsole.Blit(backgroundConsole, 0, 0, backgroundConsole.Width, backgroundConsole.Height, Console, 0, 0);
        }

        private bool IsInSubconsole(int x, int y)
        {
            foreach(var config in IOSystemConfiguration.MapConfigurations)
            {
                if (config.Position.Contains(x,y))
                {
                    return true;
                }
            }

            foreach (var config in IOSystemConfiguration.StatsConfigurations)
            {
                if (config.Position.Contains(x, y))
                {
                    return true;
                }
            }

            foreach (var config in IOSystemConfiguration.MessageConfigurations)
            {
                if (config.Position.Contains(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        private void RenderMessages(MessageConfiguration messageConfiguration, ISystemContainer systemContainer)
        {
            var MessageConsole = Consoles[messageConfiguration];

            MessageConsole.Clear();

            var messages = systemContainer.MessageSystem.RecentMessages(messageConfiguration.NumberOfMessages);
            messages.Reverse();

            int y = messageConfiguration.Position.Height-1;
            foreach (Message message in messages)
            {
                MessageConsole.Print(0, y--, 1, message.Text, message.Color.ToRLColor(), null, MessageConsole.Width);
            }

            RLConsole.Blit(MessageConsole, 0, 0, MessageConsole.Width, MessageConsole.Height, Console, messageConfiguration.Position.Left, messageConfiguration.Position.Top);
        }

        private void RenderStats(StatsConfiguration statsConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            //throw new NotImplementedException();
            //var statsConsole = Consoles[statsConfiguration];

            //statsConsole.Clear();

            //var player = systemContainer.PlayerSystem.Player;
            //int line = 1;

            //foreach(StatsDisplay display in statsConfiguration.Displays)
            //{
            //    var statsDisplayer = statsDisplayers.Single(s => s.DisplayType == display.ControlType);
            //    statsDisplayer.Display(statsConsole, display, systemContainer, player, playerFov, ref line);
            //}

            //RLConsole.Blit(statsConsole, 0, 0, statsConsole.Width, statsConsole.Height, Console, statsConfiguration.Position.Left, statsConfiguration.Position.Top);
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

        private void CalculateLines()
        {
            byte connectTop = 1;
            byte connectRight = 2;
            byte connectBottom = 4;
            byte connectLeft = 8;

            var mapping = new Dictionary<byte, char>
            {
                {0, (char)254 },
                {1, (char)186 },
                {2, (char)205 },
                {3, (char)200 },
                {4, (char)186 },
                {5, (char)186 },
                {6, (char)201 },
                {7, (char)204 },
                {8, (char)205 },
                {9, (char)188 },
                {10, (char)205 },
                {11, (char)202 },
                {12, (char)187 },
                {13, (char)185 },
                {14, (char)203 },
                {15, (char)206 },
            };

            var lines = new bool[Console.Width, Console.Height];
            _lineChars = new char?[Console.Width, Console.Height];

            for (int x = 0; x < Console.Width; x++)
            {
                for (int y = 0; y < Console.Height; y++)
                {
                    lines[x, y] = !IsInSubconsole(x, y);
                }
            }

            for (int x = 0; x < Console.Width; x++)
            {
                for (int y = 0; y < Console.Height; y++)
                {
                    if (lines[x, y])
                    {
                        byte tileConnects = 0;
                        if (x != 0 && lines[x - 1, y]) tileConnects |= connectLeft;
                        if (y != 0 && lines[x, y - 1]) tileConnects |= connectTop;
                        if (y != Console.Height - 1 && lines[x, y + 1]) tileConnects |= connectBottom;
                        if (x != Console.Width - 1 && lines[x + 1, y]) tileConnects |= connectRight;

                        _lineChars[x, y] = mapping[tileConnects];
                    }
                    else
                    {
                        _lineChars[x, y] = null;
                    }
                }
            }
        }
    }
}
