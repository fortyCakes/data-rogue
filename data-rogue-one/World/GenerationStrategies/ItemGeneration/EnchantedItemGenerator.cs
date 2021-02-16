using System.Collections.Generic;
using data_rogue_core;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.World.GenerationStrategies;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    internal class EnchantedItemGenerator : IItemGenerator
    {
        private ISystemContainer systemContainer;
        private List<IEntity> itemList;

        public EnchantedItemGenerator(ISystemContainer systemContainer, List<IEntity> itemList)
        {
            this.systemContainer = systemContainer;
            this.itemList = itemList;
        }

        public IEntity GenerateItem(List<IEntity> itemList, int itemLevel, IRandom random)
        {
            throw new System.NotImplementedException();
        }
    }
}