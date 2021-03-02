using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Action<IEntity> action = (entityToAdd) => { AddCommandToMap(map, mapCoordinate, entityToAdd, systemContainer); };

            var entities = systemContainer.EntityEngine.AllEntities.Where(e => e.Has<CanAddToMap>() && e.Has<Prototype>());

            var entityCreationActivity = new EntityPickerMenuActivity(systemContainer.ActivitySystem.DefaultPosition, systemContainer.ActivitySystem.DefaultPadding, entities, systemContainer, "Pick an entity to add", action);
            entityCreationActivity.HoverPrefix = "";
            entityCreationActivity.NoCellHoverText = "(no entity selected)";

            systemContainer.ActivitySystem.Push(entityCreationActivity);
        }

        private void AddCommandToMap(IMap map, MapCoordinate mapCoordinate, IEntity entityToAdd, ISystemContainer systemContainer)
        {
            string commandType;
            string parameters;
            bool paramsIncludeEntityName;
            if (entityToAdd.Has<MapGenerationCommand>())
            {
                var component = entityToAdd.Get<MapGenerationCommand>();
                commandType = component.Command;
                parameters = component.Parameters;
                paramsIncludeEntityName = false;
            }
            else
            {
                commandType = MapGenCommandType.Entity;
                parameters = null;
                paramsIncludeEntityName = true;
            }

            var entityName = entityToAdd.Get<Prototype>().Name;

            var mapAdd = entityToAdd.Get<CanAddToMap>();

            if (mapAdd.SettableProperty != null)
            {
                Action<string> callback = (settablePropertyValue) =>
                    CompleteEntityCommand(
                        map,
                        mapCoordinate.ToVector(),
                        commandType,
                        Parameterise(entityName, mapAdd.SettableProperty, settablePropertyValue, parameters, paramsIncludeEntityName));

                var inputBox = new TextInputActivity(systemContainer.ActivitySystem, "Enter a value for property {mapAdd.SettableProperty}:", callback);

                systemContainer.ActivitySystem.Push(inputBox);
            }
            else
            {
                CompleteEntityCommand(map, mapCoordinate.ToVector(), commandType, Parameterise(entityName, null, null, parameters, paramsIncludeEntityName));
            }
        }

        private string Parameterise(string entityName, string settableProperty, string settablePropertyValue, string existingParameters, bool paramsIncludeEntityName)
        {
            var sb = new StringBuilder();
            if (paramsIncludeEntityName) sb.Append(entityName);
            if (!string.IsNullOrEmpty(existingParameters)) sb.Append($"|{existingParameters}");
            if (!string.IsNullOrEmpty(settableProperty)) sb.Append($"|{settableProperty}={settablePropertyValue}");

            return sb.ToString();
        }

        private void CompleteEntityCommand(IMap map, Vector vector, string command, string parameters)
        {
            var entityCommand = new MapGenCommand { MapGenCommandType = command, Parameters = parameters, Vector = vector };
            map.AddCommand(entityCommand);
        }

        public IEnumerable<MapCoordinate> GetTargetedCoordinates(IMap map, MapCoordinate mapCoordinate)
        {
            return new List<MapCoordinate> { mapCoordinate };
        }

        public virtual IEnumerable<MapCoordinate> GetInternalCoordinates(IMap map, MapCoordinate secondCoordinate) => new List<MapCoordinate>();
    }
}