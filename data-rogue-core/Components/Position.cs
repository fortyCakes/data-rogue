using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Position : IEntityComponent
    {
        public MapCoordinate MapCoordinate;

        public void Move(Vector vector)
        {
            MapCoordinate = MapCoordinate + vector;
        }
    }
}
