using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Portal : IEntityComponent
    {
        public StairDirection Direction;
        public MapCoordinate Destination;
    }

    public enum StairDirection
    {
        Up = 1,
        Down = 2,
        Out = 3
    }
}
