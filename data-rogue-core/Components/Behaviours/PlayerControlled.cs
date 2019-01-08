using data_rogue_core.Components.Behaviours;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    public class PlayerControlled : IEntityComponent, IBehaviour
    {
        public BehaviourResult Act()
        {
            return new BehaviourResult { WaitForInput = true };
        }
    }
}
