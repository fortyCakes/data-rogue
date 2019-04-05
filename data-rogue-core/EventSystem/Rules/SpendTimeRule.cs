using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class SpendTimeRule : IEventRule
    {
        public SpendTimeRule(ISystemContainer systemContainer)
        {
            TimeSystem = systemContainer.TimeSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.SpendTime };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;

        private ITimeSystem TimeSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var spendTimeEventData = (SpendTimeEventData) eventData;

            TimeSystem.SpendTicks(sender, spendTimeEventData.Ticks);
            
            if (sender.Has<PlayerControlledBehaviour>())
            {
                TimeSystem.WaitingForInput = false;
            }

            return true;
        }
    }
}
