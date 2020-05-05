using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Behaviours
{
    public class MoveAwayFromPlayerBehaviour : BaseBehaviour
    {
        public decimal Chance = 1;

        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IMapSystem _mapSystem;
        private readonly IRandom _random;

        public MoveAwayFromPlayerBehaviour(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventRuleSystem = systemContainer.EventSystem;
            _playerSystem = systemContainer.PlayerSystem;
            _mapSystem = systemContainer.MapSystem;
            _random = systemContainer.Random;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            if (Chance >= _random.Between(1, 100) / 100M)
            {
                var position = _positionSystem.CoordinateOf(entity);
                var playerPosition = _positionSystem.CoordinateOf(_playerSystem.Player);

                if (position == null ||
                    playerPosition == null ||
                    Math.Abs(position.X - playerPosition.X) > 3 ||
                    Math.Abs(position.Y - playerPosition.Y) > 3)
                {
                    return null;
                }

                var adjacentToMe = position.AdjacentCells().Where(c => !_positionSystem.IsBlocked(c));
                var adjacentToPlayer = playerPosition.AdjacentCells();

                var validCells = adjacentToMe.Except(adjacentToPlayer).Except(new [] {playerPosition}).ToList();

                if (validCells.Any())
                {
                    var vector = _random.PickOne(validCells) - position;

                    return new ActionEventData { Action = ActionType.Move, Parameters = vector.ToString(), Speed = entity.Get<Actor>().Speed };
                }
            }

            return null;
        }
    }
}