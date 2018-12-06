using System.Collections.Generic;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.EventSystem
{
    public interface IEventRuleSystem
    {
        bool Try(EventType eventType, Entity sender, object eventData);

        void RegisterRule(IEventRule eventRule);
    }

    public interface IEventRule
    {
        List<EventType> EventTypes {get;}

        int RuleOrder { get; }

        bool Apply(EventType type, Entity sender, object eventData);
    }

    public enum EventType
    {
        Input = 1
    }
}