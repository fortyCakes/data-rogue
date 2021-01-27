using System.Collections.Generic;

namespace data_rogue_core.Maps
{
    public interface IFovCache
    {
        void Invalidate();
        List<MapCoordinate> TryGetCachedFov(MapCoordinate mapCoordinate, int range);
        void Cache(MapCoordinate mapCoordinate, int range, List<MapCoordinate> visibleCells);
    }
}