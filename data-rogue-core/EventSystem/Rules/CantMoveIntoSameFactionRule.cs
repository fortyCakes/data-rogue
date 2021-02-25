using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class CantMoveIntoSameFactionRule : IEventRule
    {
        private IPositionSystem _positionSystem;
        private IFactionSystem _factionSystem;

        public CantMoveIntoSameFactionRule(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _factionSystem = systemContainer.FactionSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Move };
        public uint RuleOrder => 10;
        public EventRuleType RuleType => EventRuleType.BeforeEvent;


        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var vector = (Vector)eventData;
            var targetCoordinate = _positionSystem.CoordinateOf(sender) + vector;

            var entitiesAtPosition = _positionSystem.EntitiesAt(targetCoordinate);

            if (entitiesAtPosition.Any(e => _factionSystem.IsSameFaction(e, sender)))
            {
                return false;
            }

            return true;
        }
    }
}
