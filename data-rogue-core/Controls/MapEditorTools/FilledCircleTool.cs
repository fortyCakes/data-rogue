using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Utils;
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

        protected override IEnumerable<MapCoordinate> GetInternalAffected(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            var algorithm = new BresenhamLineDrawingAlgorithm();

            var onCircleCells = GetAffectedCoordinates(firstCoordinate, mapCoordinate).ToList();

            var midPoint = new MapCoordinate(firstCoordinate.Key, (firstCoordinate.X + mapCoordinate.X) / 2, (firstCoordinate.Y + mapCoordinate.Y) / 2);

            Func<MapCoordinate, bool> canFillInto = (coordinate) => { return !onCircleCells.Contains(coordinate); };

            return FillTool.FloodFill(midPoint, canFillInto);
        }
    }
}