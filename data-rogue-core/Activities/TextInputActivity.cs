using data_rogue_core.Controls;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class TextInputActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.TextInput;
        public override bool RendersEntireSpace => false;
        public override bool AcceptsInput => true;

        public string Text { get; set; }

        public string InputText { get; set; } = "";

        private readonly IActivitySystem _activitySystem;
        private Action<string> _callback;

        public TextInputActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, string staticText, Action<string> callback = null) : base(position, padding)
        {
            Text = staticText;
            _activitySystem = activitySystem;
            _callback = callback;
        }

        public TextInputActivity(IActivitySystem activitySystem, string staticText, Action<string> callback = null) : base(activitySystem.DefaultPosition, activitySystem.DefaultPadding)
        {
            Text = staticText;
            _activitySystem = activitySystem;
            _callback = callback;
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key == Key.Escape)
            {
                Close();
            }

            if (keyboard.Key == Key.BackSpace || keyboard.Key == Key.Back)
            {
                string text = (string)InputText;

                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    InputText = text;
                }

                return;
            }

            var enteredChar = keyboard.ToChar();
            if (enteredChar != null)
            {
                if (InputText.ToString().Length < 29)
                {
                    InputText += enteredChar;
                }
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
            if (action != null && action.Action == ActionType.Select)
            {
                Select();
            }
        }

        private void Select()
        {
            _callback(InputText);
            Close();
        }

        public override void InitialiseControls()
        {
            var label = new TextControl { Parameters = Text };
            var textBox = new TextBoxControl { Value = InputText };
            var background = new BackgroundControl { Position = Position };
            var topFlow = new FlowContainerControl();

            topFlow.Controls.Add(label);
            topFlow.Controls.Add(textBox);

            Controls.Add(background);
            Controls.Add(topFlow);
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
