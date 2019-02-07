using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Behaviours
{
    public interface IBehaviour : IEntityComponent
    {
        int BehaviourPriority { get; }

        BehaviourResult Act(IEntity entity);
    }
}
