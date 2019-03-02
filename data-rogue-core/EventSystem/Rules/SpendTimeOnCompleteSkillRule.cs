using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{

    class SpendTimeOnCompleteSkillRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public SpendTimeOnCompleteSkillRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.CompleteSkill };
        public int RuleOrder => 0;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            systemContainer.EventSystem.Try(EventType.SpendTime, sender, new SpendTimeEventData { Ticks = 1000 });

            return true;
        }
    }

}
