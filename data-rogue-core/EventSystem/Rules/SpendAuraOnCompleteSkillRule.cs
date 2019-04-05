using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class SpendAuraOnCompleteSkillRule : IEventRule
    {
        private ISystemContainer systemContainer;

        public SpendAuraOnCompleteSkillRule(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.CompleteSkill };
        public uint RuleOrder => 0;
        public EventRuleType RuleType => EventRuleType.AfterSuccess;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var fighter = sender.Get<AuraFighter>();
            var skill = systemContainer.PrototypeSystem.Get((eventData as CompleteSkillEventData).SkillName).Get<Skill>();
            var cost = skill.Cost;

            fighter.Aura.Subtract(cost);

            return true;
        }
    }
}
