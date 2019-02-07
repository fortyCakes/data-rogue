using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    class EnemiesInViewAddTensionRule : IEventRule
    {
        public EnemiesInViewAddTensionRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            PositionSystem = systemContainer.PositionSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public int RuleOrder => 100;

        private IEntityEngine EntityEngine { get; }
        public IPositionSystem PositionSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case Stat.Tension:

                    var enemies = GetEnemiesInFov();

                    data.Value += enemies.Count();

                    break;
            }

            return true;
        }

        private IEnumerable<IEntity> GetEnemiesInFov()
        {
            var world = Game.WorldState;
            var currentMap = world.Maps[world.CameraPosition.Key];

            MapCoordinate playerPosition = world.Player.Get<Position>().MapCoordinate;
            var playerFov = currentMap.FovFrom(playerPosition, 9);

            foreach(MapCoordinate coord in playerFov)
            {
                var entities = PositionSystem.EntitiesAt(coord);

                foreach(var entity in entities)
                {
                    if (IsEnemy(entity))
                    {
                        yield return entity;
                    }
                }
            }
        }

        private static bool IsEnemy(IEntity entity)
        {
            return entity.Has<Actor>() && entity != Game.WorldState.Player;
        }
    }
}
