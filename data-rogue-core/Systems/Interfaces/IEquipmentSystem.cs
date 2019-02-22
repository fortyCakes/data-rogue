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
        void Equip(IEntity entity, IEntity equipment);

        void Unequip(IEntity entity, IEntity equipment);

        Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> GetEquipmentSlots(IEntity entity);
    }
}
