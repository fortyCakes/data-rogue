using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

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

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, ISystemContainer systemContainer)
        {
            Action<IEntity> action = (entityToAdd) => { AddEntityCommandToMap(map, mapCoordinate, entityToAdd); };

            var entities = systemContainer.EntityEngine.AllEntities.Where(e => e.Has<CanAddToMap>());

            var entityCreationActivity = new EntityPickerMenuActivity(entities, systemContainer, "Pick an entity to add", action);

            systemContainer.ActivitySystem.ActivityStack.Push(entityCreationActivity);
        }

        private void AddEntityCommandToMap(IMap map, MapCoordinate mapCoordinate, IEntity entityToAdd)
        {
            var prototypeName = entityToAdd.Get<Prototype>().Name;

            var entityCommand = new MapGenCommand { MapGenCommandType = MapGenCommandType.Entity, Parameters = prototypeName, Vector = mapCoordinate.ToVector() };
            map.AddCommand(entityCommand);
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            return new List<MapCoordinate> { mapCoordinate };
        }

        public virtual IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate) => new List<MapCoordinate>();
    }
}