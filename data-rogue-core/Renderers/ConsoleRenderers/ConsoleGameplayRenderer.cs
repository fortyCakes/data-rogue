using System.Linq;
using data_rogue_core.Activities;
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

            var currentMap = world.CurrentMap;
            var player = world.Player;
            var playerPosition = player.Get<Position>().MapCoordinate;

            var consoleWidth = Console.Width;
            var consoleHeight = Console.Height;

            int offsetX = consoleWidth / 2;
            int offsetY = consoleHeight / 2;

            for (int y = 0; y < consoleHeight; y++) 
            {
                for (int x = 0; x < consoleWidth; x++)
                {
                    var lookupX = playerPosition.X - offsetX + x;
                    var lookupY = playerPosition.Y - offsetY + y;

                    var appearance = positionSystem
                        .EntitiesAt(new MapCoordinate ( currentMap.MapKey, lookupX, lookupY))
                        .Select(e => e.Get<Appearance>())
                        .OrderByDescending(a => a.ZOrder)
                        .First();

                    Console.Set(x, y, appearance.Color.ToRLColor(), RLColor.Black, appearance.Glyph);
                }
            }
        }
    }
}
