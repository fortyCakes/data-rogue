using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Utils
{
    public static class EventRuleSystemExtensions
    {
        public static decimal GetStat(this IEventSystem eventSystem, IEntity sender, string stat)
        {
            GetStatEventData data = new GetStatEventData {Stat = stat};
            eventSystem.Try(EventType.GetStat, sender, data);

            return data.Value;
        }
    }
}
