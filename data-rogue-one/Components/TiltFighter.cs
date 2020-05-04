using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Components
{
    public class TiltFighter : IEntityComponent, ITickUpdate
    {
        public Counter Tilt;
        public int BrokenTicks = 0;
        public void Tick(ISystemContainer systemContainer, IEntity entity, ulong currentTime)
        {
            if (BrokenTicks > 0)
            {
                BrokenTicks--;
                if (BrokenTicks == 0)
                {
                    Tilt.Current /= 2;
                }

                return;
            }

            if (Tilt.Current > 0 && IsTiltTick(currentTime))
            {
                Tilt.Current--;
            }
        }

        private static bool IsTiltTick(ulong currentTime)
        {
            return currentTime % 1000 == 0;
        }
    }
}
