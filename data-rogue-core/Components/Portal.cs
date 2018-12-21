using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Portal : IEntityComponent
    {
        public uint? BranchLink;
        public MapCoordinate Destination;
    }
}
