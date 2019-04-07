using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesGameplayRenderer : IGameplayRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;

        public BLTTilesGameplayRenderer(IOSystemConfiguration ioSystemConfiguration)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public void Render(ISystemContainer systemContainer)
        {
            throw new System.NotImplementedException();
        }
    }
}