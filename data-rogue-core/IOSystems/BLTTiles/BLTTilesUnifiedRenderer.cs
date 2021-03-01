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
using data_rogue_core.Utils;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesUnifiedRenderer : IUnifiedRenderer
    {
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

            Layout(systemContainer, activity, playerFov);

            Paint(systemContainer, activity, playerFov, activityIndex);
        }

        private void Paint(ISystemContainer systemContainer, IActivity activity, List<MapCoordinate> playerFov, int activityIndex)
        {
            foreach (var control in activity.Controls)
            {
                if (control.Visible)
                {
                    control.ActivityIndex = activityIndex;
                    control.Paint(_controlRenderers, _spriteManager, systemContainer, playerFov);
                }
            }
        }

        private void Layout(ISystemContainer systemContainer, IActivity activity, List<MapCoordinate> playerFov)
        {
            var activityPosition = activity.Position;

            foreach (var control in activity.Controls)
            {
                if (control.Visible)
                {
                    control.Layout(_controlRenderers, systemContainer, activity, playerFov, activityPosition);
                }
            }
        }

        public MapCoordinate GetGameplayMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            return GetMapCoordinateFromMousePosition(_ioSystemConfiguration.GameplayWindowControls.OfType<MapControl>(), cameraPosition, x, y);
        }

        public MapCoordinate GetMapEditorMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            var lookupX = cameraPosition.X - BLT.State(BLT.TK_WIDTH) / (2 * BLTTilesIOSystem.TILE_SPACING) + x / BLTTilesIOSystem.TILE_SPACING;
            var lookupY = cameraPosition.Y - BLT.State(BLT.TK_HEIGHT) / (2 * BLTTilesIOSystem.TILE_SPACING) + y / BLTTilesIOSystem.TILE_SPACING;

            return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(IEnumerable<MapControl> maps, MapCoordinate cameraPosition, int x, int y)
        {

            var onMaps = maps.Where(m => IsOnMap(m, x, y));

            var map = onMaps.Last();

            if (map.GetType() == typeof(MapControl) || map.GetType() == typeof(MinimapControl))
            {
                x -= map.Position.Left;
                y -= map.Position.Top;
                
                var lookupX = cameraPosition.X - map.Position.Width / (2 * BLTTilesIOSystem.TILE_SPACING) + x / BLTTilesIOSystem.TILE_SPACING;
                var lookupY = cameraPosition.Y - map.Position.Height / (2 * BLTTilesIOSystem.TILE_SPACING) + y / BLTTilesIOSystem.TILE_SPACING;

                return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
            }

            if (map.GetType() == typeof(MinimapControl))
            {
                x -= map.Position.Left;
                y -= map.Position.Top;

                var lookupX = cameraPosition.X - map.Position.Width / 2 + x;
                var lookupY = cameraPosition.Y - map.Position.Height / 2 + y;

                return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
            }

            return null;
        }

        private bool IsOnMap(MapControl map, int x, int y)
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
            Layout(systemContainer, activity, playerFov);

            var mousePoint = new Point(x, y);

            var onControls = activity.Controls.Where(c => c.CanHandleMouse && c.Position.Contains(mousePoint));

            return onControls.LastOrDefault();
        }
    }
}
