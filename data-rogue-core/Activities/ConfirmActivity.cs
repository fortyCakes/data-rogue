using System;
using System.Collections.Generic;
using System.Drawing;
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

        public ConfirmActivity(ISystemContainer systemContainer, string text, Action callback)
        {
            _systemContainer = systemContainer;
            _text = text;
            _callback = callback;
        }

        public override ActivityType Type => ActivityType.Menu;

        public override bool RendersEntireSpace => false;

        public override bool AcceptsInput => true;

        public bool OkSelected = true;

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var text = new TextControl { Position = new Rectangle(), Parameters = _text };
            var okButton = new ButtonControl { Position = new Rectangle(), Text = "OK" };
            var cancelButton = new ButtonControl { Position = new Rectangle(), Text = "Cancel" };

            SetSize(text, systemContainer, rendererHandle, controlRenderers, playerFov);
            SetSize(okButton, systemContainer, rendererHandle, controlRenderers, playerFov);
            SetSize(cancelButton, systemContainer, rendererHandle, controlRenderers, playerFov);

            text.Position = new Rectangle(new Point(renderer.ActivityPadding.Left, renderer.ActivityPadding.Top), text.Position.Size);
            okButton.Position = new Rectangle(new Point(renderer.ActivityPadding.Left, renderer.ActivityPadding.Top * 2 + text.Position.Height), okButton.Position.Size);
            cancelButton.Position = new Rectangle(new Point(renderer.ActivityPadding.Left * 2 + okButton.Position.Width, renderer.ActivityPadding.Top * 2 + text.Position.Height), cancelButton.Position.Size);

            okButton.IsFocused = OkSelected;
            okButton.OnClick += OkButton_OnClick;
            cancelButton.IsFocused = !OkSelected;
            cancelButton.OnClick += CancelButton_OnClick;

            var totalWidth = text.Position.Width + renderer.ActivityPadding.Left + renderer.ActivityPadding.Right;
            var totalHeight = text.Position.Height * 2 + renderer.ActivityPadding.Top * 2 + renderer.ActivityPadding.Bottom;

            var background = new BackgroundControl { Position = new Rectangle(0, 0, totalWidth, totalHeight) };

            var controls = new List<IDataRogueControl> { text, background, okButton, cancelButton };
            CenterControls(controls, width, height, totalWidth, totalHeight);

            return controls;
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