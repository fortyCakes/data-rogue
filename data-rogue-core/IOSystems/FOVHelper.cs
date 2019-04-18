using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.IOSystems
{
    public static class FOVHelper
    {
        public static FieldOfView CalculatePlayerFov(ISystemContainer systemContainer)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var playerFov = currentMap.FovFrom(playerPosition, 9);

            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

            return (FieldOfView)playerFov;
        }
    }

    public class FieldOfView : List<MapCoordinate> { }
}
