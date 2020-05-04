using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Maps;

namespace data_rogue_core.Utils
{
    public static class MapCoordinateExtensions
    {
        public static List<MapCoordinate> AdjacentCells(this MapCoordinate cell)
        {
            return new List<MapCoordinate>
            {
                cell + new Vector(-1, -1),
                cell + new Vector(-1, +0),
                cell + new Vector(-1, +1),

                cell + new Vector(+0, -1),
                cell + new Vector(+0, -1),

                cell + new Vector(+1, -1),
                cell + new Vector(+1, +0),
                cell + new Vector(+1, +1),
            };
        }
    }
}
