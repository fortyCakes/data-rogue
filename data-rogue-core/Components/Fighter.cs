using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Fighter : IEntityComponent
    {
        public Counter Health;
        public Counter Tilt;
        public Counter Aura;
        public int BaseAura;

        public int Muscle;
        public int Agility;

        public int Willpower;
        public int Intellect;

        public int BrokenTicks = 0;
    }
}
