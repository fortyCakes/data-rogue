using System.Collections.Generic;

namespace data_rogue_one.World.GenerationStrategies.ItemGeneration
{
    public class RarityPicker
    {
        public string Rarity;
        public int Weight;
        public static RarityPicker Normal => new RarityPicker { Rarity = "Normal", Weight = 1000 };
        public static RarityPicker Magic => new RarityPicker { Rarity = "Magic", Weight = 1000 };
        public static RarityPicker Rare => new RarityPicker { Rarity = "Rare", Weight = 50 };

        public static List<RarityPicker> RarityPickers = new List<RarityPicker>
        {
            Normal,
            Magic,
            Rare,
        };
    }
}