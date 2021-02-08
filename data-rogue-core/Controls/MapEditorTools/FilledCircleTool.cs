using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Controls.MapEditorTools
{

    public class FilledCircleTool : BaseFilledShapeTool
    {
        public override IEntity Entity => new Entity(0, "FilledCircleTool",
            new IEntityComponent[] {
                new Description {Name = "Filled Circle Tool", Detail = "A tool for drawing filled circles." },
                new Appearance { Glyph = 'c' },
                new SpriteAppearance { Bottom = "filled_circle_tool" }
            });

        protected override IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            return CircleTool.GetCircleCells(firstCoordinate, mapCoordinate);
        }

        protected override IEnumerable<MapCoordinate> GetInternalCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            throw new NotImplementedException();

            //var onCircleCells = GetAffectedCoordinates(firstCoordinate, mapCoordinate).ToList();

            //var minX = Math.Min(firstCoordinate.X, mapCoordinate.X);
            //var maxX = Math.Max(firstCoordinate.X, mapCoordinate.X);
            //var minY = Math.Min(firstCoordinate.Y, mapCoordinate.Y);
            //var maxY = Math.Max(firstCoordinate.Y, mapCoordinate.Y);

            //double xRadius = (maxX - minX) / 2;
            //double yRadius = (maxY - minY) / 2;
            //double centerX = minX + xRadius + 0.5;
            //double centerY = minY + yRadius + 0.5;

            //if (minX <= maxX && minY <= maxY)
            //{
            //    for (int x = minX; x <= maxX; x++)
            //    {
            //        for (int y = minY; y <= maxY; y++)
            //        {
            //            var normalisedX = x - centerX;
            //            var normalisedY = y - centerY;

            //            var normalisedDistance = (normalisedX * normalisedX) / (xRadius * xRadius) + (normalisedY * normalisedY) / (yRadius * yRadius);

            //            var coordinate = new MapCoordinate(firstCoordinate.Key, x, y);

            //            if (normalisedDistance <= 1 && !onCircleCells.Contains(coordinate))
            //            {
            //                yield return coordinate;
            //            }
            //        }
            //    }
            //}
        }
    }
}