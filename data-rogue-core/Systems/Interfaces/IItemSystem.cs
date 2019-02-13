using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public interface IItemSystem : ISystem, IInitialisableSystem
    {
        void MoveToInventory(IEntity item, Inventory inventory);
        void DropItemFromInventory(IEntity item);
        void Use(IEntity user, IEntity item);
        void DestroyItem(IEntity item);
    }
}