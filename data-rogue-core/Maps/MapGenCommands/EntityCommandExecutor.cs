using data_rogue_core.Components;
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

    class EntityStackCommandExecutor : ICommandExecutor
    {
        public MapGenCommandType CommandType => MapGenCommandType.EntityStack;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offsetVector)
        {
            var coordinate = new MapCoordinate(map.MapKey, offsetVector + command.Vector);

            var splits = command.Parameters.Split(',');
            var entityName = splits[0];
            var stackSize = int.Parse(splits[1]);

            var entity = systemContainer.PrototypeSystem.CreateAt(entityName, coordinate);
            entity.Get<Stackable>().StackSize = stackSize;
        }
    }
}
