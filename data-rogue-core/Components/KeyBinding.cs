using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class KeyBinding : IEntityComponent
    {
        public KeyCombination Key;
        public string Action;
    }
}
