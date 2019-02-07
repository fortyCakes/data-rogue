using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using data_rogue_core.Renderers;
using RLNET;

namespace data_rogue_core.Systems
{
    public class TargetingSystem : ITargetingSystem
    {
        private IPositionSystem PositionSystem;

        public TargetingSystem(IPositionSystem positionSystem)
        {
            PositionSystem = positionSystem;
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

            if (Game.ActivityStack.Peek() is TargetingActivity activity)
            {
                var gameplayRenderer = Game.RendererFactory.GetRendererFor(ActivityType.Gameplay) as IGameplayRenderer;

                var hoveredLocation = gameplayRenderer.GetMapCoordinateFromMousePosition(Game.WorldState, x, y);

                if (hoveredLocation != null)
                {
                    MapCoordinate playerPosition = PositionSystem.PositionOf(Game.WorldState.Player);

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

        private void GetTargetForPlayer(TargetingData data, Action<MapCoordinate> callback)
        {
            var activity = new TargetingActivity(data, callback, Game.RendererFactory);

            Game.ActivityStack.Push(activity);
        }

        private void GetTargetForNonPlayer(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            throw new NotImplementedException();
        }
    }
}
