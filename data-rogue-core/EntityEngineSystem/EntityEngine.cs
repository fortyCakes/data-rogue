﻿using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EntityEngineSystem
{
    [Serializable]
    public class EntityEngine : IEntityEngine
    {
        private uint EntityKey = 0;

        public IEntityDataProvider StaticDataProvider { get; }

        public IList<Type> ComponentTypes { get; set; }
            

        public IEnumerable<IEntity> AllEntities
        {
            get => _allEntities;
            private set => _allEntities = value.ToList();
        }

        public IEnumerable<IEntity> MutableEntities => AllEntities.Where(e => !e.IsStatic).ToList();

        public IEnumerable<ISystem> Systems
        {
            get => _systems;
            private set => _systems = value.ToList();
        }
        
        private List<ISystem> _systems = new List<ISystem>();
        private List<IEntity> _allEntities = new List<IEntity>();

        public EntityEngine(IEntityDataProvider staticDataProvider, IList<Type> additionalComponentTypes = null)
        {
            StaticDataProvider = staticDataProvider;

            ComponentTypes = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IEntityComponent).IsAssignableFrom(p))
            .ToList();

            if (additionalComponentTypes != null)
            {
                ((List<Type>)ComponentTypes).AddRange(additionalComponentTypes);
            }
        }

        public IEntity New(string name, params IEntityComponent[] components)
        {
            var entity = new Entity(EntityKey++, name, components);
            _allEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            return entity;
        }

        private void RegisterEntityWithSystems(IEntity entity)
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
            var entity = Get(entityId);

            _allEntities.Remove(entity);

            foreach (var system in Systems)
            {
                if (entity.HasAll(system.RequiredComponents))
                {
                    system.RemoveEntity(entity);
                }
            }

            entity.Removed = true;
        }

        public void Destroy(IEntity entity)
        {
            Destroy(entity.EntityId);
        }

        public void Register(ISystem system)
        {
            _systems.Add(system);
        }

        public IEnumerable<IEntity> FilterByComponentName(IEnumerable<IEntity> entities, string componentName)
        {
            return entities.Where(e => e.Components.Any(c => c.GetType().Name == componentName));
        }

        public void Initialise(ISystemContainer systemContainer)
        {
            EntityKey = 0;
            AllEntities = new List<Entity>();
            foreach (var system in Systems)
            {
                system.Initialise();
            }

            var staticEntityData = StaticDataProvider.GetData();

            EntitySerializer.DeserializeAll(systemContainer, staticEntityData);

            _allEntities.ForEach(e => e.IsStatic = true);
        }

        public void AddComponent(IEntity entity, IEntityComponent component)
        {
            entity.Components.Add(component);

            RecheckSystemRegistration(entity);
        }

        public void RemoveComponent(IEntity entity, IEntityComponent component)
        {
            entity.Components.Remove(component);

            RecheckSystemRegistration(entity);
        }

        public void RemoveComponent<T>(IEntity entity) where T : IEntityComponent
        {
            RemoveComponent(entity, entity.Get<T>());
        }

        public IEntity Load(uint EntityId, IEntity entity)
        {
            _allEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            EntityKey = Math.Max(EntityKey, EntityId + 1);

            return entity;
        }

        public IEntity Get(uint entityId)
        {
            return _allEntities.ToList().Single(e => e.EntityId == entityId);
        }

        public IEnumerable<IEntity> GetEntitiesWithName(string entityName)
        {
            return _allEntities.Where(e => e.Name == entityName);
        }

        public IEntity GetEntityWithName(string entityName)
        {
            return GetEntitiesWithName(entityName).Single();
        }

        public IEnumerable<IEntity> EntitiesWith<T>(bool includePrototypes = false) where T: IEntityComponent
        {
            return _allEntities.Where(e => e.Has<T>() && (includePrototypes || !e.Has<Prototype>()));
        }

        public IEnumerable<T> GetAll<T>(bool includePrototypes = false) where T : IEntityComponent
        {
            return EntitiesWith<T>(includePrototypes).SelectMany(e => e.Components.OfType<T>()).ToList();
        }

        private void RecheckSystemRegistration(IEntity entity)
        {
            foreach (var system in Systems)
            {
                if (entity.HasAll(system.RequiredComponents) && entity.HasNone(system.ForbiddenComponents))
                {
                    if (!system.HasEntity(entity))
                    {
                        system.AddEntity(entity);
                    }
                }
                else
                {
                    if (system.HasEntity(entity))
                    {
                        system.RemoveEntity(entity);
                    }
                }
            }
        }
    }
}