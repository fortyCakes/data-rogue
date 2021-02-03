using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ExplorationAddsScoreRule : IEventRule
    {
        private readonly IMapSystem _mapSystem;

        public ExplorationAddsScoreRule(ISystemContainer systemContainer)
        {
            _mapSystem = systemContainer.MapSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.GetStat };
        public uint RuleOrder => 0;

        public EventRuleType RuleType => EventRuleType.EventResolution;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case "Score":
                    data.Value += GetExplorationAmount();
                    break;
            }

            return true;
        }

        private decimal GetExplorationAmount()
        {
            var accumulator = 0;

            foreach(var map in _mapSystem.MapCollection.Values)
            {
                accumulator += map.SeenCoordinates.Count;
            }

            return accumulator;
        }
    }
}
