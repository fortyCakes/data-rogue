using data_rogue_core.EntityEngine;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ITimeSystem : ISystem
    {
        void Tick();

        bool WaitingForInput { get; set; }
        ulong CurrentTime { get; set; }

        void SpendTicks(IEntity entity, int ticks);

        string TimeString { get; }
    }
}