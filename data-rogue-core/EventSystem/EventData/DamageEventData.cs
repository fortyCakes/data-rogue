using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class DamageEventData
    {
        public int Damage { get; set; }

        public IEntity DamagedBy;
        public bool Absorbed;
    }
}
