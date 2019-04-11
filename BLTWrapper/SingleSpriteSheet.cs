using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{

    public class SingleSpriteSheet : ISpriteSheet
    {
        public string Name { get; }

        private int _offset;

        public SingleSpriteSheet(string name, int offset)
        {
            Name = name;
            _offset = offset;
        }

        public int Tile(TileDirections directions)
        {
            return _offset;
        }
    }
}
