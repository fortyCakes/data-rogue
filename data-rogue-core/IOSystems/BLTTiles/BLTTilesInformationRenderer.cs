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

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesInformationRenderer : IInformationRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private int _height;
        private int _width;
        private readonly ISpriteManager _spriteManager;
        private readonly List<IStatsRendererHelper> _statsDisplayers;

        public BLTTilesInformationRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _spriteManager = spriteManager;

            _statsDisplayers = BLTStatsRendererHelper.DefaultStatsDisplayers.OfType<IStatsRendererHelper>().ToList();

            _statsDisplayers.AddRange(ioSystemConfiguration.AdditionalStatsDisplayers);
        }

        public void Render(ISystemContainer systemContainer, List<StatsConfiguration> statsDisplays, bool rendersEntireSpace)
        {
            BLT.Clear();

            var playerFov = FOVHelper.CalculatePlayerFov(systemContainer);

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            BLTTilesBackgroundRenderer.RenderBackground(_width, _height, _spriteManager.Get("textbox_blue"));

            foreach (var statsConfiguration in statsDisplays)
            {
                RenderStats(statsConfiguration, systemContainer, playerFov);
            }
        }

        private void RenderStats(StatsConfiguration statsConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var player = systemContainer.PlayerSystem.Player;
            int y = statsConfiguration.Position.Top;

            foreach (StatsDisplay display in statsConfiguration.Displays)
            {
                IStatsRendererHelper statsDisplayer = _statsDisplayers.Single(s => s.DisplayType == display.DisplayType);
                var tuple = new ValueTuple<int, ISpriteManager>(statsConfiguration.Position.Left, _spriteManager);
                statsDisplayer.Display(tuple, display, systemContainer, player, playerFov, ref y);
            }
        }
    }
}
