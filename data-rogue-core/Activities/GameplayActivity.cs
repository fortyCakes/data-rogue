using data_rogue_core.EventSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using data_rogue_core.Controls;
using System.Drawing;
using data_rogue_core.Components;
using System.Windows.Forms;

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

        public GameplayActivity(Rectangle position, Padding padding, IOSystemConfiguration ioSystemConfiguration, IPlayerSystem playerSystem) : base(position, padding)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _playerSystem = playerSystem;
        }

        public override void InitialiseControls()
        {
            Controls = _ioSystemConfiguration.GameplayWindowControls;

            Controls.Add(new LinesControl { Position = Position, Configuration = _ioSystemConfiguration });
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
    }
}