using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{

    public class RandomlyMoveBehaviour : BaseBehaviour, IEntityComponent
    {
        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IRandom _random;

        public RandomlyMoveBehaviour(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventRuleSystem = systemContainer.EventSystem;
            _random = systemContainer.Random;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var isXMove = _random.PickOne(new List<bool>{true, false});
            var move = _random.PickOne(new List<int> {-1, 1});

            var vector = new Vector(isXMove ? move : 0, !isXMove ? move : 0);

            return new ActionEventData { Action = ActionType.Move, Parameters = vector.ToString(), Speed = entity.Get<Actor>().Speed };
        }
    }
}