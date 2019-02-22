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

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Use, MenuAction.Drop, MenuAction.Equip };

        public InventoryMenu(ISystemContainer systemContainer, Inventory inventory) : base("Inventory", GetCallback(systemContainer), GetInventoryMenuItems(systemContainer, inventory))
        {
            this.systemContainer = systemContainer;
            this.SelectedAction = MenuAction.Use;
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

            var item = systemContainer.EntityEngine.GetEntity((uint)selectedItem.Value);

            switch (selectedAction)
            {
                case MenuAction.Drop:
                    
                    if (systemContainer.EventSystem.Try(EventType.DropItem, Game.WorldState.Player, new DropItemEventData { Item = item }))
                    {
                        systemContainer.ItemSystem.DropItemFromInventory(item);
                        systemContainer.MessageSystem.Write($"You drop the {item.Get<Description>().Name}.");
                        SpendATurn(systemContainer);
                    }

                    Game.ActivityStack.Pop();
                    break;
                case MenuAction.Use:
                    if (systemContainer.EventSystem.Try(EventType.UseItem, Game.WorldState.Player, new DropItemEventData {Item = item}))
                    {
                        systemContainer.ItemSystem.Use(Game.WorldState.Player, item);
                    }

                    Game.ActivityStack.Pop();
                    break;
                case MenuAction.Equip:
                    if (item.Has<Equipment>())
                    {
                        var done = systemContainer.EquipmentSystem.Equip(Game.WorldState.Player, item);

                        if (done)
                        {
                            SpendATurn(systemContainer);
                            Game.ActivityStack.Pop();
                            systemContainer.MessageSystem.Write($"You equip the {item.DescriptionName}.");
                        }
                    }

                    break;
                default:
                    throw new ApplicationException("Unknown MenuAction in InventoryMenu");
            }
        }

        private static void SpendATurn(ISystemContainer systemContainer)
        {
            systemContainer.EventSystem.Try(EventType.SpendTime, Game.WorldState.Player, new SpendTimeEventData { Ticks = 1000 });
        }
    }
}
