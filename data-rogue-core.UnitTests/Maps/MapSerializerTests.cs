﻿using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.IO;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    public class MapSerializerTests
    {
        private ISystemContainer systemContainer;
        private IEntity wallCell;
        private IEntity floorCell;

        [SetUp]
        public void SetUp()
        {
            systemContainer = new SystemContainer();

            systemContainer.EntityEngine = new EntityEngine.EntityEngine(new NullStaticEntityLoader());
            systemContainer.PrototypeSystem = Substitute.For<IPrototypeSystem>();

            wallCell = CreateCell('#', "Cell:Wall");
            systemContainer.PrototypeSystem.Create("Cell:Wall").Returns(wallCell);

            floorCell = CreateCell('.', "Cell:Empty");
            systemContainer.PrototypeSystem.Create("Cell:Empty").Returns(floorCell);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
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

            return new[] { testMap0, testMap1, testMap2 };
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
