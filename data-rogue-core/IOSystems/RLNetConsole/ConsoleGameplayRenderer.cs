using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
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
            var fighter = player.Get<Health>();
            var tilt = player.Get<TiltFighter>();
            var aura = player.Get<AuraFighter>();

            statsConsole.Print(1,1, player.Get<Description>().Name, RLColor.White, RLColor.Black);
            statsConsole.Print(1,2, " the Adventurer", RLColor.White, RLColor.Black);

            ConsoleRendererHelper.PrintBar(statsConsole, 1, 4, statsConsole.Width - 2, "hp", fighter.HP, RLColor.Red);
            ConsoleRendererHelper.PrintBar(statsConsole, 1, 6, statsConsole.Width - 2, "aura", aura.Aura, RLColor.Yellow);
            var tension = systemContainer.EventSystem.GetStat(player, "Tension");
            statsConsole.Print(1, 7, $"Tension: {tension}", RLColor.White, RLColor.Black);


            if (tilt.BrokenTicks > 0)
            {
                statsConsole.Print(1, 9, $" DEFENCE BREAK {((decimal)tilt.BrokenTicks/100).ToString("F2")} ", RLColor.White, RLColor.Red);
            }
            else
            {

                ConsoleRendererHelper.PrintBar(statsConsole, 1, 9, statsConsole.Width - 2, "tilt", tilt.Tilt, RLColor.Magenta);
            }

            ConsoleRendererHelper.PrintStat(statsConsole, 1, 10, "Armour", (int)systemContainer.EventSystem.GetStat(player, "AC"), RLColor.White);
            ConsoleRendererHelper.PrintStat(statsConsole, 1, 11, "Evasion", (int)systemContainer.EventSystem.GetStat(player, "EV"), RLColor.White);
            ConsoleRendererHelper.PrintStat(statsConsole, 1, 12, "Block", (int)systemContainer.EventSystem.GetStat(player, "SH"), RLColor.White);

            var currentAegis = (int)systemContainer.EventSystem.GetStat(player, "CurrentAegisLevel");
            var maxAegis = (int)systemContainer.EventSystem.GetStat(player, "Aegis");

            ConsoleRendererHelper.PrintBar(statsConsole, 1, 13, statsConsole.Width - 2, "Aegis", new Counter { Current = currentAegis, Max = maxAegis }, RLColor.LightBlue);

            statsConsole.Print(1, 15, "Location:", RLColor.White, RLColor.Black);

            var mapname = systemContainer.PositionSystem.CoordinateOf(player).Key.Key;
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            statsConsole.Print(1, 16, mapname, RLColor.White, RLColor.Black);

            statsConsole.Print(1, 18, $"Time: {systemContainer.TimeSystem.TimeString}", RLColor.White, RLColor.Black);

            statsConsole.Print(1, 20, $"Gold: {systemContainer.ItemSystem.CheckWealth(player, "Gold")}", Color.Gold.ToRLColor());

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
