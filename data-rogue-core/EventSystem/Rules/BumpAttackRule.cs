using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class BumpAttackRule : IEventRule
    {
        public BumpAttackRule(ISystemContainer systemContainer)
        {
            PositionSystem = systemContainer.PositionSystem;
            FighterSystem = systemContainer.FighterSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public int RuleOrder => 1;

        public IPositionSystem PositionSystem { get; }
        private IFighterSystem FighterSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (IsFighter(sender))
            {
                var vector = (Vector) eventData;
                var targetCoordinate = sender.Get<Position>().MapCoordinate + vector;

                var entitiesAtPosition = PositionSystem.EntitiesAt(targetCoordinate);

                if (entitiesAtPosition.Any(IsFighter))
                {
                    var defender = entitiesAtPosition.Single(e => IsFighter(e));

                    FighterSystem.BasicAttack(sender, defender);
                    return false;
                }
            }

            return true;
        }

        private static bool IsFighter(IEntity e)
        {
            return e.Has<Fighter>();
        }
    }
}
