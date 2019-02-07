using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Behaviours
{
    public abstract class BaseBehaviour : IBehaviour, IEntityComponent
    {
        public int Priority;
        public int BehaviourPriority => Priority;

        public abstract BehaviourResult Act(IEntity entity);
    }
}