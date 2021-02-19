using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISkillSystem : ISystem, IInitialisableSystem
    {
        void Learn(IEntity learner, IEntity skill, bool suppressMessage = false);
        void Forget(IEntity learner, IEntity skill);

        void Use(IEntity user, string skillName);
        void Use(IEntity user, IEntity skill);
        KnownSkill GetKnownSkillByIndex(IEntity entity, int index);

        IEntity GetSkillFromKnown(KnownSkill knownSkill);

        void OnComplete(IEntity user, IEntity skill);
        IEnumerable<IEntity> KnownSkills(IEntity entity);
    }
}
