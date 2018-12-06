namespace data_rogue_core.EntitySystem
{
    public interface IEntityEngineSystem : IInitialisableSystem
    {
        void Destroy(Entity entity);
        Entity New(params IEntityComponent[] components);
        void Register(ISystem system);
    }
}