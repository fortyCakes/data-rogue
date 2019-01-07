using System.Collections.Generic;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem
{
    public interface IEventRuleSystem : IInitialisableSystem
    {
        bool Try(EventType eventType, IEntity sender, object eventData);

        void RegisterRule(IEventRule eventRule);
        void RegisterRules(params IEventRule[] rules);
    }

    public class EventTypeList : List<EventType>
    {
    }
}