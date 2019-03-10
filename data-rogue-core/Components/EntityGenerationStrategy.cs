using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class EntityGenerationStrategy : IEntityComponent
    {
        public string EntityGenerationType;
        public int BasePower = 0;
        public int PowerIncrement = 1;
        public decimal Density;
    }
}
