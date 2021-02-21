using data_rogue_core;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_one.Utils
{
    public abstract class BaseShopGenerator : IShopGenerator
    {
        public abstract IEnumerable<IEntity> GenerateShopItems(ISystemContainer systemContainer, int numberOfItems, int itemLevel, IEnumerable<IEntity> itemList, IRandom random);

        public IEntity GenerateShop(ISystemContainer systemContainer, int numberOfItems, int itemLevel, IEnumerable<IEntity> itemList, IRandom random)
        {
            var shopItems = GenerateShopItems(systemContainer, numberOfItems, itemLevel, itemList, random);

            var shop = systemContainer.PrototypeSystem.Get("Props:Shop");

            var inventory = shop.Get<Inventory>();

            foreach (var item in shopItems)
            {
                systemContainer.ItemSystem.AddItemDirectlyToInventory(item, inventory);
            }

            return shop;
        }

    }
}
