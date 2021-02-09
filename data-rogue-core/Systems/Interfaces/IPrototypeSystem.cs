using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IPrototypeSystem : ISystem 
    {
        IEntity Get(string entityName);

        IEntity Get(IEntity entity);

        IEntity Get(int entityId);

        IEntity CreateAt(string entityName, MapCoordinate mapCoordinate);

        IEntity CreateAt(IEntity entity, MapCoordinate mapCoordinate);

        IEntity CreateAt(int entityId, MapCoordinate mapCoordinate);
        IEntity TryGet(string entityName);
        IEntity GetPrototype(string entityName);
    }
}
