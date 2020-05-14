using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{
    public interface ISpriteSheet
    {
        string Name { get; }

        int Tile(TileDirections directions, int frame);
        int Tile(TileDirections directions);

        int Frames { get; }
    }
}
