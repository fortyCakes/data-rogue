using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using data_rogue_core.Controls;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class ConfirmActivity : BaseActivity
    {
        private ISystemContainer _systemContainer;
        private string _text;
        private Action _callback;

        public ConfirmActivity(Rectangle position, Padding padding, ISystemContainer systemContainer, string text, Action callback) : base(position, padding)
        {
            _systemContainer = systemContainer;
            _text = text;
            _callback = callback;
        }

        public override ActivityType Type => ActivityType.Menu;

        public override bool RendersEntireSpace => false;

        public override bool AcceptsInput => true;

        public bool OkSelected = true;

        public override void InitialiseControls()
        {
            var background = new BackgroundControl { Position = Position, ShrinkToContents = !RendersEntireSpace };
            var text = new TextControl { Position = new Rectangle(), Parameters = _text };
            var okButton = new ButtonControl { Position = new Rectangle(), Text = "OK" };
            var cancelButton = new ButtonControl { Position = new Rectangle(), Text = "Cancel" };

            var verticalFlow = new FlowContainerControl();
            var horizontalFlow = new FlowContainerControl { FlowDirection = FlowDirection.RightToLeft };

            okButton.IsFocused = OkSelected;
            okButton.OnClick += OkButton_OnClick;
            cancelButton.IsFocused = !OkSelected;
            cancelButton.OnClick += CancelButton_OnClick;

            Controls.Add(background);
            background.Controls.Add(verticalFlow);
            verticalFlow.Controls.Add(text);
            verticalFlow.Controls.Add(horizontalFlow);
            horizontalFlow.Controls.Add(okButton);
            horizontalFlow.Controls.Add(cancelButton);
        }

        private void CancelButton_OnClick(object sender, PositionEventHandlerArgs args)
        {
            OkSelected = false;
            Select();
        }

        private void OkButton_OnClick(object sender, PositionEventHandlerArgs args)
        {
            OkSelected = true;
            Select();
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            if (action != null)
            {
                switch(action.Action)
                {
                    case ActionType.Move:
                        OkSelected = !OkSelected;
                        break;
                    case ActionType.Select:
                        Select();
                        break;
                }
            }
        }

        private void Select()
        {
            if (OkSelected)
            {
                _callback();
            }

            _systemContainer.ActivitySystem.RemoveActivity(this);
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
        }
    }
}