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
    public class FovCacheTests
    {
        private const string MAP_KEY = "MAP_KEY";
        IFovCache _fovCache;

        [SetUp]
        public void SetUp()
        {
            _fovCache = new FovCache();
        }

        [Test]
        public void Cache_SavesFov()
        {
            var coord = new MapCoordinate(MAP_KEY, 0, 0);
            var fov = new List<MapCoordinate> { coord };

            _fovCache.TryGetCachedFov(coord, 1).Should().BeNull();

            _fovCache.Cache(coord, 1, fov);

            _fovCache.TryGetCachedFov(coord, 1).Should().BeEquivalentTo(fov);
        }

        [Test]
        public void TryGetCachedFov_DifferentParameters_ReturnsNull()
        {
            var coord = new MapCoordinate(MAP_KEY, 0, 0);
            var differentCoord = new MapCoordinate(MAP_KEY, 1, 0);
            var fov = new List<MapCoordinate> { coord };

            _fovCache.Cache(coord, 1, fov);

            _fovCache.TryGetCachedFov(coord, 2).Should().BeNull();
            _fovCache.TryGetCachedFov(differentCoord, 1).Should().BeNull();
        }

        [Test]
        public void Invalidate_ClearsFov()
        {
            var coord = new MapCoordinate(MAP_KEY, 0, 0);
            var fov = new List<MapCoordinate> { coord };

            _fovCache.Cache(coord, 1, fov);

            _fovCache.TryGetCachedFov(coord, 1).Should().BeEquivalentTo(fov);

            _fovCache.Invalidate();

            _fovCache.TryGetCachedFov(coord, 1).Should().BeNull();
        }
    }
}
