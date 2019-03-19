using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class EntityEngineTests
    {
        private IEntityEngine engine;
        private IStaticEntityLoader loader;

        [SetUp]
        public void SetUp()
        {
            loader = Substitute.For<IStaticEntityLoader>();

            engine = new EntityEngine(loader);
        }

        [Test]
        public void ComponentTypes_ShouldBeAllEntityComponents()
        {
            var result = engine.ComponentTypes;

            var expected = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IEntityComponent).IsAssignableFrom(p))
                .ToList();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void New_AddsEntityToList()
        {
            var testEntity = engine.New("Test Entity");

            engine.AllEntities.Single().Should().Be(testEntity);
        }

        [Test]
        public void GetEntity_GetsEntity()
        {
            var testEntity = engine.New("Test Entity");

            var result = engine.Get(testEntity.EntityId);

            result.Should().Be(testEntity);
        }

        [Test]
        public void GetEntityWithName_GetsEntity()
        {
            var testEntity = engine.New("Test Entity");

            var result = engine.GetEntityWithName("Test Entity");

            result.Should().Be(testEntity);
        }

        [Test]
        public void GetEntitiesWithName_GetsEntity()
        {
            var testEntity = engine.New("Test Entity");

            var result = engine.GetEntitiesWithName("Test Entity");

            result.Single().Should().Be(testEntity);
        }

        [Test]
        public void GetMutableEntities_EntityIsStatic_DoesNotGetEntity()
        {
            var testEntity = engine.New("Test Entity");

            testEntity.IsStatic = true;

            var result = engine.MutableEntities;

            result.Should().NotContain(testEntity);
        }

        [Test]
        public void New_RegisteredSystem_EntityDoesNotMeetRequirements_DoesNotRegister()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents {typeof(Appearance)});
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            var testEntity = engine.New("testEntity");

            system.DidNotReceive().AddEntity(testEntity);
        }

        [Test]
        public void New_RegisteredSystem_EntityMeetsRequirements_Registers()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            var testEntity = engine.New("testEntity", new Appearance());

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void Destroy_RemovesEntity()
        {
            var testEntity = engine.New("Test Entity");

            engine.Destroy(testEntity.EntityId);

            engine.AllEntities.Should().NotContain(testEntity);
        }

        [Test]
        public void Destroy_RegisteredEntity_RemovesEntityFromSystem()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            var testEntity = engine.New("testEntity", new Appearance());

            engine.Destroy(testEntity.EntityId);

            system.Received(1).RemoveEntity(testEntity);
        }

        [Test]
        public void Initialise_InitialisesSystems()
        {
            var systemContainer = new SystemContainer();
            var system = Substitute.For<ISystem>();

            engine.Register(system);

            engine.Initialise(systemContainer);

            system.Received(1).Initialise();
        }

        [Test]
        public void Initialise_LoadsStaticEntities()
        {
            var systemContainer = new SystemContainer();
            systemContainer.EntityEngine = engine;
            IEntity loadedEntity = null;

            loader.When(l => l.Load(systemContainer)).Do(c =>
            {
                var sc = c.Arg<SystemContainer>();
                loadedEntity = sc.EntityEngine.New("LoadedEntity");
            });

            engine.Initialise(systemContainer);

            loader.Received(1).Load(systemContainer);

            (loadedEntity as Entity)?.IsStatic.Should().BeTrue();
        }

        [Test]
        public void AddComponent_AddsComponentToEntity()
        {
            var testEntity = engine.New("TestEntity");

            Appearance component = new Appearance();
            engine.AddComponent(testEntity, component);

            testEntity.Get<Appearance>().Should().Be(component);
        }

        [Test]
        public void AddComponent_NowMeetsRequirementsForSystem_Registers()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            var testEntity = engine.New("TestEntity");

            system.DidNotReceive().AddEntity(testEntity);

            Appearance component = new Appearance();
            engine.AddComponent(testEntity, component);

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void AddComponent_NoLongerMeetsRequirementsForSystem_Removes()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents() );
            system.ForbiddenComponents.Returns(new SystemComponents { typeof(Appearance) });

            engine.Register(system);

            var testEntity = engine.New("TestEntity");

            system.Received(1).AddEntity(testEntity);

            system.HasEntity(testEntity).Returns(true);

            Appearance component = new Appearance();
            engine.AddComponent(testEntity, component);

            system.Received(1).RemoveEntity(testEntity);
        }

        [Test]
        public void AddComponent_AlreadyMetRequirements_DoesNotReregister()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());
            system.HasEntity(Arg.Any<IEntity>()).Returns(false);

            engine.Register(system);

            var testEntity = engine.New("TestEntity", new Appearance());

            system.Received(1).AddEntity(testEntity);

            system.HasEntity(Arg.Any<IEntity>()).Returns(true);

            Appearance component = new Appearance();
            engine.AddComponent(testEntity, component);

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void RemoveComponent_RemovesComponent()
        {
            Appearance component = new Appearance();
            var testEntity = engine.New("TestEntity", component);

            
            engine.RemoveComponent(testEntity, component);

            testEntity.Components.Should().NotContain(component);
        }

        [Test]
        public void RemoveComponent_NowMeetsRequirementsForSystem_Registers()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { });
            system.ForbiddenComponents.Returns(new SystemComponents { typeof(Appearance) });

            engine.Register(system);

            Appearance component = new Appearance();
            var testEntity = engine.New("TestEntity", component);

            system.DidNotReceive().AddEntity(testEntity);

            engine.RemoveComponent(testEntity, component);

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void RemoveComponent_AlreadyMetRequirements_DoesNotReregister()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());
            system.HasEntity(Arg.Any<IEntity>()).Returns(false);

            engine.Register(system);

            Position component = new Position();
            var testEntity = engine.New("TestEntity", new Appearance(), component);

            system.Received(1).AddEntity(testEntity);

            system.HasEntity(Arg.Any<IEntity>()).Returns(true);
            
            engine.RemoveComponent(testEntity, component);

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void Load_AddsEntity()
        {
            Entity testEntity = new Entity(10, "Entity", new IEntityComponent[0]);
            engine.Load(10, testEntity);

            engine.AllEntities.Single().Should().Be(testEntity);
            testEntity.EntityId.Should().Be(10);
        }

        [Test]
        public void Load_NewEntitiesHaveHigherId()
        {
            Entity testEntity = new Entity(10, "Entity", new IEntityComponent[0]);
            engine.Load(10, testEntity);

            var newEntity = engine.New("testEntity2");

            newEntity.EntityId.Should().BeGreaterThan(10);
        }

        [Test]
        public void Load_RegisteredSystem_EntityDoesNotMeetRequirements_DoesNotRegister()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            Entity entity = new Entity(10, "Entity", new IEntityComponent[0]);
            var testEntity = engine.Load(1, entity);

            system.DidNotReceive().AddEntity(testEntity);
        }

        [Test]
        public void Load_RegisteredSystem_EntityMeetsRequirements_Registers()
        {
            var system = Substitute.For<ISystem>();
            system.RequiredComponents.Returns(new SystemComponents { typeof(Appearance) });
            system.ForbiddenComponents.Returns(new SystemComponents());

            engine.Register(system);

            Entity entity = new Entity(10, "Entity", new IEntityComponent[1] {new Appearance()});
            var testEntity = engine.New("testEntity", new Appearance());

            system.Received(1).AddEntity(testEntity);
        }

        [Test]
        public void EntitiesWith_ReturnsEntitiesWithComponent()
        {
            var testEntity1 = engine.New("testEntity1", new Appearance());
            var testEntity2 = engine.New("testEntity2");
            var testEntity3 = engine.New("testEntity3", new Appearance(), new Prototype());

            var result = engine.EntitiesWith<Appearance>();

            result.Select(e => e.Name).Should().BeEquivalentTo(new List<string> { "testEntity1" });

            result = engine.EntitiesWith<Appearance>(includePrototypes: true);

            result.Select(e => e.Name).Should().BeEquivalentTo(new List<string> {"testEntity1", "testEntity3"});
        }

        [Test]
        public void GetAll_ReturnsAllInstancesOfComponent()
        {
            var appearance1 = new Appearance();
            var testEntity1 = engine.New("testEntity1", appearance1);
            var testEntity2 = engine.New("testEntity2");
            var appearance3 = new Appearance();
            var appearance3a = new Appearance();
            var testEntity3 = engine.New("testEntity3", appearance3, appearance3a, new Prototype());

            var result = engine.GetAll<Appearance>();

            result.Should().BeEquivalentTo(new List<Appearance> { appearance1 });

            result = engine.GetAll<Appearance>(includePrototypes: true);

            result.Should().BeEquivalentTo(new List<Appearance> { appearance1, appearance3, appearance3a });
        }
    }
}
