using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Menus
{
    public class Menu
    {
        public string MenuName { get; set; }

        public List<MenuItem> MenuItems { get; private set; }
        public virtual List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Select };

        public delegate void MenuItemSelected(MenuItem selectedItem, MenuAction selectedAction);

        protected MenuItemSelected OnSelectCallback;
        protected readonly IActivitySystem _activitySystem;

        public MenuItem SelectedItem { get; protected set; }
        public MenuAction SelectedAction { get; protected set; }

        public int SelectedIndex => MenuItems.IndexOf(SelectedItem);

        public Menu(IActivitySystem activitySystem , string menuName, MenuItemSelected onSelectCallback, params MenuItem[] items)
        {
            MenuItems = new List<MenuItem>(items);
            MenuName = menuName;
            OnSelectCallback += onSelectCallback;
            SelectedItem = MenuItems.FirstOrDefault();
            _activitySystem = activitySystem;
        }
        public virtual void HandleAction(ActionEventData action)
        {
            if (action != null)
            {
                if (action.Action == ActionType.Move)
                {
                    switch (action.Parameters)
                    {
                        case "0,-1":
                            Previous();
                            break;
                        case "0,1":
                            Next();
                            break;
                        case "1,0":
                            NextAction();
                            break;
                        case "-1,0":
                            PreviousAction();
                            break;
                    }
                }

                if (action.Action == ActionType.Select)
                {
                    Select();
                }
            }
        }

        private void Next()
        {
            var newIndex = MenuItems.IndexOf(SelectedItem) + 1;
            if (newIndex >= MenuItems.Count())
            {
                newIndex = 0;
            }

            SelectedItem = MenuItems[newIndex];
        }

        private void Previous()
        {
            var newIndex = MenuItems.IndexOf(SelectedItem) - 1;
            if (newIndex < 0)
            {
                newIndex = MenuItems.Count() - 1;
            }

            SelectedItem = MenuItems[newIndex];
        }
        
        private void Select()
        {
            if (SelectedItem.Enabled)
            {
                OnSelectCallback?.Invoke(SelectedItem, SelectedAction);
            }
        }

        private void PreviousAction()
        {
            var index = AvailableActions.IndexOf(SelectedAction);

            if (index == 0)
            {
                SelectedAction = AvailableActions[AvailableActions.Count - 1];
            }
            else
            {
                SelectedAction = AvailableActions[index - 1];
            }
        }

        private void NextAction()
        {
            var index = AvailableActions.IndexOf(SelectedAction);

            if (index == AvailableActions.Count - 1)
            {
                SelectedAction = AvailableActions[0];
            }
            else
            {
                SelectedAction = AvailableActions[index + 1];
            }
        }
    }
}