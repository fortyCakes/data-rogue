using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class ConsoleUnifiedRenderer : IUnifiedRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private List<IDataRogueControlRenderer> _controlRenderers;

        public ConsoleUnifiedRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration, List<IDataRogueControlRenderer> controlRenderers)
        {
            _controlRenderers = controlRenderers;
            _ioSystemConfiguration = ioSystemConfiguration;
            _console = console;
        }

        public RLConsole _console { get; }

        public Padding ActivityPadding => new Padding(1);

        public void Render(ISystemContainer systemContainer, IActivity activity, int activityIndex)
        {
            if (activity.RendersEntireSpace)
            {
                _console.Clear();
            }

            var playerFov = systemContainer.ActivitySystem.GameplayActivity.Running ? FOVHelper.CalculatePlayerFov(systemContainer) : null;
            DoLayout(systemContainer, activity, playerFov);

            foreach (var control in activity.Controls)
            {
                control.ActivityIndex = activityIndex;
                IDataRogueControlRenderer controlRenderer = GetRendererFor(control);
                controlRenderer.Paint(_console, control, systemContainer, playerFov);
            }
        }

        private void DoLayout(ISystemContainer systemContainer, IActivity activity, List<MapCoordinate> playerFov)
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
            foreach (MapControl map in _ioSystemConfiguration.GameplayWindowControls.OfType<MapControl>())
            {
                if (IsOnMap(map, x, y))
                {
                    var lookupX = cameraPosition.X - map.Position.Width / 2 + x;
                    var lookupY = cameraPosition.Y - map.Position.Height / 2 + y;

                    return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
                }
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
            DoLayout(systemContainer, activity, playerFov);

            var mousePoint = new Point(x, y);

            var onControls = activity.Controls.Where(c => c.Position.Contains(mousePoint));

            return onControls.LastOrDefault();
        }

        public MapCoordinate GetMapEditorMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            return new MapCoordinate(cameraPosition.Key, x, y);
        }
    }
}