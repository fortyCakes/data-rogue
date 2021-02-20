using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_one.Components;
using data_rogue_one.Utils;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    public class ItemNamer
    {
        private List<RareItemAdjective> _adjectives;
        private List<RareItemClassNoun> _classNouns;

        public ItemNamer(ISystemContainer systemContainer)
        {
            _adjectives = LoadAdjectives(systemContainer);
            _classNouns = LoadClassNouns(systemContainer);
        }

        public void NameMagicItem(IEntity item, IEntity enchantment1, IEntity enchantment2)
        {
            var baseName = item.DescriptionName;
            var prefix = enchantment1.Get<EnchantmentGeneration>().Prefix + " ";
            var suffix = "";
            if (enchantment2 != null)
            {
                suffix = " " + enchantment2.Get<EnchantmentGeneration>().Suffix;
            }

            Description description = item.Get<Description>();
            description.Name = $"{prefix}{baseName}{suffix}";
            description.Color = Color.FromArgb(0x59, 0x7D, 0xCE);

        }

        public void NameRareItem(IEntity item, ItemClass itemClass, IRandom random)
        {
            var adjective = GetAdjective(random);
            var classNoun = GetClassNoun(itemClass, random);

            Description description = item.Get<Description>();
            description.Name = $"{adjective.Adjective} {classNoun.Noun}";
            description.Color = Color.FromArgb(0xDA, 0xD4, 0x5E);
        }

        private RareItemClassNoun GetClassNoun(ItemClass itemClass, IRandom random)
        {
            return random.PickOne(_classNouns.Where(c => c.ItemClass == itemClass).ToList());
        }

        private RareItemAdjective GetAdjective(IRandom random)
        {
            return random.PickOne(_adjectives);
        }

        private List<RareItemAdjective> LoadAdjectives(ISystemContainer systemContainer)
        {
            var entity = systemContainer.PrototypeSystem.Get("RareItemAdjectives");

            return entity.Components.OfType<RareItemAdjective>().ToList();
        }

        private List<RareItemClassNoun> LoadClassNouns(ISystemContainer systemContainer)
        {
            var entity = systemContainer.PrototypeSystem.Get("RareItemClassNouns");

            return entity.Components.OfType<RareItemClassNoun>().ToList();
        }
    }
}