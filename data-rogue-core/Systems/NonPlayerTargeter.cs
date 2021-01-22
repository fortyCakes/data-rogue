using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public static class NonPlayerTargeter
    {
        public static void GetTargetForNonPlayer(ISystemContainer systemContainer, IEntity sender, Targeting data, Action<MapCoordinate> callback)
        {
            var position = systemContainer.PositionSystem.CoordinateOf(sender);
            var player = systemContainer.PlayerSystem.Player;
            var origin = systemContainer.PositionSystem.CoordinateOf(sender);
            IEntity intendedTarget = null;

            if (data.Friendly)
            {
                intendedTarget = PickRandomFriendlyTarget(systemContainer, data, position, origin, player);
            }
            else
            {
                intendedTarget = systemContainer.PlayerSystem.Player;
            }

            if (data.Range > 0)
            {
                bool canSeeTarget = CanSeeTarget(systemContainer, data, position, origin, intendedTarget);

                if (!canSeeTarget) return;
            }

            if (intendedTarget == null)
            {
                return;
            }

            MapCoordinate targetCoordinate = systemContainer.PositionSystem.CoordinateOf(intendedTarget);

            var targetableCells = systemContainer.TargetingSystem.TargetableCellsFrom(data, position);

            if (targetableCells.Contains(targetCoordinate))
            {
                callback(targetCoordinate);
            }
        }

        private static IEntity PickRandomFriendlyTarget(ISystemContainer systemContainer, Targeting data, MapCoordinate position, MapCoordinate origin, IEntity player)
        {
            IEntity intendedTarget;
            var currentMap = systemContainer.MapSystem.MapCollection[position.Key];
            var fov = currentMap.FovFrom(systemContainer.PositionSystem, origin, data.Range);

            var friendlies = fov
                .SelectMany(c => systemContainer.PositionSystem.EntitiesAt(c))
                .Where(e => e.Has<Health>())
                .Except(new[] {player})
                .ToList();

            intendedTarget = systemContainer.Random.PickOne(friendlies);
            return intendedTarget;
        }

        private static bool CanSeeTarget(ISystemContainer systemContainer, Targeting data, MapCoordinate position, MapCoordinate origin, IEntity target)
        {
            var currentMap = systemContainer.MapSystem.MapCollection[position.Key];
            var fov = currentMap.FovFrom(systemContainer.PositionSystem, origin, data.Range);

            var checkCoord = systemContainer.PositionSystem.CoordinateOf(target);
            var canSeeTarget = fov.Contains(checkCoord);
            return canSeeTarget;
        }
    }
}