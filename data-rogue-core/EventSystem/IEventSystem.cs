using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem
{
    public interface IEventSystem : IInitialisableSystem
    {
        bool Try(EventType eventType, IEntity sender, object eventData);
        void RegisterRule(IEventRule eventRule);
        void RegisterRules(params IEventRule[] rules);


    }

    public class EventTypeList : List<EventType>
    {
    }
}