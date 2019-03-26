using System.Collections.Generic;
using System.Drawing;
using System.IO;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Data
{
    [TestFixture]
    class SaveStateSerializerTests
    {
        private ISystemContainer systemContainer;
        private IEntity wallCell;
        private IEntity floorCell;
        private IEntityDataProvider entityDataProvider;

        [SetUp]
        public void SetUp()
        {
            entityDataProvider = Substitute.For<IEntityDataProvider>();

            systemContainer = new SystemContainer();
            systemContainer.EntityEngine = new EntityEngine(Substitute.For<IEntityDataProvider>());

            wallCell = CreateCell('#', "Cell:Wall");
            floorCell = CreateCell('.', "Cell:Empty");
        }

        [Test]
        [TestCase(0)]
        public void TestSaveState_Deserialize_MatchesTestSaveState(int testCase)
        {
            string testData = LoadSerializedData(testCase);

            var entity = SaveStateSerializer.Deserialize(testData);

            var expected = GetTestSaveState(testCase);

            entity.Should().BeEquivalentTo(expected);
        }

        [Test]
        [TestCase(0)]
        public void SaveState_Serialize_MatchesSerializedData(int testCase)
        {
            var testEntity = GetTestSaveState(testCase);

            var serialised = SaveStateSerializer.Serialize(testEntity);

            var expected = LoadSerializedData(testCase);

            serialised.Should().BeEquivalentTo(expected);
        }

        private static string LoadSerializedData(int index)
        {
            return LoadFile($"Systems/TestData/ExpectedSerialization_SaveState{index}.txt");
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

        private SaveState GetTestSaveState(int saveState)
        {
            return new SaveState() {
                Seed = "TestSeed",
                Maps = new List<string> { MapSerializer.Serialize(GetTestMap(0))},
                Entities = new List<string>() { EntitySerializer.Serialize( GetTestEntity(0))},
                Messages =  new List<string> { "This is a test message"},
                Time = 42
            } ;
        }

        private Map GetTestMap(int v)
        {
            return SetUpTestMaps()[v];
        }

        private Entity[] SetUpTestEntities()
        {
            var testEntity0 = new Entity(3, "TestEntity", new[] {
                new Appearance() { Glyph = '£', Color = Color.FromArgb(255, 0, 0)
                }});
            var testEntity1 = new Entity(4, "TestEntity2", new[] {
                new Appearance() { Glyph = '£', Color = Color.FromArgb(255, 0, 0)
                }});

            return new[] { testEntity0, testEntity1 };
        }

        private Map[] SetUpTestMaps()
        {
            var testMap0 = new Map("TestMapKey", wallCell);

            testMap0.SetCellsInRange(-4, 3, -2, 2, floorCell);
            testMap0.SetCell(1, 1, wallCell);

            var testMap1 = new Map("TestMapKey2", wallCell);

            testMap1.SetCellsInRange(0, 11, 0, 4, floorCell);

            testMap1.RemoveCellsInRange(1, 2, 1, 1);
            testMap1.RemoveCellsInRange(5, 6, 2, 3);

            return new[] { testMap0, testMap1 };
        }

        private IEntity CreateCell(char glyph, string name)
        {
            return systemContainer.EntityEngine.New(name,
               new Appearance { Glyph = glyph },
               new Physical()
               );
        }
    }
}
