using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using data_rogue_one.Utils;

namespace data_rogue_one.World.GenerationStrategies
{

    public class ShopPlacement : BaseEntityGenerationStrategy
    {
        private class ItemList: Dictionary<int, HashSet<IEntity>> { }

        public int ItemLevel = 0;

        public override void Generate(ISystemContainer systemContainer, GeneratedBranch generatedBranch, IEntity branch, IRandom random, IProgress<string> progress)
        {
            progress.Report("Placing shops");

            var itemLevel = BasePower;

            var itemList = systemContainer.ItemSystem.GetSpawnableItems();

            itemList = itemList.Where(i => !i.Has<Wealth>()).ToList();

            var shopGenerator = new EnchantedItemShopGenerator();

            foreach (var map in generatedBranch.Maps)
            {
                FillMap(map, systemContainer, itemLevel, itemList, random, shopGenerator);

                itemLevel += PowerIncrement;
            }
        }

        private void FillMap(IMap map, ISystemContainer systemContainer, int power, List<IEntity> itemList, IRandom random, IShopGenerator shopGenerator)
        {
            var mapSize = map.Cells.Count;

            var numberOfItems = Density * mapSize;

            int integerItems = (int)Math.Floor(numberOfItems);
            var fractionalItems = numberOfItems - integerItems;

            if (random.ZeroToOne() < (double)fractionalItems)
            {
                integerItems++;
            }

            numberOfItems = integerItems;

            for (int i = 0; i < numberOfItems; i++)
            {
                SpawnShop(map, systemContainer, power, itemList, random, shopGenerator);
            }
        }

        private void SpawnShop(IMap map, ISystemContainer systemContainer, int power, List<IEntity> itemList, IRandom random, IShopGenerator shopGenerator)
        {
            int retries = 25;

            MapCoordinate emptyLocation = null;
            IEntity shop = null;

            emptyLocation = map.GetEmptyPosition(systemContainer.PositionSystem, random);

            if (emptyLocation == null) return;

            for (int i = 0; i < retries; i++)
            {
                var itemLevel = random.Between(0, power + 2);
                shop = shopGenerator.GenerateShop(systemContainer, random.Between(4, 10), itemLevel, itemList, random);

                if (shop != null) break;
            }

            if (shop == null) return;

            systemContainer.PositionSystem.SetPosition(shop, emptyLocation);
        }
    }
}
