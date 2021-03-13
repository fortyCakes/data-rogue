using System;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class RendererSystem : IRendererSystem
    {
        private readonly IPlayerSystem _playerSystem;
        private readonly IActivitySystem _activitySystem;

        public RendererSystem(IPlayerSystem playerSystem, IActivitySystem activitySystem, IUnifiedRenderer renderer)
        {
            _playerSystem = playerSystem;
            _activitySystem = activitySystem;
            Renderer = renderer;
        }

        public IUnifiedRenderer Renderer { get; set; }

        public bool LockCameraToPlayer { get; set; } = true;
        public MapCoordinate CameraPosition {
            get
            {
                var mapActivity = _activitySystem.GetMapActivity();

                return mapActivity?.CameraPosition;
            }
        }

        public IOSystemConfiguration IOSystemConfiguration { get; set; }
    }
}