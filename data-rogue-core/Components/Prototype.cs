using data_rogue_core.EntitySystem;

namespace data_rogue_core.Components
{
    class Prototype : IEntityComponent
    {
        public bool Singleton = true;
        public string Name;
    }
}
