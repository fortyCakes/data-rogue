using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.Utils;
using System.Linq;

namespace data_rogue_core.Maps.MapGenCommands
{
    public class GenerateShopCommandExecutor : ICommandExecutor
    {
        public string CommandType => AdditionalMapGenCommandType.GenerateShop;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset)
        {
            var coordinate = new MapCoordinate(map.MapKey, offset + command.Vector);

            var itemList = systemContainer.ItemSystem.GetSpawnableItems();

            itemList = itemList.Where(i => !i.Has<Wealth>()).ToList();

            var shop = new EnchantedItemShopGenerator().GenerateShop(systemContainer, systemContainer.Random.Between(4, 10), int.Parse(command.Parameters), itemList);

            systemContainer.PositionSystem.SetPosition(shop, coordinate);
        }
    }
}
