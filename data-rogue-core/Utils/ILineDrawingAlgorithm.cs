using data_rogue_core.Maps;
using System.Collections.Generic;

namespace data_rogue_core.Utils
{
    public interface ILineDrawingAlgorithm
    {
        IEnumerable<MapCoordinate> DrawLine(MapCoordinate origin, MapCoordinate destination);
    }
}