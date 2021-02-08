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

        public static IEnumerable<MapCoordinate> GetCircleCells(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            throw new NotImplementedException();
        }
    }
}