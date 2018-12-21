using System.Collections.Generic;

namespace data_rogue_core.EntityEngine
{
    public interface IEntity
    {
        uint EntityId { get; }

        string Name { get; }

        List<IEntityComponent> Components { get; }

        T Get<T>() where T : IEntityComponent;
        bool Has<T>() where T : IEntityComponent;
    }
}