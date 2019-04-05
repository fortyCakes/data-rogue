using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    public class SpendTimeOnCompleteSkillRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public SpendTimeOnCompleteSkillRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.CompleteSkill };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            systemContainer.EventSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData { Ticks = 1000 });

            return true;
        }
    }
}
