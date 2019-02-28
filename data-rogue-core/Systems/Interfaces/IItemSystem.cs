using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public interface IItemSystem : ISystem, IInitialisableSystem
    {
        bool MoveToInventory(IEntity item, Inventory inventory);
        bool DropItemFromInventory(IEntity item);
        bool Use(IEntity user, IEntity item);
        bool DestroyItem(IEntity item);
    }
}