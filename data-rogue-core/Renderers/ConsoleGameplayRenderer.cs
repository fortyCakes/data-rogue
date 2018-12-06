﻿using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.Extensions;
using RLNET;
using System.Linq;

namespace data_rogue_core.Renderers
{
    public class ConsoleGameplayRenderer
    {
        public static void Render(RLConsole console, WorldState world)
        {
            var currentMap = world.CurrentMap;
            var player = world.Player;
            var playerPosition = player.Get<Position>().MapCoordinate;

            var consoleWidth = console.Width;
            var consoleHeight = console.Height;

            int offsetX = consoleWidth / 2;
            int offsetY = consoleHeight / 2;

            console.Clear();

            for (int y = 0; y < consoleHeight; y++) 
            {
                for (int x = 0; x < consoleWidth; x++)
                {
                    var lookupX = playerPosition.X - offsetX + x;
                    var lookupY = playerPosition.Y - offsetY + y;

                    var appearance = world.PositionSystem
                        .EntitiesAt(new MapCoordinate ( currentMap.MapKey, lookupX, lookupY))
                        .Select(e => e.Get<Appearance>())
                        .OrderByDescending(a => a.ZOrder)
                        .FirstOrDefault();

                    if (appearance == null)
                    {
                        var mapCell = currentMap.CellAt(lookupX, lookupY);
                        appearance = mapCell.Get<Appearance>();
                    }

                    console.Set(x, y, appearance.Color.ToRLColor(), RLColor.Black, appearance.Glyph);
                }
            }
        }
    }
}
