using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public class PositionSystem : BaseSystem, IPositionSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Position) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate)
        {
            return EntitiesAt_Internal(coordinate).ToList();
        }

        private IEnumerable<IEntity> EntitiesAt_Internal(MapCoordinate coordinate)
        {
            foreach (var entity in Entities)
            {
                Position entityPosition = entity.Get<Position>();
                if (entityPosition.MapCoordinate.Equals(coordinate))
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

        public void Move(IEntity entity, Vector vector)
        {
            Move(entity.Get<Position>(), vector);
        }

        public IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, X, Y));
        }

        public void SetPosition(IEntity entity, MapCoordinate mapCoordinate)
        {
            SetPosition(entity.Get<Position>(), mapCoordinate);
        }

        public void SetPosition(Position position, MapCoordinate mapCoordinate)
        {
            position.MapCoordinate = mapCoordinate;
        }

        public bool Any(MapCoordinate key)
        {
            return Entities.Any(e => e.Get<Position>().MapCoordinate == key);
        }

        public IEnumerable<MapCoordinate> Path(MapCoordinate origin, MapCoordinate destination)
        {
            if (origin.Key != destination.Key)
            {
                // on different levels
                return null;
            }

            var map = Game.WorldState.Maps[origin.Key];

            return AStar.Path(map, origin, destination);
        }
    }
}