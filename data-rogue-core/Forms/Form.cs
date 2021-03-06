﻿using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using OpenTK.Input;

namespace data_rogue_core.Forms
{
    public class Form
    {
        protected readonly IActivitySystem _activitySystem;
        private Dictionary<string, FormData> _fields;
        public virtual string Title { get; }
        public FormButton Buttons { get; }
        public FormButtonSelected OnSelectCallback { get; protected set; }
        public Dictionary<string, FormData> Fields
        {
            get => _fields;
            set
            {
                _fields = value;
                SelectField(_fields.First().Key, true);
            }
        }
        public FormActivity Activity { get; set; }

        private List<string> FieldsKeyList => Fields.OrderBy(f => f.Value.Order).Select(f => f.Key).ToList();

        public delegate void FormButtonSelected(FormButton selectedButton, Form form);

        private List<string> ButtonNames => Buttons.GetFlags().Select(s => s.ToString()).ToList();

        public FormSelection FormSelection { get; set; } = new FormSelection();

        public Form(IActivitySystem activitySystem, string title, FormButton buttons, FormButtonSelected onSelectCallback)
        {
            _activitySystem = activitySystem;
            Buttons = buttons;
            OnSelectCallback = onSelectCallback;
            Title = title;
        }

        public void HandleKeyboard(KeyCombination keyboard)
        {
            if (Fields.ContainsKey(FormSelection.SelectedItem))
            {
                var selectedFormData = Fields[FormSelection.SelectedItem];
                if (selectedFormData.FormDataType == FormDataType.Text)
                {
                    HandleKeyboard_Text(keyboard);
                }
            }
        }


        public void HandleAction(ActionEventData action)
        {
            if (action == null) return;

            var handled = false;

            if (Fields.ContainsKey(FormSelection.SelectedItem))
            {
                var selectedFormData = Fields[FormSelection.SelectedItem];
                switch (selectedFormData.FormDataType)
                {
                    case FormDataType.Text:
                        break;
                    case FormDataType.MultipleChoice:
                        handled = HandleAction_MultipleChoice(action, selectedFormData);
                        break;
                    case FormDataType.StatArray:
                        handled = HandleAction_StatArray(action, selectedFormData, FormSelection.SubItem);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (!handled)
            {
                if (action.Action == ActionType.Move)
                {
                    switch (action.Parameters)
                    {
                        case "0,-1":
                            MoveUp();
                            break;
                        case "0,1":
                            MoveDown();
                            break;
                        case "1,0":
                            MoveRight();
                            break;
                        case "-1,0":
                            MoveLeft();
                            break;
                    }
                }

                if (action.Action == ActionType.Select)
                {
                    Select();
                }
            }
        }

        protected void CloseActivity()
        {
            _activitySystem.RemoveActivity(Activity);
        }

        private bool HandleAction_StatArray(ActionEventData action, FormData selectedFormData, string subItem)
        {
            if (ActionIsMoveRight(action))
            {
                ((StatsFormData)selectedFormData).ChangeStat(subItem, increase: true);
                return true;
            }
            if (ActionIsMoveLeft(action))
            {
                ((StatsFormData)selectedFormData).ChangeStat(subItem, increase: false);
                return true;
            }

            return false;
        }

        private static bool ActionIsMoveLeft(ActionEventData action)
        {
            return action.Action == ActionType.Move && action.Parameters == "-1,0";
        }

        private static bool ActionIsMoveRight(ActionEventData action)
        {
            return action.Action == ActionType.Move && action.Parameters == "1,0";
        }

        private bool HandleAction_MultipleChoice(ActionEventData action, FormData selectedFormData)
        {
            if (ActionIsMoveRight(action))
            {
                ((MultipleChoiceFormData)selectedFormData).ChangeSelection(+1);
                return true;
            }
            if (ActionIsMoveLeft(action))
            {
                ((MultipleChoiceFormData)selectedFormData).ChangeSelection(-1);
                return true;
            }

            return false;
        }

        private bool HandleKeyboard_Text(KeyCombination keyboard)
        {
            var selectedFormData = Fields[FormSelection.SelectedItem];

            if (keyboard.Key == Key.BackSpace || keyboard.Key == Key.Back)
            {
                string text = (string)selectedFormData.Value;

                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    selectedFormData.Value = text;
                }

                return true;
            }

            var enteredChar = keyboard.ToChar();
            if (enteredChar != null && selectedFormData.Value.ToString().Length < 29)
            {
                selectedFormData.Value = selectedFormData.Value.ToString() + enteredChar;
            }

            return false;
        }

        private void MoveUp()
        {
            if (FieldIsSelected() && Fields[FormSelection.SelectedItem].HasSubFields)
            {
                var done = TryMoveUpSubItem();
                if (done) return;
            }

            if (FormSelection.SelectedItem == Fields.First().Key)
            {
                SelectField(GetFirstButton(), false);
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(FormSelection.SelectedItem))
            {
                SelectField(Fields.Last().Key, false);
            }
            else
            {
                MoveIndex(-1);
            }
        }

        private void SelectField(string field, bool fromAbove)
        {
            FormSelection.SelectedItem = field;

            if (FieldIsSelected())
            {
                var selectedField = Fields[field];

                if (selectedField.HasSubFields)
                {
                    if (fromAbove)
                    {
                        FormSelection.SubItem = selectedField.GetSubItems().First();
                    }
                    else
                    {
                        FormSelection.SubItem = selectedField.GetSubItems().Last();
                    }
                }
            }
        }

        private bool TryMoveUpSubItem()
        {
            var subItem = FormSelection.SubItem;

            var subItems = Fields[FormSelection.SelectedItem].GetSubItems();

            var index = subItems.IndexOf(subItem);

            if (index == 0) return false;

            SelectSubItem(subItems[index - 1]);

            return true;
        }

        private bool TryMoveDownSubItem()
        {
            var subItem = FormSelection.SubItem;

            var subItems = Fields[FormSelection.SelectedItem].GetSubItems();

            var index = subItems.IndexOf(subItem);

            if (index == subItems.Count() - 1) return false;

            SelectSubItem(subItems[index + 1]);

            return true;
        }

        private void SelectSubItem(string subItem)
        {
            FormSelection.SubItem = subItem;
        }

        private bool FieldIsSelected()
        {
            return !ButtonNames.Contains(FormSelection.SelectedItem);
        }

        private void MoveDown()
        {
            if (FieldIsSelected() && Fields[FormSelection.SelectedItem].HasSubFields)
            {
                var done = TryMoveDownSubItem();
                if (done) return;
            }

            if (FormSelection.SelectedItem == Fields.Last().Key)
            {
                SelectField(GetFirstButton(), true);
                return;
            }
            else if (Enum.GetNames(typeof(FormButton)).Contains(FormSelection.SelectedItem))
            {
                SelectField(Fields.First().Key, true);
            }
            else
            {
                MoveIndex(+1);
            }
        }

        private void MoveLeft()
        {
            if (FormSelection.SelectedItem == ButtonNames.First())
            {
                SelectField(ButtonNames.Last(), false);
            }
            else if (ButtonNames.Contains(FormSelection.SelectedItem))
            {
                MoveButtonIndex(-1);
            }
        }

        private void MoveButtonIndex(int move)
        {
            var currentIndex = ButtonNames.IndexOf(ButtonNames.Single(f => f == FormSelection.SelectedItem));

            SelectField(ButtonNames[currentIndex + move], false);
        }

        private void MoveRight()
        {
            if (FormSelection.SelectedItem == ButtonNames.Last())
            {
                SelectField(ButtonNames.First(), false);
            }
            else if (ButtonNames.Contains(FormSelection.SelectedItem))
            {
                MoveButtonIndex(+1);
            }
        }

        public void Select()
        {
            if (ButtonNames.Contains(FormSelection.SelectedItem))
            {
                OnSelectCallback((FormButton)Enum.Parse(typeof(FormButton), FormSelection.SelectedItem), this);
            }
            else
            {
                MoveDown();
            }
        }

        private void MoveIndex(int move)
        {
            var currentIndex = FieldsKeyList.IndexOf(FormSelection.SelectedItem);

            SelectField(FieldsKeyList[currentIndex + move], move > 0);
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