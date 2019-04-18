using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class ConsoleUnifiedRenderer : IUnifiedRenderer
    {
        private Dictionary<IRendereringConfiguration, RLConsole> Consoles = new Dictionary<IRendereringConfiguration, RLConsole>();
        private IOSystemConfiguration _ioSystemConfiguration;
        private List<IDataRogueControlRenderer> _controlRenderers;

        public ConsoleUnifiedRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration, List<IDataRogueControlRenderer> controlRenderers)
        {
            _controlRenderers = controlRenderers;
            _ioSystemConfiguration = ioSystemConfiguration;
            _console = console;
        }

        public RLConsole _console { get; }

        public void Render(ISystemContainer systemContainer, IActivity activity)
        {
            _console.Clear();
            var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

            var height = _console.Height;
            var width = _console.Width;

            foreach (var control in activity.GetLayout(systemContainer, _console, _controlRenderers, playerFov, width, height))
            {
                IDataRogueControlRenderer controlRenderer = _controlRenderers.Single(s => s.DisplayType == control.GetType());
                controlRenderer.Display(_console, control, systemContainer, playerFov);
            }
        }


    }
}