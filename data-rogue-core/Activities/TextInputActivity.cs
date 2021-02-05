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

        public TextInputActivity(IActivitySystem activitySystem, string staticText, Action<string> callback = null) 
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

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var minWidth = 64 + 32;
            var minHeight = 16;

            var label = new TextControl { Position = new Rectangle(1, 1, width, height), Parameters = Text };

            var textRenderer = systemContainer.RendererSystem.Renderer.GetRendererFor(label);
            var textSize = textRenderer.GetSize(rendererHandle, label, systemContainer, playerFov);

            var backgroundSize = new Size(Math.Max(textSize.Width + 8, minWidth), Math.Max(textSize.Height + 8, minHeight));
            var backgroundPosition = new Point(30, 30);
            var textLocation = new Point(backgroundPosition.X + 4, backgroundPosition.Y + 4);

            label.Position = new Rectangle(textLocation, textSize);

            var textBoxPosition = new Rectangle(textLocation.X, textLocation.Y + 6, textSize.Width, textSize.Height);

            var textBox = new TextBoxControl { Position = textBoxPosition, Value = InputText };

            var background = new BackgroundControl { Position = new Rectangle(backgroundPosition, backgroundSize) };

            return new List<IDataRogueControl> { background, label, textBox };
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
