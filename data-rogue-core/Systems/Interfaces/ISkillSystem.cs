using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISkillSystem : ISystem, IInitialisableSystem
    {
        void Learn(IEntity learner, IEntity skill);
        void Forget(IEntity learner, IEntity skill);

        void Use(IEntity user, string skillName);
    }
}
