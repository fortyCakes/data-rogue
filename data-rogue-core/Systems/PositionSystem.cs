using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Systems
{
    public class PositionSystem : IPositionSystem
    {
        public SystemComponents SystemComponents => new SystemComponents { typeof(Position) };

        public List<IEntity> Entities { get; private set; } = new List<IEntity>();

        public void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate)
        {
            foreach(var entity in Entities)
            {
                if (entity.Get<Position>().MapCoordinate.Equals(coordinate))
                {
                    yield return entity;
                }
            }
        }

        public MapCoordinate PositionOf(IEntity entity)
        {
            return entity.Get<Position>().MapCoordinate;
        }

        public IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, X, Y));
        }
    }
}