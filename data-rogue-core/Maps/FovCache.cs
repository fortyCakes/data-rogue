using System.Collections.Generic;

namespace data_rogue_core.Maps
{
    public class FovCache : IFovCache
    {
        private Dictionary<(MapCoordinate, int), List<MapCoordinate>> _cache;

        public FovCache()
        {
            _cache = new Dictionary<(MapCoordinate, int), List<MapCoordinate>>();
        }

        public void Cache(MapCoordinate mapCoordinate, int range, List<MapCoordinate> visibleCells)
        {
            _cache[(mapCoordinate, range)] = visibleCells;
        }

        public void Invalidate()
        {
            _cache.Clear();
        }

        public List<MapCoordinate> TryGetCachedFov(MapCoordinate mapCoordinate, int range)
        {
            if (_cache.ContainsKey((mapCoordinate, range)))
            {
                return _cache[(mapCoordinate, range)];
            }

            return null;
        }
    }
}