using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;

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

        private void GetTargetForPlayer(TargetingData data, Action<MapCoordinate> callback)
        {
            var playerPosition = _systemContainer.PositionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

            var activity = new TargetingActivity(data, callback, _systemContainer, playerPosition, _rendererSystem.IOSystemConfiguration);

            _activitySystem.Push(activity);
        }

        private void GetTargetForNonPlayer(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            throw new NotImplementedException();
        }
    }
}
