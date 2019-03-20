using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Ammunition : IEntityComponent
    {
        public AmmunitionType AmmunitionType;
    }

    public class RequiresAmmunition : IEntityComponent
    {
        public AmmunitionType AmmunitionType;
    }
    
    public enum AmmunitionType
    {
        Arrow
    }
}
