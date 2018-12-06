namespace data_rogue_core.EntitySystem
{
    public interface ISystem
    {
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);

        SystemComponents RequiredComponents {get;}
    }
}