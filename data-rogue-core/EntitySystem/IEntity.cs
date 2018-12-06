using data_rogue_core.Components;

namespace data_rogue_core.EntitySystem
{
    public interface IEntity
    {
        uint EntityId { get; }

        T Get<T>() where T : IEntityComponent;
        bool Has<T>() where T : IEntityComponent;
    }
}