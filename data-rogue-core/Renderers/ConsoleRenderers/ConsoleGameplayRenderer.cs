using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleGameplayRenderer : BaseConsoleRenderer, IGameplayRenderer
    {
        public const int STATS_WIDTH = 22;
        public const int MESSAGE_HEIGHT = 15;

        private RLConsole MapConsole { get; set; }
        private RLConsole StatsConsole { get; set; }
        private RLConsole MessageConsole { get; set; }

        public ConsoleGameplayRenderer(RLConsole console) : base(console)
        {
            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

            MapConsole = new RLConsole(consoleWidth - STATS_WIDTH - 1, consoleHeight - MESSAGE_HEIGHT - 1);
            StatsConsole = new RLConsole(STATS_WIDTH, consoleHeight - 1);
            MessageConsole = new RLConsole(consoleWidth - STATS_WIDTH - 1, MESSAGE_HEIGHT);

        }

        public void Render(ISystemContainer systemContainer)
        {
            Console.Clear();

            if (ReferenceEquals(systemContainer?.PlayerSystem?.Player, null))
            {
                return;
            }

            RenderMap(systemContainer, out var playerFov);

            RenderStats(systemContainer, playerFov);

            RenderMessages(systemContainer);

            RenderLines(systemContainer.PlayerSystem.Player.Get<TiltFighter>().BrokenTicks > 0);
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            if (IsOnMap(x, y))
            {
                var lookupX = cameraPosition.X - MapConsole.Width / 2 + x;
                var lookupY = cameraPosition.Y - MapConsole.Height / 2 + y;

                return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
            }

            return null;
        }

        private bool IsOnMap(int x, int y)
        {
            var mapWidth = MapConsole.Width;
            var mapHeight = MapConsole.Height;
            return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
        }

        private void RenderLines(bool alert)
        {
            var foreColor = alert? RLColor.Red : RLColor.White;
            var backColor = RLColor.Black;

            for (int x = 0; x < Console.Width - STATS_WIDTH - 1; x++)
            {
                Console.Set(x, Console.Height - MESSAGE_HEIGHT - 1, foreColor, backColor, 196);
            }
            for (int y = 0; y < Console.Height; y++)
            {
                Console.Set(Console.Width - STATS_WIDTH - 1, y, foreColor, backColor, 179);
            }

            Console.Set(Console.Width - STATS_WIDTH - 1, Console.Height - MESSAGE_HEIGHT - 1, foreColor, backColor, 180);
        }

        private void RenderMessages(ISystemContainer systemContainer)
        {
            MessageConsole.Clear();

            var messages = systemContainer.MessageSystem.RecentMessages(15);
            messages.Reverse();

            int y = 14;
            foreach (Message message in messages)
            {
                MessageConsole.Print(0, y--, 1, message.Text, message.Color.ToRLColor(), null, MessageConsole.Width);
            }

            RLConsole.Blit(MessageConsole, 0, 0, MessageConsole.Width, MessageConsole.Height, Console, 0, Console.Height - 15);
        }

        private void RenderStats(ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            StatsConsole.Clear();

            var player = systemContainer.PlayerSystem.Player;
            var fighter = player.Get<Health>();
            var tilt = player.Get<TiltFighter>();
            var aura = player.Get<AuraFighter>();

            StatsConsole.Print(1,1, player.Get<Description>().Name, RLColor.White, RLColor.Black);
            StatsConsole.Print(1,2, " the Adventurer", RLColor.White, RLColor.Black);

            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 4, STATS_WIDTH - 2, "hp", fighter.HP, RLColor.Red);
            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 6, STATS_WIDTH - 2, "aura", aura.Aura, RLColor.Yellow);
            var tension = systemContainer.EventSystem.GetStat(player, "Tension");
            StatsConsole.Print(1, 7, $"Tension: {tension}", RLColor.White, RLColor.Black);


            if (tilt.BrokenTicks > 0)
            {
                StatsConsole.Print(1, 9, $" DEFENCE BREAK {((decimal)tilt.BrokenTicks/100).ToString("F2")} ", RLColor.White, RLColor.Red);
            }
            else
            {

                ConsoleRendererHelper.PrintBar(StatsConsole, 1, 9, STATS_WIDTH - 2, "tilt", tilt.Tilt, RLColor.Magenta);
            }

            ConsoleRendererHelper.PrintStat(StatsConsole, 1, 10, "Armour", (int)systemContainer.EventSystem.GetStat(player, "AC"), RLColor.White);
            ConsoleRendererHelper.PrintStat(StatsConsole, 1, 11, "Evasion", (int)systemContainer.EventSystem.GetStat(player, "EV"), RLColor.White);
            ConsoleRendererHelper.PrintStat(StatsConsole, 1, 12, "Block", (int)systemContainer.EventSystem.GetStat(player, "SH"), RLColor.White);

            var currentAegis = (int)systemContainer.EventSystem.GetStat(player, "CurrentAegisLevel");
            var maxAegis = (int)systemContainer.EventSystem.GetStat(player, "Aegis");

            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 13, STATS_WIDTH - 2, "Aegis", new Counter { Current = currentAegis, Max = maxAegis }, RLColor.LightBlue);

            StatsConsole.Print(1, 15, "Location:", RLColor.White, RLColor.Black);

            var mapname = systemContainer.PositionSystem.CoordinateOf(player).Key.Key;
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            StatsConsole.Print(1, 16, mapname, RLColor.White, RLColor.Black);

            StatsConsole.Print(1, 18, $"Time: {systemContainer.TimeSystem.TimeString}", RLColor.White, RLColor.Black);

            StatsConsole.Print(1, 20, $"Gold: {systemContainer.ItemSystem.CheckWealth(player, "Gold")}", Color.Gold.ToRLColor());

            StatsConsole.Print(1, 21, $"Skills:", RLColor.White, RLColor.Black);

            var skillsToPrint = player.Components.OfType<KnownSkill>().Where(s => s.Order > 0).OrderBy(s => s.Order).Take(5).ToList();

            for(int i = 0; i < skillsToPrint.Count(); i++)
            {
                var skillName = systemContainer.PrototypeSystem.Get(skillsToPrint[i].Skill).Get<Description>().Name;
                StatsConsole.Print(1, 21 + i, $"{i+1}: {skillName}", RLColor.White, RLColor.Black);
            }

            var hoveredCoordinate = systemContainer.PlayerControlSystem.HoveredCoordinate;

            if  (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                ConsoleRendererHelper.DisplayEntitySummary(StatsConsole, 0, 50, hoveredEntity);
            }


            RLConsole.Blit(StatsConsole, 0, 0, StatsConsole.Width, StatsConsole.Height, Console, Console.Width - 22, 0);
        }
        

        private void RenderMap(ISystemContainer systemContainer, out List<MapCoordinate> playerFov)
        {
            MapConsole.Clear();

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            playerFov = currentMap.FovFrom(playerPosition, 9);
            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

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

                    MapRendererHelper.DrawCell(MapConsole, x, y, systemContainer.PositionSystem, currentMap, lookupX, lookupY, playerFov);
                }
            }

            RLConsole.Blit(MapConsole, 0, 0, MapConsole.Width, MapConsole.Height, Console, 0, 0);
        }
    }
}
