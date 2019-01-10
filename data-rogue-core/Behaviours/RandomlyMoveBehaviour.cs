using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    public class RandomlyMoveBehaviour : IBehaviour
    {
        private readonly IPositionSystem _positionSystem;
        private readonly IEventRuleSystem _eventRuleSystem;
        private readonly IRandom _random;

        public RandomlyMoveBehaviour(IPositionSystem positionSystem, IEventRuleSystem eventRuleSystem, IRandom random)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _random = random;
        }

        public BehaviourResult Act(IEntity entity)
        {
            var isXMove = _random.PickOne(new List<bool>{true, false});
            var move = _random.PickOne(new List<int> {-1, 1});

            var vector = new Vector(isXMove ? move : 0, !isXMove ? move : 0);

            var ok = _eventRuleSystem.Try(EventType.Move, entity, vector);
            if (ok)
            {
                _positionSystem.Move(entity.Get<Position>(), vector);
                _eventRuleSystem.Try(EventType.SpendTime, entity, 1000);
            }

            return new BehaviourResult();
        }
    }
}