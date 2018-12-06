using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.EntitySystem
{
    public class Entity : IEntity
    {
        public uint EntityId { get; private set; }

        public List<IEntityComponent> Components = new List<IEntityComponent>();

        public Entity(uint entityId, IEntityComponent[] components)
        {
            EntityId = entityId;
            Components = new List<IEntityComponent>(components);
        }

        public bool Has<T>() where T : IEntityComponent
        {
            return Components.Any(t => t.GetType() == typeof(T));
        }

        internal bool HasAll(IEnumerable<Type> systemComponents)
        {
            return systemComponents.All(sc => Components.Any(c => c.GetType() == sc));
        }

        public T Get<T>() where T : IEntityComponent
        {
            return (T)Components.SingleOrDefault(t => t.GetType() == typeof(T));
        }
    }
}
