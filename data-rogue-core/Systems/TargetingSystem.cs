using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.IOSystems;
using data_rogue_core.Utils;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;

namespace data_rogue_core.Systems
{
    public class TargetingSystem : ITargetingSystem
    {
        private IPositionSystem _positionSystem;
        private IActivitySystem _activitySystem;
        private IRendererSystem _rendererSystem;
        private ISystemContainer _systemContainer;

        public TargetingSystem(ISystemContainer systemContainer)
        {
            _positionSystem = systemContainer.PositionSystem;
            _activitySystem = systemContainer.ActivitySystem;
            _rendererSystem = systemContainer.RendererSystem;
            _systemContainer = systemContainer;
        }

        public void GetTarget(IEntity sender, Targeting data, Action<MapCoordinate> callback)
        {
            if (sender.IsPlayer)
            {
                GetTargetForPlayer(data, callback);
            }
            else
            {
                NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, sender, data, callback);
            }
        }

        private void GetTargetForPlayer(Targeting data, Action<MapCoordinate> callback)
        {
            var playerPosition = _systemContainer.PositionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

            var activity = new TargetingActivity(data, callback, _systemContainer, playerPosition, _rendererSystem.IOSystemConfiguration);

            _activitySystem.Push(activity);
        }

        public HashSet<MapCoordinate> TargetableCellsFrom(Targeting data, MapCoordinate playerPosition)
        {
            var targetableCells = new HashSet<MapCoordinate>();

            if (data.ValidVectors?.Any() == true)
            {
                foreach (var vector in data.ValidVectors)
                {
                    targetableCells.Add(playerPosition + vector);
                }
            }
            else
            {
                var range = data.Range;

                if (range == 0)
                {
                    foreach (var vector in GetAdjacentCellVectors())
                    {
                        targetableCells.Add(playerPosition + vector);
                    }
                }

                if (range == 1)
                {
                    foreach (var vector in GetAdjacentAndDiagonalCellVectors())
                    {
                        targetableCells.Add(playerPosition + vector);
                    }
                }

                if (range > 1)
                {
                    if (data.PathToTarget)
                    {
                        var map = _systemContainer.MapSystem.MapCollection[playerPosition.Key];

                        Func<Vector, bool> passableTest = (Vector v) =>
                        {
                            var entities = _systemContainer.PositionSystem.EntitiesAt(playerPosition + v);
                            var physicals = entities.Select(e => e.TryGet<Physical>()).Where(p => p != null);
                            return !physicals.Any(p => p.Passable == false);
                        };

                        var cellsWithPathTo = map.FovFrom(_systemContainer.PositionSystem, playerPosition, data.Range, passableTest);
                    }

                    for (int x = -range; x <= range; x++)
                        for (int y = -range; y <= range; y++)
                        {
                            if (x * x + y * y <= range * range && (data.TargetOrigin || x != 0 || y != 0))
                            {
                                targetableCells.Add(playerPosition + new Vector(x, y));
                            }
                        }
                }
            }

            return targetableCells;
        }

        private static Vector[] GetAdjacentCellVectors()
        {
            return new[]
            {
                new Vector(-1, 0),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, 0)
            };
        }

        private static Vector[] GetAdjacentAndDiagonalCellVectors()
        {
            return new[]
            {
                new Vector(-1, -1), new Vector(-1, 0), new Vector(-1, 1),
                new Vector(0, -1), new Vector(0, 1),
                new Vector(1, -1), new Vector(1, 0), new Vector(1, 1)
            };
        }
    }
}
