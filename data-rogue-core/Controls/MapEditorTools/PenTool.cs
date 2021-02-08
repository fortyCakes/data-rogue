using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class PenTool : IMapEditorTool
    {
        public IEntity Entity => new Entity(0, "PenTool",
            new IEntityComponent[] {
                new Description {Name = "Pen Tool", Detail = "A tool for drawing single tiles." },
                new Appearance { Glyph = 'P' },
                new SpriteAppearance { Bottom = "pen_tool" }
            });

        public bool RequiresClick => false;

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell)
        {
            map.SetCell(mapCoordinate, currentCell);
        }
    }
}