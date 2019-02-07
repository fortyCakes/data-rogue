using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{

    public class RandomlyMoveBehaviour : BaseBehaviour, IEntityComponent
    {
        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IRandom _random;

        public RandomlyMoveBehaviour(IPositionSystem positionSystem, IEventSystem eventRuleSystem, IRandom random)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _random = random;
        }

        public override BehaviourResult Act(IEntity entity)
        {
            var isXMove = _random.PickOne(new List<bool>{true, false});
            var move = _random.PickOne(new List<int> {-1, 1});

            var vector = new Vector(isXMove ? move : 0, !isXMove ? move : 0);

            _eventRuleSystem.Try(EventType.Move, entity, vector);

            return new BehaviourResult();
        }
    }
}