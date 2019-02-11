using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

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

        public void DropItemFromInventory(IEntity item)
        {
            var inventory = GetInventoryContaining(item);

            if (inventory != null)
            {
                inventory.Contents.Remove(item);

                var owner = GetInventoryOwner(inventory);
                var position = owner.Get<Position>();

                entityEngine.AddComponent(item, new Position { MapCoordinate = (MapCoordinate)position.MapCoordinate.Clone() });
            }
            else
            {
                throw new Exception("Can't drop item: Item is not in an inventory");
            }

        }

        private Inventory GetInventoryContaining(IEntity item)
        {
            return AllInventories.SingleOrDefault(inv => inv.Contents.Contains(item));
        }

        private IEntity GetInventoryOwner(Inventory inventory)
        {
            return Entities.Single(e => e.Components.Contains(inventory));
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