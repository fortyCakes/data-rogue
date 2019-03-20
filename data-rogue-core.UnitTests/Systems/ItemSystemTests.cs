using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using FluentAssertions;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class ItemSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            entityId = 0;

            systemContainer = new SystemContainer();

            systemContainer.CreateSystems("test");

            entity = GetTestEntity();
            inventory = entity.Get<Inventory>();
        }

        private uint entityId;

        private IEntity entity;
        private Inventory inventory;

        private IItemSystem itemSystem => systemContainer.ItemSystem;
        private SystemContainer systemContainer;

        [Test]
        public void MoveItemToInventory_AddsToInventory()
        {
            var item = GetTestItem();

            var result = itemSystem.MoveToInventory(item, inventory);

            result.Should().BeTrue();

            var expected = new List<uint> { item.EntityId };

            inventory.Contents.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void MoveItemToInventory_InventoryIsFull_Fails()
        {
            var item = GetTestItem();
            var item2 = GetTestItem();
            var item3 = GetTestItem();

            itemSystem.MoveToInventory(item, inventory);
            itemSystem.MoveToInventory(item2, inventory);
            var result = itemSystem.MoveToInventory(item3, inventory);

            result.Should().BeFalse();

            var expected = new List<uint> { item.EntityId, item2.EntityId };

            inventory.Contents.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DropItemFromInventory_HasItem_Drops()
        {
            var item = GetTestItem();

            itemSystem.MoveToInventory(item, inventory);

            var result = itemSystem.DropItemFromInventory(item);

            result.Should().BeTrue();
            item.Has<Position>().Should().BeTrue();
        }

        [Test]
        public void DropItemFromInventory_NotInInventory_CantDrop()
        {
            var item = GetTestItem();

            var result = itemSystem.DropItemFromInventory(item);

            result.Should().BeFalse();
        }

        [Test]
        public void TransferWealth_BothHaveWealth_Transfers()
        {
            var entity1 = GetTestEntity();
            var entity2 = GetTestEntity();

            entity1.Get<Wealth>().Amount = 10;

            var result = systemContainer.ItemSystem.TransferWealth(entity1, entity2, "TestCurrency", 2);

            result.Should().BeTrue();

            systemContainer.ItemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(8);
            systemContainer.ItemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(2);
        }

        [Test]
        public void TransferWealth_DoesntHaveEnough_Fails()
        {
            var entity1 = GetTestEntity();
            var entity2 = GetTestEntity();

            entity1.Get<Wealth>().Amount = 10;

            var result = systemContainer.ItemSystem.TransferWealth(entity1, entity2, "TestCurrency", 100);

            result.Should().BeFalse();

            systemContainer.ItemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(10);
            systemContainer.ItemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(0);
        }

        [Test]
        public void TransferWealth_DoesntHaveWealth_Fails()
        {
            var entity1 = GetTestEntity();
            var entity2 = GetTestEntity();

            systemContainer.EntityEngine.RemoveComponent(entity1, entity1.Get<Wealth>());

            var result = systemContainer.ItemSystem.TransferWealth(entity1, entity2, "TestCurrency", 100);

            result.Should().BeFalse();

            systemContainer.ItemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(0);
            systemContainer.ItemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(0);
        }

        [Test]
        public void RemoveWealth_HasWealth_RemovesWealth()
        {
            entity.Get<Wealth>().Amount = 10;

            var result = systemContainer.ItemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            systemContainer.ItemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        [Test]
        public void RemoveWealth_InsufficientWealth_Fails()
        {
            entity.Get<Wealth>().Amount = 2;

            var result = systemContainer.ItemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeFalse();

            systemContainer.ItemSystem.CheckWealth(entity, "TestCurrency").Should().Be(2);

        }

        [Test]
        public void RemoveWealth_DoesntHaveWealth_Fails()
        {
            systemContainer.EntityEngine.RemoveComponent(entity, entity.Get<Wealth>());

            var result = systemContainer.ItemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeFalse();

            systemContainer.ItemSystem.CheckWealth(entity, "TestCurrency").Should().Be(0);

        }

        [Test]
        public void AddWealth_DoesntHaveWealth_Adds()
        {
            systemContainer.EntityEngine.RemoveComponent(entity, entity.Get<Wealth>());

            var result = systemContainer.ItemSystem.AddWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            systemContainer.ItemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        [Test]
        public void AddWealth_HasWealth_Adds()
        {
            var result = systemContainer.ItemSystem.AddWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            systemContainer.ItemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        private IEntity GetTestItem(string itemName = null, bool hasPosition = true)
        {
            var components = new List<IEntityComponent>() { new Item() };

            if (hasPosition)
            {

                components.Add(new Position {MapCoordinate = new MapCoordinate("TestMap", 0, 0)});
            }

            return systemContainer.EntityEngine.New(itemName ?? $"Item{entityId++}", components.ToArray());
        }

        private IEntity GetTestEntity(int capacity = 2)
        {
            return systemContainer.EntityEngine.New("Inventory", 
                new Inventory { Capacity = capacity, Contents = new EntityReferenceList() },
                new Position { MapCoordinate = new MapCoordinate("Test Map", 0, 0)},
                new Wealth { Currency="TestCurrency"});
        }
    }
}