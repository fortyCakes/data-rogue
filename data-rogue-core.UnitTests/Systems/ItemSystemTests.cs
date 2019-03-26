using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using FluentAssertions;
using NSubstitute;
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

            entityDataProvider = Substitute.For<IEntityDataProvider>();
            entityDataProvider.GetData().Returns(new List<string>());

            systemContainer = new SystemContainer(entityDataProvider);

            systemContainer.CreateSystems("test");

            systemContainer.EventSystem.Initialise();

            entity = GetTestEntity();
            inventory = entity.Get<Inventory>();
        }

        private uint entityId;

        private IEntity entity;
        private Inventory inventory;

        private IItemSystem itemSystem => systemContainer.ItemSystem;
        private SystemContainer systemContainer;
        private IEntityDataProvider entityDataProvider;

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

        [Test]
        public void StackableItems_GetNewStackableItem_AlreadyHaveSome_StacksAddTogether()
        {
            var item = GetTestItem(stackable: true);
            var item2 = GetTestItem(stackable: true);

            systemContainer.ItemSystem.MoveToInventory(item, inventory);
            systemContainer.ItemSystem.MoveToInventory(item2, inventory);

            inventory.Contents.Should().NotContain(item2.EntityId);
            item.Get<Stackable>().StackSize.Should().Be(2);
        }

        [Test]
        public void StackableItems_RemoveStackableItem_HaveOne_RemovesFromInventory()
        {
            var item = GetTestItem(stackable: true);
            item.Get<Stackable>().StackSize = 1;

            systemContainer.ItemSystem.MoveToInventory(item, inventory);

            systemContainer.ItemSystem.RemoveItemFromInventory(item);

            inventory.Contents.Should().NotContain(item.EntityId);
        }

        [Test]
        public void StackableItems_RemoveStackableItem_HaveMoreThanOne_ReducesStack()
        {
            var item = GetTestItem(stackable: true);
            item.Get<Stackable>().StackSize = 2;

            systemContainer.ItemSystem.MoveToInventory(item, inventory);

            systemContainer.ItemSystem.RemoveItemFromInventory(item);

            inventory.Contents.Should().Contain(item.EntityId);
            item.Get<Stackable>().StackSize.Should().Be(1);
        }

        [Test]
        public void Use_CallsScriptExecutorWithItemScript()
        {
            var item = GetTestItem();
            item.Get<Item>().UseScript = "TestScript";

            var guid = Guid.NewGuid().ToString();

            GenerateTestScript(guid);

            systemContainer.ItemSystem.Use(entity, item);

            systemContainer.MessageSystem.AllMessages.Single().Should().BeEquivalentTo(
                new Message {Text = guid, Color = Color.White});
        }


        [Test]
        public void Use_Consumable_UsesRemaining_DecreasesUses()
        {
            var item = GetTestItem();
            item.Get<Item>().UseScript = "TestScript";
            systemContainer.EntityEngine.AddComponent(item, new Consumable {Uses = new Counter{Current = 2, Max = 2}});

            var guid = Guid.NewGuid().ToString();

            GenerateTestScript(guid);

            systemContainer.ItemSystem.Use(entity, item);

            item.Get<Consumable>().Uses.ToString().Should().Be("1/2");
        }

        [Test]
        public void Use_Consumable_NoUsesRemaining_DestroysItem()
        {
            var item = GetTestItem();
            systemContainer.ItemSystem.MoveToInventory(item, inventory);

            item.Get<Item>().UseScript = "TestScript";
            systemContainer.EntityEngine.AddComponent(item, new Consumable { Uses = new Counter { Current = 1, Max = 2 } });

            var guid = Guid.NewGuid().ToString();

            GenerateTestScript(guid);

            systemContainer.ItemSystem.Use(entity, item);

            inventory.Contents.Should().BeEmpty();
        }

        [Test]
        public void Use_ItemIsNotUsable_PrintsNothingInterestingMessage()
        {
            var item = GetTestItem();

            systemContainer.EntityEngine.AddComponent(entity, new Description { Name = "Test User" });
            systemContainer.EntityEngine.AddComponent(item, new Description { Name = "Test Item" });

            systemContainer.ItemSystem.Use(entity, item);

            var result = systemContainer.MessageSystem.AllMessages.Single();

            result.Should().BeEquivalentTo(new Message {Color = Color.White, Text = "Test User tries to use Test Item. Nothing interesting happens." });
        }

        [Test]
        public void Destroy_ItemIsInInventory_RemovesReference()
        {
            var item = GetTestItem();

            systemContainer.ItemSystem.MoveToInventory(item, inventory);

            systemContainer.ItemSystem.DestroyItem(item);

            inventory.Contents.Should().BeEmpty();
        }

        private void GenerateTestScript(string testMessage = "")
        {
            systemContainer.EntityEngine.New("TestScript",
                new Prototype { Name = "TestScript", Singleton = true },
                new Script { Text = $"SystemContainer.MessageSystem:Write('{testMessage}')" });
        }

        private IEntity GetTestItem(string itemName = null, bool hasPosition = true, bool stackable = false)
        {
            var components = new List<IEntityComponent>() { new Item() };

            if (hasPosition)
            {
                components.Add(new Position());
            }

            if (stackable)
            {
                components.Add(new Stackable());
            }

            return systemContainer.EntityEngine.New(itemName ?? $"Item{entityId++}", components.ToArray());
        }

        private IEntity GetTestEntity(int capacity = 2)
        {
            return systemContainer.EntityEngine.New("Inventory", 
                new Inventory { Capacity = capacity, Contents = new EntityReferenceList() },
                new Position { MapCoordinate = new MapCoordinate("TEST_MAP", 0, 0)},
                new Wealth { Currency="TestCurrency"});
        }
    }
}