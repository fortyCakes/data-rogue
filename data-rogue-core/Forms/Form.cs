using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Forms;
using data_rogue_core.Utils;
using RLNET;

namespace data_rogue_core.Forms
{
    public class Form
    {
        public string Title { get; }
        public FormButton Buttons { get; }
        public FormButtonSelected OnSelectCallback { get; }
        public List<FormData> FormData { get; set; }

        public delegate void FormButtonSelected(FormButton selectedButton, Form form);

        private List<string> ButtonNames => Buttons.GetFlags().Select(s => s.ToString()).ToList();

        public string Selected { get; set; } = "";

        public Form(string title, FormButton buttons, FormButtonSelected onSelectCallback, params FormData[] formData)
        {
            Buttons = buttons;
            OnSelectCallback = onSelectCallback;
            FormData = formData.ToList();

            Selected = FormData.First().Name;
        }

        public void HandleKeyPress(RLKeyPress keyPress)
        {
            switch(keyPress.Key)
            {
                case RLKey.Up:
                    MoveUp();
                    break;
                case RLKey.Down:
                    MoveDown();
                    break;
                case RLKey.Left:
                    MoveLeft();
                    break;
                case RLKey.Right:
                    MoveRight();
                    break;
                case RLKey.Enter:
                    Select();
                    break;
            }

            var selectedFormData = FormData.SingleOrDefault(f => f.Name == Selected);
            if (selectedFormData != null)
            {
                switch (selectedFormData.FormDataType)
                {
                    case FormDataType.Text:
                        HandleKeyPressText(keyPress, selectedFormData);

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static void HandleKeyPressText(RLKeyPress keyPress, FormData selectedFormData)
        {
            if (keyPress.Key >= RLKey.A && keyPress.Key <= RLKey.Z || keyPress.Key == RLKey.Space)
            {
                if (selectedFormData.Value.ToString().Length < 29)
                {
                    var key = keyPress.Key == RLKey.Space ? " " : keyPress.Key.ToString();

                    selectedFormData.Value += keyPress.Shift ? key.ToUpper() : key.ToLower();
                }
            }

            if (keyPress.Key == RLKey.BackSpace)
            {
                string text = (string)selectedFormData.Value;

                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    selectedFormData.Value = text;
                }
            }
        }

        private void MoveUp()
        {
            if (Selected == FormData.First().Name)
            {
                Selected = GetFirstButton();
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(Selected))
            {
                Selected = FormData.Last().Name;
            }
            else
            {
                MoveIndex(-1);
            }
        }

        private void MoveDown()
        {
            if (Selected == FormData.Last().Name)
            {
                Selected = GetFirstButton();
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(Selected))
            {
                Selected = FormData.First().Name;
            }
            else
            {
                MoveIndex(+1);
            }
        }

        private void MoveLeft()
        {
            if (Selected == ButtonNames.First())
            {
                Selected = ButtonNames.Last();
            }
            else if (ButtonNames.Contains(Selected))
            {
                MoveButtonIndex(-1);
            }
        }

        private void MoveButtonIndex(int move)
        {
            var currentIndex = ButtonNames.IndexOf(ButtonNames.Single(f => f == Selected));

            Selected = ButtonNames[currentIndex + move];
        }

        private void MoveRight()
        {
            if (Selected == ButtonNames.Last())
            {
                Selected = ButtonNames.First();
            }
            else if (ButtonNames.Contains(Selected))
            {
                MoveButtonIndex(+1);
            }
        }

        private void Select()
        {
            if (ButtonNames.Contains(Selected))
            {
                OnSelectCallback((FormButton)Enum.Parse(typeof(FormButton), Selected), this);
            }
            else
            {
                MoveDown();
            }
        }

        private void MoveIndex(int move)
        {
            var currentIndex = FormData.IndexOf(FormData.Single(f => f.Name == Selected));

            Selected = FormData[currentIndex + move].Name;
        }

        private string GetFirstButton()
        {
            foreach (FormButton button in Enum.GetValues(typeof(FormButton)))
            {
                if (button != FormButton.None && Buttons.HasFlag(button))
                {
                    return button.ToString();
                }
            }

            return null;
        }
    }
}