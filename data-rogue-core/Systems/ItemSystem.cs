using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public class ItemSystem : BaseSystem, IItemSystem
    {
        private readonly IEntityEngine entityEngine;

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Inventory) };

        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        private IEnumerable<Inventory> AllInventories => Entities.Select(e => e.Get<Inventory>());

        public ItemSystem(IEntityEngine entityEngine)
        {
            this.entityEngine = entityEngine;
        }

        public void MoveToInventory(IEntity item, Inventory inventory)
        {
            var hasRemovedItemFromSomewhere = false;

            hasRemovedItemFromSomewhere |= TryRemoveItemFromMap(item);

            hasRemovedItemFromSomewhere |= TryRemoveItemFromCurrentInventory(item);

            if (hasRemovedItemFromSomewhere)
            {
                inventory.Contents.Add(item);
            }
            else
            {
                throw new ApplicationException($"Could not remove item {item} from a location");
            }
        }

        private bool TryRemoveItemFromMap(IEntity item)
        {
            if (item.Has<Position>())
            {
                entityEngine.RemoveComponent<Position>(item);

                return true;
            }

            return false;
        }

        private bool TryRemoveItemFromCurrentInventory(IEntity item)
        {
            var originalInventory = AllInventories.FirstOrDefault(inv => inv.Contents.Contains(item));

            if (originalInventory != null)
            {
                originalInventory.Contents.Remove(item);
                return true;
            }

            return false;
        }
    }
}