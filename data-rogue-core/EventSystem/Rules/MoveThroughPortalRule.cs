using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class MoveThroughPortalRule : IEventRule
    {
        private IEventSystem _eventSystem;
        private IPositionSystem _positionSystem;

        public MoveThroughPortalRule(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventSystem = systemContainer.EventSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Move };
        public uint RuleOrder => 1;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var vector = (Vector)eventData;
            var targetCoordinate = _positionSystem.CoordinateOf(sender);

            var entitiesAtPosition = _positionSystem.EntitiesAt(targetCoordinate);

            if (entitiesAtPosition.Any(IsAutoPortal))
            {
                var portal = entitiesAtPosition.Single(e => IsAutoPortal(e));

                var action = new ActionEventData { Action = ActionType.Enter, Parameters = $"{StairDirection.Down}", Speed = null, KeyPress = null };

                if (_eventSystem.Try(EventType.Action, sender, action))
                {
                }

                return false;
            }

            return true;
        }

        private static bool IsAutoPortal(IEntity e)
        {
            return e.Has<Portal>() && e.Get<Portal>().Automatic;
        }
    }
}
