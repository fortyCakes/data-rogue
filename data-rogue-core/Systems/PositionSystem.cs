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

        public IList<IEntity> EntitiesAt(MapCoordinate coordinate, bool includeMapCells = true)
        {
            return EntitiesAt_Internal(coordinate, includeMapCells).ToList();
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

        public IList<IEntity> EntitiesAt(MapKey mapKey, int x, int y, bool includeMapCells = true)
        {
            return EntitiesAt(new MapCoordinate(mapKey, x, y), includeMapCells);
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

        public IEnumerable<MapCoordinate> DirectPath(MapCoordinate origin, MapCoordinate destination)
        {
            if (origin == destination || origin.Key != destination.Key) return null;

            var path = new List<MapCoordinate>();

            IMap map = _mapSystem.MapCollection[origin.Key];

            var diff = origin - destination;

            // Normalise the vector
            var size = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
            var ndx = diff.X / size;
            var ndy = diff.Y / size;

            double currentX = origin.X + 0.5;
            double currentY = origin.Y + 0.5;

            while (!((int)Math.Floor(currentX) == destination.X && (int)Math.Floor(currentY) == destination.Y))
            {
                var currentCoordinate = new MapCoordinate(origin.Key, (int)Math.Floor(currentX), (int)Math.Floor(currentY));

                if (!path.Contains(currentCoordinate))
                {
                    var cell = map.CellAt(currentCoordinate);
                    if (!cell.Get<Physical>().Passable)
                    {
                        break;
                    }
                    
                    path.Add(currentCoordinate);
                }

                currentX += ndx / 100;
                currentY += ndy / 100;
            }

            path.Remove(origin);

            return path;
        }

        public bool UnblockedPathExists(MapCoordinate origin, MapCoordinate destination)
        {
            if (origin == destination) return true;
            if (origin.Key != destination.Key) return false;

            var path = new List<MapCoordinate>();

            IMap map = _mapSystem.MapCollection[origin.Key];

            var diff = origin - destination;

            // Normalise the vector
            var size = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
            var ndx = diff.X / size;
            var ndy = diff.Y / size;

            double currentX = origin.X + 0.5;
            double currentY = origin.Y + 0.5;

            while (!((int)Math.Floor(currentX) == destination.X && (int)Math.Floor(currentY) == destination.Y))
            {
                var currentCoordinate = new MapCoordinate(origin.Key, (int)Math.Floor(currentX), (int)Math.Floor(currentY));

                if (!path.Contains(currentCoordinate))
                {
                    var cell = map.CellAt(currentCoordinate);
                    if (!cell.Get<Physical>().Passable)
                    {
                        return false;
                    }

                    path.Add(currentCoordinate);
                }

                currentX += ndx / 100;
                currentY += ndy / 100;
            }

            path.Remove(origin);

            var isBlocked = path.Any(c => EntitiesAt(c).Any(e => e.TryGet<Physical>()?.Passable == false));

            return !isBlocked;
        }

        public bool IsBlocked(MapCoordinate key, bool cellsOnly = false, IEntity except = null)
        {
            var entities = cellsOnly ? 
                new List<IEntity> { _mapSystem.MapCollection[key.Key].CellAt(key) } 
                : EntitiesAt(key).Where(e => e != except);

            var components = entities
                .Select(e => e.TryGet<Physical>())
                .Where(p => p != null);

            return components.Any(p => !p.Passable);
        }

        private IEnumerable<IEntity> EntitiesAt_Internal(MapCoordinate coordinate, bool includeMapCells = true)
        {
            if (_entityCache.ContainsKey(coordinate))
            {
                foreach (var entity in _entityCache[coordinate])
                {
                    yield return entity;
                }
            }

            if (includeMapCells)
            {
                IEntity cell = _mapSystem.MapCollection[coordinate.Key].CellAt(coordinate.X, coordinate.Y);
                yield return cell;
            }
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