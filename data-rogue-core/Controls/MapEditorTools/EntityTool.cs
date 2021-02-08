using System;
using System.Collections.Generic;
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

        public void Apply(IMap map, MapCoordinate mapCoordinate, IEntity currentCell, IEntity alternateCell, IActivitySystem activitySystem)
        {
            Action<string> action = (entityToAdd) => { AddCommandToMap(map, mapCoordinate, entityToAdd); };

            var entityCreationActivity = new TextInputActivity(activitySystem, "Entity name", action);
            entityCreationActivity.InputText = "Props:Smoke";

            activitySystem.ActivityStack.Push(entityCreationActivity);
        }

        private void AddCommandToMap(IMap map, MapCoordinate mapCoordinate, string entityToAdd)
        {
            var entityCommand = new MapGenCommand { MapGenCommandType = MapGenCommandType.Entity, Parameters = entityToAdd, Vector = mapCoordinate.ToVector() };
            map.AddCommand(entityCommand);
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            return new List<MapCoordinate> { mapCoordinate };
        }
    }
}