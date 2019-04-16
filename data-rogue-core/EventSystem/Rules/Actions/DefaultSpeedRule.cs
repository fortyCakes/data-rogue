using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class DefaultSpeedRule : IEventRule
    {
        public DefaultSpeedRule(ISystemContainer systemContainer)
        {

        }

        private const int DEFAULT_SPEED = 1000;

        public EventTypeList EventTypes => new EventTypeList {EventType.Action};
        public uint RuleOrder => int.MaxValue;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;
        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as ActionEventData;

            if (data != null && !data.Speed.HasValue)
            {
                data.Speed = DEFAULT_SPEED;
            }

            return true;
        }
    }
}