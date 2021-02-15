using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Equipped : IEntityComponent
    {
        public EquipmentMappingList EquippedItems = new EquipmentMappingList();
    }
}
