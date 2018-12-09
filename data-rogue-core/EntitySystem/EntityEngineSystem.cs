using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EntitySystem
{
    [Serializable]
    public class EntityEngineSystem : IEntityEngineSystem
    {
        private uint EntityKey = 0;

        public List<Entity> AllEntities { get; private set; } = new List<Entity>();

        [JsonIgnore]
        public List<ISystem> Systems = new List<ISystem>();

        public Entity New(params IEntityComponent[] components)
        {
            var entity = new Entity(EntityKey++, components);
            AllEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            return entity;
        }

        private void RegisterEntityWithSystems(Entity entity)
        {
            foreach (var system in Systems)
            {
                if (entity.HasAll(system.RequiredComponents))
                {
                    system.AddEntity(entity);
                }
            }
        }

        public Entity New(string name, params IEntityComponent[] components)
        {
            var entity = New(components);
            entity.Name = name;
            return entity;
        }

        public void Destroy(Entity entity)
        {
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

        public void Initialise()
        {
            AllEntities = new List<Entity>();
            foreach (var system in Systems)
            {
                system.Initialise();
            }
        }

        public void Load(uint EntityId, Entity entity)
        {
            AllEntities.Add(entity);

            RegisterEntityWithSystems(entity);

            EntityKey = Math.Max(EntityKey, EntityId + 1);
        }

        public Entity GetEntity(uint entityId)
        {
            return AllEntities.Single(e => e.EntityId == entityId);
        }
    }
}