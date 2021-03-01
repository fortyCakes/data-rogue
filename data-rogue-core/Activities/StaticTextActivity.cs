using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;

namespace data_rogue_core.Activities
{
    public class StaticTextActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.TextInput;
        public override bool RendersEntireSpace => false;
        public override bool AcceptsInput => true;

        public virtual string Text { get; set; }
        public bool CloseOnKeyPress { get; }

        private readonly IActivitySystem _activitySystem;
        protected IEntity _displayEntity;
        private BackgroundControl Background;

        public StaticTextActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, string staticText, bool closeOnKeyPress = true, IEntity displayEntity = null) : base(position, padding)
        {
            Text = staticText;
            CloseOnKeyPress = closeOnKeyPress;
            _activitySystem = activitySystem;
            _displayEntity = displayEntity;
        }

        public override void InitialiseControls()
        {
            Background = new BackgroundControl { Position = Position, ShrinkToContents = !RendersEntireSpace };

            var flow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight };
            Background.Controls.Add(flow);

            if (_displayEntity != null)
            {
                var entityControl = new MenuEntityControl { Entity = _displayEntity };
                flow.Controls.Add(entityControl);
            }

            flow.Controls.Add(new TextControl { Parameters = Text, VerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center });
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
            if (mouse.IsLeftClick && CloseOnKeyPress)
            {
                Close();
            }
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            //throw new System.NotImplementedException();
        }

        protected virtual void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
