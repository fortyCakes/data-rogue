using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class EquipmentSystem : BaseSystem, IEquipmentSystem
    {
        private ISystemContainer systemContainer;

        public EquipmentSystem(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }
       
        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Equipment)};
        public override SystemComponents ForbiddenComponents => new SystemComponents {typeof(Prototype)};
        public void Equip(IEntity entity, IEntity equipment)
        {
            var inventory = entity.Get<Inventory>();

            var hasItem = inventory.Contents.Contains(equipment);

            var ok = hasItem && systemContainer.EventSystem.Try(EventType.EquipItem, entity, new EquipItemEventData { Equipment =  equipment});

            if (ok)
            {
                bool slotsAvailable = HasSufficientEmptySlots(entity, equipment);

                if (!slotsAvailable && HasExactSlotsFor(entity, equipment))
                {
                    slotsAvailable = UnequipItemsInSlots(entity, equipment);
                }

                if (slotsAvailable)
                {
                    var equip = entity.Get<Equipment>();

                    inventory.Contents.Remove(equipment);

                    equip.EquippedItems.Add(equipment);
                }
                else
                {
                    // Can't equip that.
                }
            }
        }

        public void Unequip(IEntity entity, IEntity equipment)
        {
            var equip = entity.Get<Equipment>();

            var hasItem = equip.EquippedItems.Contains(equipment);

            var ok = hasItem && systemContainer.EventSystem.Try(EventType.UnequipItem, entity, new EquipItemEventData { Equipment = equipment });

            if (ok)
            {
                var inventory = entity.Get<Inventory>();

                equip.EquippedItems.Remove(equipment);

                inventory.Contents.Add(equipment);
            }
        }

        private bool UnequipItemsInSlots(IEntity entity, IEntity equipment)
        {
            throw new NotImplementedException();
        }

        private bool HasExactSlotsFor(IEntity entity, IEntity equipment)
        {
            throw new NotImplementedException();
        }

        private bool HasSufficientEmptySlots(IEntity entity, IEntity equipment)
        {
            var entitySlots = GetEquipmentSlots(entity);
            var currentlyUsedSlots = GetCurrentlyUsedSlots(entity);

            var availableSlots = new Dictionary<EquipmentSlot, int>();

            foreach (var kvp in currentlyUsedSlots)
            {
                int available = 0;
                entitySlots.TryGetValue(kvp.Key, out available);

                available = Math.Max(0, available - kvp.Value);

                availableSlots[kvp.Key] = available;
            }

            var requiredSlots = GetRequiredSlots(equipment);

            foreach (var requirement in requiredSlots)
            {
                int available = 0;
                availableSlots.TryGetValue(requirement.Key, out available);

                if (available < requirement.Value) return false;
            }

            return true;
        }

        private Dictionary<EquipmentSlot, int> GetCurrentlyUsedSlots(IEntity entity)
        {
            throw new NotImplementedException();
        }

        private Dictionary<EquipmentSlot, int> GetRequiredSlots(IEntity equipment)
        {
            throw new NotImplementedException();
        }

        private Dictionary<EquipmentSlot, int> GetEquipmentSlots(IEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
