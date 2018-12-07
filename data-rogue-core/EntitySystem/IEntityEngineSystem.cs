using data_rogue_core.Systems;

namespace data_rogue_core.EntitySystem
{
    public interface IEntityEngineSystem : IInitialisableSystem
    {
        void Destroy(Entity entity);
        Entity New(params IEntityComponent[] components);
        Entity New(string Name, params IEntityComponent[] components);
        void Register(ISystem system);
    }
}