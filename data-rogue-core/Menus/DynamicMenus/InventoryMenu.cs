using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Systems;

namespace data_rogue_core.Menus.DynamicMenus
{
    public class InventoryMenu : Menu
    {
        private ISystemContainer systemContainer;

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Use, MenuAction.Examine, MenuAction.Drop, MenuAction.Equip };

        public InventoryMenu(ISystemContainer systemContainer, Inventory inventory) : base(systemContainer.ActivitySystem, "Inventory", null, GetInventoryMenuItems(systemContainer, inventory))
        {
            this.systemContainer = systemContainer;
            SelectedAction = MenuAction.Use;
            OnSelectCallback += GetCallback(systemContainer);
        }

        private static MenuItem[] GetInventoryMenuItems(ISystemContainer systemContainer, Inventory inventory)
        {
            var cancelItem = new[] { new MenuItem("Cancel", null) };

            return inventory.Contents.Select(i => ConvertItemToMenuItem(i, systemContainer.EntityEngine)).Concat(cancelItem).ToArray();
        }

        private static MenuItem ConvertItemToMenuItem(uint itemId, IEntityEngine entityEngine)
        {
            var item = entityEngine.Get(itemId);

            var itemText = item.Get<Description>().Name;

            if (item.Has<Stackable>())
            {
                itemText += $" (x{item.Get<Stackable>().StackSize})";
            }

            if (item.Has<Consumable>())
            {
                itemText += $" ({item.Get<Consumable>().Uses})";
            }

            return new MenuItem(itemText, item.EntityId);
        }

        private MenuItemSelected GetCallback(ISystemContainer systemContainer)
        {
            return (item, action) => DoInventoryStuff(systemContainer, item, action);
        }

        private void DoInventoryStuff(ISystemContainer systemContainer, MenuItem selectedItem, MenuAction selectedAction)
        {
            if (selectedItem.Text == "Cancel")
            {
                systemContainer.ActivitySystem.Pop();
                return;
            }

            var item = systemContainer.EntityEngine.Get((uint)selectedItem.Value);

            switch (selectedAction)
            {
                case MenuAction.Drop:
                    CloseActivity();

                    if (systemContainer.EventSystem.Try(EventType.DropItem, systemContainer.PlayerSystem.Player, new DropItemEventData { Item = item }))
                    {
                        systemContainer.ItemSystem.DropItemFromInventory(item);
                        systemContainer.MessageSystem.Write($"You drop the {item.Get<Description>().Name}.");
                        SpendATurn(systemContainer);
                    }

                    break;
                case MenuAction.Use:
                    CloseActivity();

                    if (systemContainer.EventSystem.Try(EventType.UseItem, systemContainer.PlayerSystem.Player, new DropItemEventData {Item = item}))
                    {
                        systemContainer.ItemSystem.Use(systemContainer.PlayerSystem.Player, item);
                    }

                    break;
                case MenuAction.Equip:
                    if (item.Has<Equipment>())
                    {
                        var done = systemContainer.EquipmentSystem.Equip(systemContainer.PlayerSystem.Player, item);

                        if (done)
                        {
                            CloseActivity();
                            SpendATurn(systemContainer);
                            systemContainer.MessageSystem.Write($"You equip the {item.DescriptionName}.");
                        }
                    }

                    break;
                case MenuAction.Examine:
                    var actionData = new ActionEventData {Action = ActionType.Examine, Parameters = item.EntityId.ToString()};
                    systemContainer.EventSystem.Try(EventType.Action, systemContainer.PlayerSystem.Player, actionData);
                    break;
                default:
                    throw new ApplicationException("Unknown MenuAction in InventoryMenu");
            }
        }

        private static void SpendATurn(ISystemContainer systemContainer)
        {
            systemContainer.EventSystem.Try(EventType.SpendTime, systemContainer.PlayerSystem.Player, new SpendTimeEventData { Ticks = 1000 });
        }
    }
}
