using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;

namespace data_rogue_core.Activities
{
    public class GameplayActivity : IActivity
    {
        public ActivityType Type => ActivityType.Gameplay;
        public object Data => null;

        public bool Running { get; set; } = false;

        public bool RendersEntireSpace => true;
        public IGameplayRenderer Renderer { get; set; }
        private IPathfindingAlgorithm _pathfindingAlgorithm = new AStarPathfindingAlgorithm();

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IGameplayRenderer)renderer;
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (systemContainer.TimeSystem.WaitingForInput && action != null)
            {
                systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
            }
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            //throw new System.NotImplementedException();
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            MapCoordinate mapCoordinate = Renderer.GetMapCoordinateFromMousePosition(systemContainer.RendererSystem.CameraPosition, mouse.X, mouse.Y);
            systemContainer.ControlSystem.HoveredCoordinate = mapCoordinate;
            var player = systemContainer.PlayerSystem.Player;

            if (mouse.IsLeftClick && systemContainer.TimeSystem.WaitingForInput)
            {
                var playerLocation = systemContainer.PositionSystem.CoordinateOf(player);
                var map = systemContainer.MapSystem.MapCollection[systemContainer.RendererSystem.CameraPosition.Key];
                var path = _pathfindingAlgorithm.Path(map, playerLocation, mapCoordinate);

                if (path != null)
                {
                    var action = new ActionEventData { Action = ActionType.FollowPath, Parameters = string.Join(";", path.Select(m => m.ToString())) };

                    systemContainer.EventSystem.Try(EventType.Action, player, action);
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

                    var action = new ActionEventData {Action = ActionType.Examine, Parameters = entityToShow.EntityId.ToString()};

                    systemContainer.EventSystem.Try(EventType.Action, player, action);
                }
            }
        }

        public IEnumerable<IDataRogueControl> GetLayout(int width, int height)
        {
            throw new System.NotImplementedException();
        }
    }
}