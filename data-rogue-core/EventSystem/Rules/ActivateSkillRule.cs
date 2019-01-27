using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class ActivateSkillRule : IEventRule
    {
        private readonly ISkillSystem skillSystem;

        public ActivateSkillRule(ISystemContainer systemContainer)
        {
            entityEngine = systemContainer.EntityEngine;
            skillSystem = systemContainer.SkillSystem;
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

                skillSystem.Use(sender, skill.Skill);

                return true;
            }

            return false;
        }
    }
}
