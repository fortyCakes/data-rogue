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
using System.Drawing;

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

        public void Render(ISystemContainer systemContainer, IActivity activity, int activityIndex)
        {
            if (activity.RendersEntireSpace)
            {
                BLT.Clear();
            }


            var playerFov = systemContainer.ActivitySystem.GameplayActivity.Running ? FOVHelper.CalculatePlayerFov(systemContainer) : null;
            DoLayout(systemContainer, activity, playerFov);

            foreach (var control in activity.Controls)
            {
                if (control.Visible)
                {
                    control.ActivityIndex = activityIndex;
                    GetRendererFor(control).Display(_spriteManager, control, systemContainer, playerFov);
                }
            }
        }

        private void DoLayout(ISystemContainer systemContainer, IActivity activity, List<MapCoordinate> playerFov)
        {
            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            activity.Layout(this, systemContainer, _spriteManager, _controlRenderers, playerFov, _width, _height);
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            var onMaps = _ioSystemConfiguration.MapConfigurations.Where(m => IsOnMap(m, x, y));

            var map = onMaps.Last();

            if (map.GetType() == typeof(MapConfiguration))
            {
                x -= map.Position.Left;
                y -= map.Position.Top;
                
                var lookupX = cameraPosition.X - map.Position.Width / (2 * BLTTilesIOSystem.TILE_SPACING) + x / BLTTilesIOSystem.TILE_SPACING;
                var lookupY = cameraPosition.Y - map.Position.Height / (2 * BLTTilesIOSystem.TILE_SPACING) + y / BLTTilesIOSystem.TILE_SPACING;

                return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
            }

            if (map.GetType() == typeof(MinimapConfiguration))
            {
                x -= map.Position.Left;
                y -= map.Position.Top;

                var lookupX = cameraPosition.X - map.Position.Width / 2 + x;
                var lookupY = cameraPosition.Y - map.Position.Height / 2 + y;

                return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
            }

            return null;
        }

        private bool IsOnMap(MapConfiguration map, int x, int y)
        {
            return x >= map.Position.Left && x <= map.Position.Right && y >= map.Position.Top && y < map.Position.Bottom;
        }

        public IDataRogueControlRenderer GetRendererFor(IDataRogueControl control)
        {
            return _controlRenderers.Single(s => s.DisplayType == control.GetType());
        }

        public IDataRogueControl GetControlFromMousePosition(ISystemContainer systemContainer, IActivity activity, MapCoordinate cameraPosition, int x, int y)
        {
            var playerFov = systemContainer.ActivitySystem.GameplayActivity.Running ? FOVHelper.CalculatePlayerFov(systemContainer) : null;
            DoLayout(systemContainer, activity, playerFov);

            var mousePoint = new Point(x, y);

            var onControls = activity.Controls.Where(c => c.Position.Contains(mousePoint));

            return onControls.LastOrDefault();
        }
    }
}
