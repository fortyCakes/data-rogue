using System;
using System.Collections.Generic;
using data_rogue_core.Components;

namespace data_rogue_core.EntityEngineSystem
{
    public interface IEntity
    {
        uint EntityId { get; }

        string Name { get; }

        string DescriptionName { get; }

        bool IsStatic { get; set; }

        List<IEntityComponent> Components { get; }

        T Get<T>() where T : IEntityComponent;
        bool Has<T>() where T : IEntityComponent;

        bool IsPlayer { get; }

        Fighter GetFighter();
        Description GetDescription();
        bool HasAll(SystemComponents systemComponents);

        bool HasNone(SystemComponents systemComponents);

        T TryGet<T>() where T : class, IEntityComponent;
    }
}