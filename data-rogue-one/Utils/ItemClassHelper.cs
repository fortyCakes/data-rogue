using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_one.World.GenerationStrategies.ItemGeneration;

namespace data_rogue_one.Utils
{
    public static class ItemClassHelper
    {
        public static Dictionary<ItemClass, List<IEntity>> SplitItemsByClass(List<IEntity> itemList)
        {
            var ItemsByClass = GenerateBlankItemDictionary();

            foreach(var item in itemList)
            {
                var itemClass = Classify(item);

                ItemsByClass[itemClass].Add(item);
            }

            return ItemsByClass;
        }

        public static ItemClass Classify(IEntity item)
        {
            if (item.Has<Wealth>()) return ItemClass.Wealth;

            var itemSlot = item.Get<Equipment>()?.EquipmentSlot;

            if (item.Has<Weapon>())
            {
                var weapon = item.Get<Weapon>();
                switch(weapon.Class)
                {
                    case "Heavy": return ItemClass.HeavyWeapon;
                    case "Light": return ItemClass.LightWeapon;
                    case "Blast": return ItemClass.MagicWeapon;
                    case "Launcher": return ItemClass.RangedWeapon;
                    default: throw new ApplicationException("Unknown weapon class " + weapon.Class);
                }
            }
            else if (itemSlot == EquipmentSlot.Hand)
            {
                return ItemClass.Shield;
            }

            switch(itemSlot)
            {
                case EquipmentSlot.Boots: return ItemClass.Boots;
                case EquipmentSlot.Gloves: return ItemClass.Gloves;
                case EquipmentSlot.Chest: return ItemClass.Body;
                case EquipmentSlot.Head: return ItemClass.Head;
                case EquipmentSlot.Legs: return ItemClass.Legs;
                case EquipmentSlot.Neck: return ItemClass.Amulet;
                case EquipmentSlot.Ring: return ItemClass.Ring;
            }

            if (item.Has<Ammunition>())
            {
                return ItemClass.Ammunition;
            }

            if (item.Has<Consumable>())
            {
                return ItemClass.Potion;
            }

            if (item.Get<Prototype>().Name == "Item:SkillBook") return ItemClass.SkillBook;

            if (item.DescriptionName == "Junk") return ItemClass.Junk;

            return ItemClass.Other;
        }

        private static Dictionary<ItemClass, List<IEntity>> GenerateBlankItemDictionary()
        {
            var dictionary = new Dictionary<ItemClass, List<IEntity>>();

            foreach (ItemClass itemClass in Enum.GetValues(typeof(ItemClass)))
            {
                dictionary.Add(itemClass, new List<IEntity>());
            }

            return dictionary;
        }
    }
}