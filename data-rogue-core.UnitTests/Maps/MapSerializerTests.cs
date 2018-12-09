using data_rogue_core.Components;
using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    public class MapSerializerTests
    {
        private EntityEngineSystem _entityEngine;
        private IEntity _wallCell;
        private IEntity _floorCell;

        [SetUp]
        public void SetUp()
        {
            _entityEngine = new EntityEngineSystem();

            _wallCell = CreateCell('#');
            _floorCell = CreateCell('.');
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
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
        public void Deserialize_File1_ReturnsTestMap(int testCase)
        {
            string serialisedText = LoadSerializedData(testCase);

            var result = MapSerializer.Deserialize(serialisedText, _entityEngine);

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
            var testMap0 = new Map("testMapKey", _wallCell);

            testMap0.SetCellsInRange(-4, 3, -2, 2, _floorCell);
            testMap0.SetCell(1, 1, _wallCell);

            var testMap1 = new Map("testMapKey2", _wallCell);

            testMap1.SetCellsInRange(0, 11, 0, 4, _floorCell);

            testMap1.RemoveCellsInRange(1, 2, 1, 1);
            testMap1.RemoveCellsInRange(5, 6, 2, 3);

            return new[] { testMap0, testMap1 };
        }

        private IEntity CreateCell(char glyph)
        {
             return _entityEngine.New("cell",
                new Appearance { Glyph = glyph },
                new Physical()
                );
        }
    }
}
