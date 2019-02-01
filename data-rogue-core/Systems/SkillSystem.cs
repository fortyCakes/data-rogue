﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EventSystem;

namespace data_rogue_core.Systems
{
    public class SkillSystem : BaseSystem, ISkillSystem
    {
        private readonly IPrototypeSystem prototypeSystem;
        private readonly IScriptExecutor scriptExecutor;
        private readonly IEventSystem eventSystem;

        public override SystemComponents RequiredComponents => new SystemComponents() { typeof(SkillDefinition) };

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
            var skill = prototypeSystem.Create(skillName);

            var script = skill.Get<SkillDefinition>().Script;

            scriptExecutor.Execute(user, script);
        }
    }
}
