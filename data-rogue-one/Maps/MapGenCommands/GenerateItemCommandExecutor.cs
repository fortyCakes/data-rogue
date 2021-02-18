using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.World.GenerationStrategies.ItemGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace data_rogue_core.Maps.MapGenCommands
{
    public class GenerateItemCommandExecutor : ICommandExecutor
    {
        public string CommandType => AdditionalMapGenCommandType.GenerateItem;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset)
        {
            var coordinate = new MapCoordinate(map.MapKey, offset + command.Vector);

            List<IEntity> itemList = systemContainer.ItemSystem.GetSpawnableItems();

            var itemGenerator = new EnchantedItemGenerator(systemContainer, itemList);
            var item = itemGenerator.GenerateItem(itemList, 1, systemContainer.Random);

            systemContainer.PositionSystem.SetPosition(item, new MapCoordinate(map.MapKey, offset + command.Vector));
        }
    }
}
