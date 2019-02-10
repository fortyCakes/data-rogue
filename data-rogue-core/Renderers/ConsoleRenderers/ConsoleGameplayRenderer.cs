using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
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

        public void Render(WorldState world, ISystemContainer systemContainer)
        {
            Console.Clear();

            if (ReferenceEquals(world, null) || ReferenceEquals(systemContainer.PositionSystem, null))
            {
                return;
            }

            RenderMap(world, systemContainer);

            RenderStats(world, systemContainer);

            RenderMessages(systemContainer);

            RenderLines(world.Player.Get<Fighter>().BrokenTicks > 0);
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(WorldState world, int x, int y)
        {
            if (IsOnMap(x, y))
            {
                var lookupX = world.CameraPosition.X - MapConsole.Width / 2 + x;
                var lookupY = world.CameraPosition.Y - MapConsole.Height / 2 + y;

                return new MapCoordinate(world.CameraPosition.Key, lookupX, lookupY);
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

        private void RenderStats(WorldState world, ISystemContainer systemContainer)
        {
            StatsConsole.Clear();

            var player = world.Player;
            var fighter = player.Get<Fighter>();

            StatsConsole.Print(1,1, player.Get<Description>().Name, RLColor.White, RLColor.Black);
            StatsConsole.Print(1,2, " the (TITLE)", RLColor.White, RLColor.Black);

            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 4, STATS_WIDTH - 2, "hp", fighter.Health, RLColor.Red);
            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 6, STATS_WIDTH - 2, "aura", fighter.Aura, RLColor.Yellow);
            ConsoleRendererHelper.PrintBar(StatsConsole, 1, 8, STATS_WIDTH - 2, "tilt", fighter.Tilt, RLColor.Magenta);

            if (fighter.BrokenTicks > 0)
            {
                StatsConsole.Print(1, 9, $" DEFENCE BREAK {((decimal)fighter.BrokenTicks/100).ToString("F2")} ", RLColor.White, RLColor.Red);
            }
            else
            {
                var tension = systemContainer.EventSystem.GetStat(player, Stat.Tension);
                StatsConsole.Print(1, 9, $"Tension: {tension}", RLColor.White, RLColor.Black);
            }

            StatsConsole.Print(1, 11, "Location:", RLColor.White, RLColor.Black);

            var mapname = player.Get<Position>().MapCoordinate.Key.Key.ToString();
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            StatsConsole.Print(1, 12, mapname, RLColor.White, RLColor.Black);

            StatsConsole.Print(1, 14, $"Time: {world.TimeSystem.TimeString}", RLColor.White, RLColor.Black);

            StatsConsole.Print(1, 16, $"Skills:", RLColor.White, RLColor.Black);

            var skillsToPrint = player.Components.OfType<KnownSkill>().Where(s => s.Order > 0).OrderBy(s => s.Order).Take(5).ToList();

            for(int i = 0; i < skillsToPrint.Count(); i++)
            {
                var skillName = systemContainer.PrototypeSystem.Create(skillsToPrint[i].Skill).Get<Description>().Name;
                StatsConsole.Print(1, 17 + i, $"{i+1}: {skillName}", RLColor.White, RLColor.Black);
            }

            if (systemContainer.PlayerControlSystem.HoveredEntity != null)
            {
                ConsoleRendererHelper.DisplayEntitySummary(StatsConsole, 0, 50, systemContainer.PlayerControlSystem.HoveredEntity);
            }


            RLConsole.Blit(StatsConsole, 0, 0, StatsConsole.Width, StatsConsole.Height, Console, Console.Width - 22, 0);
        }

        

        

        private void RenderMap(WorldState world, ISystemContainer systemContainer)
        {
            MapConsole.Clear();

            var currentMap = world.Maps[world.CameraPosition.Key];
            var cameraX = world.CameraPosition.X;
            var cameraY = world.CameraPosition.Y;

            MapCoordinate playerPosition = world.Player.Get<Position>().MapCoordinate;
            var playerFov = currentMap.FovFrom(playerPosition, 9);
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
