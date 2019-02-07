using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    class Actor : IEntityComponent
    {
        public ulong NextTick;
        public bool HasActed;
    }
}
