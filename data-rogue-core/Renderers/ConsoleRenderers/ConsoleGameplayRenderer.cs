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

                    MapCoordinate coordinate = new MapCoordinate ( currentMap.MapKey, lookupX, lookupY);

                    var appearance = positionSystem
                        .EntitiesAt(coordinate)
                        .Select(e => e.Get<Appearance>())
                        .OrderByDescending(a => a.ZOrder)
                        .First();

                    var backColor = playerFov.Contains(coordinate) ? new RLColor(128,128,0) : RLColor.Black;

                    Console.Set(x, y, appearance.Color.ToRLColor(), backColor, appearance.Glyph);
                }
            }

            // TODO REMOVE
            Console.Print(0, 0, Game.WorldState.CameraPosition.Key.Key, RLColor.Black, RLColor.White);
        }
    }
}
