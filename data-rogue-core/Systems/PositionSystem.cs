using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Renderers
{
    public class PositionSystem : IPositionSystem
    {
        public SystemComponents SystemComponents => new SystemComponents { typeof(Position) };

        public List<Entity> Entities { get; private set; } = new List<Entity>();

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);
        }

        public IEnumerable<Entity> EntitiesAt(MapCoordinate coordinate)
        {
            foreach(var entity in Entities)
            {
                if (entity.Get<Position>().MapCoordinate.Equals(coordinate))
                {
                    yield return entity;
                }
            }
        }

        public MapCoordinate PositionOf(Entity entity)
        {
            return entity.Get<Position>().MapCoordinate;
        }

        public IEnumerable<Entity> EntitiesAt(MapKey mapKey, int X, int Y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, X, Y));
        }
    }
}