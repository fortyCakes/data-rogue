﻿using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class MoveToPlayerBehaviour : BaseBehaviour
    {
        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IMapSystem _mapSystem;

        public MoveToPlayerBehaviour(IPositionSystem positionSystem, IEventSystem eventRuleSystem, IPlayerSystem playerSystem, IMapSystem mapSystem)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _playerSystem = playerSystem;
            _mapSystem = mapSystem;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var position = entity.Get<Position>().MapCoordinate;
            var playerPosition = _positionSystem.PositionOf(_playerSystem.Player);

            if (Math.Abs(position.X - playerPosition.X) > 9 || Math.Abs(position.Y - playerPosition.Y) > 9)
            {
                return null;
            }

            var monsterFov = _mapSystem.MapCollection[position.Key].FovFrom(position, 9);

            if (monsterFov.Contains(playerPosition))
            {

                var path = _positionSystem.Path(position, playerPosition);

                if (path != null)
                {
                    var targetPosition = path.First();
                    var vector = new Vector(0, 0);

                    if (targetPosition.X > position.X) vector.X = 1;
                    if (targetPosition.X < position.X) vector.X = -1;
                    if (targetPosition.Y > position.Y) vector.Y = 1;
                    if (targetPosition.Y < position.Y) vector.Y = -1;

                    return new ActionEventData { Action = ActionType.Move, Parameters = vector.ToString() };
                }
            }

            return null;
        }
    }
}