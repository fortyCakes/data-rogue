using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class BranchLink : IEntityComponent
    {
        public BranchLinkEnd From;
        public BranchLinkEnd To;
    }
}
