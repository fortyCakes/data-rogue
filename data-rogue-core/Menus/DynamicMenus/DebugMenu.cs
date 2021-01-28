using System;
using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Menus.DynamicMenus
{
    public class DebugMenu : Menu
    {
        public DebugMenu(ISystemContainer systemContainer) : base(systemContainer.ActivitySystem, "Debug", null, GetDebugMenuItems())
        {
            OnSelectCallback += GetCallback(systemContainer);
        }

        private MenuItemSelected GetCallback(ISystemContainer systemContainer)
        {
            return (item, action) => ApplyAction(systemContainer, item, action);
        }

        private void ApplyAction(ISystemContainer systemContainer, MenuItem item, MenuAction action)
        {
            switch(item.Text)
            {
                case "Cancel":
                    systemContainer.ActivitySystem.Pop();
                    return;
                case "Spawn":
                    var textInput = new TextInputActivity(systemContainer.ActivitySystem, "Spawn what entity?", ExecuteSpawnCommand);
                    systemContainer.ActivitySystem.Push(textInput);
                    break;
                default:
                    throw new ApplicationException($"Unhandled menu action {item.Text} in DebugMenu.");
            }
        }

        private void ExecuteSpawnCommand(string command)
        {
            throw new NotImplementedException();
        }

        private static MenuItem[] GetDebugMenuItems()
        {
            var spawn = new MenuItem("Spawn", null);
            var learn = new MenuItem("Learn", null, false);
            var setCell = new MenuItem("SetCell", null, false);

            var cancelItem = new MenuItem("Cancel", null);

            return new MenuItem[]
            {
                spawn,
                learn,
                setCell,
                cancelItem
            };
        }
    }
}
