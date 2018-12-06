namespace data_rogue_core.EntitySystem
{
    public interface ISystem : IInitialisableSystem
    {
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);

        SystemComponents RequiredComponents {get;}
    }
}