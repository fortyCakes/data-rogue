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
        private IEventSystem _eventSystem;
        private IPositionSystem _positionSystem;

        public BumpAttackRule(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventSystem = systemContainer.EventSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.Move };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (IsFighter(sender))
            {
                var vector = (Vector) eventData;
                var targetCoordinate = _positionSystem.CoordinateOf(sender) + vector;

                var entitiesAtPosition = _positionSystem.EntitiesAt(targetCoordinate);

                if (entitiesAtPosition.Any(IsFighter))
                {
                    var defender = entitiesAtPosition.Single(e => IsFighter(e));

                    var action = new ActionEventData { Action = ActionType.MeleeAttack, Parameters = $"{sender.EntityId},{defender.EntityId}", Speed = null, KeyPress = null };

                    _eventSystem.Try(EventType.Action, sender, action);

                    return false;
                }
            }

            return true;
        }

        private static bool IsFighter(IEntity e)
        {
            return e.Has <Health>();
        }
    }
}
