using System;
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

        public static decimal GetStatByName(this IEventSystem eventSystem, IEntity sender, string statName)
        {
            Stat stat = (Stat)Enum.Parse(typeof(Stat), statName);
            return GetStat(eventSystem, sender, stat);
        }
    }
}
