using System;
using System.Collections.Generic;

using data_rogue_core.Systems;

namespace data_rogue_core.EntityEngine
{
    public interface IEntityEngine : IInitialisableSystem
    {
        IEnumerable<Type> ComponentTypes { get; }

        void Destroy(Entity entity);
        Entity New(string Name, params IEntityComponent[] components);

        Entity GetEntity(uint entityId);

        Entity Load(uint EntityId, Entity entity);

        void Register(ISystem system);


        List<Entity> AllEntities { get; }
        List<Entity> MutableEntities { get; }


        List<Entity> EntitiesWith<T>(bool includePrototypes = false) where T: IEntityComponent;

        List<T> GetAll<T>() where T : IEntityComponent;

        //Entity GetEntityWithName(string name);
        //IEnumerable<Entity> GetEntitiesWithName(string name);
    }
}