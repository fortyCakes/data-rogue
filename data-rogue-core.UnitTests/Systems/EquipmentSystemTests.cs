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

            systemContainer.EventSystem = Substitute.For<IEventSystem>();
            systemContainer.EventSystem.Try(Arg.Any<EventType>(), null, null).ReturnsForAnyArgs(true);

            entityEngine = Substitute.For<IEntityEngine>();
            systemContainer.EntityEngine = entityEngine;

            SetUpTestMappings();

            equipmentSystem = new EquipmentSystem(systemContainer);
        }

        private IEquipmentSystem equipmentSystem;
        private SystemContainer systemContainer;
        private IPrototypeSystem prototypeSystem;
        private IEntityEngine entityEngine;

        [Test]
        public void GetEquipmentSlots_WithMapping_ReturnsMappedSlots()
        {
            var entity = GetTestEntity();

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
                    EquipmentSlot.Glove, new List<EquipmentSlotDetails>
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
            var entity = GetTestEntity();
            var item = GetTestHelm(3);

            entity.Get<Inventory>().Contents.Add(item);

            equipmentSystem.Equip(entity, item);

            var expected = new EquipmentMappingList
            {
                new EquipmentMappingListItem {EquipmentId = item.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Head, BodyPartLocation = BodyPartLocation.Main, Index = 0}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Equip_HasSlotAndSomethingEquipped_SingleSlot_ReplacesEquippedAndPutsOldInInventory()
        {
            var entity = GetTestEntity();
            var item = GetTestHelm(3);

            entityEngine.GetEntity(3).Returns(item);

            entity.Get<Inventory>().Contents.Add(item);

            equipmentSystem.Equip(entity, item);

            var newItem = GetTestHelm(4);
            entity.Get<Inventory>().Contents.Add(newItem);

            equipmentSystem.Equip(entity, newItem);

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
            var entity = GetTestEntity();
            var item = GetTestHelm(3);

            prototypeSystem.Get("EquipmentMappings").Returns(new Entity(1, "EquipmentMappings", new IEntityComponent[0]));

            entity.Get<Inventory>().Contents.Add(item);

            equipmentSystem.Equip(entity, item);

            var expected = new EquipmentMappingList();

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList {item};

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void Equip_HasMultipleSlots_EquipsInEmptyOnes()
        {
            var entity = GetTestEntity();
            var item1 = GetTestHandItem(3);
            var item2 = GetTestHandItem(4);

            entity.Get<Inventory>().Contents.Add(item1);
            entity.Get<Inventory>().Contents.Add(item2);

            equipmentSystem.Equip(entity, item1);
            equipmentSystem.Equip(entity, item2);

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
            var entity = GetTestEntity();
            var item1 = GetTestHandItem(3);
            var item2 = GetTestHandItem(4);
            var item3 = GetTestHandItem(5);

            entity.Get<Inventory>().Contents.Add(item1);
            entity.Get<Inventory>().Contents.Add(item2);
            entity.Get<Inventory>().Contents.Add(item3);

            equipmentSystem.Equip(entity, item1);
            equipmentSystem.Equip(entity, item2);
            equipmentSystem.Equip(entity, item3);

            var expected = new EquipmentMappingList{
                new EquipmentMappingListItem {EquipmentId = item1.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 0}},
                new EquipmentMappingListItem {EquipmentId = item2.EntityId, Slot = new EquipmentSlotDetails {BodyPartType = BodyPartType.Arms, BodyPartLocation = BodyPartLocation.Main, Index = 1}}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList() {item3};

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        private static IEntity GetTestHelm(uint entityId)
        {
            return new Entity(entityId, "TestHelm", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Head}
            });
        }

        private static IEntity GetTestHandItem(uint entityId)
        {
            return new Entity(entityId, "TestHandItem", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Hand}
            });
        }

        private void SetUpTestMappings()
        {
            Entity mappings = new Entity(1, "EquipmentMappings", new IEntityComponent[]
            {
                new EquipmentMapping {BodyPart = BodyPartType.Head, Slot = EquipmentSlot.Head},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Hand},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Hand},
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Glove}
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