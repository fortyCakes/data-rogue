using System;
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
            systemContainer = Substitute.For<ISystemContainer>();

            prototypeSystem = Substitute.For<IPrototypeSystem>();
            systemContainer.PrototypeSystem.ReturnsForAnyArgs(prototypeSystem);

            eventSystem = Substitute.For<IEventSystem>();
            systemContainer.EventSystem.ReturnsForAnyArgs(eventSystem);
            eventSystem.Try(Arg.Any<EventType>(), null, null).ReturnsForAnyArgs(true);

            entityEngine = Substitute.For<IEntityEngine>();
            systemContainer.EntityEngine.ReturnsForAnyArgs(entityEngine);

            SetUpTestMappings();

            equipmentSystem = new EquipmentSystem(systemContainer);

            entity = GetTestEntity();
        }

        private IEntity entity;

        private IEquipmentSystem equipmentSystem;
        private ISystemContainer systemContainer;
        private IPrototypeSystem prototypeSystem;
        private IEntityEngine entityEngine;
        private IEventSystem eventSystem;
    
        private EquipmentSlotDetails HeadSlot => new EquipmentSlotDetails(EquipmentSlot.Head, BodyPartType.Head, BodyPartLocation.Main, 0);
        private EquipmentSlotDetails GloveSlot => new EquipmentSlotDetails(EquipmentSlot.Gloves, BodyPartType.Arms, BodyPartLocation.Main, 0);
        private EquipmentSlotDetails HandSlot => new EquipmentSlotDetails(EquipmentSlot.Hand, BodyPartType.Arms, BodyPartLocation.Main, 0);
        private EquipmentSlotDetails HandSlot2 => new EquipmentSlotDetails(EquipmentSlot.Hand, BodyPartType.Arms, BodyPartLocation.Main, 1);
        private EquipmentSlotDetails LegsSlot => new EquipmentSlotDetails(EquipmentSlot.Legs, BodyPartType.Legs, BodyPartLocation.Lower, 0);
        private EquipmentSlotDetails BootsSlot => new EquipmentSlotDetails(EquipmentSlot.Boots, BodyPartType.Legs, BodyPartLocation.Lower, 0);

        [Test]
        public void GetEquipmentSlots_WithMapping_ReturnsMappedSlots()
        {
            var result = equipmentSystem.GetEquipmentSlots(entity);

            var expected = new Dictionary<EquipmentSlot, List<EquipmentSlotDetails>>
            {
                {
                    EquipmentSlot.Head, new List<EquipmentSlotDetails>
                    {
                        HeadSlot
                    }
                },
                {
                    EquipmentSlot.Gloves, new List<EquipmentSlotDetails>
                    {
                        GloveSlot
                    }
                },
                {
                    EquipmentSlot.Hand, new List<EquipmentSlotDetails>
                    {
                        HandSlot,
                        HandSlot2,
                    }
                },
                {
                    EquipmentSlot.Legs, new List<EquipmentSlotDetails>
                    {
                        LegsSlot
                    }
                },
                {
                    EquipmentSlot.Boots, new List<EquipmentSlotDetails>
                    {
                        BootsSlot
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
                new EquipmentMappingListItem {EquipmentId = item.EntityId, Slot = HeadSlot }
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
        public void Equip_Boots_DoesNotUnequipTrousers()
        {
            var trousers = GiveTestItem(3, EquipmentSlot.Legs);
            var boots = GiveTestItem(4, EquipmentSlot.Boots);

            var result = equipmentSystem.Equip(entity, trousers);

            result.Should().BeTrue();

            result = equipmentSystem.Equip(entity, boots);

            result.Should().BeTrue();

            var expected = new EquipmentMappingList {
                new EquipmentMappingListItem { EquipmentId = trousers.EntityId, Slot = new EquipmentSlotDetails(EquipmentSlot.Legs, BodyPartType.Legs, BodyPartLocation.Lower, 0) },
                new EquipmentMappingListItem { EquipmentId = boots.EntityId, Slot = new EquipmentSlotDetails(EquipmentSlot.Boots, BodyPartType.Legs, BodyPartLocation.Lower, 0) }
            };

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
                new EquipmentMappingListItem {EquipmentId = newItem.EntityId, Slot = HeadSlot}
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
                new EquipmentMappingListItem {EquipmentId = item1.EntityId, Slot = HandSlot},
                new EquipmentMappingListItem {EquipmentId = item2.EntityId, Slot = HandSlot2}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList();

            entity.Get<Inventory>().Contents.Should().BeEquivalentTo(expectedInventory);
        }

        [Test]
        public void Equip_HasMultipleFullSlots_UnequipsFirst()
        {
            var item1 = GiveHandTestItem(3);
            var item2 = GiveHandTestItem(4);
            var item3 = GiveHandTestItem(5);

            equipmentSystem.Equip(entity, item1);
            equipmentSystem.Equip(entity, item2);
            var result = equipmentSystem.Equip(entity, item3);

            result.Should().BeTrue();

            var expected = new EquipmentMappingList{
                new EquipmentMappingListItem {EquipmentId = item3.EntityId, Slot = HandSlot},
                new EquipmentMappingListItem {EquipmentId = item2.EntityId, Slot = HandSlot2}
            };

            entity.Get<Equipped>().EquippedItems.Should().BeEquivalentTo(expected);

            var expectedInventory = new EntityReferenceList() {item1};

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

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, HandSlot);

            result.Should().Be(item1);
        }

        [Test]
        public void GetItemInSlot_ItemInSlotOnSameBodyPart_ReturnsNull()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Gloves, HandSlot);

            result.Should().BeNull();
        }

        [Test]
        public void GetItemInSlot_ItemInOtherSlot_ReturnsNull()
        {
            var item1 = GiveHandTestItem(3);
            equipmentSystem.Equip(entity, item1);

            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, HandSlot2);

            result.Should().BeNull();
        }

        [Test]
        public void GetItemInSlot_SlotEmpty_ReturnsNull()
        {
            var result = equipmentSystem.GetItemInSlot(entity, EquipmentSlot.Hand, LegsSlot);

            result.Should().BeNull();
        }

        [Test]
        public void EquipTwoHandedItem_OnlyHaveOneHand_CantEquip()
        {
            Entity mappings = new Entity(1, "EquipmentMappings", new IEntityComponent[]
            {
                new EquipmentMapping {BodyPart = BodyPartType.Arms, Slot = EquipmentSlot.Hand}
            });

            prototypeSystem.Get("EquipmentMappings").Returns(mappings);
            var twoHandedItem = GiveHandTestItem(3);
            twoHandedItem.Get<Equipment>().AdditionalEquipmentSlot = EquipmentSlot.Hand;

            var equipped2H = equipmentSystem.Equip(entity, twoHandedItem);

            equipped2H.Should().BeFalse();
        }

        [Test]
        public void EquipTwoHandedItem_ItemInBothHands_UnequipsBoth()
        {
            var firstItem = GiveHandTestItem(1);
            var secondItem = GiveHandTestItem(2);
            var twoHandedItem = GiveHandTestItem(3);
            twoHandedItem.Get<Equipment>().AdditionalEquipmentSlot = EquipmentSlot.Hand;

            equipmentSystem.Equip(entity, firstItem);
            equipmentSystem.Equip(entity, secondItem);

            var equipped2H = equipmentSystem.Equip(entity, twoHandedItem);

            equipped2H.Should().BeTrue();

            ShouldNotBeEquipped(entity, firstItem);
            ShouldNotBeEquipped(entity, secondItem);
        }

        [Test]
        public void EquipOneHandedItem_HoldingTwoHandedItem_UnequipsTwoHandedItem()
        {
            var firstItem = GiveHandTestItem(1);
            var twoHandedItem = GiveHandTestItem(3);
            twoHandedItem.Get<Equipment>().AdditionalEquipmentSlot = EquipmentSlot.Hand;

            var equipped2H = equipmentSystem.Equip(entity, twoHandedItem);

            equipped2H.Should().BeTrue();
            equipmentSystem.Equip(entity, firstItem);

            ShouldBeEquipped(entity, firstItem);
            ShouldNotBeEquipped(entity, twoHandedItem);
        }

        private void ShouldBeEquipped(IEntity entity, IEntity item)
        {
            equipmentSystem.GetEquippedItems(entity).Should().Contain(item);
        }

        private void ShouldNotBeEquipped(IEntity entity, IEntity item)
        {
            equipmentSystem.GetEquippedItems(entity).Should().NotContain(item);
        }

        private IEntity GiveHelmTestItem(uint entityId)
        {
            var item = new Entity(entityId, "TestHelm", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Head}
            });

            entityEngine.Get(entityId).Returns(item);

            entity.Get<Inventory>().Contents.Add(item);

            return item;
        }

        private IEntity GiveTestItem(uint entityId, EquipmentSlot slot)
        {
            var item = new Entity(entityId, "Test" + slot.ToString(), new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = slot}
            });

            entityEngine.Get(entityId).Returns(item);

            entity.Get<Inventory>().Contents.Add(item);

            return item;
        }

        private IEntity GiveHandTestItem(uint entityId)
        {
            var item = new Entity(entityId, "TestHandItem", new IEntityComponent[]
            {
                new Equipment() {EquipmentSlot = EquipmentSlot.Hand}
            });

            entityEngine.Get(entityId).Returns(item);

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
                new EquipmentMapping {BodyPart = BodyPartType.Legs, Slot = EquipmentSlot.Legs},
                new EquipmentMapping {BodyPart = BodyPartType.Legs, Slot = EquipmentSlot.Boots},
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
                new BodyPart {Location = BodyPartLocation.Lower, Type = BodyPartType.Legs},
                new Inventory(),
                new Equipped()
            });
        }
    }
}