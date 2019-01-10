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
        private readonly IEventRuleSystem eventRuleSystem;

        public MoveInPlayerDirectionBehaviour(IPositionSystem positionSystem, IEventRuleSystem eventRuleSystem)
        {
            this.positionSystem = positionSystem;
            this.eventRuleSystem = eventRuleSystem;
        }

        public BehaviourResult Act(IEntity entity)
        {
            var position = entity.Get<Position>().MapCoordinate;
            var playerPosition = positionSystem.PositionOf(Game.WorldState.Player);

            if (position.Key == playerPosition.Key)
            {
                var vector = new Vector(0, 0);

                if (playerPosition.X > position.X) vector.X = 1;
                if (playerPosition.X < position.X) vector.X = -1;
                if (playerPosition.Y > position.Y) vector.Y = 1;
                if (playerPosition.Y < position.Y) vector.Y = -1;

                eventRuleSystem.Try(EventType.Move, entity, vector);
            }

            return new BehaviourResult();
        }
    }
}