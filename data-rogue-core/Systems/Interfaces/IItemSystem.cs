using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;

namespace data_rogue_core.Systems
{
    public interface IItemSystem : ISystem, IInitialisableSystem
    {
        bool MoveToInventory(IEntity item, Inventory inventory);
        bool DropItemFromInventory(IEntity item);
        bool Use(IEntity user, IEntity item);
        void DestroyItem(IEntity item, bool all = true);

        bool TransferWealth(IEntity sender, IEntity reciever, string currency, int amount);
        bool AddWealth(IEntity entity, string currency, int amount);
        bool RemoveWealth(IEntity entity, string currency, int amount);
        int CheckWealth(IEntity entity, string currency);
        bool RemoveItemFromInventory(IEntity item);
        List<IEntity> GetInventory(IEntity entity);

        List<IEntity> GetSpawnableItems();
    }
}