using System;
using System.Collections.Generic;

using data_rogue_core.Systems;

namespace data_rogue_core.EntitySystem
{
    public interface IEntityEngineSystem : IInitialisableSystem
    {
        IEnumerable<Type> ComponentTypes { get; }

        void Destroy(Entity entity);
        Entity New(string Name, params IEntityComponent[] components);

        Entity GetEntity(uint entityId);

        Entity Load(uint EntityId, Entity entity);

        void Register(ISystem system);

        List<Entity> AllEntities { get; }
    }
}