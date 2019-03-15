using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{

    public class DefenceEventData
    {
        public AttackEventData ForAttack { get; set; }
        public string DefenceType { get; set; }
    }
}
