using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.EventSystem;
using System.Linq;

namespace data_rogue_core.Systems
{
    public class SkillSystem : BaseSystem, ISkillSystem
    {
        private readonly ISystemContainer systemContainer;

        public override SystemComponents RequiredComponents => new SystemComponents() { typeof(Skill) };

        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public SkillSystem(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }

        public void Forget(IEntity learner, IEntity skill)
        {
            var knownSkills = learner.Components.OfType<KnownSkill>();

            var knownSkill = knownSkills.SingleOrDefault(k => k.Skill == skill.Get<Prototype>().Name);

            if (knownSkill != null)
            {
                var index = knownSkill.Order;
                systemContainer.EntityEngine.RemoveComponent(learner, knownSkill);
                knownSkills.Where(k => k.Order > index).ToList().ForEach(k => k.Order--);
            }
            else
            {
                systemContainer.MessageSystem.Write($"{learner.DescriptionName} doesn't know {skill.DescriptionName} so they can't forget it.");
            }
        }

        public void Learn(IEntity learner, IEntity skill)
        {
            var knownSkills = learner.Components.OfType<KnownSkill>();

            if (knownSkills.Any(k => k.Skill == skill.Get<Prototype>().Name))
            {
                systemContainer.MessageSystem.Write($"{learner.DescriptionName} already knows {skill.DescriptionName}.");
            }
            else
            {
                systemContainer.MessageSystem.Write($"{learner.DescriptionName} learns {skill.DescriptionName}!");
                var newOrder = knownSkills.Any() ? knownSkills.Max(k => k.Order) + 1 : 1;

                systemContainer.EntityEngine.AddComponent(learner, new KnownSkill { Order = newOrder, Skill = skill.Get<Prototype>().Name });
            }
        }

        public void Use(IEntity user, string skillName)
        {
            var skill = systemContainer.PrototypeSystem.Get(skillName);

            var scriptName = skill.Get<Skill>().ScriptName;

            var script = systemContainer.PrototypeSystem.Get(scriptName);

            var scriptText = script.Get<Script>().Text;

            var onCompleteAction = new Action(() => OnComplete(user, skill));

            // Construct a new one rather than using the one in container so that it doesn't use up the onComplete slot.
            var scriptExecutor = new ScriptExecutor(systemContainer);

            scriptExecutor.Execute(user, scriptText, skill, onCompleteAction);
        }

        public void OnComplete(IEntity user, IEntity skill)
        {
            systemContainer.EventSystem.Try(EventType.CompleteSkill, user, new CompleteSkillEventData { SkillName = skill.Get<Prototype>().Name });
        }

        public KnownSkill GetKnownSkillByIndex(IEntity entity, int index)
        {
            var skills = entity.Components.OfType<KnownSkill>().ToList();

            if (skills.Count >= index)
            {
                return skills[index - 1];
            }

            return null;
        }

        public IEntity GetSkillFromKnown(KnownSkill knownSkill)
        {
            return systemContainer.PrototypeSystem.Get(knownSkill.Skill);
        }
    }
}
