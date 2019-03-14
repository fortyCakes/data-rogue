using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Item : IEntityComponent
    {
        public string UseScript;
        public int ItemLevel = 0;
    }
}
