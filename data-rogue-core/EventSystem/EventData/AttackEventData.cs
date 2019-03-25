using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class AttackEventData
    {
        public IEntity Attacker;
        public IEntity Defender;
        public string AttackClass;
        public int? Speed;
        public int? Accuracy;
        public int? Damage;
        public int? AttackRoll;

        public IEntity Weapon;

        public bool SpendTime = true;

        public string[] Tags;
        public string AttackName;
        public string SuccessfulDefenceType;
    }
}
