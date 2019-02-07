using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class AttackEventData
    {
        public IEntity Defender;
        public AttackType AttackType = AttackType.Physical;
    }
}
