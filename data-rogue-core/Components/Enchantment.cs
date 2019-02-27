using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Components
{
    public abstract class Enchantment : IEntityComponent
    {
        public string Name;
    }

    public class StatBoostEnchantment : Enchantment
    {
        public Stat Stat;
        public decimal Value;
    }
}
