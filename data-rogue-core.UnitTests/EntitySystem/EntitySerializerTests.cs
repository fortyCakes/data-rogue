using System.Collections.Generic;
using System.Drawing;
using System.IO;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Data
{
    [TestFixture]
    partial class EntitySerializerTests
    {
        private IEntityEngine _entityEngineSystem;
        private IPositionSystem PositionSystem;
        private IEventSystem EventSystem;
        private BehaviourFactory BehaviourFactory;
        private IRandom Random;

        [SetUp]
        public void SetUp()
        {
            PositionSystem = Substitute.For<IPositionSystem>();
            EventSystem = Substitute.For<IEventSystem>();
            Random = Substitute.For<IRandom>();
            BehaviourFactory = new BehaviourFactory(PositionSystem, EventSystem, Random);

            _entityEngineSystem = Substitute.For<IEntityEngine>();
            _entityEngineSystem.New(Arg.Any<string>(), Arg.Any<IEntityComponent[]>()).ReturnsForAnyArgs(callInfo =>
            {
                var entity = new Entity(0, "entity", callInfo.ArgAt<IEntityComponent[]>(1));
                entity.Name = callInfo.ArgAt<string>(0);
                return entity;
            });

            _entityEngineSystem.Load(Arg.Any<uint>(), Arg.Any<Entity>()).ReturnsForAnyArgs(callInfo =>
            {
 
                var entity = callInfo.ArgAt<Entity>(1);
                
                return entity;
            });
            _entityEngineSystem.ComponentTypes.ReturnsForAnyArgs(new[] { typeof(Appearance), typeof(Position), typeof(Stairs), typeof(MoveToPlayerBehaviour) });
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void TestEntity_Deserialize_MatchesTestEntity(int testCase)
        {
            string testData = LoadSerializedData(testCase);

            var entity = new List<Entity> { EntitySerializer.Deserialize(testData, _entityEngineSystem, BehaviourFactory) };

            var expected = new List<Entity> { GetTestEntity(testCase) };

            entity.Should().BeEquivalentTo(expected, options => options.Using(new EntityListEquivalence()));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void TestEntity_Serialize_MatchesSerializedData(int testCase)
        {
            var testEntity = GetTestEntity(testCase);

            var serialised = EntitySerializer.Serialize(testEntity);

            var expected = LoadSerializedData(testCase);

            serialised.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DeserializeMultiple_ReturnsMultipleEntities()
        {
            var multipleSerialised = LoadFile("EntitySystem/TestData/ExpectedSerialization_MultipleEntities.txt");

            var entities = EntitySerializer.DeserializeMultiple(multipleSerialised, _entityEngineSystem, BehaviourFactory);

            var expected = new List<Entity>
            {
                GetTestEntity(0),
                GetTestEntity(0)
            };

            entities.Should().BeEquivalentTo(expected, options => options.Using(new EntityListEquivalence()));

        }

        private static string LoadSerializedData(int index)
        {
            return LoadFile($"EntitySystem/TestData/ExpectedSerialization_Entity{index}.txt");
        }

        private static string LoadFile(string fileName)
        {
            var testFolder = TestContext.CurrentContext.TestDirectory;

            var expectedFile = Path.Combine(testFolder, fileName);

            var serialisedText = File.ReadAllText(expectedFile);
            return serialisedText;
        }

        private Entity GetTestEntity(int entity)
        {
            return SetUpTestEntities()[entity];
        }

        private Entity[] SetUpTestEntities()
        {
            var testEntity0 = new Entity(0, "TestEntity", new IEntityComponent[] {
                new Appearance() { Glyph = '£', Color = Color.FromArgb(255, 0, 0)},
                new MoveToPlayerBehaviour(PositionSystem, EventSystem) { Priority = 100 }
            });

            var testEntity1 = new Entity(1, "EntityWithPosition", new[]
            {
                new Position() { MapCoordinate = new MapCoordinate("TestMapKey", 1, 1)}
            });

            var testEntity2 = new Entity(1, "EntityWithStairs", new[]
            {
                new Stairs() { Destination = null, Direction = StairDirection.Down}
            });

            return new[] { testEntity0, testEntity1, testEntity2 };
        }
    }
}
