using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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

        public StaticTextActivity(IActivitySystem activitySystem, string staticText, bool closeOnKeyPress = true, IEntity displayEntity = null) : base(activitySystem.DefaultPosition, activitySystem.DefaultPadding)
        {
            Text = staticText;
            CloseOnKeyPress = closeOnKeyPress;
            _activitySystem = activitySystem;
            _displayEntity = displayEntity;
        }

        public override void InitialiseControls()
        {
            var flow = new FlowContainerControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                ApplyAlignment = true
            };

            Background = new BackgroundControl { Position = Position, ShrinkToContents = !RendersEntireSpace, Padding = new Padding(4) };

            var innerflow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight, VerticalAlignment = VerticalAlignment.Center, ShrinkToContents = true };
            Background.Controls.Add(innerflow);

            if (_displayEntity != null)
            {
                var entityControl = new MenuEntityControl { Entity = _displayEntity, VerticalAlignment = VerticalAlignment.Center };
                innerflow.Controls.Add(entityControl);
            }

            innerflow.Controls.Add(new TextControl { Parameters = Text, VerticalAlignment = VerticalAlignment.Center, Margin = new Padding(2) });

            Controls.Add(flow);
            flow.Controls.Add(Background);
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
