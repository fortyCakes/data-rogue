using System;
using data_rogue_core.EntitySystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    class EntityCommandExecutor : ICommandExecutor
    {
        public MapGenCommandType CommandType => MapGenCommandType.Entity;

        public void Execute(Map map, IEntityEngine entityEngineSystem, IPrototypeSystem prototypeSystem, MapGenCommand command, Vector offsetVector)
        {
            var coordinate = new MapCoordinate(map.MapKey, offsetVector + command.Vector);

            prototypeSystem.CreateAt(command.Parameters, Guid.NewGuid().ToString(), coordinate);
        }
    }
}
