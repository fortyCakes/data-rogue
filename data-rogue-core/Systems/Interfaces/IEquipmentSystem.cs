using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IEquipmentSystem: ISystem, IInitialisableSystem
    {
        bool Equip(IEntity entity, IEntity equipment);

        bool Unequip(IEntity entity, IEntity equipment);

        Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> GetEquipmentSlots(IEntity entity);

        List<IEntity> GetEquippedItems(IEntity equippedEntity);

        IEntity GetItemInSlot(IEntity equippedEntity, EquipmentSlot slot, EquipmentSlotDetails slotDetails);
    }
}
