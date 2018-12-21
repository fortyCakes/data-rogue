using System.Collections.Generic;
using data_rogue_core.EntityEngine;

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

        public abstract SystemComponents ForbiddenComponents { get; }

        public List<IEntity> Entities { get; set; }

        public void Initialise()
        {
            Entities = new List<IEntity>();
        }
    }
}