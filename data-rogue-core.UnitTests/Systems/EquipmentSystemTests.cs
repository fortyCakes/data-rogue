using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class EquipmentSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            systemContainer = new SystemContainer();

            prototypeSystem = Substitute.For<IPrototypeSystem>();
            systemContainer.PrototypeSystem = prototypeSystem;

            eventSystem = Substitute.For<IEventSystem>();
            systemContainer.EventSystem = eventSystem;
            systemContainer.EventSystem.Try(Arg.Any<EventType>(), null, null).ReturnsForAnyArgs(true);

            entityEngine = Substitute.For<IEntityEngine>();
            systemContainer.EntityEngine = entityEngine;

            SetUpTestMappings();

            equipmentSystem = new EquipmentSystem(systemContainer);

            entity = GetTestEntity();
        }

        private IEntity entity;

        private IEquipmentSystem equipmentSystem;
        private SystemContainer systemContainer;
        private IPrototypeSystem prototypeSystem;
        private IEntityEngine entityEngine;
        private IEventSystem eventSystem;

        [Test]
        public void GetEquipmentSlots_WithMapping_ReturnsMappedSlots()
        {
            var result = equipmentSystem.GetEquipmentSlots(entity);

            var expected = new Dictionary<EquipmentSlot, List<EquipmentSlotDetails>>
            {
                {
                    EquipmentSlot.Head, new List<EquipmentSlotDetails>
                    {
                        new EquipmentSlotDetails {BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Head, Index = 0}
                    }
                },
                {
                    EquipmentSlot.Gloves, new List<EquipmentSlotDetails>
                    {
                        new EquipmentSlotDetails {BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 0}
                    }
                },
                {
                    EquipmentSlot.Hand, new List<EquipmentSlotDetails>
                    {
                        new EquipmentSlotDetails {BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 0},
                        new EquipmentSlotDetails {BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 1}
                    }
                }
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Equip_HasSlotAndNothingEquipped_AddsToEquipped()
        {
            var item = GiveHelmTestItem(3);

            var result = equipmentSystem.Equip(entity, item);

            result.Should().BeTrue();

            var expected = new EquipmentMappingList
            {
                new EquipmentMappingListItem {EquipmentId = item.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Head, BodyPartLocation = BodyPartLocation.Main, Index = 0}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Equip_EquipItemEventFails_DoesNotEquip()
        {
            var item = GiveHelmTestItem(3);
            eventSystem.Try(EventType.EquipItem, entity, Arg.Any<object>()).Returns(false);

            var result = equipmentSystem.Equip(entity, item);

            result.Should().BeFalse();

            var expected = new EquipmentMappingList {};

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Equip_HasSlotAndSomethingEquipped_SingleSlot_ReplacesEquippedAndPutsOldInInventory()
        {
            var item = GiveHelmTestItem(3);

            equipmentSystem.Equip(entity, item);

            var newItem = GiveHelmTestItem(4);

            var result = equipmentSystem.Equip(entity, newItem);

            result.Should().BeTrue();

            var expected = new EquipmentMappingList
            {
                new EquipmentMappingListItem {EquipmentId = newItem.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Head, BodyPartLocation = BodyPartLocation.Main, Index = 0}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList { item };

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void Equip_HasNoSlotFor_DoesNotEquipAndRemainsInInventory()
        {
            var item = GiveHelmTestItem(3);

            prototypeSystem.Get("EquipmentMappings").Returns(new Entity(1, "EquipmentMappings", new IEntityComponent[0]));

            var result = equipmentSystem.Equip(entity, item);

            result.Should().BeFalse();

            var expected = new EquipmentMappingList();

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList {item};

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void Equip_HasMultipleSlots_EquipsInEmptyOnes()
        {
            var item1 = GiveHandTestItem(3);
            var item2 = GiveHandTestItem(4);

            equipmentSystem.Equip(entity, item1);
            var result = equipmentSystem.Equip(entity, item2);

            result.Should().BeTrue();

            var expected = new EquipmentMappingList{
                new EquipmentMappingListItem {EquipmentId = item1.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 0}},
                new EquipmentMappingListItem {EquipmentId = item2.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 1}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList();

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void Equip_HasMultipleFullSlots_DoesNotEquip()
        {
            var item1 = GiveHandTestItem(3);
            var item2 = GiveHandTestItem(4);
            var item3 = GiveHandTestItem(5);

            equipmentSystem.Equip(entity, item1);
            equipmentSystem.Equip(entity, item2);
            var result = equipmentSystem.Equip(entity, item3);

            result.Should().BeFalse();

            var expected = new EquipmentMappingList{
                new EquipmentMappingListItem {EquipmentId = item1.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 0}},
                new EquipmentMappingListItem {EquipmentId = item2.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 1}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList() {item3};

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void GetEquippedItems_ReturnsEquippedItems()
        {
            var item1 = GiveHandTestItem(3);
            var item2 = GiveHandTestItem(4);

            equipmentSystem.Equip(entity, item1);
            equipmentSystem.Equip(entity, item2);

            var result = equipmentSystem.GetEquippedItems(entity);

            var expected = new List<IEntity>
            {
                item1,
                item2
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Unequip_ReturnsItemToInventory()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var result = equipmentSystem.Unequip(entity, item1);

            result.Should().BeTrue();

            var equippedItems = equipmentSystem.GetEquippedItems(entity);

            var expected = new List<IEntity> { };

            equippedItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Unequip_UnequipEventFails_DoesNotUnequip()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            eventSystem.Try(EventType.UnequipItem, entity, Arg.Any<object>()).Returns(false);

            var result = equipmentSystem.Unequip(entity, item1);

            result.Should().BeFalse();

            var equippedItems = equipmentSystem.GetEquippedItems(entity);

            var expected = new List<IEntity> { item1 };

            equippedItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetItemInSlot_ItemIsInSlot_ReturnsItem()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var slot = new EquipmentSlotDetails { BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 0 };

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, slot);

            result.Should().Be(item1);
        }

        [Test]
        public void GetItemInSlot_ItemInSlotOnSameBodyPart_ReturnsNull()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var slot = new EquipmentSlotDetails { BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 0 };

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Gloves, slot);

            result.Should().BeNull();
        }

        [Test]
        public void GetItemInSlot_ItemInOtherSlot_ReturnsNull()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var slot = new EquipmentSlotDetails { BodyPartLocation = BodyPartLocation.Main, BodyPartType = BodyPartType.Arms, Index = 1 };

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, slot);

            result.Should().BeNull();
        }

        [Test]
        public void GetItemInSlot_SlotEmpty_ReturnsNull()
        {
            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, new EquipmentSlotDetails { BodyPartType = BodyPartType.Legs });

            result.Should().BeNull();
        }

        private IEntity GiveHelmTestItem(uint entityId)
        {
            var item = new Entity(entityId, "TestHelm", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Head}
            });

            entityEngine.GetEntity(entityId).Returns(item);

            entity.Get<Inventory>().Contents.Add(item);

            return item;
        }

        private IEntity GiveHandTestItem(uint entityId)
        {
            var item = new Entity(entityId, "TestHandItem", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Hand}
            });

            entityEngine.GetEntity(entityId).Returns(item);

            entity.Get<Inventory>().Contents.Add(item);

            return item;
        }

        private void SetUpTestMappings()
        {
            Entity mappings = new Entity(1, "EquipmentMappings", new IEntityComponent[]
            {
                new EquipmentMapping {BodyPart = BodyPartType.Head, Slot = EquipmentSlot.Head},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Hand},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Hand},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Gloves}
            });

            prototypeSystem.Get("EquipmentMappings").Returns(mappings);
        }

        private static IEntity GetTestEntity()
        {
            return new Entity(0, "TestEntity", new IEntityComponent[]
            {
                new BodyPart {Location = BodyPartLocation.Main, Type = BodyPartType.Head},
                new BodyPart {Location = BodyPartLocation.Main, Type = BodyPartType.Arms},
                new Inventory(),
                new Equipped()
            });
        }
    }
}