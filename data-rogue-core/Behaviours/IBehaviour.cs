using data_rogue_core.EntityEngine;

namespace data_rogue_core.Behaviours
{
    public interface IBehaviour : IEntityComponent
    {
        int BehaviourPriority { get; }

        BehaviourResult Act(IEntity entity);
    }
}
