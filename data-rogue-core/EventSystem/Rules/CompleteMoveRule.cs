using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class CompleteMoveRule : IEventRule
    {
        private readonly IPositionSystem positionSystem;
        private readonly IEventSystem eventRuleSystem;

        public CompleteMoveRule(ISystemContainer systemContainer)
        {
            positionSystem = systemContainer.PositionSystem;
            eventRuleSystem = systemContainer.EventSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            positionSystem.Move(sender, (Vector)eventData);
            eventRuleSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData(){Ticks = sender.Get<Actor>().Speed});

            return true;
        }
    }
}
