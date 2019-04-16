using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EventSystem.Rules
{
    public class FollowPathBehaviour : BaseBehaviour
    {
        private IPositionSystem _positionSystem;
        private IEventSystem _eventRuleSystem;
        private IPlayerSystem _playerSystem;
        private IMapSystem _mapSystem;
        private IEntityEngine _entityEngine;
        private IMessageSystem _messageSystem;
        private ITimeSystem _timeSystem;
        public List<MapCoordinate> Path = new List<MapCoordinate>();

        public FollowPathBehaviour(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventRuleSystem = systemContainer.EventSystem;
            _playerSystem = systemContainer.PlayerSystem;
            _mapSystem = systemContainer.MapSystem;
            _entityEngine = systemContainer.EntityEngine;
            _messageSystem = systemContainer.MessageSystem;
            _timeSystem = systemContainer.TimeSystem;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var currentCoordinate = _positionSystem.CoordinateOf(entity);

            if (Path.FirstOrDefault() == currentCoordinate)
            {
                Path.Remove(currentCoordinate);
            }

            if (Path.Any())
            {
                SetWaitingForInputIfPlayer(entity, false);

                if (_playerSystem.Player == entity && _eventRuleSystem.GetStat(entity, "Tension") > 0)
                {
                    _messageSystem.Write("You can't auto-walk when tension is this high.");
                    EndPath(entity);
                    return null;
                }

                var coordinate = Path.First();
                var vector = currentCoordinate - coordinate;

                return new ActionEventData { Action = ActionType.Move, Parameters = vector.ToString() };
            }
            else
            {
                EndPath(entity);
                return null;
            }
        }

        private void SetWaitingForInputIfPlayer(IEntity entity, bool setTo)
        {
            if (entity == _playerSystem.Player)
            {
                _timeSystem.WaitingForInput = setTo;
            }
        }

        private void EndPath(IEntity entity)
        {
            _entityEngine.RemoveComponent(entity, this);
            SetWaitingForInputIfPlayer(entity, true);
        }
    }
}