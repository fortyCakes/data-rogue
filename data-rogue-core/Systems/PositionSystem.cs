using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Systems
{
    public class PositionSystem : BaseSystem, IPositionSystem
    {
        private readonly IMapSystem _mapSystem;
        private readonly IEntityEngine _engine;
        private readonly IPathfindingAlgorithm _pathfindingAlgorithm;

        public PositionSystem(IMapSystem mapSystem, IEntityEngine engine, IPathfindingAlgorithm pathfindingAlgorithm)
        {
            _mapSystem = mapSystem;
            _engine = engine;
            _pathfindingAlgorithm = pathfindingAlgorithm;
        }

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

            var cell = _mapSystem.MapCollection[coordinate.Key].CellAt(coordinate.X, coordinate.Y);
            yield return cell;
        }

        public MapCoordinate CoordinateOf(IEntity entity)
        {
            return entity.Get<Position>()?.MapCoordinate;
        }

        private void Move(Position position, Vector vector)
        {
            position.Move(vector);
        }

        public void Move(IEntity entity, Vector vector)
        {
            var position = entity.Get<Position>();

            if (position == null)
            {
                throw new InvalidOperationException("Can't move an entity that doesn't have a Position");
            }

            Move(position, vector);
        }

        public IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, X, Y));
        }

        public void SetPosition(IEntity entity, MapCoordinate mapCoordinate)
        {
            Position position = entity.Get<Position>();

            if (position == null)
            {
                position = new Position();
                _engine.AddComponent(entity, position);
            }

            SetPosition(position, mapCoordinate);
        }

        public void SetPosition(Position position, MapCoordinate mapCoordinate)
        {
            position.MapCoordinate = mapCoordinate;
        }

        public void RemovePosition(IEntity entity)
        {
            Position position = entity.Get<Position>();

            if (position != null)
            {
                _engine.RemoveComponent(entity, position);
            }
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

            var map = _mapSystem.MapCollection[origin.Key];

            return _pathfindingAlgorithm.Path(map, origin, destination);
        }
    }
}