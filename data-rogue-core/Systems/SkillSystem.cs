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
            throw new NotImplementedException();
        }

        public void Learn(IEntity learner, IEntity skill)
        {
            throw new NotImplementedException();
        }

        public void Use(IEntity user, string skillName)
        {
            var skill = systemContainer.PrototypeSystem.Get(skillName);

            var scriptName = skill.Get<Skill>().ScriptName;

            var script = systemContainer.PrototypeSystem.Get(scriptName);

            var scriptText = script.Get<Script>().Text;

            var onCompleteAction = new Action(() => onComplete(user, skill));

            // Construct a new one rather than using the one in container so that it doesn't use up the onComplete slot.
            var scriptExecutor = new ScriptExecutor(systemContainer);

            scriptExecutor.Execute(user, scriptText, onCompleteAction);
        }

        public void onComplete(IEntity user, IEntity skill)
        {
            systemContainer.EventSystem.Try(EventType.CompleteSkill, user, new CompleteSkillEventData { SkillName = skill.Get<Prototype>().Name });
        }

        public KnownSkill GetKnownSkillByIndex(IEntity entity, int index)
        {
            var skills = entity.Components.OfType<KnownSkill>().ToList();

            if (skills.Count >= index)
            {
                return skills[index-1];
            }

            return null;
        }
    }
}
