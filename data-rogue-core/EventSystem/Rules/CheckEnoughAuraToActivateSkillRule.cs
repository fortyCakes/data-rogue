using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class CheckEnoughAuraToActivateSkillRule : IEventRule
    {
        private readonly ISkillSystem skillSystem;
        private IPrototypeSystem prototypeSystem;
        private IMessageSystem messageSystem;

        public CheckEnoughAuraToActivateSkillRule(ISystemContainer systemContainer)
        {
            entityEngine = systemContainer.EntityEngine;
            skillSystem = systemContainer.SkillSystem;
            prototypeSystem = systemContainer.PrototypeSystem;
            messageSystem = systemContainer.MessageSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.SelectSkill };
        public int RuleOrder => 0;

        private IEntityEngine entityEngine { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (ActivateSkillEventData)eventData;

            var skillDefinition = prototypeSystem.Get(data.SkillName).Get<Skill>();

            var userAura = sender.Get<AuraFighter>().Aura;

            if (userAura.Current < skillDefinition.Cost)
            {
                if (sender.IsPlayer)
                {
                    messageSystem.Write("You don't have enough Aura!");
                }
                return false;
            }

            return true;
        }
    }
}
