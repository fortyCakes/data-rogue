using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTEquippedItemListDisplayer : BLTBaseTextDisplayer
    {
        public override Type DisplayType => typeof(EquippedItemsListControl);

        protected override string GetText(IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as EquippedItemsListControl;

            var entity = display.Entity;

            var equipped = systemContainer.EquipmentSystem.GetEquippedItems(entity);
            equipped.OrderBy(e => e.Get<Equipment>().EquipmentSlot);

            var equipmentString = "Equipped: " + string.Join(", ", equipped.Select(e => e.GetBLTName()));

            return equipmentString;
        }
    }
}