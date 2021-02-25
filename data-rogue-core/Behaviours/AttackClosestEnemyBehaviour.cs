using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Behaviours
{
    public class AttackClosestEnemyBehaviour : BaseBehaviour
    {
        private readonly IPositionSystem _positionSystem;
        private readonly IEventSystem _eventRuleSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IMapSystem _mapSystem;
        private readonly IFactionSystem _factionSystem;

        public AttackClosestEnemyBehaviour(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _eventRuleSystem = systemContainer.EventSystem;
            _playerSystem = systemContainer.PlayerSystem;
            _mapSystem = systemContainer.MapSystem;
            _factionSystem = systemContainer.FactionSystem;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            var position = _positionSystem.CoordinateOf(entity);

            var monsterFov = _mapSystem.MapCollection[position.Key].FovFrom(_positionSystem, position, 9);

            var enemiesInFov = GetEnemiesIn(monsterFov, entity);

            if (enemiesInFov.Any())
            {
                (MapCoordinate, IEntity) closest = GetClosestEnemy(enemiesInFov, position);

                var target = closest.Item1;

                return MoveTowards(entity, position, target);
            }

            return null;
        }

        private (MapCoordinate, IEntity) GetClosestEnemy(IEnumerable<(MapCoordinate, IEntity)> enemiesInFov, MapCoordinate position)
        {
            var ordered = enemiesInFov.OrderBy(e => e.Item1.DistanceFrom(position));

            return ordered.FirstOrDefault();
        }

        private IEnumerable<(MapCoordinate, IEntity)> GetEnemiesIn(List<MapCoordinate> monsterFov, IEntity sender)
        {
            foreach(var mapCoordinate in monsterFov)
            {
                foreach(var entity in _positionSystem.EntitiesAt(mapCoordinate, false))
                {
                    if (!_factionSystem.IsSameFaction(sender, entity) && entity.Has<Health>())
                    {
                        yield return (mapCoordinate, entity);
                    }
                }
            }
        }

        private ActionEventData MoveTowards(IEntity entity, MapCoordinate position, MapCoordinate playerPosition)
        {
            var path = _positionSystem.Path(position, playerPosition);

            if (path != null)
            {
                var targetPosition = path.First();
                var vector = new Vector(0, 0);

                if (targetPosition.X > position.X) vector.X = 1;
                if (targetPosition.X < position.X) vector.X = -1;
                if (targetPosition.Y > position.Y) vector.Y = 1;
                if (targetPosition.Y < position.Y) vector.Y = -1;

                return new ActionEventData { Action = ActionType.Move, Parameters = vector.ToString(), Speed = entity.Get<Actor>().Speed };
            }

            return null;
        }
    }
}