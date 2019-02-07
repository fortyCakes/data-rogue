using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Prototype : IEntityComponent
    {
        public bool Singleton = true;
        public string Name;
    }
}
