using System.Collections.Generic;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Systems
{
    public interface IPositionSystem : ISystem
    {
        IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate);
        IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y);

        MapCoordinate PositionOf(IEntity entity);
    }
}