using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class ConsoleInformationRenderer : IInformationRenderer
    {
        private Dictionary<IRendereringConfiguration, RLConsole> Consoles = new Dictionary<IRendereringConfiguration, RLConsole>();
        private IOSystemConfiguration _ioSystemConfiguration;
        private List<IStatsRendererHelper> statsDisplayers;

        public ConsoleInformationRenderer(RLConsole console, IOSystemConfiguration ioSystemConfiguration)
        {
            _ioSystemConfiguration = ioSystemConfiguration;

            statsDisplayers = RLNetStatsRendererHelper.DefaultStatsDisplayers.OfType<IStatsRendererHelper>().ToList();


            statsDisplayers.AddRange(ioSystemConfiguration.AdditionalStatsDisplayers);
            Console = console;
        }

        public RLConsole Console { get; }

        public void Render(ISystemContainer systemContainer, List<StatsConfiguration> statsDisplays, bool rendersEntireSpace)
        {
            Console.Clear();

            var consoles = new Dictionary<IRendereringConfiguration, RLConsole>();
            foreach (IRendereringConfiguration config in statsDisplays)
            {
                consoles.Add(config, new RLConsole(config.Position.Width, config.Position.Height));
            }

            var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

            foreach (var statsConfiguration in _ioSystemConfiguration.StatsConfigurations)
            {
                RenderStats(statsConfiguration, systemContainer, playerFov);
            }
        }

        private void RenderStats(StatsConfiguration statsConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var statsConsole = Consoles[statsConfiguration];

            statsConsole.Clear();

            var player = systemContainer.PlayerSystem.Player;
            int line = 1;

            foreach (StatsDisplay display in statsConfiguration.Displays)
            {
                var statsDisplayer = statsDisplayers.Single(s => s.DisplayType == display.DisplayType);
                statsDisplayer.Display(statsConsole, display, systemContainer, player, playerFov, ref line);
            }

            RLConsole.Blit(statsConsole, 0, 0, statsConsole.Width, statsConsole.Height, Console, statsConfiguration.Position.Left, statsConfiguration.Position.Top);
        }
    }
}