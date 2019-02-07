using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem
{
    public interface IEventRule
    {
        EventTypeList EventTypes {get;}

        int RuleOrder { get; }

        bool Apply(EventType type, IEntity sender, object eventData);
    }
}