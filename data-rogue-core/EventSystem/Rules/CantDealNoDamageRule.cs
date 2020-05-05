using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class CantDealNoDamageRule : IEventRule
    {
        public CantDealNoDamageRule(ISystemContainer systemContainer)
        {

        }

        public EventTypeList EventTypes => new EventTypeList {EventType.Damage};
        public EventRuleType RuleType => EventRuleType.BeforeEvent;
        public uint RuleOrder => 0;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as DamageEventData;

            if (data.Damage <= 0)
            {
                return false;
            }

            return true;
        }
    }
}