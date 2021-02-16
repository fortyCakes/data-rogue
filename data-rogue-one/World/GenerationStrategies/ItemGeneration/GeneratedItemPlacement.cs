using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.World.GenerationStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    class GeneratedEnchantedItemPlacement : RandomItemPlacement
    {
        protected override IItemGenerator GetItemGenerator(ISystemContainer systemContainer, List<IEntity> itemList)
        {
            return new EnchantedItemGenerator(systemContainer, itemList);
        }
    }
}
