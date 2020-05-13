using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyModifiedDamageRule : IEventRule
    {
        public ApplyModifiedDamageRule(ISystemContainer systemContainer)
        {
            EventRuleSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        private IEventSystem EventRuleSystem { get; }
        public IMessageSystem MessageSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            var modifier = sender.TryGet<ModifiedDamage>();

            if (modifier != null)
            {
                data.Damage += modifier.By;
            }

            return true;
        }
    }
}