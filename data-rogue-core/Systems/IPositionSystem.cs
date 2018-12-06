using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Collections.Generic;

namespace data_rogue_core.Renderers
{
    public interface IPositionSystem : ISystem
    {
        IEnumerable<Entity> EntitiesAt(MapCoordinate coordinate);
        IEnumerable<Entity> EntitiesAt(MapKey mapKey, int X, int Y);

        MapCoordinate PositionOf(Entity entity);
    }
}