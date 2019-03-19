using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class UnrolledAccuracyRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public UnrolledAccuracyRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public int RuleOrder => 500;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;

            if (data.AttackRoll == null)
            {
                data.AttackRoll = data.Accuracy.Value;
            }

            return true;
        }
    }
}
