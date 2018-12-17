using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public class PositionSystem : BaseSystem, IPositionSystem
    {
        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Position) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate)
        {
            foreach(var entity in Entities)
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
    }
}