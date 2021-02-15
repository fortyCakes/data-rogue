using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class Equipment : IEntityComponent
    {
        public EquipmentSlot EquipmentSlot;
        public EquipmentSlot? AdditionalEquipmentSlot;
    }
}
