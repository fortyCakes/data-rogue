using System;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    class EntityCommandExecutor : ICommandExecutor
    {
        public MapGenCommandType CommandType => MapGenCommandType.Entity;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offsetVector)
        {
            var coordinate = new MapCoordinate(map.MapKey, offsetVector + command.Vector);

            systemContainer.PrototypeSystem.CreateAt(command.Parameters, coordinate);
        }
    }
}
