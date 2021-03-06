﻿using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class EnemiesInViewAddTensionRule : IEventRule
    {
        public EnemiesInViewAddTensionRule(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _mapSystem = systemContainer.MapSystem;
            _playerSystem = systemContainer.PlayerSystem;
            _rendererSystem = systemContainer.RendererSystem;
        }

        public EventTypeList EventTypes => new EventTypeList{ EventType.GetStat };
        public EventRuleType RuleType => EventRuleType.EventResolution;
        public uint RuleOrder => 100;
        
        private readonly IPositionSystem _positionSystem;
        private readonly IMapSystem _mapSystem;
        private readonly IPlayerSystem _playerSystem;
        private readonly IRendererSystem _rendererSystem;

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            var data = (GetStatEventData)eventData;

            switch (data.Stat)
            {
                case "Tension":

                    var enemies = GetEnemiesInFov();

                    data.Value += enemies.Count();

                    break;
            }

            return true;
        }

        private IEnumerable<IEntity> GetEnemiesInFov()
        {
            var currentMap = _mapSystem.MapCollection[_rendererSystem.CameraPosition.Key];

            MapCoordinate playerPosition = _positionSystem.CoordinateOf(_playerSystem.Player);
            var playerFov = currentMap.FovFrom(_positionSystem, playerPosition, 9);

            foreach(MapCoordinate coord in playerFov)
            {
                var entities = _positionSystem.EntitiesAt(coord);

                foreach(var entity in entities)
                {
                    if (IsEnemy(entity))
                    {
                        yield return entity;
                    }
                }
            }
        }

        private bool IsEnemy(IEntity entity)
        {
            return entity.Has<Challenge>() && entity != _playerSystem.Player;
        }
    }
}
