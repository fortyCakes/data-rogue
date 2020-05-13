using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class ApplyModifiedAccuracyRule : IEventRule
    {
        private ISystemContainer _systemContainer;

        public ApplyModifiedAccuracyRule(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Attack };
        public EventRuleType RuleType => EventRuleType.BeforeEvent;

        public uint RuleOrder => 451;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = eventData as AttackEventData;
            var modifier = sender.TryGet<ModifiedAccuracy>();

            if (data.AttackRoll.HasValue && modifier != null)
            {
                data.AttackRoll += modifier.By;
            }

            return true;
        }
    }
}