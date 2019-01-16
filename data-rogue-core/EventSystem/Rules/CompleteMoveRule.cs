using System.Drawing;
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

        public CompleteMoveRule(IPositionSystem positionSystem, IEventSystem eventRuleSystem)
        {
            this.positionSystem = positionSystem;
            this.eventRuleSystem = eventRuleSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public int RuleOrder => int.MinValue;
        

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            positionSystem.Move(sender.Get<Position>(), (Vector)eventData);
            eventRuleSystem.Try(EventType.SpendTime, sender, 1000);

            return true;
        }
    }
}
