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
        private readonly IEntityEngine _entityEngine;
        private readonly IMapSystem _mapSystem;
        private readonly IPathfindingAlgorithm _pathfindingAlgorithm;

        private EntityCache _entityCache;
        private Dictionary<IEntity, MapCoordinate> _positionCache;

        public PositionSystem(IMapSystem mapSystem, IEntityEngine entityEngine, IPathfindingAlgorithm pathfindingAlgorithm)
        {
            _mapSystem = mapSystem;
            _entityEngine = entityEngine;
            _pathfindingAlgorithm = pathfindingAlgorithm;
        }

        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Position)};
        public override SystemComponents ForbiddenComponents => new SystemComponents {typeof(Prototype)};

        public IList<IEntity> EntitiesAt(MapCoordinate coordinate)
        {
            return EntitiesAt_Internal(coordinate).ToList();
        }

        public override void Initialise()
        {
            base.Initialise();
            _positionCache = new Dictionary<IEntity, MapCoordinate>();
            _entityCache = new EntityCache();
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            AddToCache(entity);
        }

        private void AddToCache(IEntity entity)
        {
            _positionCache.Add(entity, entity.Get<Position>().MapCoordinate);
            _entityCache.Add(entity.Get<Position>().MapCoordinate, entity);
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);
            RemoveFromCache(entity);
        }

        private void RemoveFromCache(IEntity entity)
        {
            _positionCache.Remove(entity);
            _entityCache.Remove(entity);
        }

        public MapCoordinate CoordinateOf(IEntity entity)
        {
            if (_positionCache.ContainsKey(entity)) return _positionCache[entity];

            return null;
        }

        public void Move(IEntity entity, Vector vector)
        {
            Position position = entity.Get<Position>();

            if (position == null) throw new InvalidOperationException("Can't move an entity that doesn't have a Position");

            Move(entity, position, vector);
        }

        public IList<IEntity> EntitiesAt(MapKey mapKey, int x, int y)
        {
            return EntitiesAt(new MapCoordinate(mapKey, x, y));
        }

        public void SetPosition(IEntity entity, MapCoordinate mapCoordinate)
        {
            Position position = entity.Get<Position>();

            if (position == null)
            {
                position = new Position {MapCoordinate = mapCoordinate};
                _entityEngine.AddComponent(entity, position);
            }

            SetPosition(entity, position, mapCoordinate);
        }

        public void RemovePosition(IEntity entity)
        {
            Position position = entity.Get<Position>();

            if (position != null) _entityEngine.RemoveComponent(entity, position);

            RemoveFromCache(entity);
        }

        public bool Any(MapCoordinate key)
        {
            return _entityCache.ContainsKey(key) && _entityCache[key].Count > 0;
        }

        public IEnumerable<MapCoordinate> Path(MapCoordinate origin, MapCoordinate destination)
        {
            if (origin.Key != destination.Key) return null;

            IMap map = _mapSystem.MapCollection[origin.Key];

            return _pathfindingAlgorithm.Path(map, origin, destination);
        }

        public bool IsBlocked(MapCoordinate key, IEntity except = null)
        {
            var entities = EntitiesAt(key)
                .Where(e => e != except);

            var components = entities
                .Select(e => e.TryGet<Physical>())
                .Where(p => p != null);

            return components.Any(p => !p.Passable);
        }

        private IEnumerable<IEntity> EntitiesAt_Internal(MapCoordinate coordinate)
        {
            if (_entityCache.ContainsKey(coordinate))
            {
                foreach (var entity in _entityCache[coordinate])
                {
                    yield return entity;
                }
            }

            IEntity cell = _mapSystem.MapCollection[coordinate.Key].CellAt(coordinate.X, coordinate.Y);
            yield return cell;
        }

        private void Move(IEntity entity, Position position, Vector vector)
        {
            MapCoordinate oldMapCoordinate = position.MapCoordinate;

            position.Move(vector);

            UpdateCache(entity, position.MapCoordinate, oldMapCoordinate);
        }

        private void UpdateCache(IEntity entity, MapCoordinate mapCoordinate, MapCoordinate oldMapCoordinate)
        {
            _positionCache[entity] = mapCoordinate;
            _entityCache.UpdatePosition(entity, mapCoordinate, oldMapCoordinate);
        }

        private void SetPosition(IEntity entity, Position position, MapCoordinate mapCoordinate)
        {
            MapCoordinate oldMapCoordinate = position.MapCoordinate;

            position.MapCoordinate = mapCoordinate;

            UpdateCache(entity, mapCoordinate, oldMapCoordinate);
        }
    }
}