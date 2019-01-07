using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    class TimeSystem : BaseSystem, ITimeSystem
    {
        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public ulong CurrentTime { get; set; }

        private readonly bool _waitingForInput;
        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Timeable)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();
        public void Tick()
        {
            
        }

        public bool WaitingForInput => _waitingForInput;

    }
}