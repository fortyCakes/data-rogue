using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public interface IPositionSystem : ISystem
    {
        IList<IEntity> EntitiesAt(MapCoordinate coordinate, bool includeMapCells = true);
        IList<IEntity> EntitiesAt(MapKey mapKey, int x, int y, bool includeMapCells = true);
        MapCoordinate CoordinateOf(IEntity entity);
        void Move(IEntity entity, Vector vector);
        void SetPosition(IEntity entity, MapCoordinate mapCoordinate);
        void RemovePosition(IEntity entity);
        bool Any(MapCoordinate key);
        bool IsBlocked(MapCoordinate key, bool cellsOnly = false, IEntity except = null);
        IEnumerable<MapCoordinate> Path(MapCoordinate origin, MapCoordinate destination);
        IEnumerable<MapCoordinate> DirectPath(MapCoordinate targetFrom, MapCoordinate currentTarget);
    }
}