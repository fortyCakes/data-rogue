using data_rogue_core.EntityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISkillSystem : ISystem, IInitialisableSystem
    {
        void Learn(IEntity learner, IEntity skill);
        void Forget(IEntity learner, IEntity skill);

        void Use(IEntity user, string skillName);
    }
}
