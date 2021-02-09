using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class FilledSquareTool : BaseFilledShapeTool
    {
        public override IEntity Entity => new Entity(0, "FilledSquareTool",
            new IEntityComponent[] {
                new Description {Name = "Filled Square Tool", Detail = "A tool for drawing filled squares." },
                new Appearance { Glyph = 's' },
                new SpriteAppearance { Bottom = "filled_square_tool" }
            });

        protected override IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            return SquareTool.GetSquareCells(firstCoordinate, mapCoordinate);
        }

        protected override IEnumerable<MapCoordinate> GetInternalAffected(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            var minX = Math.Min(firstCoordinate.X, mapCoordinate.X) + 1;
            var maxX = Math.Max(firstCoordinate.X, mapCoordinate.X) - 1;
            var minY = Math.Min(firstCoordinate.Y, mapCoordinate.Y) + 1;
            var maxY = Math.Max(firstCoordinate.Y, mapCoordinate.Y) - 1;

            if (minX <= maxX && minY <= maxY)
            {
                for (int x = minX; x <= maxX; x++)
                    for (int y = minY; y <= maxY; y++)
                        yield return new MapCoordinate(firstCoordinate.Key, x, y);
            }
            else
            {
                yield break;
            }
        }
    }
}