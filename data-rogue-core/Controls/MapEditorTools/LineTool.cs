using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Utils;
using System.Collections.Generic;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class LineTool: BaseShapeTool
    {
        public override IEntity Entity => new Entity(0, "LineTool",
            new IEntityComponent[] {
                new Description {Name = "Line Tool", Detail = "A tool for drawing lines." },
                new Appearance { Glyph = '\\' },
                new SpriteAppearance { Bottom = "line_tool" }
            });

        protected override IEnumerable<MapCoordinate> GetAffectedCoordinates(MapCoordinate firstCoordinate, MapCoordinate mapCoordinate)
        {
            var algorithm = new BresenhamLineDrawingAlgorithm();

            return algorithm.DrawLine(firstCoordinate, mapCoordinate);
        }
    }
}