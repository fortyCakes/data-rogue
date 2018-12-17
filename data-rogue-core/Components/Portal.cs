using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Portal : IEntityComponent
    {
        public string BranchLink;
        public MapCoordinate Destination;
    }
}
