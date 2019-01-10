using data_rogue_core.EntityEngine;

namespace data_rogue_core.Behaviours
{
    public interface IBehaviour
    {
        BehaviourResult Act(IEntity entity);
    }
}
