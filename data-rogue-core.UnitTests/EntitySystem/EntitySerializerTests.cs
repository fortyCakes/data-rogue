using System.Collections.Generic;
using System.Drawing;
using System.IO;
using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Data
{
    [TestFixture]
    partial class EntitySerializerTests
    {
        private IEntityEngineSystem _entityEngineSystem;

        [SetUp]
        public void SetUp()
        {
            _entityEngineSystem = Substitute.For<IEntityEngineSystem>();
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
            _entityEngineSystem.ComponentTypes.ReturnsForAnyArgs(new[] { typeof(Appearance), typeof(Position), typeof(Portal) });
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void TestEntity_Deserialize_MatchesTestEntity(int testCase)
        {
            string testData = LoadSerializedData(testCase);

            var entity = new List<Entity> { EntitySerializer.Deserialize(testData, _entityEngineSystem) };

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

            var entities = EntitySerializer.DeserializeMultiple(multipleSerialised, _entityEngineSystem);

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
            var testEntity0 = new Entity(0, "TestEntity", new[] {
                new Appearance() { Glyph = '£', Color = Color.FromArgb(255, 0, 0)
                }});

            var testEntity1 = new Entity(1, "EntityWithPosition", new[]
            {
                new Position() { MapCoordinate = new MapCoordinate("TestMapKey", 1, 1)}
            });

            var testEntity2 = new Entity(1, "EntityWithPortal", new[]
            {
                new Portal() { Destination = null, Direction = StairDirection.Down}
            });

            return new[] { testEntity0, testEntity1, testEntity2 };
        }
    }
}
