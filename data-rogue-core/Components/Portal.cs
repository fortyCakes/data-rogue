using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Portal : IEntityComponent
    {
        public string BranchLink;
        public MapCoordinate Destination;
        public bool Automatic = false;
    }
}
