using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Controls;
using System.Drawing;
using System;
using System.Linq;
using data_rogue_core.Maps;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class InformationActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Information;
        public override bool RendersEntireSpace { get; }
        public IEntity Entity { get; }
        public List<IDataRogueControl> StatsControls { get; set; }
        public bool CloseOnKeyPress { get; }
        public override bool AcceptsInput => true;

        private readonly IActivitySystem _activitySystem;
        private BackgroundControl Background;

        public InformationActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, List<IDataRogueControl> statsControls, IEntity entity, bool closeOnKeyPress = true, bool rendersEntireSpace = false): base(position, padding)
        {
            Entity = entity;
            StatsControls = statsControls;
            CloseOnKeyPress = closeOnKeyPress;
            RendersEntireSpace = rendersEntireSpace;
            _activitySystem = activitySystem;
        }

        public override void InitialiseControls()
        {
            Background = new BackgroundControl { Position = Position, ShrinkToContents = !RendersEntireSpace };
            Controls.Add(Background);

            foreach (var config in StatsControls)
            {
                Background.Controls.Add(config);
            }
        }

        public void Initialise()
        {
        }
        
        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown && CloseOnKeyPress)
            {
                Close();
            }
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
