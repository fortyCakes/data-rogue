namespace data_rogue_core.EntitySystem
{
    public interface IEntity
    {
        uint EntityId { get; }

        string Name { get; }

        T Get<T>() where T : IEntityComponent;
        bool Has<T>() where T : IEntityComponent;
    }
}