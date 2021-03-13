using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
        private ButtonControl OkButton;
        private ButtonControl CancelButton;

        public override void InitialiseControls()
        {
            var flow = new FlowContainerControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                ApplyAlignment = true,
                Padding = new Padding(10)
            };


            var background = new BackgroundControl { ShrinkToContents = true, Padding = new Padding(2) };
            var text = new TextControl { Position = new Rectangle(), Parameters = _text, Margin = new Padding(1) };
            OkButton = new ButtonControl { Position = new Rectangle(), Text = "OK", Margin = new Padding(1) };
            CancelButton = new ButtonControl { Position = new Rectangle(), Text = "Cancel", Margin = new Padding(1) };

            var verticalFlow = new FlowContainerControl() { ShrinkToContents = true };
            var horizontalFlow = new FlowContainerControl { FlowDirection = FlowDirection.LeftToRight, ShrinkToContents = true };

            OkButton.IsFocused = OkSelected;
            OkButton.OnClick += OkButton_OnClick;
            CancelButton.IsFocused = !OkSelected;
            CancelButton.OnClick += CancelButton_OnClick;

            Controls.Add(flow);
            flow.Controls.Add(background);
            background.Controls.Add(verticalFlow);
            verticalFlow.Controls.Add(text);
            verticalFlow.Controls.Add(horizontalFlow);
            horizontalFlow.Controls.Add(OkButton);
            horizontalFlow.Controls.Add(CancelButton);
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
                        SetButtonFocused();
                        break;
                    case ActionType.Select:
                        Select();
                        break;
                }
            }
        }

        private void SetButtonFocused()
        {
            OkButton.IsFocused = OkSelected;
            CancelButton.IsFocused = !OkSelected;
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