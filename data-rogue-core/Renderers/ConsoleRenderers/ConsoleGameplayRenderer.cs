using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleGameplayRenderer : BaseConsoleRenderer, IGameplayRenderer
    {
        private const bool DEBUG_SEEALL = false;
        private const int STATS_WIDTH = 22;
        private const int MESSAGE_HEIGHT = 15;

        private RLConsole MapConsole { get; set; }
        private RLConsole StatsConsole { get; set; }
        private RLConsole MessageConsole { get; set; }

        public ConsoleGameplayRenderer(RLConsole console) : base(console)
        {
            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

            MapConsole = new RLConsole(consoleWidth - STATS_WIDTH - 1, consoleHeight - MESSAGE_HEIGHT - 1);
            StatsConsole = new RLConsole(STATS_WIDTH, consoleHeight - MESSAGE_HEIGHT - 1);
            MessageConsole = new RLConsole(consoleWidth, MESSAGE_HEIGHT);

        }

        public void Render(WorldState world, IPositionSystem positionSystem, IMessageSystem messageSystem)
        {
            Console.Clear();

            if (ReferenceEquals(world, null) || ReferenceEquals(positionSystem, null))
            {
                return;
            }

            RenderMap(world, positionSystem);

            RenderStats(world);

            RenderMessages(messageSystem);

            RenderLines();
        }

        private void RenderLines()
        {
            for (int x = 0; x < Console.Width - STATS_WIDTH - 1; x++)
            {
                Console.Set(x, Console.Height - MESSAGE_HEIGHT - 1, RLColor.White, RLColor.Black, 196);
            }
            for (int y = 0; y < Console.Height; y++)
            {
                Console.Set(Console.Width - STATS_WIDTH - 1, y, RLColor.White, RLColor.Black, 179);
            }

            Console.Set(Console.Width - STATS_WIDTH - 1, Console.Height - MESSAGE_HEIGHT - 1, RLColor.White, RLColor.Black, 180);
        }

        private void RenderMessages(IMessageSystem messageSystem)
        {
            MessageConsole.Clear();

            var messages = messageSystem.RecentMessages(15);
            messages.Reverse();

            int y = 14;
            foreach (Message message in messages)
            {
                MessageConsole.Print(0, y--, 1, message.Text, message.Color.ToRLColor(), null, MessageConsole.Width);
            }

            RLConsole.Blit(MessageConsole, 0, 0, MessageConsole.Width, MessageConsole.Height, Console, 0, Console.Height - 15);
        }

        private void RenderStats(WorldState world)
        {
            StatsConsole.Clear();

            var player = world.Player;
            var fighter = player.Get<Fighter>();

            StatsConsole.Print(1,1, player.Name, RLColor.White, RLColor.Black);
            StatsConsole.Print(1,2, " the (TITLE)", RLColor.White, RLColor.Black);

            PrintBar(StatsConsole, 1, 4, STATS_WIDTH - 2, "hp", fighter.Health, RLColor.Red);
            PrintBar(StatsConsole, 1, 6, STATS_WIDTH - 2, "aura", fighter.Aura, RLColor.Yellow);
            PrintBar(StatsConsole, 1, 8, STATS_WIDTH - 2, "tilt", fighter.Tilt, RLColor.Magenta);

            StatsConsole.Print(1, 10, "Location:", RLColor.White, RLColor.Black);

            var mapname = player.Get<Position>().MapCoordinate.Key.Key.ToString();
            if (mapname.StartsWith("Branch:"))
            {
                mapname = mapname.Substring(7);
            }

            StatsConsole.Print(1, 11, mapname, RLColor.White, RLColor.Black);

            StatsConsole.Print(1, 13, $"Time: {world.TimeSystem.TimeString}", RLColor.White, RLColor.Black);

            RLConsole.Blit(StatsConsole, 0, 0, StatsConsole.Width, StatsConsole.Height, Console, Console.Width - 22, 0);
        }

        private void PrintBar(RLConsole console, int x, int y, int length, string name, StatCounter counter, RLColor color)
        {
            var counterStart = name.Length + 2;
            var counterText = counter.ToString();

            for (int i = 0; i < length; i++)
            {
                char glyph = ' ';

                if (i < name.Length)
                {
                    glyph = name[i];
                }
                else if (i == name.Length)
                {
                    glyph = ':';
                }
                else if (i >= counterStart && i < counterStart + counterText.Length)
                {
                    glyph = counterText[i - counterStart];
                }

                if ((decimal)i / length < (decimal)counter.Current / counter.Max)
                {
                    console.Set(x + i, y, RLColor.Black, color, glyph);
                }
                else
                {
                    console.Set(x + i, y, color, RLColor.Black, glyph);
                }

                
            }
        }

        private void RenderMap(WorldState world, IPositionSystem positionSystem)
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

                    DrawCell(x, y, positionSystem, currentMap, lookupX, lookupY, playerFov);
                }
            }

            RLConsole.Blit(MapConsole, 0, 0, MapConsole.Width, MapConsole.Height, Console, 0, 0);
        }

        private void DrawCell(int x, int y, IPositionSystem positionSystem, Map currentMap, int lookupX, int lookupY, List<MapCoordinate> playerFov)
        {
            MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);

            var isInFov = playerFov.Contains(coordinate) || DEBUG_SEEALL;

            Appearance appearance = null;

            if (isInFov)
            {
                appearance = positionSystem
                    .EntitiesAt(coordinate)
                    .Select(e => e.Get<Appearance>())
                    .OrderByDescending(a => a.ZOrder)
                    .First();
            }
            else if (currentMap.SeenCoordinates.Contains(coordinate))
            {
                appearance = currentMap.CellAt(coordinate).Get<Appearance>();
            }
            else
            {
                appearance = new Appearance()
                {
                    Color = Color.Black,
                    Glyph = ' ',
                    ZOrder = 0
                };
            }
            var foreColor = isInFov ? appearance.Color.ToRLColor() : RLColor.Gray;
            var backColor = RLColor.Black;

            MapConsole.Set(x, y, foreColor, backColor, appearance.Glyph);
        }
    }
}
