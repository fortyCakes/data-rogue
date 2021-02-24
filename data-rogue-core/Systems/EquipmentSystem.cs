using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class EquipmentSystem : BaseSystem, IEquipmentSystem
    {
        private const string EQUIPMENT_MAPPINGS = "EquipmentMappings";

        private ISystemContainer systemContainer;

        public EquipmentSystem(ISystemContainer systemContainer)
        {
            this.systemContainer = systemContainer;
        }
       
        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Equipped)};
        public override SystemComponents ForbiddenComponents => new SystemComponents {typeof(Prototype)};

        public bool Equip(IEntity entity, IEntity equipment)
        {
            var inventory = entity.Get<Inventory>();

            var hasItem = inventory.Contents.Contains(equipment);

            var ok = hasItem && systemContainer.EventSystem.Try(EventType.EquipItem, entity, new EquipItemEventData { Equipment =  equipment});

            if (ok)
            {
                if (!HasSlotFor(entity, equipment)) return false;

                EnsureSlotsAreEmptyFor(entity, equipment);

                Equip_WhenEmptySlotExists(entity, equipment);

                return true;
            }

            return false;
        }

        private void Equip_WhenEmptySlotExists(IEntity entity, IEntity equipment)
        {
            var equip = entity.Get<Equipped>();

            entity.Get<Inventory>().Contents.Remove(equipment);

            var emptySlots = GetEmptySlotsFor(entity, equipment);

            foreach(var emptySlot in emptySlots)
            {
                equip.EquippedItems.Add(new EquipmentMappingListItem { Slot = emptySlot, EquipmentId = equipment.EntityId });
            }
        }

        private void EnsureSlotsAreEmptyFor(IEntity entity, IEntity equipment)
        {
            var equip = equipment.Get<Equipment>();
            var requiredSlots = new List<EquipmentSlot>() { equip.EquipmentSlot };
            if (equip.AdditionalEquipmentSlot.HasValue) requiredSlots.Add(equip.AdditionalEquipmentSlot.Value);

            var allSlots = GetEquipmentSlots(entity);
            var usedSlots = GetUsedEquipmentSlots(entity);
            var emptySlots = GetEquipmentSlots(entity);
            foreach (var slot in usedSlots)
            {
                emptySlots[slot.EquipmentSlot].Remove(slot);
                if (!emptySlots[slot.EquipmentSlot].Any()) emptySlots.Remove(slot.EquipmentSlot);
            }

            foreach (var slot in requiredSlots)
            {
                if (emptySlots.ContainsKey(slot))
                {
                    emptySlots[slot].Remove(emptySlots[slot].First());
                    if (!emptySlots[slot].Any()) emptySlots.Remove(slot);
                }
                else
                {
                    UnequipItemInSlot(entity, slot);
                }
            }

        }

        public bool Unequip(IEntity entity, IEntity equipment)
        {
            var equip = entity.Get<Equipped>();

            var equipDetails = equip.EquippedItems.Where(eq => eq.EquipmentId == equipment.EntityId).ToList();

            var ok = equipDetails != null && systemContainer.EventSystem.Try(EventType.UnequipItem, entity, new EquipItemEventData { Equipment = equipment });

            if (ok)
            {
                var inventory = entity.Get<Inventory>();

                foreach (var equippedSlot in equipDetails)
                {
                    equip.EquippedItems.Remove(equippedSlot);
                }

                inventory.Contents.Add(equipment);

                return true;
            }

            return false;
        }

        public Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> GetEquipmentSlots(IEntity entity)
        {
            var slots = new Dictionary<EquipmentSlot, List<EquipmentSlotDetails>>();

            var bodyParts = entity.Components.OfType<BodyPart>();
            var mappings = systemContainer.PrototypeSystem.Get(EQUIPMENT_MAPPINGS).Components.OfType<EquipmentMapping>();

            foreach (var bodyPart in bodyParts)
            {
                var mappedSlots = mappings.Where(s => s.BodyPart == bodyPart.Type).Select(m => m.Slot);

                foreach (var slot in mappedSlots)
                {
                    AddToSlotsDictionary(slots, slot, bodyPart);
                }
            }

            return slots;
        }

        public List<IEntity> GetEquippedItems(IEntity equippedEntity)
        {
            var equipped = equippedEntity.Get<Equipped>();

            return equipped.EquippedItems.Select(e => systemContainer.EntityEngine.Get(e.EquipmentId)).ToList();
        }

        public IEntity GetItemInSlot(IEntity equippedEntity, EquipmentSlot slot, EquipmentSlotDetails slotDetails)
        {
            var equipped = equippedEntity.Get<Equipped>();

            return equipped.EquippedItems.Where(e => e.Slot == slotDetails).Select(i => systemContainer.EntityEngine.Get(i.EquipmentId)).Where(i => i.Get<Equipment>().EquipmentSlot == slot).SingleOrDefault();
        }

        private EquipmentSlotDetails UnequipItemInSlot(IEntity entity, EquipmentSlot neededSlot)
        {
            var slot = GetFilledEquipmentSlots(entity, neededSlot).FirstOrDefault();

            if (slot != null)
            {

                var equippedItem = GetEquippedItem(entity, slot);

                Unequip(entity, equippedItem);
            }

            return slot;
        }

        private List<EquipmentSlotDetails> GetFilledEquipmentSlots(IEntity entity, EquipmentSlot neededSlot)
        {
            return GetEquipmentSlots(entity)[neededSlot].Where(s => IsItemInSlot(entity, s)).ToList();
        }

        private bool IsItemInSlot(IEntity entity, EquipmentSlotDetails slot)
        {
            var equipped = entity.Get<Equipped>();

            return equipped.EquippedItems.Any(i => i.Slot == slot);
        }

        private IEntity GetEquippedItem(IEntity entity, EquipmentSlotDetails slot)
        {
            var equipped = entity.Get<Equipped>();

            var equippedItemId = equipped.EquippedItems.First(e => e.Slot.EquipmentSlot == slot.EquipmentSlot).EquipmentId;

            return systemContainer.EntityEngine.Get(equippedItemId);
        }

        private bool HasSlotFor(IEntity entity, IEntity equipment)
        {
            var neededSlot = equipment.Get<Equipment>().EquipmentSlot;
            var additionalSlot = equipment.Get<Equipment>().AdditionalEquipmentSlot;

            var slots = GetEquipmentSlots(entity);

            if (!slots.ContainsKey(neededSlot))
            {
                return false;
            }

            var details = slots[neededSlot];
            details.Remove(details.First());

            if (!details.Any()) slots.Remove(neededSlot);

            if (additionalSlot.HasValue && !slots.ContainsKey(additionalSlot.Value))
            {
                return false;
            }

            return true;
        }

        private List<EquipmentSlotDetails> GetEmptySlotsFor(IEntity entity, IEntity equipment)
        {
            var neededSlot = equipment.Get<Equipment>().EquipmentSlot;
            var additionalSlot = equipment.Get<Equipment>().AdditionalEquipmentSlot;

            List<EquipmentSlotDetails> slots = SlotsOfType(entity, neededSlot);

            List<EquipmentSlotDetails> usedSlots = GetUsedEquipmentSlots(entity);

            slots.RemoveAll(e => usedSlots.Contains(e));

            var slotForNeeded = slots.FirstOrDefault();

            if (additionalSlot.HasValue)
            {
                slots = SlotsOfType(entity, additionalSlot.Value);
                slots.RemoveAll(e => usedSlots.Contains(e));
                slots.Remove(slotForNeeded);

                var slotForAdditional = slots.First();

                return new List<EquipmentSlotDetails> { slotForNeeded, slotForAdditional };
            }

            return new List<EquipmentSlotDetails> { slotForNeeded };
        }

        private List<EquipmentSlotDetails> SlotsOfType(IEntity entity, EquipmentSlot neededSlot)
        {
            return GetEquipmentSlots(entity).Where(e => e.Key == neededSlot).SelectMany(e => e.Value).ToList();
        }

        private List<EquipmentSlotDetails> GetUsedEquipmentSlots(IEntity entity)
        {
            return entity.Get<Equipped>().EquippedItems.Select(e => e.Slot).ToList();
        }

        private static void AddToSlotsDictionary(Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> slots, EquipmentSlot slot, BodyPart bodyPart)
        {
            if (slots.ContainsKey(slot))
            {
                EquipmentSlotDetails equipDetails = CreateEquipmentSlotDetails(slot, bodyPart, slots[slot].Count);

                slots[slot].Add(equipDetails);
            }
            else
            {
                EquipmentSlotDetails equipDetails = CreateEquipmentSlotDetails(slot, bodyPart, 0);

                slots[slot] = new List<EquipmentSlotDetails> {equipDetails};
            }
        }

        private static EquipmentSlotDetails CreateEquipmentSlotDetails(EquipmentSlot slot, BodyPart bodyPart, int index)
        {
            return new EquipmentSlotDetails(slot, bodyPart.Type, bodyPart.Location, index);
        }
    }
}
