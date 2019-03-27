using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.Renderers;
using RLNET;
using System.Linq;

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

        public void GetTarget(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            if (sender.IsPlayer)
            {
                GetTargetForPlayer(data, callback);
            }
            else
            {
                GetTargetForNonPlayer(sender, data, callback);
            }
        }

        public void HandleMouseInput(RLMouse mouse)
        {
            var x = mouse.X;
            var y = mouse.Y;

            if (_activitySystem.Peek() is TargetingActivity activity)
            {
                IGameplayRenderer gameplayRenderer = GetGameplayRenderer();

                var hoveredLocation = gameplayRenderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                if (hoveredLocation != null)
                {
                    MapCoordinate playerPosition = _positionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

                    if (activity.TargetingActivityData.TargetingData.TargetableCellsFrom(playerPosition).Contains(hoveredLocation))
                    {
                        activity.TargetingActivityData.CurrentTarget = hoveredLocation;
                    }
                    else
                    {
                        activity.TargetingActivityData.CurrentTarget = null;
                    }
                }

                if (mouse.GetLeftClick())
                {
                    activity.Complete();
                }
            }
        }

        private IGameplayRenderer GetGameplayRenderer()
        {
            GameplayActivity gameplayActivity = (GameplayActivity)_activitySystem.ActivityStack.Single(a => a.Type == ActivityType.Gameplay);

            return gameplayActivity.Renderer;
        }

        private void GetTargetForPlayer(TargetingData data, Action<MapCoordinate> callback)
        {
            var activity = new TargetingActivity(data, callback, _systemContainer);

            _activitySystem.Push(activity);
        }

        private void GetTargetForNonPlayer(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            throw new NotImplementedException();
        }
    }
}
