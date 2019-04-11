using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public interface IPositionSystem : ISystem
    {
        IList<IEntity> EntitiesAt(MapCoordinate coordinate);
        IList<IEntity> EntitiesAt(MapKey mapKey, int x, int y);
        MapCoordinate CoordinateOf(IEntity entity);
        void Move(IEntity entity, Vector vector);
        void SetPosition(IEntity entity, MapCoordinate mapCoordinate);
        void RemovePosition(IEntity entity);
        bool Any(MapCoordinate key);
        IEnumerable<MapCoordinate> Path(MapCoordinate origin, MapCoordinate destination);
    }
}