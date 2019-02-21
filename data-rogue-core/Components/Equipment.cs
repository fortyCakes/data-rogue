using System;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Equipment : IEntityComponent
    {
        public EquipmentMappingList EquippedItems;
    }
}
