using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem.Rules
{
    class PhsyicalCollisionRule : IEventRule
    {
        public PhsyicalCollisionRule(IPositionSystem positionSystem)
        {
            PositionSystem = positionSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public int RuleOrder => 0;

        private IPositionSystem PositionSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (IsSolid(sender))
            {
                var vector = (Vector) eventData;
                var targetCoordinate = sender.Get<Position>().MapCoordinate + vector;

                var entitiesAtPosition = PositionSystem.EntitiesAt(targetCoordinate);

                if (entitiesAtPosition.Any(IsSolid))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsSolid(IEntity e)
        {
            return e.Get<Physical>()?.Passable == false;
        }
    }
}
