using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class AttackDefinition : IEntityComponent
    {
        public string AttackClass;
        public bool SpendTime;
        public string Damage;
        public int? Accuracy;
        public string AttackName;
        public ulong? Speed;
        public string Tags;
    }
}