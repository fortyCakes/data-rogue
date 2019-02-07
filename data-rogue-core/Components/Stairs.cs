using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Stairs : IEntityComponent
    {
        public StairDirection Direction;
        public MapCoordinate Destination;
    }

    public enum StairDirection
    {
        Up = 1,
        Down = 2
    }
}
