using System.Collections.Generic;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    internal partial class EnchantedItemGenerator
    {
        public class RarityPicker
        {
            public string Rarity;
            public int Weight;

            public static List<RarityPicker> RarityPickers = new List<RarityPicker>
            {
                new RarityPicker { Rarity = "Normal", Weight = 1000 },
                new RarityPicker { Rarity = "Magic", Weight = 1000 },
                new RarityPicker { Rarity = "Rare", Weight = 50 },
            };
        }
    }
}