using System;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
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
                    //case RLKey.L:
                    //    Game.ActivityStack.Push(new StaticTextActivity(@"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", Game.RendererFactory));
                    //    break;
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