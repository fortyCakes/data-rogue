using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class CompleteMoveRule : IEventRule
    {
        private readonly IPositionSystem positionSystem;
        private readonly IEventSystem eventRuleSystem;

        public CompleteMoveRule(ISystemContainer systemContainer)
        {
            positionSystem = systemContainer.PositionSystem;
            eventRuleSystem = systemContainer.EventSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public int RuleOrder => int.MinValue;
        

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            positionSystem.Move(sender.Get<Position>(), (Vector)eventData);
            eventRuleSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData(){Ticks = 1000});

            return true;
        }
    }
}
