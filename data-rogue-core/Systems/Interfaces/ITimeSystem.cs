using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ITimeSystem : ISystem
    {
        void Tick();

        bool WaitingForInput { get; set; }
        ulong CurrentTime { get; set; }

        void SpendTicks(IEntity entity, ulong ticks);

        string TimeString { get; }
    }
}