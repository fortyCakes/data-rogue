using data_rogue_core.Maps;
using FluentAssertions;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    class MapKeyTests
    {
        [Test]
        public void Equals_SameString_ReturnsTrue()
        {
            var mapKey1 = new MapKey("testMap");
            var mapKey2 = new MapKey("testMap");

            mapKey1.Equals(mapKey2).Should().BeTrue();

            (mapKey1 == mapKey2).Should().BeTrue();

            mapKey1.GetHashCode().Should().Be(mapKey2.GetHashCode());

            (mapKey1 != mapKey2).Should().BeFalse();
        }
    }
}
