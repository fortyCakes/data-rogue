using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class CircleTool : BaseShapeTool
    {
        public override IEntity Entity => new Entity(0, "CircleTool",
            new IEntityComponent[] {
                new Description {Name = "Circle Tool", Detail = "A tool for drawing circles." },
                new Appearance { Glyph = 'O' },
                new SpriteAppearance { Bottom = "circle_tool" }
            });

        protected override IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            return GetCircleCells(FirstCoordinate, mapCoordinate);
        }

        public static IEnumerable<MapCoordinate> GetCircleCells(MapCoordinate firstCoordinate, MapCoordinate secondCoordinate)
        {
            

            var x0 = Math.Min(firstCoordinate.X, secondCoordinate.X);
            var x1 = Math.Max(firstCoordinate.X, secondCoordinate.X);
            var y0 = Math.Min(firstCoordinate.Y, secondCoordinate.Y);
            var y1 = Math.Max(firstCoordinate.Y, secondCoordinate.Y);

            int xDiameter = Math.Abs(x1 - x0);
            int yDiameter = Math.Abs(y1 - y0);
            int b1 = yDiameter & 1; 
            double deltaX = 4 * (1 - xDiameter) * yDiameter * yDiameter, dy = 4 * (b1 + 1) * xDiameter * xDiameter;
            double err = deltaX + dy + b1 * xDiameter * xDiameter;
            double error2;

            y0 += (yDiameter + 1) / 2; y1 = y0 - b1;
            xDiameter *= 8 * xDiameter; b1 = 8 * yDiameter * yDiameter;

            do
            {
                yield return new MapCoordinate(firstCoordinate.Key, x1, y0);
                yield return new MapCoordinate(firstCoordinate.Key, x0, y0);
                yield return new MapCoordinate(firstCoordinate.Key, x0, y1);
                yield return new MapCoordinate(firstCoordinate.Key, x1, y1);
                error2 = 2 * err;
                if (error2 <= dy) { y0++; y1--; err += dy += xDiameter; } 
                if (error2 >= deltaX || 2 * err > dy) { x0++; x1--; err += deltaX += b1; }
            } while (x0 <= x1);

            while (y0 - y1 < yDiameter)
            {
                yield return new MapCoordinate(firstCoordinate.Key, x0 - 1, y0);
                yield return new MapCoordinate(firstCoordinate.Key, x1 + 1, y0++);
                yield return new MapCoordinate(firstCoordinate.Key, x0 - 1, y1);
                yield return new MapCoordinate(firstCoordinate.Key, x1 + 1, y1--);
            }
    
        }
    }
}