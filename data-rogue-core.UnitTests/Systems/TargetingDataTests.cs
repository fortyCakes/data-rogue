using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class TargetingDataTests
    {
        private MapCoordinate testCoordinate;

        [SetUp]
        public void SetUp()
        {
            testCoordinate = new MapCoordinate("key", 0, 0);
        }

        [Test]
        public void TargetableCellsFrom_Range0_CorrectSet()
        {
            var targetingData = SetUpTestTargetingData(range: 0);

            var results = targetingData.TargetableCellsFrom(testCoordinate);

            var expected = new List<MapCoordinate>
            {
                testCoordinate + new Vector(0, 1),
                testCoordinate + new Vector(1, 0),
                testCoordinate + new Vector(0, -1),
                testCoordinate + new Vector(-1, 0)
            };

            results.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void TargetableCellsFrom_Range1_CorrectSet()
        {
            var targetingData = SetUpTestTargetingData(range: 1);

            var results = targetingData.TargetableCellsFrom(testCoordinate);

            var expected = new List<MapCoordinate>
            {
                testCoordinate + new Vector(0, 1),
                testCoordinate + new Vector(1, 0),
                testCoordinate + new Vector(0, -1),
                testCoordinate + new Vector(-1, 0),
                testCoordinate + new Vector(1, 1),
                testCoordinate + new Vector(1, -1),
                testCoordinate + new Vector(-1, -1),
                testCoordinate + new Vector(-1, 1)
            };

            results.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void TargetableCellsFrom_Range2_CorrectSet()
        {
            var targetingData = SetUpTestTargetingData(range: 2);

            var results = targetingData.TargetableCellsFrom(testCoordinate);

            var expected = new List<MapCoordinate>
            {

                testCoordinate + new Vector(-2, 0),

                testCoordinate + new Vector(-1, -1),
                testCoordinate + new Vector(-1, 0),
                testCoordinate + new Vector(-1, 1),

                testCoordinate + new Vector(0, -2),
                testCoordinate + new Vector(0, -1),
                testCoordinate + new Vector(0, 1),
                testCoordinate + new Vector(0, 2),

                testCoordinate + new Vector(1, -1),
                testCoordinate + new Vector(1, 0),
                testCoordinate + new Vector(1, 1),

                testCoordinate + new Vector(2, 0)
            };

            results.Should().BeEquivalentTo(expected);
        }

        private static TargetingData SetUpTestTargetingData(int range, List<Vector> cellsHit = null, bool friendly = false, bool moveToCell = false, bool rotatable = false, List<Vector> validVectors = null, bool targetOrigin = false)
        {
            if (cellsHit == null) cellsHit = new List<Vector> {new Vector(0, 0)};
            return new TargetingData { Range = range, CellsHit = cellsHit, Friendly = friendly, MoveToCell = moveToCell, Rotatable = rotatable, ValidVectors = validVectors, TargetOrigin = targetOrigin};
        }
    }
}
