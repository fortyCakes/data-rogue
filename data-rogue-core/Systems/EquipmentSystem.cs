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
        public void Equip(IEntity entity, IEntity equipment)
        {
            var inventory = entity.Get<Inventory>();

            var hasItem = inventory.Contents.Contains(equipment);

            var ok = hasItem && systemContainer.EventSystem.Try(EventType.EquipItem, entity, new EquipItemEventData { Equipment =  equipment});

            if (ok)
            {
                EquipmentSlotDetails slot = GetEmptySlotFor(entity, equipment);

                if (slot == null && HasExactSlotFor(entity, equipment))
                {
                    slot = UnequipItemInSlot(entity, equipment);
                }

                if (slot != null)
                {
                    var equip = entity.Get<Equipped>();

                    inventory.Contents.Remove(equipment);

                    equip.EquippedItems.Add(new EquipmentMappingListItem { Slot = slot, EquipmentId = equipment.EntityId });
                }
                else
                {
                    // Can't equip that.
                }
            }
        }

        public void Unequip(IEntity entity, IEntity equipment)
        {
            var equip = entity.Get<Equipped>();

            var equipDetails = equip.EquippedItems.SingleOrDefault(eq => eq.EquipmentId == equipment.EntityId);

            var ok = equipDetails != null && systemContainer.EventSystem.Try(EventType.UnequipItem, entity, new EquipItemEventData { Equipment = equipment });

            if (ok)
            {
                var inventory = entity.Get<Inventory>();

                equip.EquippedItems.Remove(equipDetails);

                inventory.Contents.Add(equipment);
            }
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

        private EquipmentSlotDetails UnequipItemInSlot(IEntity entity, IEntity equipment)
        {
            throw new NotImplementedException();
        }

        private bool HasExactSlotFor(IEntity entity, IEntity equipment)
        {
            var neededSlot = equipment.Get<Equipment>().EquipmentSlot;

            var slots = GetEquipmentSlots(entity);

            if (slots.ContainsKey(neededSlot) && slots[neededSlot].Count == 1)
            {
                return true;
            }

            return false;
        }

        private EquipmentSlotDetails GetEmptySlotFor(IEntity entity, IEntity equipment)
        {
            var neededSlot = equipment.Get<Equipment>().EquipmentSlot;

            List<EquipmentSlotDetails> slots = GetEquipmentSlots(entity).Where(e => e.Key == neededSlot).SelectMany(e => e.Value).ToList();

            List<EquipmentSlotDetails> usedSlots = GetUsedEquipmentSlots(entity);

            slots.RemoveAll(e => usedSlots.Contains(e));

            return slots.FirstOrDefault();
        }

        private List<EquipmentSlotDetails> GetUsedEquipmentSlots(IEntity entity)
        {
            return entity.Get<Equipped>().EquippedItems.Select(e => e.Slot).ToList();
        }

        private static void AddToSlotsDictionary(Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> slots, EquipmentSlot slot, BodyPart bodyPart)
        {
            if (slots.ContainsKey(slot))
            {
                EquipmentSlotDetails equipDetails = CreateEquipmentSlotDetails(bodyPart, slots[slot].Count);

                slots[slot].Add(equipDetails);
            }
            else
            {
                EquipmentSlotDetails equipDetails = CreateEquipmentSlotDetails(bodyPart, 0);

                slots[slot] = new List<EquipmentSlotDetails> {equipDetails};
            }
        }

        private static EquipmentSlotDetails CreateEquipmentSlotDetails(BodyPart bodyPart, int index)
        {
            Dictionary<EquipmentSlot, List<EquipmentSlotDetails>> slots;
            EquipmentSlot slot;
            var equipDetails = new EquipmentSlotDetails
            {
                BodyPartLocation = bodyPart.Location,
                BodyPartType = bodyPart.Type,
                Index = index
            };
            return equipDetails;
        }
    }
}
