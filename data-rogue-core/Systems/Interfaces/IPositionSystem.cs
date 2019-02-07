using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems
{
    public interface IPositionSystem : ISystem
    {
        IEnumerable<IEntity> EntitiesAt(MapCoordinate coordinate);
        IEnumerable<IEntity> EntitiesAt(MapKey mapKey, int X, int Y);

        MapCoordinate PositionOf(IEntity entity);
        void Move(Position position, Vector vector);

        void SetPosition(IEntity entity, MapCoordinate mapCoordinate);
        void SetPosition(Position position, MapCoordinate mapCoordinate);
        bool Any(MapCoordinate key);
        IEnumerable<MapCoordinate> Path(MapCoordinate origin, MapCoordinate destination);
    }
}