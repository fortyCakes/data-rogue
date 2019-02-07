using data_rogue_core.Systems;

namespace data_rogue_core.EntityEngineSystem
{
    public interface ISystem : IInitialisableSystem
    {
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
        bool HasEntity(IEntity entity);

        SystemComponents RequiredComponents {get;}
        SystemComponents ForbiddenComponents { get; }
    }
}