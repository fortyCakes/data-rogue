using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{

    public class EntityStackCommandExecutor : ICommandExecutor
    {
        public string CommandType => MapGenCommandType.EntityStack;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset)
        {
            var coordinate = new MapCoordinate(map.MapKey, offset + command.Vector);

            var splits = command.Parameters.Split(',');
            var entityName = splits[0];
            var stackSize = int.Parse(splits[1]);

            var entity = systemContainer.PrototypeSystem.CreateAt(entityName, coordinate);
            entity.Get<Stackable>().StackSize = stackSize;
        }
    }
}
