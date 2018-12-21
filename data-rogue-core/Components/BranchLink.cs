using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    public class BranchLink : IEntityComponent
    {
        public BranchLinkEnd From;
        public BranchLinkEnd To;
    }
}
