﻿using System;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EventSystem.Rules
{
    public class DestroyCommonItemsOnDeathRule : IEventRule
    {
        public DestroyCommonItemsOnDeathRule(ISystemContainer systemContainer)
        {
            EntityEngine = systemContainer.EntityEngine;
            MessageSystem = systemContainer.MessageSystem;
            PlayerSystem = systemContainer.PlayerSystem;
            ItemSystem = systemContainer.ItemSystem;
            EquipmentSystem = systemContainer.EquipmentSystem;
        }

        public EventTypeList EventTypes => new EventTypeList { EventType.Death };
        public uint RuleOrder => 15;

        public EventRuleType RuleType => EventRuleType.EventResolution;

        private IEntityEngine EntityEngine { get; }
        public IMessageSystem MessageSystem { get; }
        public IPlayerSystem PlayerSystem { get; }
        public IItemSystem ItemSystem { get; }
        public IEquipmentSystem EquipmentSystem { get; }

        public bool Apply(EventType type, IEntity sender, object eventData)
        {
            if (sender.Has<Inventory>())
            {
                var inventoryItems = ItemSystem.GetInventory(sender);

                foreach (var item in inventoryItems.ToList())
                {
                    if (ItemIsJunk(item))
                    {
                        ItemSystem.DestroyItem(item);
                    }
                }
            }

            return true;
        }

        private bool ItemIsJunk(IEntity item)
        {
            return !item.Components.Any(c => c is Enchantment);
        }
    }
}
