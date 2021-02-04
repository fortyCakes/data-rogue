using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using data_rogue_core.Controls;
using System.Drawing;
using data_rogue_core.Components;

namespace data_rogue_core.Activities
{

    public class GameplayActivity : BaseActivity, IMapActivity
    {
        public override ActivityType Type => ActivityType.Gameplay;

        public bool Running { get; set; } = false;

        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        public MapCoordinate CameraPosition => _playerSystem.Player.Get<Position>().MapCoordinate;

        private readonly IOSystemConfiguration _ioSystemConfiguration;
        private readonly IPlayerSystem _playerSystem;

        public GameplayActivity(IOSystemConfiguration ioSystemConfiguration, IPlayerSystem playerSystem)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _playerSystem = playerSystem;
        }

        public void Initialise()
        {
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (systemContainer.TimeSystem.WaitingForInput && action != null)
            {
                systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, action);
            }
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {

        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var controls = new List<IDataRogueControl>();

            var player = systemContainer.PlayerSystem.Player;

            controls.Add( new LinesControl { Position = new Rectangle(0, 0, width, height), Configuration = _ioSystemConfiguration });
            controls.AddRange(ControlFactory.GetControls(_ioSystemConfiguration.GameplayRenderingConfiguration, renderer, systemContainer, rendererHandle, controlRenderers, playerFov));

            return controls;
        }
    }
}