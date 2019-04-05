using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem
{
    public interface IEventRule
    {
        EventTypeList EventTypes { get; }

        EventRuleType RuleType { get; }

        uint RuleOrder { get; }

        bool Apply(EventType type, IEntity sender, object eventData);
    }

    public enum EventRuleType
    {
        BeforeEvent,
        EventResolution,
        AfterSuccess,
        AfterFailure,
        Finally
    }
}