using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Inventory : IEntityComponent
    {
        public int Capacity = 26;
        public EntityReferenceList Contents = new EntityReferenceList();
    }
}
