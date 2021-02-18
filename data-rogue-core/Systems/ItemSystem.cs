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
                AddItemToInventory(item, inventory);
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

        private void AddItemToInventory(IEntity item, Inventory inventory)
        {
            bool addedToStack = false;

            if (item.Has<Stackable>())
            {
                foreach (var itemId in inventory.Contents)
                {
                    var itemInInventory = entityEngine.Get(itemId);
                    if (itemInInventory.Has<Stackable>() && itemInInventory.Get<Stackable>().StacksWith == item.Get<Stackable>().StacksWith)
                    {
                        itemInInventory.Get<Stackable>().StackSize += item.Get<Stackable>().StackSize;
                        DestroyItem(item);
                        addedToStack = true;
                    }
                }
            }

            if (!addedToStack)
            {
                inventory.Contents.Add(item);
            }
           
        }

        public bool DropItemFromInventory(IEntity item)
        {
            var inventory = GetInventoryContaining(item);

            if (inventory != null)
            { 
                DropItemToMap(item, inventory);

                return RemoveItemFromInventory(item);
            }

            return false;
        }

        private void DropItemToMap(IEntity item, Inventory inventory)
        {
            var owner = GetInventoryOwner(inventory);
            var position = owner.Get<Position>();

            entityEngine.AddComponent(item, new Position {MapCoordinate = (MapCoordinate) position.MapCoordinate.Clone()});
        }

        public bool RemoveItemFromInventory(IEntity item)
        {
            var inventory = GetInventoryContaining(item);

            if (inventory != null)
            {
                RemoveSingleItemFromInventory(item, inventory);
            }
            else
            {
                return false;
            }

            return true;
        }

        private void RemoveSingleItemFromInventory(IEntity item, Inventory inventory)
        {
            if (item.Has<Stackable>())
            {
                var stack = item.Get<Stackable>();

                if (stack.StackSize > 1)
                {
                    stack.StackSize--;
                }
                else
                {
                    inventory.Contents.Remove(item);
                }
            }
            else
            {
                inventory.Contents.Remove(item);
            }
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

        public void DestroyItem(IEntity item, bool all = true)
        {
            var inventory = GetInventoryContaining(item);

            if (item.Has<Stackable>() && !all)
            {
                var stack = item.Get<Stackable>();

                if (stack.StackSize > 1)
                {
                    stack.StackSize--;
                }
                else
                {
                    DestroyItem_Internal(item, inventory);
                    
                }
            }
            else
            {
                DestroyItem_Internal(item, inventory);
            }
        }
        private void DestroyItem_Internal(IEntity item, Inventory inventory)
        {
            if (inventory != null)
            {
                inventory.Contents.Remove(item);
            }

            entityEngine.Destroy(item);
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

        public bool TransferWealth(IEntity sender, IEntity reciever, string currency, int amount)
        {
            var from = GetWealthByCurrency(sender, currency);
            var to = GetWealthByCurrency(reciever, currency);

            if (from == null || from.Amount < amount)
            {
                return false;
            }
            
            if (reciever == null)
            {
                entityEngine.AddComponent(reciever, new Wealth { Currency = currency, Amount = 0 });
                to = GetWealthByCurrency(reciever, currency);
            }

            from.Amount -= amount;
            to.Amount += amount;

            if (from.Amount == 0)
            {
                entityEngine.RemoveComponent(sender, from);
            }

            return true;
        }

        private static Wealth GetWealthByCurrency(IEntity entity, string currency)
        {
            return entity.Components.OfType<Wealth>().SingleOrDefault(w => w.Currency == currency);
        }

        public bool RemoveWealth(IEntity entity, string currency, int amount)
        {
            var wealth = GetWealthByCurrency(entity, currency);

            if (wealth == null || wealth.Amount < amount)
            {
                return false;
            }

            wealth.Amount -= amount;

            return true;
        }

        public bool AddWealth(IEntity entity, string currency, int amount)
        {
            var wealth = GetWealthByCurrency(entity, currency);

            if (wealth == null)
            {
                wealth = new Wealth { Currency = currency, Amount = 0 };
                entityEngine.AddComponent(entity, wealth);
            }

            wealth.Amount += amount;

            return true;
        }

        public int CheckWealth(IEntity entity, string currency)
        {
            return entity.Components.OfType<Wealth>().SingleOrDefault(w => w.Currency == currency)?.Amount ?? 0;
        }

        public List<IEntity> GetInventory(IEntity entity)
        {
            if (!entity.Has<Inventory>()) return null;

            return entity.Get<Inventory>().Contents.Select(i => entityEngine.Get(i)).ToList();
        }

        public List<IEntity> GetSpawnableItems()
        {
            return entityEngine
                .AllEntities
                .Where(e => e.Has<Prototype>() && e.Has<Item>())
                .Where(e => !e.Get<Item>().DoNotGenerate)
                .ToList();
        }
    }
}

       