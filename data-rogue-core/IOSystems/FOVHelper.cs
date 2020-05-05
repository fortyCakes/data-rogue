using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;

namespace data_rogue_core.IOSystems
{
    public static class FOVHelper
    {
        public static List<MapCoordinate> CalculatePlayerFov(ISystemContainer systemContainer)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var playerFov = currentMap.FovFrom(systemContainer.PositionSystem, playerPosition, 9);

            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

            return playerFov;
        }
    }
}
