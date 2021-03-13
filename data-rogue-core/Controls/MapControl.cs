using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Linq;

namespace data_rogue_core.Controls
{

    public class MapControl : BaseControl
    {
        private IPathfindingAlgorithm _pathfindingAlgorithm = new AStarPathfindingAlgorithm();

        public override bool CanHandleMouse => true;
        public override bool FillsContainer => true;

        public override ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer)
        {
            MapCoordinate mapCoordinate = GetMapCoordinate(mouse, systemContainer);
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

        protected virtual MapCoordinate GetMapCoordinate(MouseData mouse, ISystemContainer systemContainer)
        {
            return systemContainer.RendererSystem.Renderer.GetGameplayMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
        }

        private bool CanAutowalkToCoordinate(ISystemContainer systemContainer, MapCoordinate mapCoordinate)
        {
            return
                mapCoordinate != null &&
                systemContainer.MapSystem.MapCollection[mapCoordinate.Key].SeenCoordinates.Contains(mapCoordinate);
        }
    }
}
