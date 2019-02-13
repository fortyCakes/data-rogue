using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace data_rogue_core.Menus.DynamicMenus
{
    public class InventoryMenu : Menu
    {
        private ISystemContainer systemContainer;

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Drop };

        public InventoryMenu(ISystemContainer systemContainer, Inventory inventory) : base("Inventory", GetCallback(systemContainer), GetInventoryMenuItems(systemContainer, inventory))
        {
            this.systemContainer = systemContainer;
            this.SelectedAction = MenuAction.Drop;
        }

        private static MenuItem[] GetInventoryMenuItems(ISystemContainer systemContainer, Inventory inventory)
        {
            var cancelItem = new[] { new MenuItem("Cancel", null) };

            return inventory.Contents.Select(i => ConvertItemToMenuItem(i, systemContainer.EntityEngine)).Concat(cancelItem).ToArray();
        }

        private static MenuItem ConvertItemToMenuItem(uint itemId, IEntityEngine entityEngine)
        {
            var item = entityEngine.GetEntity(itemId);

            return new MenuItem(item.Get<Description>().Name, item.EntityId);
        }

        private static MenuItemSelected GetCallback(ISystemContainer systemContainer)
        {
            return (item, action) => DoInventoryStuff(systemContainer, item, action);
        }

        private static void DoInventoryStuff(ISystemContainer systemContainer, MenuItem selectedItem, MenuAction selectedAction)
        {
            if (selectedItem.Text == "Cancel")
            {
                Game.ActivityStack.Pop();
                return;
            }

            switch(selectedAction)
            {
                case MenuAction.Drop:
                    var item = systemContainer.EntityEngine.GetEntity((uint)selectedItem.Value);
                    if (systemContainer.EventSystem.Try(EventType.DropItem, Game.WorldState.Player, new DropItemEventData { Item = item }))
                    {
                        systemContainer.ItemSystem.DropItemFromInventory(item);
                        systemContainer.MessageSystem.Write($"You drop the {item.Get<Description>().Name}.");
                        systemContainer.EventSystem.Try(EventType.SpendTime, Game.WorldState.Player, new SpendTimeEventData { Ticks = 1000 });
                    }

                    Game.ActivityStack.Pop();
                    break;
                default:
                    throw new ApplicationException("Unknown MenuAction in InventoryMenu");
            }
        }
        
    }
}
