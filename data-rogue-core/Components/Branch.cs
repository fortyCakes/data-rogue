using data_rogue_core.EntitySystem;

namespace data_rogue_core.Components
{
    public class Branch : IEntityComponent
    {
        public string BranchName;
        public string GenerationType;
        public int Depth;
    }
}
