using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class TestBehaviour : BaseBehaviour
    {
        public string Memory;

        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IMapSystem _mapSystem;

        public TestBehaviour(IPositionSystem positionSystem, IEventSystem eventRuleSystem, IPlayerSystem playerSystem, IMapSystem mapSystem)
        {
            _positionSystem = positionSystem;
            _eventRuleSystem = eventRuleSystem;
            _playerSystem = playerSystem;
            _mapSystem = mapSystem;
        }

        public override BehaviourResult Act(IEntity entity)
        {
            var position = entity.Get<Position>().MapCoordinate;
            var playerPosition = _positionSystem.PositionOf(_playerSystem.Player);

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

                    _eventRuleSystem.Try(EventType.Move, entity, vector);
                }

                return new BehaviourResult();
            }
            else
            {
                return new BehaviourResult{Acted = false};
            }
        }
    }
}