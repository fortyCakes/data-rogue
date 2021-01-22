using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class AttackAnimationRule : IEventRule
    {
        private IAnimationSystem _animationSystem;

        public AttackAnimationRule(ISystemContainer systemContainer)
        {
            _animationSystem = systemContainer.AnimationSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (sender.Has<Animated>())
            {
                _animationSystem.SetAnimation(sender, AnimationType.Attack);
            }

            return true;
        }
    }
}