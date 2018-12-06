using data_rogue_core.EntitySystem;

namespace data_rogue_core.Components
{
    public class Physical : IEntityComponent
    {
        public Physical(bool passable, bool transparent)
        {
            Passable = passable;
            Transparent = transparent;
        }

        public bool Passable;
        public bool Transparent;
    }
}
