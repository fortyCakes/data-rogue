using data_rogue_core.Maps;
using data_rogue_core.Utils;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Utils
{
    [TestFixture]
    public class BresenhamLineDrawingAlgorithmTests
    {
        private BresenhamLineDrawingAlgorithm _algorithm;
        private MapKey KEY = new MapKey("KEY");

        [SetUp]
        public void SetUp()
        {
            _algorithm = new BresenhamLineDrawingAlgorithm();
        }

        [Test]
        public void DrawLine_Horizontal()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 3, 0);

            var line = _algorithm.DrawLine(origin, destination);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 1, 0),
                new MapCoordinate(KEY, 2, 0),
                new MapCoordinate(KEY, 3, 0)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Vertical()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 0, 3);

            var line = _algorithm.DrawLine(origin, destination);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 0, 1),
                new MapCoordinate(KEY, 0, 2),
                new MapCoordinate(KEY, 0, 3)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Diagonal()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 3, 3);

            var line = _algorithm.DrawLine(origin, destination);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 1, 1),
                new MapCoordinate(KEY, 2, 2),
                new MapCoordinate(KEY, 3, 3)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Diagonal_Low()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 6, 3);

            var line = _algorithm.DrawLine(origin, destination);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 1, 0),
                new MapCoordinate(KEY, 2, 1),
                new MapCoordinate(KEY, 3, 1),
                new MapCoordinate(KEY, 4, 2),
                new MapCoordinate(KEY, 5, 2),
                new MapCoordinate(KEY, 6, 3)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Diagonal_High()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 3, 6);

            var line = _algorithm.DrawLine(origin, destination);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 0, 1),
                new MapCoordinate(KEY, 1, 2),
                new MapCoordinate(KEY, 1, 3),
                new MapCoordinate(KEY, 2, 4),
                new MapCoordinate(KEY, 2, 5),
                new MapCoordinate(KEY, 3, 6)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Diagonal_Low_Reverse()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 6, 3);

            var line = _algorithm.DrawLine(destination, origin);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 1, 0),
                new MapCoordinate(KEY, 2, 1),
                new MapCoordinate(KEY, 3, 1),
                new MapCoordinate(KEY, 4, 2),
                new MapCoordinate(KEY, 5, 2),
                new MapCoordinate(KEY, 6, 3)
            };

            line.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void DrawLine_Diagonal_High_Reverse()
        {
            var origin = new MapCoordinate(KEY, 0, 0);
            var destination = new MapCoordinate(KEY, 3, 6);

            var line = _algorithm.DrawLine(destination, origin);

            var expected = new List<MapCoordinate>
            {
                new MapCoordinate(KEY, 0, 0),
                new MapCoordinate(KEY, 0, 1),
                new MapCoordinate(KEY, 1, 2),
                new MapCoordinate(KEY, 1, 3),
                new MapCoordinate(KEY, 2, 4),
                new MapCoordinate(KEY, 2, 5),
                new MapCoordinate(KEY, 3, 6)
            };

            line.Should().BeEquivalentTo(expected);
        }
    }
}
