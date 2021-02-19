using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.World.GenerationStrategies.ItemGeneration;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_one.Utils
{
    public class EnchantedItemShopGenerator : BaseShopGenerator
    {
        public override IEnumerable<IEntity> GenerateShopItems(ISystemContainer systemContainer, int numberOfItems, int itemLevel, IEnumerable<IEntity> itemList)
        {
            var itemGenerator = new EnchantedItemGenerator(systemContainer, itemList.ToList());

            var rarities = new List<RarityPicker> { RarityPicker.Magic, RarityPicker.Rare };

            for (int i = 0; i < numberOfItems; i++)
            {
                yield return itemGenerator.TunedGenerateItem(itemList.ToList(), itemLevel, systemContainer.Random, rarities);
            }
        }
    }
}
