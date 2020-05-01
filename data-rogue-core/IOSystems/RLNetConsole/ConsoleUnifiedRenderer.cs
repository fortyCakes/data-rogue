using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class ConsoleUnifiedRenderer : IUnifiedRenderer
    {
        private Dictionary<IRenderingConfiguration, RLConsole> Consoles = new Dictionary<IRenderingConfiguration, RLConsole>();
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

        public void Render(ISystemContainer systemContainer, IActivity activity)
        {
            if (activity.RendersEntireSpace)
            {
                _console.Clear();
            }

            var playerFov = systemContainer.ActivitySystem.GameplayActivity.Running ? FOVHelper.CalculatePlayerFov(systemContainer) : null;

            var height = _console.Height;
            var width = _console.Width;

            activity.Layout(this, systemContainer, _console, _controlRenderers, playerFov, width, height);
            foreach (var control in activity.Controls)
            {
                IDataRogueControlRenderer controlRenderer = _controlRenderers.Single(s => s.DisplayType == control.GetType());
                controlRenderer.Display(_console, control, systemContainer, playerFov);
            }
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            foreach (MapConfiguration map in _ioSystemConfiguration.MapConfigurations)
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

        private bool IsOnMap(MapConfiguration map, int x, int y)
        {
            return x >= map.Position.Left && x <= map.Position.Right && y >= map.Position.Top && y < map.Position.Bottom;
        }
    }
}