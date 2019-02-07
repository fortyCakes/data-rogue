using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Behaviours
{
    public class PlayerControlledBehaviour : BaseBehaviour
    {
        public override BehaviourResult Act(IEntity entity)
        {
            return new BehaviourResult {WaitForInput = true};
        }
    }
}