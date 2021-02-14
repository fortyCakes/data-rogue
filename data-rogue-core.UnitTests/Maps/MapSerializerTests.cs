using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    public class MapSerializerTests
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

            var entityLoader = Substitute.For<IEntityDataProvider>();

            systemContainer.EntityEngine = new EntityEngine(entityLoader);
            systemContainer.PrototypeSystem = Substitute.For<IPrototypeSystem>();

            wallCell = CreateCell('#', "Cell:Wall");
            systemContainer.PrototypeSystem.Get("Cell:Wall").Returns(wallCell);

            floorCell = CreateCell('.', "Cell:Empty");
            systemContainer.PrototypeSystem.Get("Cell:Empty").Returns(floorCell);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Serialize_File1_TestMap_ReturnsExpectedSerialization(int testCase)
        {
            var testMap = GetTestMap(testCase);

            var result = MapSerializer.Serialize(testMap);

            var expected = LoadSerializedData(testCase);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Deserialize_File1_ReturnsTestMap(int testCase)
        {
            string serialisedText = LoadSerializedData(testCase);

            var result = MapSerializer.Deserialize(systemContainer, serialisedText);

            var reserialised = MapSerializer.Serialize(result);

            var expected = GetTestMap(testCase);

            result.Should().BeEquivalentTo(expected);
        }

        private static string LoadSerializedData(int index)
        {
            var testFolder = TestContext.CurrentContext.TestDirectory;

            var expectedFile = Path.Combine(testFolder, $"Maps/TestData/ExpectedSerialization_Map{index}.txt");

            var serialisedText = File.ReadAllText(expectedFile);
            return serialisedText;
        }

        private Map GetTestMap(int map)
        {
            return SetUpTestMaps()[map];
        }

        private Map[] SetUpTestMaps()
        {
            var testMap0 = new Map("testMapKey", wallCell);

            testMap0.SetCellsInRange(-4, 3, -2, 2, floorCell);
            testMap0.SetCell(1, 1, wallCell);

            testMap0.SetSeen(new MapCoordinate("testMapKey", 0, 0));
            testMap0.SetSeen(new MapCoordinate("testMapKey", 1, 1));

            var testMap1 = new Map("testMapKey2", wallCell);

            testMap1.SetCellsInRange(0, 11, 0, 4, floorCell);

            testMap1.RemoveCellsInRange(1, 2, 1, 1);
            testMap1.RemoveCellsInRange(5, 6, 2, 3);

            var testMap2 = new Map("testMapKey3", wallCell);

            testMap2.SetCellsInRange(0, 11, 0, 4, floorCell);
            testMap2.RemoveCellsInRange(1, 2, 1, 1);
            testMap2.RemoveCellsInRange(5, 6, 2, 3);

            testMap2.MapGenCommands.Add(
                new MapGenCommand {
                    MapGenCommandType = MapGenCommandType.Entity,
                    Parameters = "Portal:Portal",
                    Vector = new Vector(1,2)
            });

            var testMap3 = new Map("testMapKey4", wallCell);

            testMap3.SetCellsInRange(0, 11, 0, 4, floorCell);
            testMap3.RemoveCellsInRange(1, 2, 1, 1);
            testMap3.RemoveCellsInRange(5, 6, 2, 3);

            testMap3.MapGenCommands.Add(
                new MapGenCommand
                {
                    MapGenCommandType = MapGenCommandType.Entity,
                    Parameters = "Portal:Portal",
                    Vector = new Vector(1, 2)
                });

            testMap3.Biomes = new List<Biome> { new Biome { Name = "Forest" }, new Biome { Name = "Swamp" } };
            testMap3.VaultWeight = 0.5;

            return new[] { testMap0, testMap1, testMap2, testMap3 };
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
