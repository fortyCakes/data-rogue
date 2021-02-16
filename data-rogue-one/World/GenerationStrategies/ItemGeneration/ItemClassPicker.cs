using System.Collections.Generic;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    internal partial class EnchantedItemGenerator
    {
        public class ItemClassPicker
        {
            public ItemClass ItemClass;
            public int Weight;
            public int MinLevel = 0;

            public static List<ItemClassPicker> ItemClassPickers = new List<ItemClassPicker>
            {
                new ItemClassPicker{ItemClass = ItemClass.Junk, Weight = 5 },
                new ItemClassPicker{ItemClass = ItemClass.Wealth, Weight = 100 },
                new ItemClassPicker{ItemClass = ItemClass.Potion, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Body, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Legs, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Head, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Boots, Weight = 5 },
                new ItemClassPicker{ItemClass = ItemClass.Gloves, Weight = 5 },
                new ItemClassPicker{ItemClass = ItemClass.LightWeapon, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.HeavyWeapon, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.RangedWeapon, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Ammunition, Weight = 40 },
                new ItemClassPicker{ItemClass = ItemClass.MagicWeapon, Weight = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Shield, Weight = 10 },
                //new ItemClassPicker{ItemClass = ItemClass.SkillBook, Weight = 5 },
                new ItemClassPicker{ItemClass = ItemClass.Ring, Weight = 3, MinLevel = 10 },
                new ItemClassPicker{ItemClass = ItemClass.Amulet, Weight = 3, MinLevel = 20 },
            };
        }
    }
}