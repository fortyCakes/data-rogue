using System.Collections.Generic;

namespace data_rogue_core.EntityEngineSystem
{
    public interface IEntity
    {
        uint EntityId { get; }

        string Name { get; }

        string DescriptionName { get; }

        bool Removed { get; set; }

        bool IsStatic { get; set; }

        List<IEntityComponent> Components { get; }

        T Get<T>() where T : IEntityComponent;
        bool Has<T>() where T : IEntityComponent;

        bool IsPlayer { get; }

        IEntityComponent Get(string typeName);

        bool HasAll(SystemComponents systemComponents);

        bool HasNone(SystemComponents systemComponents);

        T TryGet<T>() where T : class, IEntityComponent;
    }
}