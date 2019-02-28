using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;

namespace data_rogue_core.Components
{

    public class ProcEnchantment : Enchantment
    {
        public string ScriptName;
        public EventType EventType;
        public int ProcChance;
    }
}
