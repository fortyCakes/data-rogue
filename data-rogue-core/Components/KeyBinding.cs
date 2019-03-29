using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;

namespace data_rogue_core.Components
{

    public class KeyBinding : IEntityComponent
    {
        public KeyCombination Key;
        public string Action;
    }
}
