using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.EventSystem.Rules
{
    public class AddAgilityToEvasionRule : IEventRule
    {
        private readonly ISystemContainer _systemContainer;

        public AddAgilityToEvasionRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GetStat };
        public uint RuleOrder => 0;


        public EventRuleType RuleType => EventRuleType.EventResolution;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            if (data.Stat == "EV")
            {
                data.Value += _systemContainer.EventSystem.GetStat(sender, "Agility") * 1.5m;
            }

            return true;
        }
    }
}