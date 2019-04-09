using BearLib;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesGameplayRenderer : IGameplayRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private readonly ISpriteManager _spriteManager;

        public BLTTilesGameplayRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _spriteManager = spriteManager;
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            return new MapCoordinate(cameraPosition.Key, x/8, y/8);
        }

        public void Render(ISystemContainer systemContainer)
        {
            BLT.Clear();

            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer);
            }
        }

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer)
        {
            for (int x = mapConfiguration.Position.Left; x < mapConfiguration.Position.Right; x++)
            {
                for (int y = mapConfiguration.Position.Top; y < mapConfiguration.Position.Bottom; y++)
                {
                    if (IsFullTile(x, y))
                    {
                        BLT.Layer(BLTLayers.Background);
                        BLT.Font("");
                        BLT.Put(x,y,_spriteManager.Tile("unknown"));
                    }
                }
            }
        }

        private static bool IsFullTile(int x, int y)
        {
            return x % BLTTilesIOSystem.TILE_SPACING == 0 && y % BLTTilesIOSystem.TILE_SPACING == 0;
        }
    }
}