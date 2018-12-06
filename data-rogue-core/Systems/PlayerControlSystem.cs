using System;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.EventSystem;
using RLNET;

namespace data_rogue_core.Systems
{
    public class PlayerControlSystem : BaseSystem, IPlayerControlSystem
    {
        private readonly IPositionSystem PositionSystem;
        private readonly IEventRuleSystem EventSystem;

        public PlayerControlSystem(IPositionSystem positionSystem, IEventRuleSystem eventRuleSystem)
        {
            PositionSystem = positionSystem;
            EventSystem = eventRuleSystem;
        }

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(PlayerControlled), typeof(Position) };

        public void HandleKeyPress(RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.W:
                        MoveEntities(0, -1);
                        break;
                    case RLKey.A:
                        MoveEntities(-1, 0);
                        break;
                    case RLKey.S:
                        MoveEntities(0, 1);
                        break;
                    case RLKey.D:
                        MoveEntities(1, 0);
                        break;
                }
            }
        }

        private void MoveEntities(int x, int y)
        {
            foreach (var entity in Entities)
            {
                var vector = new Vector(x, y);
                if (EventSystem.Try(EventType.Move, entity, vector))
                {
                    PositionSystem.Move(entity.Get<Position>(), vector);
                }
            }
        }
    }
}