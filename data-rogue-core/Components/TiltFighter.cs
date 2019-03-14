using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class TiltFighter : IEntityComponent
    {
        public Counter Tilt;
        public int BrokenTicks = 0;
    }
}
