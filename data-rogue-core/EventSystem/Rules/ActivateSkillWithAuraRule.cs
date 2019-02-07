using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class ActivateSkillWithAuraRule : IEventRule
    {
        private readonly ISkillSystem skillSystem;
        private IPrototypeSystem prototypeSystem;
        private IMessageSystem messageSystem;

        public ActivateSkillWithAuraRule(ISystemContainer systemContainer)
        {
            entityEngine = systemContainer.EntityEngine;
            skillSystem = systemContainer.SkillSystem;
            prototypeSystem = systemContainer.PrototypeSystem;
            messageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.ActivateSkill };
        public int RuleOrder => 0;

        private IEntityEngine entityEngine { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (ActivateSkillEventData)eventData;

            var skills = sender.Components.OfType<KnownSkill>().OrderBy(s => s.Order).ToList();

            var index = data.SkillIndex - 1;

            if (skills.Count() > index)
            {
                var skill = skills[index];

                var skillDefinition = prototypeSystem.Create(skill.Skill).Get<SkillDefinition>();

                var userAura = sender.Get<Fighter>().Aura;

                if (userAura.Current >= skillDefinition.Cost)
                {
                    skillSystem.Use(sender, skill.Skill);
                    userAura.Subtract(skillDefinition.Cost);
                }
                else
                {
                    if (sender.IsPlayer)
                    {
                        messageSystem.Write("You don't have enough Aura!");
                    }
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
