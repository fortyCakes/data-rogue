using System;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EntityEngineSystem
{
    public interface IEntityEngine
    {
        IEnumerable<Type> ComponentTypes { get; }

        void Destroy(uint entityId);
        IEntity New(string Name, params IEntityComponent[] components);

        IEntity GetEntity(uint entityId);

        IEntity Load(uint EntityId, IEntity entity);

        void Register(ISystem system);


        IEnumerable<IEntity> AllEntities { get; }
        IEnumerable<IEntity> MutableEntities { get; }


        IEnumerable<IEntity> EntitiesWith<T>(bool includePrototypes = false) where T: IEntityComponent;

        IEnumerable<T> GetAll<T>(bool includePrototypes = false) where T : IEntityComponent;

        void Initialise(ISystemContainer behaviourFactory);

        void AddComponent(IEntity entity, IEntityComponent component);
        void RemoveComponent(IEntity entity, IEntityComponent component);
        IEntity GetEntityWithName(string testEntity);
        IEnumerable<IEntity> GetEntitiesWithName(string testEntity);
    }
}