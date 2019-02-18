using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class EquipmentMapping : IEntityComponent
    {
        public BodyPartType BodyPart;
        public EquipmentSlot Slot;
    }
}