using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class BumpAttackRule : IEventRule
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

                    var action = new ActionEventData { Action = ActionType.MeleeAttack, Parameters = $"{sender.EntityId},{defender.EntityId}", Speed = null, KeyPress = null };

                    return false;
                }
            }

            return true;
        }

        private static bool IsFighter(IEntity e)
        {
            return e.Has<TiltFighter>();
        }
    }
}
