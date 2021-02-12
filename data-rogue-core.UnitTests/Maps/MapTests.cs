using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    public class MapTests
    {
        private IEntity _defaultCell;
        private IEntity _voidCell;
        private IEntity _floorCell;

        [SetUp]
        public void SetUp()
        {
            var physical = new Physical { };
            var appearance = new Appearance { };

            _defaultCell = new Entity(0, "defaultCell", new IEntityComponent[] { physical, appearance });
            _voidCell = new Entity(0, "voidCell", new IEntityComponent[] { physical, appearance });
            _floorCell = new Entity(0, "floorCell", new IEntityComponent[] { physical, appearance });
        }

        [Test]
        public void PlaceSubMap_OffCentre_PlacesCorrectly()
        {
            var map = new Map("Main", _defaultCell);
            var subMap = new Map("Vault", _voidCell);

            subMap.SetCell(3, 3, _floorCell);
            subMap.SetCell(4, 4, _floorCell);
            subMap.SetCell(5, 5, _floorCell);
            subMap.SetCell(3, 5, _floorCell);
            subMap.MapGenCommands.Add(new MapGenCommand { MapGenCommandType = MapGenCommandType.Entity, Parameters = "Test", Vector = new Vector(3, 5) });

            map.PlaceSubMap(new MapCoordinate("Main", 10, 10), subMap);

            map.CellAt(10, 10).Should().Be(_floorCell);
            map.CellAt(11, 11).Should().Be(_floorCell);
            map.CellAt(12, 12).Should().Be(_floorCell);
            map.CellAt(10, 12).Should().Be(_floorCell);

            map.MapGenCommands.Single().Vector.Should().Be(new Vector(10, 12));

        }

        [Test]
        public void Rotate_By90_RotatesCorrectly()
        {
            var subMap = new Map("Vault", _voidCell);

            subMap.SetCell(3, 0, _floorCell);
            subMap.SetCell(4, 0, _floorCell);
            subMap.SetCell(5, 0, _floorCell);
            subMap.SetCell(3, 5, _floorCell);
            subMap.MapGenCommands.Add(new MapGenCommand { MapGenCommandType = MapGenCommandType.Entity, Parameters = "Test", Vector = new Vector(3, 5) });

            subMap.Rotate(Matrix.Rotate90);

            subMap.CellAt(0, 3).Should().Be(_floorCell);
            subMap.CellAt(3, 0).Should().Be(_voidCell);
            subMap.CellAt(0, 4).Should().Be(_floorCell);
            subMap.CellAt(4, 0).Should().Be(_voidCell);
            subMap.CellAt(0, 5).Should().Be(_floorCell);
            subMap.CellAt(5, 0).Should().Be(_voidCell);

            subMap.CellAt(-5, 3).Should().Be(_floorCell);

            var rotatedVector = new Vector(-5, 3);

            subMap.MapGenCommands.Single().Vector.Should().Be(rotatedVector);

        }
    }
}
