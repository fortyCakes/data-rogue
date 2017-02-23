using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Display;
using data_rogue_core.Interfaces;
using RLNET;

namespace data_rogue_core.Map
{
    static class ConsoleMapRenderer
    {
        public static void RenderMap(int viewpointX, int viewpointY, RLConsole console, DungeonMap map, IEnumerable<IDrawable> drawables)
        {
            var consoleWidth = console.Width;
            var consoleHeight = console.Height;

            var cellsLeft = Math.Max(0, viewpointX - consoleWidth/2);
            var cellsRight = Math.Min(map.Width, cellsLeft + consoleWidth);
            if (cellsRight >= map.Width) cellsLeft = map.Width - consoleWidth -1;

            var cellsTop = Math.Max(0, viewpointY - consoleHeight/2);
            var cellsBottom = Math.Min(map.Height, cellsTop + consoleHeight);
            if (cellsBottom >= map.Height) cellsTop = map.Height - consoleHeight -1;

            for (int x = 0; x <= consoleWidth; x++)
            {
                for (int y = 0; y <= consoleHeight; y++)
                {
                    SetConsoleSymbolForCell(console, map, map.GetCell(cellsLeft + x, cellsTop + y), cellsLeft, cellsTop);
                }
            }

            foreach (IDrawable drawable in drawables)
            {
                drawable.Draw(console, map, cellsLeft, cellsTop);
            }
        }

        private static void SetConsoleSymbolForCell(RLConsole console, DungeonMap map, DungeonCell cell, int xOffset, int yOffset)
        {
            // When we haven't explored a cell yet, we don't want to draw anything
            if (!cell.IsExplored)
            {
                return;
            }

            // When a cell is currently in the field-of-view it should be drawn with ligher colors
            if (map.IsInFov(cell.X, cell.Y))
            {
                // Choose the symbol to draw based on if the cell is walkable or not
                // '.' for floor and '#' for walls
                if (cell.IsWalkable)
                {
                    console.Set(cell.X - xOffset, cell.Y - yOffset, Colors.FloorFov, Colors.FloorBackgroundFov, cell.Symbol);
                }
                else
                {
                    console.Set(cell.X - xOffset, cell.Y - yOffset, Colors.WallFov, Colors.WallBackgroundFov, cell.Symbol);
                }
            }
            // When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X - xOffset, cell.Y - yOffset, Colors.Floor, Colors.FloorBackground, cell.Symbol);
                }
                else
                {
                    console.Set(cell.X - xOffset, cell.Y - yOffset, Colors.Wall, Colors.WallBackground, cell.Symbol);
                }
            }
        }
    }
}
