using data_rogue_core.Data;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    public class Fighter : IEntityComponent
    {
        public StatCounter Health;
        public StatCounter Tilt;
        public StatCounter Aura;
        public int BaseAura;

        public int Muscle;
        public int Agility;

        public int Willpower;
        public int Intellect;

        public int BrokenTicks = 0;
    }
}
