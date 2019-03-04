using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class ItemSystem : BaseSystem, IItemSystem
    {
        private readonly IEntityEngine entityEngine;
        private readonly IPrototypeSystem prototypeSystem;
        private readonly IScriptExecutor scriptExecutor;
        private readonly IMessageSystem messageSystem;
        private readonly IEventSystem eventSystem;

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Inventory) };

        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        private IEnumerable<Inventory> AllInventories => Entities.Select(e => e.Get<Inventory>());

        public ItemSystem(IEntityEngine entityEngine, IPrototypeSystem prototypeSystem, IScriptExecutor scriptExecutor, IMessageSystem messageSystem, IEventSystem eventSystem)
        {
            this.entityEngine = entityEngine;
            this.prototypeSystem = prototypeSystem;
            this.scriptExecutor = scriptExecutor;
            this.messageSystem = messageSystem;
            this.eventSystem = eventSystem;
        }

        public bool MoveToInventory(IEntity item, Inventory inventory)
        {
            var hasRemovedItemFromSomewhere = false;

            hasRemovedItemFromSomewhere |= TryRemoveItemFromMap(item);

            hasRemovedItemFromSomewhere |= TryRemoveItemFromCurrentInventory(item);

            var hasSpace = inventory.Capacity > inventory.Contents.Count();

            if (hasRemovedItemFromSomewhere && hasSpace)
            {
                inventory.Contents.Add(item);
            }
            else
            {
                if (!hasSpace)
                {
                    messageSystem.Write("Couldn't add the item to the inventory because the inventory is full.");
                }

                return false;
            }

            return true;
        }

        public bool DropItemFromInventory(IEntity item)
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
                return false;
            }

            return true;
        }

        public bool Use(IEntity user, IEntity item)
        {
            var scriptName = item.Get<Item>().UseScript;

            if (!string.IsNullOrEmpty(scriptName))
            {
                ExecuteItemScript(user, scriptName, item);

                ApplyConsumption(item);
            }
            else
            {
                var userName = user.Get<Description>().Name;
                var itemName = item.Get<Description>().Name;
                messageSystem.Write($"{userName} tries to use {itemName}. Nothing interesting happens.");
            }

            return true;
        }

        private void ExecuteItemScript(IEntity user, string scriptName, IEntity item)
        {
            var script = prototypeSystem.Get(scriptName);

            var scriptText = script.Get<Script>().Text;

            scriptExecutor.Execute(user, scriptText, item);
        }

        private void ApplyConsumption(IEntity item)
        {
            if (item.Has<Consumable>())
            {
                var consumable = item.Get<Consumable>();

                consumable.Uses.Subtract(1);

                if (consumable.Uses.Current == 0)
                {
                    if (eventSystem.Try(EventType.ConsumableRunOut, item, null))
                    {
                        DestroyItem(item);
                    }
                }
            }
        }

        public bool DestroyItem(IEntity item)
        {
            var inventory = GetInventoryContaining(item);

            if (inventory != null)
            {
                inventory.Contents.Remove(item);
            }

            entityEngine.Destroy(item);

            return true;
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