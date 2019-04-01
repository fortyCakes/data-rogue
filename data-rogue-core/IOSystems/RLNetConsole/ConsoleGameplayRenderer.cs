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

            RenderLines(systemContainer.PlayerSystem.Player.Get<TiltFighter>().BrokenTicks > 0);
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

        private void RenderLines(bool alert)
        {
            var foreColor = alert? RLColor.Red : RLColor.White;
            var backColor = RLColor.Black;

            // Work out line renderering later
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
                DisplayStatsItem(statsConsole, display, systemContainer, player, playerFov, ref line);
            }

            /*
             * 
            statsConsole.Print(1, 21, $"Skills:", RLColor.White, RLColor.Black);

            var skillsToPrint = player.Components.OfType<KnownSkill>().Where(s => s.Order > 0).OrderBy(s => s.Order).Take(5).ToList();

            for(int i = 0; i < skillsToPrint.Count(); i++)
            {
                var skillName = systemContainer.PrototypeSystem.Get(skillsToPrint[i].Skill).Get<Description>().Name;
                statsConsole.Print(1, 21 + i, $"{i+1}: {skillName}", RLColor.White, RLColor.Black);
            }

            var hoveredCoordinate = systemContainer.PlayerControlSystem.HoveredCoordinate;

            if  (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                ConsoleRendererHelper.DisplayEntitySummary(statsConsole, 0, 50, hoveredEntity);
            }*/

            RLConsole.Blit(statsConsole, 0, 0, statsConsole.Width, statsConsole.Height, Console, statsConfiguration.Position.Left, statsConfiguration.Position.Top);
        }

        private void DisplayStatsItem(RLConsole console, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            switch(display.DisplayType)
            {
                case DisplayType.Name:
                    console.Print(1, line, player.Get<Description>().Name, display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.Title:
                    console.Print(1, line, " the Untitled", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.Location:
                    var mapname = systemContainer.PositionSystem.CoordinateOf(player).Key.Key;
                    if (mapname.StartsWith("Branch:"))
                    {
                        mapname = mapname.Substring(7);
                    }

                    console.Print(1, line, "Location:", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    console.Print(1, line, mapname, display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.Time:
                    console.Print(1, line, $"Time: {systemContainer.TimeSystem.TimeString}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.Wealth:
                    var wealthType = display.Parameters;
                    console.Print(1, line, $"{wealthType}: {systemContainer.ItemSystem.CheckWealth(player, wealthType)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.Spacer:
                    line++;
                    break;
                case DisplayType.Stat:
                    var statName = display.Parameters;
                    console.Print(1, line, $"{statName}: {systemContainer.EventSystem.GetStat(player, statName)}", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                case DisplayType.ComponentCounter:
                    var componentCounterSplits = display.Parameters.Split(',');
                    var componentName = componentCounterSplits[0];
                    var counterName = componentCounterSplits[1];

                    var component = player.Get(componentName);
                    FieldInfo[] fields = component.GetType().GetFields();
                    var field = fields.Single(f => f.Name == counterName);
                    var counter = (Counter)field.GetValue(component);

                    ConsoleRendererHelper.PrintBar(console, 1, line, console.Width - 2, counterName, counter, display.BackColor.ToRLColor());

                    line++;
                    break;
                case DisplayType.StatInterpolation:
                    var interpolationSplits = display.Parameters.Split(',');
                    var format = interpolationSplits[0];

                    var statValues = interpolationSplits.Skip(1).Select(s => systemContainer.EventSystem.GetStat(player, s).ToString()).ToArray();

                    var interpolated = string.Format(format, statValues);
                    console.Print(1, line, interpolated, display.Color.ToRLColor(), display.BackColor.ToRLColor());

                    line++;
                    break;
                case DisplayType.VisibleEnemies:
                    console.Print(1, line, "TODO: ENEMIES", display.Color.ToRLColor(), display.BackColor.ToRLColor());
                    line++;
                    break;
                default:
                    throw new Exception("Unknown DisplayType in DisplayStatsItem");
            }
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
