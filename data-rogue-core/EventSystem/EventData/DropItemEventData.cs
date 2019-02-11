using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class DropItemEventData
    {
        public DropItemEventData()
        {
        }

        public IEntity Item { get; set; }
    }
}