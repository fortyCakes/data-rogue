using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IPrototypeSystem : ISystem 
    {
        IEntity Create(string entityName, string newName);

        IEntity Create(IEntity entity, string newName);

        IEntity Create(int entityId, string newName);

        IEntity CreateAt(string entityName, string newName, MapCoordinate mapCoordinate);

        IEntity CreateAt(IEntity entity, string newName, MapCoordinate mapCoordinate);

        IEntity CreateAt(int entityId, string newName, MapCoordinate mapCoordinate);
    }
}
