using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;
using System.Windows.Forms;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesUnifiedRenderer : IUnifiedRenderer
    {
        private int _height;
        private int _width;
        private readonly ISpriteManager _spriteManager;
        private readonly List<IDataRogueControlRenderer> _controlRenderers;
        private readonly IOSystemConfiguration _ioSystemConfiguration;

        public Padding ActivityPadding => new Padding(4);

        public BLTTilesUnifiedRenderer(List<IDataRogueControlRenderer> controlRenderers, ISpriteManager spriteManager, IOSystemConfiguration configuration)
        {
            _spriteManager = spriteManager;
            _controlRenderers = controlRenderers;
            _ioSystemConfiguration = configuration;
        }

        public void Render(ISystemContainer systemContainer, IActivity activity)
        {
            BLT.Clear();

            var playerFov = systemContainer.ActivitySystem.GameplayActivity.Running ? FOVHelper.CalculatePlayerFov(systemContainer) : null;

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            foreach (var control in activity.GetLayout(systemContainer, _spriteManager, _controlRenderers, playerFov, _width, _height))
            {
                IDataRogueControlRenderer statsDisplayer = _controlRenderers.Single(s => s.DisplayType == control.GetType());
                statsDisplayer.Display(_spriteManager, control, systemContainer, playerFov);
            }
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            foreach (MapConfiguration map in _ioSystemConfiguration.MapConfigurations)
            {
                if (IsOnMap(map, x, y))
                {
                    var lookupX = cameraPosition.X - map.Position.Width / (2 * BLTTilesIOSystem.TILE_SPACING) + x / BLTTilesIOSystem.TILE_SPACING;
                    var lookupY = cameraPosition.Y - map.Position.Height / (2 * BLTTilesIOSystem.TILE_SPACING) + y / BLTTilesIOSystem.TILE_SPACING;

                    return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
                }
            }

            return null;
        }

        private bool IsOnMap(MapConfiguration map, int x, int y)
        {
            return x >= map.Position.Left && x <= map.Position.Right && y >= map.Position.Top && y < map.Position.Bottom;
        }
    }
}
