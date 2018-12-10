using data_rogue_core.Maps;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    class MapCoordinateTests
    {
        [Test]
        public void Equals_SameCoordinate_ReturnsTrue()
        {
            var mapKey1 = new MapKey("testMap");
            var mapKey2 = new MapKey("testMap");

            var mapCoordinate1 = new MapCoordinate(mapKey1, 1, 1);
            var mapCoordinate2 = new MapCoordinate(mapKey2, 1, 1);

            mapCoordinate1.Equals(mapCoordinate2).Should().BeTrue();

            (mapCoordinate1 == mapCoordinate2).Should().BeTrue();

            mapCoordinate1.GetHashCode().Should().Be(mapCoordinate2.GetHashCode());

            (mapCoordinate1 != mapCoordinate2).Should().BeFalse();
        }

        [Test]
        public void DictionaryIndex()
        {
            var mapKey1 = new MapKey("testMap");
            var mapKey2 = new MapKey("testMap");

            var mapCoordinate1 = new MapCoordinate(mapKey1, 1, 1);
            var mapCoordinate2 = new MapCoordinate(mapKey2, 1, 1);

            var dictionary = new Dictionary<MapCoordinate, bool>();

            dictionary.Add(mapCoordinate1, true);

            dictionary.ContainsKey(mapCoordinate2).Should().BeTrue();
            dictionary[mapCoordinate2].Should().BeTrue();
        }
    }
}
