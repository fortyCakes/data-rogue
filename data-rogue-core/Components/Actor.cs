using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Components
{
    public class Actor : IEntityComponent
    {
        public ulong NextTick;
        public ulong Speed = 1000;
    }
}
