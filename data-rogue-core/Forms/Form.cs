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
        public Dictionary<string, FormData> Fields { get; set; }

        private List<string> FieldsKeyList => Fields.OrderBy(f => f.Value.Order).Select(f => f.Key).ToList();

        public delegate void FormButtonSelected(FormButton selectedButton, Form form);

        private List<string> ButtonNames => Buttons.GetFlags().Select(s => s.ToString()).ToList();

        public string Selected { get; set; } = "";

        public Form(string title, FormButton buttons, FormButtonSelected onSelectCallback, Dictionary<string, FormData> fields)
        {
            Buttons = buttons;
            OnSelectCallback = onSelectCallback;
            Fields = fields;

            Selected = Fields.First().Key;
        }

        public void HandleKeyPress(RLKeyPress keyPress)
        {
            var handled = false;

            if (Fields.ContainsKey(Selected))
            {
                var selectedFormData = Fields[Selected];
                switch (selectedFormData.FormDataType)
                {
                    case FormDataType.Text:
                        handled = HandleKeyPress_Text(keyPress, selectedFormData);
                        break;
                    case FormDataType.MultipleChoice:
                        handled = HandleKeyPress_MultipleChoice(keyPress, selectedFormData);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (!handled)
            {
                switch (keyPress.Key)
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
            }
            
            
        }

        private bool HandleKeyPress_MultipleChoice(RLKeyPress keyPress, FormData selectedFormData)
        {
            if (keyPress.Key == RLKey.Right)
            {
                ((MultipleChoiceFormData)selectedFormData).ChangeSelection(+1);
                return true;
            }
            if (keyPress.Key == RLKey.Left)
            {
                ((MultipleChoiceFormData)selectedFormData).ChangeSelection(-1);
                return true;
            }

            return false;
        }

        private bool HandleKeyPress_Text(RLKeyPress keyPress, FormData selectedFormData)
        {
            if (keyPress.Key >= RLKey.A && keyPress.Key <= RLKey.Z || keyPress.Key == RLKey.Space)
            {
                if (selectedFormData.Value.ToString().Length < 29)
                {
                    var key = keyPress.Key == RLKey.Space ? " " : keyPress.Key.ToString();

                    selectedFormData.Value += keyPress.Shift ? key.ToUpper() : key.ToLower();
                }

                return true;
            }

            if (keyPress.Key == RLKey.BackSpace)
            {
                string text = (string)selectedFormData.Value;

                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    selectedFormData.Value = text;
                }
                return true;
            }

            return false;
        }

        private void MoveUp()
        {
            if (Selected == Fields.First().Key)
            {
                Selected = GetFirstButton();
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(Selected))
            {
                Selected = Fields.Last().Key;
            }
            else
            {
                MoveIndex(-1);
            }
        }

        private void MoveDown()
        {
            if (Selected == Fields.Last().Key)
            {
                Selected = GetFirstButton();
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(Selected))
            {
                Selected = Fields.First().Key;
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
            var currentIndex = FieldsKeyList.IndexOf(Selected);

            Selected = FieldsKeyList[currentIndex + move];
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