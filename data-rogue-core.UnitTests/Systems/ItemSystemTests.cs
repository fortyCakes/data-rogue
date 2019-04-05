using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
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
            
            inventory = new Inventory {Capacity = 10, Contents = new EntityReferenceList()};
            position = new Position {MapCoordinate = new MapCoordinate("map", 0, 0)};

            entity = new Entity(int.MaxValue, "TestEntity", new IEntityComponent[] {inventory, position});

            entityEngine = Substitute.For<IEntityEngine>();
            prototypeSystem = Substitute.For<IPrototypeSystem>();
            scriptExecutor = Substitute.For<IScriptExecutor>();
            messageSystem = Substitute.For<IMessageSystem>();
            eventSystem = Substitute.For<IEventSystem>();

            itemSystem = new ItemSystem(entityEngine, prototypeSystem, scriptExecutor, messageSystem, eventSystem);

            itemSystem.Initialise();

            itemSystem.AddEntity(entity);
        }

        private uint entityId;

        private IEntity entity;
        private Inventory inventory;
        private Position position;

        private IItemSystem itemSystem;
        private IEntityEngine entityEngine;
        private IPrototypeSystem prototypeSystem;
        private IScriptExecutor scriptExecutor;
        private IMessageSystem messageSystem;
        private IEventSystem eventSystem;

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

            inventory.Capacity = 2;

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

            var result = itemSystem.TransferWealth(entity1, entity2, "TestCurrency", 2);

            result.Should().BeTrue();

            itemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(8);
            itemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(2);
        }

        [Test]
        public void TransferWealth_DoesntHaveEnough_Fails()
        {
            var entity1 = GetTestEntity();
            var entity2 = GetTestEntity();

            entity1.Get<Wealth>().Amount = 10;

            var result = itemSystem.TransferWealth(entity1, entity2, "TestCurrency", 100);

            result.Should().BeFalse();

            itemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(10);
            itemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(0);
        }

        [Test]
        public void TransferWealth_DoesntHaveWealth_Fails()
        {
            var entity1 = GetTestEntity();
            var entity2 = GetTestEntity();

            entityEngine.RemoveComponent(entity1, entity1.Get<Wealth>());

            var result = itemSystem.TransferWealth(entity1, entity2, "TestCurrency", 100);

            result.Should().BeFalse();

            itemSystem.CheckWealth(entity1, "TestCurrency").Should().Be(0);
            itemSystem.CheckWealth(entity2, "TestCurrency").Should().Be(0);
        }

        [Test]
        public void RemoveWealth_HasWealth_RemovesWealth()
        {
            entity.Components.Add(new Wealth{Currency = "TestCurrency", Amount = 10});

            var result = itemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            itemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        [Test]
        public void RemoveWealth_InsufficientWealth_Fails()
        {
            entity.Get<Wealth>().Amount = 2;

            var result = itemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeFalse();

            itemSystem.CheckWealth(entity, "TestCurrency").Should().Be(2);

        }

        [Test]
        public void RemoveWealth_DoesntHaveWealth_Fails()
        {
            entityEngine.RemoveComponent(entity, entity.Get<Wealth>());

            var result = itemSystem.RemoveWealth(entity, "TestCurrency", 5);

            result.Should().BeFalse();

            itemSystem.CheckWealth(entity, "TestCurrency").Should().Be(0);

        }

        [Test]
        public void AddWealth_DoesntHaveWealth_Adds()
        {
            entityEngine.RemoveComponent(entity, entity.Get<Wealth>());

            var result = itemSystem.AddWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            itemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        [Test]
        public void AddWealth_HasWealth_Adds()
        {
            var result = itemSystem.AddWealth(entity, "TestCurrency", 5);

            result.Should().BeTrue();

            itemSystem.CheckWealth(entity, "TestCurrency").Should().Be(5);
        }

        [Test]
        public void StackableItems_GetNewStackableItem_AlreadyHaveSome_StacksAddTogether()
        {
            var item = GetTestItem(stackable: true);
            var item2 = GetTestItem(stackable: true);

            itemSystem.MoveToInventory(item, inventory);
            itemSystem.MoveToInventory(item2, inventory);

            inventory.Contents.Should().NotContain(item2.EntityId);
            item.Get<Stackable>().StackSize.Should().Be(2);
        }

        [Test]
        public void StackableItems_RemoveStackableItem_HaveOne_RemovesFromInventory()
        {
            var item = GetTestItem(stackable: true);
            item.Get<Stackable>().StackSize = 1;

            itemSystem.MoveToInventory(item, inventory);

            itemSystem.RemoveItemFromInventory(item);

            inventory.Contents.Should().NotContain(item.EntityId);
        }

        [Test]
        public void StackableItems_RemoveStackableItem_HaveMoreThanOne_ReducesStack()
        {
            var item = GetTestItem(stackable: true);
            item.Get<Stackable>().StackSize = 2;

            itemSystem.MoveToInventory(item, inventory);

            itemSystem.RemoveItemFromInventory(item);

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

            itemSystem.Use(entity, item);

            messageSystem.AllMessages.Single().Should().BeEquivalentTo(
                new Message {Text = guid, Color = Color.White});
        }


        [Test]
        public void Use_Consumable_UsesRemaining_DecreasesUses()
        {
            var item = GetTestItem();
            item.Get<Item>().UseScript = "TestScript";
            entityEngine.AddComponent(item, new Consumable {Uses = new Counter{Current = 2, Max = 2}});

            var guid = Guid.NewGuid().ToString();

            GenerateTestScript(guid);

            itemSystem.Use(entity, item);

            item.Get<Consumable>().Uses.ToString().Should().Be("1/2");
        }

        [Test]
        public void Use_Consumable_NoUsesRemaining_DestroysItem()
        {
            var item = GetTestItem();
            itemSystem.MoveToInventory(item, inventory);

            item.Get<Item>().UseScript = "TestScript";
            entityEngine.AddComponent(item, new Consumable { Uses = new Counter { Current = 1, Max = 2 } });

            var guid = Guid.NewGuid().ToString();

            GenerateTestScript(guid);

            itemSystem.Use(entity, item);

            inventory.Contents.Should().BeEmpty();
        }

        [Test]
        public void Use_ItemIsNotUsable_PrintsNothingInterestingMessage()
        {
            var item = GetTestItem();

            entityEngine.AddComponent(entity, new Description { Name = "Test User" });
            entityEngine.AddComponent(item, new Description { Name = "Test Item" });

            itemSystem.Use(entity, item);

            var result = messageSystem.AllMessages.Single();

            result.Should().BeEquivalentTo(new Message {Color = Color.White, Text = "Test User tries to use Test Item. Nothing interesting happens." });
        }

        [Test]
        public void Destroy_ItemIsInInventory_RemovesReference()
        {
            var item = GetTestItem();

            itemSystem.MoveToInventory(item, inventory);

            itemSystem.DestroyItem(item);

            inventory.Contents.Should().BeEmpty();
        }

        private void GenerateTestScript(string testMessage = "")
        {
            entityEngine.New("TestScript",
                new Prototype { Name = "TestScript", Singleton = true },
                new Script { Text = $"SystemContainer.MessageSystem:Write('{testMessage}')" });
        }

        private IEntity GetTestItem(string itemName = null, bool hasPosition = true, bool stackable = false)
        {
            var item = Substitute.For<IEntity>();
            item.EntityId.Returns(entityId++);

            item.Has<Item>().Returns(true);
            item.Has<Position>().Returns(hasPosition);
            item.Has<Stackable>().Returns(stackable);

            return item;
        }

        private IEntity GetTestEntity(int capacity = 2)
        {
            return new Entity(entityId++, "TestEntity", new IEntityComponent[] {
                new Inventory { Capacity = capacity, Contents = new EntityReferenceList() },
                new Position { MapCoordinate = new MapCoordinate("TEST_MAP", 0, 0)},
                new Wealth { Currency="TestCurrency"}
            });
        }
    }
}