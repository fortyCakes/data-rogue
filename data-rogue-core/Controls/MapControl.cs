using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Controls
{
    public class MapControl : BaseControl
    {
        private IPathfindingAlgorithm _pathfindingAlgorithm = new AStarPathfindingAlgorithm();

        public override bool CanHandleMouse => true;

        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            MapCoordinate mapCoordinate = systemContainer.RendererSystem.Renderer.GetMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
            systemContainer.ControlSystem.HoveredCoordinate = mapCoordinate;

            var player = systemContainer.PlayerSystem.Player;

            if (mouse.IsLeftClick && systemContainer.TimeSystem.WaitingForInput && CanAutowalkToCoordinate(systemContainer, mapCoordinate))
            {
                var playerLocation = systemContainer.PositionSystem.CoordinateOf(player);
                var map = systemContainer.MapSystem.MapCollection[systemContainer.RendererSystem.CameraPosition.Key];
                var path = _pathfindingAlgorithm.Path(map, playerLocation, mapCoordinate);

                if (path != null)
                {
                    var action = new ActionEventData { Action = ActionType.FollowPath, Parameters = string.Join(";", path.Select(m => m.ToString())) };

                    return action;
                }
            }

            if (mouse.IsRightClick && systemContainer.TimeSystem.WaitingForInput)
            {
                var map = systemContainer.MapSystem.MapCollection[systemContainer.RendererSystem.CameraPosition.Key];

                if (map.SeenCoordinates.Contains(mapCoordinate))
                {
                    var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

                    var entities = systemContainer.PositionSystem.EntitiesAt(mapCoordinate);

                    if (!playerFov.Contains(mapCoordinate))
                    {
                        entities = entities.Where(e => e.Has<Memorable>()).ToList();
                    }

                    IEntity entityToShow = entities.OrderByDescending(e => e.Has<Appearance>() ? e.Get<Appearance>().ZOrder : int.MinValue).First();

                    var action = new ActionEventData { Action = ActionType.Examine, Parameters = entityToShow.EntityId.ToString() };

                    return action;
                }
            }

            return null;
        }



        private bool CanAutowalkToCoordinate(ISystemContainer systemContainer, MapCoordinate mapCoordinate)
        {
            return
                mapCoordinate != null &&
                systemContainer.MapSystem.MapCollection[mapCoordinate.Key].SeenCoordinates.Contains(mapCoordinate);
        }
    }
}
