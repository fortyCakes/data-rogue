﻿using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    [Serializable]
    public class EntityEngine
    {
        private uint EntityKey = 0;

        public List<Entity> AllEntities = new List<Entity>();

        public List<ISystem> Systems = new List<ISystem>();

        public Entity New(params IEntityComponent[] components)
        {
            var entity = new Entity(EntityKey++, components);
            AllEntities.Add(entity);

            foreach(var system in Systems)
            {
                if (entity.HasAll(system.SystemComponents))
                {
                    system.AddEntity(entity);
                }
            }

            return entity;
        }

        public void Destroy(Entity entity)
        {
            AllEntities.Remove(entity);

            foreach (var system in Systems)
            {
                if (entity.HasAll(system.SystemComponents))
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
    }
}