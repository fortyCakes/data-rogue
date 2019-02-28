using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.EventSystem;

namespace data_rogue_core.Systems
{
    public class SkillSystem : BaseSystem, ISkillSystem
    {
        private readonly IPrototypeSystem prototypeSystem;
        private readonly IScriptExecutor scriptExecutor;
        private readonly IEventSystem eventSystem;

        public override SystemComponents RequiredComponents => new SystemComponents() { typeof(Skill) };

        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public SkillSystem(IPrototypeSystem prototypeSystem, IScriptExecutor scriptExecutor, IEventSystem eventSystem)
        {
            this.prototypeSystem = prototypeSystem;
            this.scriptExecutor = scriptExecutor;
            this.eventSystem = eventSystem;
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
            var skill = prototypeSystem.Get(skillName);

            var scriptName = skill.Get<Skill>().ScriptName;

            var script = prototypeSystem.Get(scriptName);

            var scriptText = script.Get<Script>().Text;

            scriptExecutor.Execute(user, scriptText);
        }
    }
}
