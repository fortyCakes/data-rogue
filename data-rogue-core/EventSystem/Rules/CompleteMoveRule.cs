using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.EventSystem.Rules
{
    public class CompleteMoveRule : IEventRule
    {
        private readonly IPositionSystem positionSystem;
        private readonly IEventSystem eventRuleSystem;
        private readonly IAnimatedMovementSystem _animatedMovementSystem;

        public CompleteMoveRule(ISystemContainer systemContainer)
        {
            positionSystem = systemContainer.PositionSystem;
            eventRuleSystem = systemContainer.EventSystem;
            _animatedMovementSystem = systemContainer.AnimatedMovementSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.EventResolution;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var vector = (Vector)eventData;

            positionSystem.Move(sender, vector);
            _animatedMovementSystem.StartAnimatedMovement(sender, vector, 500);

            eventRuleSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData(){Ticks = sender.Get<Actor>().Speed});

            return true;
        }
    }
}
