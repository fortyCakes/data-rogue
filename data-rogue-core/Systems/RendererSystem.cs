using System;
using data_rogue_core.Components;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class RendererSystem: IRendererSystem
    {
        private IPlayerSystem playerSystem;

        public RendererSystem(IPlayerSystem playerSystem)
        {
            this.playerSystem = playerSystem;
        }

        public IRendererFactory RendererFactory { get; set; }
        public MapCoordinate CameraPosition => playerSystem.Player.Get<Position>().MapCoordinate;

        public IOSystemConfiguration IOSystemConfiguration { get; set; }
    }
}