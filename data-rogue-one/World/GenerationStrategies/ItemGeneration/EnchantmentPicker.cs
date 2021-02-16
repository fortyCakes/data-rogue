using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.Components;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    public class EnchantmentPicker
    {
        private List<IEntity> _enchantments;

        public EnchantmentPicker(List<IEntity> enchantments)
        {
            _enchantments = enchantments;
        }

        public IEntity PickEnchantmentFor(IEntity item, ItemClass itemClass, int enchantPower, IRandom random)
        {
            return random.PickOne(_enchantments);
        }
    }
}