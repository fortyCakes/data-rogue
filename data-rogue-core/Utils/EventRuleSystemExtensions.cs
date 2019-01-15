using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Utils
{
    public static class EventRuleSystemExtensions
    {
        public static decimal GetStat(this IEventSystem eventSystem, IEntity sender, Stat stat)
        {
            GetStatEventData data = new GetStatEventData {Stat = stat};
            eventSystem.Try(EventType.GetStat, sender, data);

            return data.Value;
        }
    }
}
