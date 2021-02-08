using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Controls.MapEditorTools
{
    public class EntityTool : IMapEditorTool
    {
        public IEntity Entity => new Entity(0, "EntityTool",
            new IEntityComponent[] {
                new Description {Name = "Entitly Tool", Detail = "A tool for setting the entity on a tile." },
                new Appearance { Glyph = 'E' },
                new SpriteAppearance { Bottom = "entity_tool" }
            });

        public bool RequiresClick => true;

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell)
        {
            // TODO: Trigger IEntity selector dialogue on click.
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            return new List<MapCoordinate> { mapCoordinate };
        }
    }
}