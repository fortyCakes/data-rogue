using System.Collections.Generic;
using data_rogue_core.Maps;

namespace data_rogue_core.Utils
{
    public interface IPathfindingAlgorithm
    {
        IEnumerable<MapCoordinate> Path(IMap map, MapCoordinate origin, MapCoordinate destination);
    }
}