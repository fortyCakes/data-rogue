using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EntityEngine
{
    [Serializable]
    public class EntityEngine : IEntityEngine
    {
        private uint EntityKey = 0;

        public BaseStaticEntityLoader StaticEntityLoader { get; }

        public IEnumerable<Type> ComponentTypes => 
            AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IEntityComponent).IsAssignableFrom(p))
            .ToList();

        public List<Entity> AllEntities { get; private set; } = new List<Entity>();

        public List<Entity> MutableEntities => AllEntities.Where(e => !e.IsStatic).ToList();

        [JsonIgnore]
        public List<ISystem> Systems = new List<ISystem>();

        public EntityEngine(BaseStaticEntityLoader loader)
        {
            StaticEntityLoader = loader;
        }

        public Entity New(string Name, params IEntityComponent[] components)
        {
            var entity = new Entity(EntityKey++, Name, components);
            AllEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            return entity;
        }

        private void RegisterEntityWithSystems(Entity entity)
        {
            foreach (var system in Systems)
            {
                if (entity.HasAll(system.RequiredComponents) && entity.HasNone(system.ForbiddenComponents))
                {
                    system.AddEntity(entity);
                }
            }
        }

        public void Destroy(uint entityId)
        {
            var entity = GetEntity(entityId);

            AllEntities.Remove(entity);

            foreach (var system in Systems)
            {
                if (entity.HasAll(system.RequiredComponents))
                {
                    system.RemoveEntity(entity);
                }
            }

            entity = null;
        }

        public void Register(ISystem system)
        {
            Systems.Add(system);
        }

        public void Initialise(ISystemContainer systemContainer)
        {
            EntityKey = 0;
            AllEntities = new List<Entity>();
            foreach (var system in Systems)
            {
                system.Initialise();
            }

            StaticEntityLoader.Load(systemContainer);

            AllEntities.ForEach(e => e.IsStatic = true);
        }

        public void AddComponent(IEntity entity, IEntityComponent component)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(IEntity entity, IEntityComponent component)
        {
            throw new NotImplementedException();
        }

        public Entity Load(uint EntityId, Entity entity)
        {
            AllEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            EntityKey = Math.Max(EntityKey, EntityId + 1);

            return entity;
        }

        public Entity GetEntity(uint entityId)
        {
            return AllEntities.Single(e => e.EntityId == entityId);
        }

        public IEnumerable<Entity> GetEntitiesWithName(string entityName)
        {
            return AllEntities.Where(e => e.Name == entityName);
        }

        public Entity GetEntityWithName(string v)
        {
            return GetEntitiesWithName(v).Single();
        }

        public List<Entity> EntitiesWith<T>(bool includePrototypes = false) where T: IEntityComponent
        {
            return AllEntities.Where(e => e.Has<T>() && (includePrototypes || !e.Has<Prototype>())).ToList();
        }

        public List<T> GetAll<T>() where T : IEntityComponent
        {
            return EntitiesWith<T>().Select(e => e.Get<T>()).ToList();
        }
    }
}