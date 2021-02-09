using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace data_rogue_core.Maps.MapGenCommands
{
    public class EntityCommandExecutor : ICommandExecutor
    {
        public MapGenCommandType CommandType => MapGenCommandType.Entity;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset)
        {
            var coordinate = new MapCoordinate(map.MapKey, offset + command.Vector);
            List<string> splits;
            string entityName = GetEntityName(command, out splits);

            var entity = systemContainer.PrototypeSystem.CreateAt(entityName, coordinate);

            splits.Remove(splits.First());

            var regexParse = new Regex("(.*)\\.(.*)=(.*)");

            foreach (var entityParameterUpdate in splits)
            {
                var results = regexParse.Match(entityParameterUpdate);

                var componentName = results.Groups[1].Value;
                var fieldName = results.Groups[2].Value;
                var value = results.Groups[3].Value;

                var component = entity.Get(componentName);

                ComponentSerializer.BindSingleValue(component, fieldName, value);
            }
        }

        public static string GetEntityName(MapGenCommand command, out List<string> splits)
        {
            splits = command.Parameters.Split('|').ToList();
            return splits[0];
        }
    }
}
