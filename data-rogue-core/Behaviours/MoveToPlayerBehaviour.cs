using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.Behaviours
{
    internal class MoveInPlayerDirectionBehaviour : IBehaviour
    {
        private readonly IPositionSystem positionSystem;
        private readonly IEventSystem eventRuleSystem;

        public MoveInPlayerDirectionBehaviour(IPositionSystem positionSystem, IEventSystem eventRuleSystem)
        {
            this.positionSystem = positionSystem;
            this.eventRuleSystem = eventRuleSystem;
        }

        public BehaviourResult Act(IEntity entity)
        {
            var position = entity.Get<Position>().MapCoordinate;
            var playerPosition = positionSystem.PositionOf(Game.WorldState.Player);

            var monsterFov = Game.WorldState.Maps[position.Key].FovFrom(position, 9);

            if (monsterFov.Contains(playerPosition))
            {

                var path = positionSystem.Path(position, playerPosition);

                if (path != null)
                {
                    var targetPosition = path.First();
                    var vector = new Vector(0, 0);

                    if (targetPosition.X > position.X) vector.X = 1;
                    if (targetPosition.X < position.X) vector.X = -1;
                    if (targetPosition.Y > position.Y) vector.Y = 1;
                    if (targetPosition.Y < position.Y) vector.Y = -1;

                    eventRuleSystem.Try(EventType.Move, entity, vector);
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