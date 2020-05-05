using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class KnownSkill : IEntityComponent
    {
        public string Skill;
        public int Order;
    }

    public class StartsWithItem : IEntityComponent
    {
        public string Item;
        public bool Equipped = false;
    }
}
