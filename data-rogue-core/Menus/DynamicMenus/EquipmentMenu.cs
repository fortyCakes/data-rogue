using data_rogue_core.Components;
using data_rogue_core.Data;
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
    public class EquipmentMenu : Menu
    {
        private ISystemContainer _systemContainer;

        public override List<MenuAction> AvailableActions { get; set; } = new List<MenuAction> { MenuAction.Unequip, MenuAction.Examine };

        public EquipmentMenu(ISystemContainer systemContainer, IEntity equippedEntity) : base(systemContainer.ActivitySystem, "Equipment", null, GetEquipmentMenuItems(systemContainer, equippedEntity))
        {
            _systemContainer = systemContainer;
            SelectedAction = MenuAction.Unequip;
            OnSelectCallback += DoEquipmentStuff;
        }

        private static MenuItem[] GetEquipmentMenuItems(ISystemContainer systemContainer, IEntity equippedEntity)
        {
            var cancelItem = new[] { new MenuItem("Cancel", null) };

            var slots = systemContainer.EquipmentSystem.GetEquipmentSlots(equippedEntity);

            return slots.SelectMany(i => i.Value.Select(o => ConvertItemToMenuItem(i.Key, o, systemContainer, equippedEntity))).Concat(cancelItem).ToArray();
        }

        private static MenuItem ConvertItemToMenuItem(EquipmentSlot slot, EquipmentSlotDetails slotDetails, ISystemContainer systemContainer, IEntity equippedEntity)
        {
            var item = systemContainer.EquipmentSystem.GetItemInSlot(equippedEntity, slot, slotDetails);

            string slotDescription = GetSlotDescription(slotDetails, slot);

            return new MenuItem($"{slotDescription}: {item?.Get<Description>().Name ?? "(none)"}", item?.EntityId, item != null );
        }

        private static string GetSlotDescription(EquipmentSlotDetails slot, EquipmentSlot slotName)
        {
            var equipmentString = slotName.ToString();
            var indexString = slot.Index == 0 ? "" : $" {slot.Index + 1}";

            return equipmentString + indexString;
        }

        private void DoEquipmentStuff(MenuItem selectedItem, MenuAction selectedAction)
        {
            if (selectedItem.Text == "Cancel")
            {
                CloseActivity();
                return;
            }

            var item = _systemContainer.EntityEngine.Get((uint)selectedItem.Value);

            switch (selectedAction)
            {
                case MenuAction.Unequip:
                    var done = _systemContainer.EquipmentSystem.Unequip(_systemContainer.PlayerSystem.Player, item);

                    if (done)
                    {
                        SpendATurn();
                        CloseActivity();
                        _systemContainer.MessageSystem.Write($"You unequip the {item.DescriptionName}.");
                    }

                    break;
                case MenuAction.Examine:
                    var actionData = new ActionEventData { Action = ActionType.Examine, Parameters = item.EntityId.ToString() };
                    _systemContainer.EventSystem.Try(EventType.Action, _systemContainer.PlayerSystem.Player, actionData);
                    break;
                default:
                    throw new ApplicationException($"Unknown MenuAction in {nameof(EquipmentMenu)}");
            }
        }

        private void SpendATurn()
        {
            _systemContainer.EventSystem.Try(EventType.SpendTime, _systemContainer.PlayerSystem.Player, new SpendTimeEventData { Ticks = 1000 });
        }
    }
}
