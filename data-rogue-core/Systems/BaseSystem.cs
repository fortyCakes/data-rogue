﻿using System.Collections.Generic;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public abstract SystemComponents RequiredComponents { get; }

        public List<IEntity> Entities { get; set; } = new List<IEntity>();
    }
}