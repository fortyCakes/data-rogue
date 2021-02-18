using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.World.GenerationStrategies;
using data_rogue_one.Components;
using data_rogue_one.Utils;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    public class EnchantedItemGenerator : IItemGenerator
    {
        private ISystemContainer _systemContainer;
        private List<IEntity> _itemList;
        private ItemNamer _itemNamer;
        private EnchantmentPicker _enchantmentPicker;
        private Dictionary<ItemClass, List<IEntity>> _itemsByClass;

        public EnchantedItemGenerator(ISystemContainer systemContainer, List<IEntity> itemList)
        {
            _systemContainer = systemContainer;
            _itemList = itemList.Where(i => !i.Get<Item>().DoNotGenerate).ToList();

            _itemNamer = new ItemNamer(_systemContainer);
            _enchantmentPicker = new EnchantmentPicker(_systemContainer.EntityEngine.AllEntities.Where(e => e.Has<EnchantmentGeneration>()).ToList());

            _itemsByClass = ItemClassHelper.SplitItemsByClass(itemList);
        }

        public IEntity GenerateItem(List<IEntity> itemList, int itemLevel, IRandom random)
        {
            var itemClassPicker = random.WeightedPickOne(ItemClassPicker.ItemClassPickers, ic => ic.Weight);
            var rarityPicker = random.WeightedPickOne(RarityPicker.RarityPickers, r => r.Weight);

            return CreateItem(itemLevel, random, itemClassPicker, rarityPicker);
        }

        public IEntity CreateItem(int itemLevel, IRandom random, ItemClassPicker itemClassPicker, RarityPicker rarityPicker)
        {
            var itemClass = itemClassPicker.ItemClass;

            var itemsInClass = _itemsByClass[itemClass];

            var selectedItem = random.PickOne(itemsInClass);
            var item = _systemContainer.PrototypeSystem.Get(selectedItem);

            ApplyItemClassSpecificChanges(item, itemClass, itemLevel, random);

            if (IsEnchantable(item, itemClass))
            {
                Enchant(item, itemClass, itemLevel, random, rarityPicker);
            }

            return item;
        }

        public void Enchant(IEntity item, ItemClass itemClass, int itemLevel, IRandom random, RarityPicker rarityPicker)
        {
            var rarity = rarityPicker.Rarity;

            if (rarity == "Normal")
            {
                return;
            }

            if (rarity == "Magic")
            {
                SetEnchantedLineInDescription(item);

                if (random.PickOneFrom(1, 2) == 1)
                {
                    var enchantPower = (int)(itemLevel * 1.5);

                    var enchantment1 = GetEnchantmentFor(item, itemClass, enchantPower, random);

                    ApplyEnchantmentTo(item, enchantment1);

                    _itemNamer.NameMagicItem(item, enchantment1, null);
                }
                else
                {
                    var enchantPower = itemLevel;

                    var enchantment1 = GetEnchantmentFor(item, itemClass, enchantPower, random);

                    ApplyEnchantmentTo(item, enchantment1);

                    var enchantment2 = GetEnchantmentFor(item, itemClass, enchantPower, random);

                    ApplyEnchantmentTo(item, enchantment2);

                    _itemNamer.NameMagicItem(item, enchantment1, enchantment2);
                }

                return;
            }

            if (rarity == "Rare")
            {
                var numberOfEnchantments = random.Between(4, 6);
                var enchantPower = itemLevel;

                for (int i = 0; i < numberOfEnchantments; i++)
                {
                    var enchantment = GetEnchantmentFor(item, itemClass, enchantPower, random);

                    ApplyEnchantmentTo(item, enchantment);
                }

                _itemNamer.NameRareItem(item, itemClass, random);

                return;
            }

            throw new ApplicationException("Unknown rarity in Enchant: " + rarity);
        }

        private void SetEnchantedLineInDescription(IEntity item)
        {
            var description = item.Get<Description>();
            description.Detail = description.Detail + Environment.NewLine + Environment.NewLine + "Enchantments:";
        }

        private void ApplyEnchantmentTo(IEntity item, IEntity enchantment)
        {
            var enchantComponents = enchantment.Components.OfType<Enchantment>();

            foreach(var enchantComponent in enchantComponents)
            {
                AddComponentToItem(item, enchantComponent);
            }

            var enchantGeneration = enchantment.Get<EnchantmentGeneration>();

            UpdateItemDescription(item, enchantGeneration);

        }

        private void UpdateItemDescription(IEntity item, EnchantmentGeneration enchantGeneration)
        {
            var description = item.Get<Description>();

            description.Detail = description.Detail + Environment.NewLine + enchantGeneration.DescriptionLine;
        }

        private void AddComponentToItem(IEntity item, Enchantment enchantComponent)
        {
            var clone = ComponentSerializer.Deserialize(_systemContainer, ComponentSerializer.Serialize(enchantComponent, 0), 0);
            _systemContainer.EntityEngine.AddComponent(item, enchantComponent);
        }

        private IEntity GetEnchantmentFor(IEntity item, ItemClass itemClass, int enchantPower, IRandom random)
        {
            var enchantment = _enchantmentPicker.PickEnchantmentFor(item, itemClass, enchantPower, random);

            return enchantment;
        }

        private bool IsEnchantable(IEntity item, ItemClass itemClass)
        {
            switch (itemClass)
            {
                case ItemClass.Junk:
                case ItemClass.Wealth:
                case ItemClass.Potion:
                case ItemClass.SkillBook:
                case ItemClass.Ammunition:
                    return false;
            }

            return true;
        }

        private void ApplyItemClassSpecificChanges(IEntity item, ItemClass itemClass, int itemLevel, IRandom random)
        {
            switch(itemClass)
            {
                case ItemClass.Wealth:
                    RandomiseWealthAmount(item, itemLevel, random);
                    break;
                case ItemClass.SkillBook:
                    SetSkillBook(item, itemLevel, random);
                    break;
            }
        }

        private void SetSkillBook(IEntity item, int itemLevel, IRandom random)
        {
            throw new NotImplementedException();
        }

        private void RandomiseWealthAmount(IEntity item, int itemLevel, IRandom random)
        {
            var wealth = item.Get<Wealth>();

            var minAmount = 1 + itemLevel * wealth.Amount;
            var maxAmount = 20 + itemLevel * wealth.Amount * 2;

            wealth.Amount = random.Between(minAmount, maxAmount);
        }
    }
}