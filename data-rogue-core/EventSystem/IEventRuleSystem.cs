using System.Collections.Generic;
using data_rogue_core.EntitySystem;
using data_rogue_core.Systems;

namespace data_rogue_core.EventSystem
{
    public interface IEventRuleSystem : IInitialisableSystem
    {
        bool Try(EventType eventType, IEntity sender, object eventData);

        void RegisterRule(IEventRule eventRule);
    }

    public class EventTypeList : List<EventType>
    {
    }

    public enum EventType
    {
        Input = 1,
        Move = 2,
        ChangeFloor = 3,
        UsePortal = 4
    }
}