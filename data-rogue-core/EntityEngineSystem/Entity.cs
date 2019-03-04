using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;

namespace data_rogue_core.EntityEngineSystem
{
    public class Entity : IEntity
    {
        public uint EntityId { get; }

        public string Name { get; set; } = "";
        public string DescriptionName => Get<Description>()?.Name;

        public bool IsStatic { get; set; } = false;

        public List<IEntityComponent> Components { get; set; } = new List<IEntityComponent>();

        public Entity(uint entityId, string name, IEntityComponent[] components)
        {
            EntityId = entityId;
            Name = name;
            Components = new List<IEntityComponent>(components);
        }

        public bool Has<T>() where T : IEntityComponent
        {
            return Components.Any(t => t.GetType() == typeof(T));
        }

        public bool HasAll(SystemComponents systemComponents)
        {
            return systemComponents.All(sc => Components.Any(c => c.GetType() == sc));
        }

        public T Get<T>() where T : IEntityComponent
        {
            return (T)Components.SingleOrDefault(t => t.GetType() == typeof(T));
        }

        public T TryGet<T>() where T : class, IEntityComponent
        {
            if (Has<T>()) return Get<T>();

            return null;
        }

        public override string ToString()
        {
            var componentTypeList = string.Join(",", Components.Select(c => c.GetType().Name));
            componentTypeList = componentTypeList.Substring(0, Math.Min(200, componentTypeList.Length));

            return $"Entity {EntityId}: {Name} ({componentTypeList})";

        }

        public bool HasNone(SystemComponents forbiddenComponents)
        {
            return !forbiddenComponents.Any(fc => Components.Any(c => c.GetType() == fc));
        }

        public bool IsPlayer => Name == "Player";

        public IEntityComponent Get(string typeName)
        {
            return Components.Where(c => c.GetType().Name == typeName).SingleOrDefault();
        }
    }
}
