using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public class PositionSystem : BaseSystem, IPositionSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Position) };

        public IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate)
        {
            foreach(var entity in Entities)
            {
                if (entity.Get<Position>().MapCoordinate.Equals(coordinate))
                {
                    yield return entity;
                }
            }

            var cell = Game.WorldState.Maps[coordinate.Key].CellAt(coordinate.X, coordinate.Y);
            yield return cell;
        }

        public MapCoordinate PositionOf(IEntity entity)
        {
            return entity.Get<Position>().MapCoordinate;
        }

        public void Move(Position position, Vector vector)
        {
            position.Move(vector);
        }

        public IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, X, Y));
        }
    }
}