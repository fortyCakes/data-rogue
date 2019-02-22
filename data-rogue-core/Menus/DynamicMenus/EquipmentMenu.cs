using data_rogue_core.Components;
using data_rogue_core.Data;
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
    public class EquipmentMenu : Menu
    {
        private ISystemContainer systemContainer;

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Unequip };

        public EquipmentMenu(ISystemContainer systemContainer, IEntity equippedEntity) : base("Equipment", GetCallback(systemContainer), GetEquipmentMenuItems(systemContainer, equippedEntity))
        {
            this.systemContainer = systemContainer;
            this.SelectedAction = MenuAction.Unequip;
        }

        private static MenuItem[] GetEquipmentMenuItems(ISystemContainer systemContainer, IEntity equippedEntity)
        {
            var cancelItem = new[] { new MenuItem("Cancel", null) };

            var slots = systemContainer.EquipmentSystem.GetEquipmentSlots(equippedEntity);

            return slots.SelectMany(i => i.Value.Select(o => ConvertItemToMenuItem(i.Key, o, systemContainer, equippedEntity))).Concat(cancelItem).ToArray();
        }

        private static MenuItem ConvertItemToMenuItem(EquipmentSlot slot, EquipmentSlotDetails slotDetails, ISystemContainer systemContainer, IEntity equippedEntity)
        {
            var item = systemContainer.EquipmentSystem.GetItemInSlot(equippedEntity, slotDetails);

            string slotDescription = GetSlotDescription(slotDetails, slot);

            return new MenuItem($"{slotDescription}: {item?.Get<Description>().Name ?? "(none)"}", item?.EntityId, item != null );
        }

        private static string GetSlotDescription(EquipmentSlotDetails slot, EquipmentSlot slotName)
        {
            var partString = slot.BodyPartType.ToString();
            var equipmentString = " " + slotName.ToString();
            var indexString = slot.Index == 0 ? "" : $" {slot.Index + 1}";

            return partString + equipmentString + indexString;
        }

        private static MenuItemSelected GetCallback(ISystemContainer systemContainer)
        {
            return (item, action) => DoEquipmentStuff(systemContainer, item, action);
        }

        private static void DoEquipmentStuff(ISystemContainer systemContainer, MenuItem selectedItem, MenuAction selectedAction)
        {
            if (selectedItem.Text == "Cancel")
            {
                Game.ActivityStack.Pop();
                return;
            }

            var item = systemContainer.EntityEngine.GetEntity((uint)selectedItem.Value);

            switch (selectedAction)
            {
                case MenuAction.Unequip:
                    var done = systemContainer.EquipmentSystem.Unequip(Game.WorldState.Player, item);

                    if (done)
                    {
                        SpendATurn(systemContainer);
                        Game.ActivityStack.Pop();
                        systemContainer.MessageSystem.Write($"You unequip the {item.DescriptionName}.");
                    }

                    break;
                default:
                    throw new ApplicationException($"Unknown MenuAction in {nameof(EquipmentMenu)}");
            }
        }

        private static void SpendATurn(ISystemContainer systemContainer)
        {
            systemContainer.EventSystem.Try(EventType.SpendTime, Game.WorldState.Player, new SpendTimeEventData { Ticks = 1000 });
        }
    }
}
