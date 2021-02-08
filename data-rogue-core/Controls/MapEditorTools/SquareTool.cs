using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class SquareTool : BaseShapeTool {

        public override IEntity Entity => new Entity(0, "SquareTool",
            new IEntityComponent[] {
                new Description {Name = "Square Tool", Detail = "A tool for drawing squares." },
                new Appearance { Glyph = 'S' },
                new SpriteAppearance { Bottom = "square_tool" }
            });
        
        protected override IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            return GetSquareCells(FirstCoordinate, mapCoordinate);
        }

        public static IEnumerable<MapCoordinate> GetSquareCells(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            var coordinates = new List<MapCoordinate>();

            var minX = Math.Min(firstCoordinate.X, mapCoordinate.X);
            var maxX = Math.Max(firstCoordinate.X, mapCoordinate.X);
            var minY = Math.Min(firstCoordinate.Y, mapCoordinate.Y);
            var maxY = Math.Max(firstCoordinate.Y, mapCoordinate.Y);

            coordinates.AddRange(HorizontalLine(firstCoordinate.Key, minX, maxX, minY));
            coordinates.AddRange(HorizontalLine(firstCoordinate.Key, minX, maxX, maxY));
            coordinates.AddRange(VerticalLine(firstCoordinate.Key, minX, minY, maxY));
            coordinates.AddRange(VerticalLine(firstCoordinate.Key, maxX, minY, maxY));

            return coordinates;
        }
    }
}