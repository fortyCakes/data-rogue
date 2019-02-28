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
    public class ItemSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            entityId = 0;

            systemContainer = new SystemContainer();

            prototypeSystem = Substitute.For<IPrototypeSystem>();
            systemContainer.PrototypeSystem = prototypeSystem;

            eventSystem = Substitute.For<IEventSystem>();
            systemContainer.EventSystem = eventSystem;
            systemContainer.EventSystem.Try(Arg.Any<EventType>(), null, null).ReturnsForAnyArgs(true);

            entityEngine = Substitute.For<IEntityEngine>();
            systemContainer.EntityEngine = entityEngine;

            messageSystem = Substitute.For<IMessageSystem>();
            systemContainer.MessageSystem = messageSystem;

            itemSystem = new ItemSystem(entityEngine, prototypeSystem, scriptExecutor, messageSystem, eventSystem);

            itemSystem.Initialise();

            entity = GetTestEntity();
            inventory = entity.Get<Inventory>();
        }

        private uint entityId;

        private IEntity entity;
        private Inventory inventory;

        private IItemSystem itemSystem;
        private SystemContainer systemContainer;
        private IPrototypeSystem prototypeSystem;
        private IEntityEngine entityEngine;
        private IEventSystem eventSystem;
        private IScriptExecutor scriptExecutor;
        private IMessageSystem messageSystem;

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

        private IEntity GetTestItem(string itemName = null, bool hasPosition = true)
        {
            var components = new List<IEntityComponent>() { new Item() };

            if (hasPosition)
            {

                components.Add(new Position());
            }

            return new Entity(entityId, itemName ?? $"Item{entityId++}", components.ToArray());
        }

        private IEntity GetTestEntity(int capacity = 2)
        {
            return new Entity(entityId++, "Inventory", new[] { new Inventory { Capacity = capacity, Contents = new EntityReferenceList() } });
        }
    }
}