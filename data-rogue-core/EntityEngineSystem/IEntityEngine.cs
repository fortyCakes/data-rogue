using System;
using System.Collections.Generic;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EntityEngineSystem
{
    public interface IEntityEngine
    {
        IList<Type> ComponentTypes { get; }
        IEnumerable<IEntity> AllEntities { get; }
        IEnumerable<IEntity> MutableEntities { get; }

        void Destroy(uint entityId);
        void Destroy(IEntity entity);

        IEntity New(string name, params IEntityComponent[] components);

        IEntity Get(uint entityId);

        IEntity Load(uint EntityId, IEntity entity);

        void Register(ISystem system);

        IEnumerable<IEntity> EntitiesWith<T>(bool includePrototypes = false) where T: IEntityComponent;
        IEnumerable<T> GetAll<T>(bool includePrototypes = false) where T : IEntityComponent;
        IEnumerable<IEntity> FilterByComponentName(IEnumerable<IEntity> entities, string componentName);

        void Initialise(ISystemContainer systemContainer);

        void AddComponent(IEntity entity, IEntityComponent component);
        void RemoveComponent(IEntity entity, IEntityComponent component);
        IEntity GetEntityWithName(string entityName);
        IEnumerable<IEntity> GetEntitiesWithName(string entityName);
        
        void RemoveComponent<T>(IEntity entity) where T : IEntityComponent;
        
    }
}