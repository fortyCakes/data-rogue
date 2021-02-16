using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Components;

namespace data_rogue_core.World.GenerationStrategies
{
    public class RandomItemGenerator : IItemGenerator
    {
        private ISystemContainer systemContainer;
        private List<IEntity> itemList;

        public RandomItemGenerator(ISystemContainer systemContainer, List<IEntity> itemList)
        {
            this.systemContainer = systemContainer;
            this.itemList = itemList.Where(i => !i.Get<Item>().DoNotGenerate).ToList();
        }

        public IEntity GenerateItem(List<IEntity> itemList, int itemLevel, IRandom random)
        {
            var randomItem = random.PickOne(itemList);

            return systemContainer.PrototypeSystem.Get(randomItem);
        }
    }
}