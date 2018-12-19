using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IPrototypeSystem : ISystem 
    {
        IEntity Create(string entityName);

        IEntity Create(IEntity entity);

        IEntity Create(int entityId);

        IEntity CreateAt(string entityName, MapCoordinate mapCoordinate);

        IEntity CreateAt(IEntity entity, MapCoordinate mapCoordinate);

        IEntity CreateAt(int entityId, MapCoordinate mapCoordinate);
    }
}
