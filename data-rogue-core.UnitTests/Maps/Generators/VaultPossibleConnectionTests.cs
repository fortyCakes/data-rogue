using BLTWrapper;
using data_rogue_core.Maps;
using data_rogue_core.Maps.Generators;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Maps.Generators
{
    [TestFixture]
    public class VaultPossibleConnectionTests
    {
        [Test]
        [TestCase(0, 0, TileDirections.Up, 3,3, TileDirections.Left, 0, 3)]
        [TestCase(1, 1, TileDirections.Up, 3, 72, TileDirections.Left, 1, 72)]
        [TestCase(5, 5, TileDirections.Down, 1, 2, TileDirections.Right, 5, 2)]
        public void GetCorner_GetsCornerCoordinate(int x0, int y0, TileDirections firstFacing, int x1, int y1, TileDirections secondFacing, int xE, int yE)
        {
            var map = Substitute.For<IMap>();
            map.Origin.Returns(new Vector(0, 0));
            map.GetSize().ReturnsForAnyArgs(new System.Drawing.Size(0, 0));

            var poss = new VaultPossibleConnection
            {
                First = new VaultConnectionPoint(new MapCoordinate("", x0, y0), new Vector(0, 0), map),
                Second = new VaultConnectionPoint(new MapCoordinate("", x1, y1), new Vector(0, 0), map)
            };

            poss.First.Facing = firstFacing;
            poss.Second.Facing = secondFacing;

            poss.IsRightAngle.Should().BeTrue();

            poss.GetCorner().Should().Be(new MapCoordinate("", xE, yE));
        }

        [Test]
        [TestCase(0, 0, TileDirections.Up, 3, 3, TileDirections.Down, 0, 1, 3, 1)]
        [TestCase(1, 1, TileDirections.Up, 3, 72, TileDirections.Down, 1, 36, 3, 36)]
        [TestCase(5, 5, TileDirections.Left, 0, 0, TileDirections.Right, 2, 5, 2, 0)]
        public void GetCorners_GetsCornersCoordinates(int x0, int y0, TileDirections firstFacing, int x1, int y1, TileDirections secondFacing, int xE, int yE, int xE2, int yE2)
        {
            var map = Substitute.For<IMap>();
            map.Origin.Returns(new Vector(0, 0));
            map.GetSize().ReturnsForAnyArgs(new System.Drawing.Size(0, 0));

            var poss = new VaultPossibleConnection
            {
                First = new VaultConnectionPoint(new MapCoordinate("", x0, y0), new Vector(0, 0), map),
                Second = new VaultConnectionPoint(new MapCoordinate("", x1, y1), new Vector(0, 0), map)
            };

            poss.First.Facing = firstFacing;
            poss.Second.Facing = secondFacing;

            poss.IsRightAngle.Should().BeFalse();

            var (first, second) = poss.GetCorners();

            first.Should().Be(new MapCoordinate("", xE, yE));
            second.Should().Be(new MapCoordinate("", xE2, yE2));
        }
    }
}
