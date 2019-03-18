using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class GainSingleXPOnKillRule : IEventRule
    {
        public GainSingleXPOnKillRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            MessageSystem = systemContainer.MessageSystem;
            EventSystem = systemContainer.EventSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Death };

        public int RuleOrder => 0;

        private IEntityEngine EntityEngine;
        public IMessageSystem MessageSystem;
        private IEventSystem EventSystem;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var killer = (eventData as DeathEventData).Killer;

            var ok = EventSystem.Try(EventType.GainXP, killer, new GainXPEventData { Amount = 1 });

            return true;
        }
    }
}
