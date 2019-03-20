using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public virtual void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
        }

        public virtual void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public bool HasEntity(IEntity entity)
        {
            return Entities.Contains(entity);
        }

        public abstract SystemComponents RequiredComponents { get; }

        public abstract SystemComponents ForbiddenComponents { get; }

        public List<IEntity> Entities { get; set; }

        public virtual void Initialise()
        {
            Entities = new List<IEntity>();
        }
    }
}