using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class DamageEventData
    {
        public int Damage { get; set; }

        public bool Overwhelming = false;

        public IEntity DamagedBy;
    }
}
