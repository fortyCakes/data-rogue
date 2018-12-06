using System.Collections.Generic;
using System.Linq;
using RLNET;

namespace data_rogue_core.Data
{
    public class Menu
    {
        public string MenuName { get; set; }

        public List<MenuItem> MenuItems { get; private set; }

        public delegate void MenuItemSelected(MenuItem selectedItem);

        private MenuItemSelected OnSelectCallback;

        public MenuItem SelectedItem { get; private set; }

        public Menu(string menuName, MenuItemSelected onSelectCallback, params MenuItem[] items)
        {
            MenuItems = new List<MenuItem>(items);
            MenuName = menuName;
            OnSelectCallback = onSelectCallback;
            SelectedItem = MenuItems.FirstOrDefault();
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
                OnSelectCallback?.Invoke(SelectedItem);
            }
        }

        public void HandleKeyPress(RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Up:
                        Previous();
                        break;
                    case RLKey.Down:
                        Next();
                        break;
                    case RLKey.Enter:
                        Select();
                        break;
                }
            }
        }
    }
}