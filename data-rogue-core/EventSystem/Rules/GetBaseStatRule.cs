using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem.Rules
{
    class GetBaseStatRule : IEventRule
    {
        public GetBaseStatRule(IEntityEngine engine)
        {
            EntityEngine = engine;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public int RuleOrder => 0;

        private IEntityEngine EntityEngine { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case Stat.Muscle:
                    data.Value = sender.Get<Fighter>().Muscle;
                    break;
                case Stat.Agility:
                    data.Value = sender.Get<Fighter>().Agility;
                    break;
                default:
                    throw new ApplicationException($"Could not resolve stat {data.Stat.ToString()}");
            }

            return true;
        }
    }
}
