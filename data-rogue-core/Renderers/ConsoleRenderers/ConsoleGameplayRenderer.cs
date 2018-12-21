using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Renderers.ConsoleRenderers
{
    public class ConsoleGameplayRenderer : BaseConsoleRenderer, IGameplayRenderer
    {
        public ConsoleGameplayRenderer(RLConsole console) : base(console)
        {
        }

        public void Render(WorldState world, IPositionSystem positionSystem)
        {
            Console.Clear();

            if (ReferenceEquals(world, null) || ReferenceEquals(positionSystem, null))
            {
                return;
            }

            var currentMap = world.Maps[world.CameraPosition.Key];
            var cameraX = world.CameraPosition.X;
            var cameraY = world.CameraPosition.Y;

            MapCoordinate playerPosition = world.Player.Get<Position>().MapCoordinate;
            var playerFov = currentMap.FovFrom(playerPosition, 9);
            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

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

            // TODO REMOVE AND PUT IN ACTUAL CONSOLES
            Console.Print(0, 0, Game.WorldState.CameraPosition.Key.Key, RLColor.Black, RLColor.White);
        }

        private void DrawCell(int x, int y, IPositionSystem positionSystem, Map currentMap, int lookupX, int lookupY, List<MapCoordinate> playerFov)
        {
            MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);

            var isInFov = playerFov.Contains(coordinate);

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

            Console.Set(x, y, foreColor, backColor, appearance.Glyph);
        }
    }
}
