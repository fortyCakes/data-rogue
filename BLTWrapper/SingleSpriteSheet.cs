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

        public int Frames { get; }

        private int _offset;

        public SingleSpriteSheet(string name, int offset, int frames)
        {
            Name = name;
            Frames = frames;
            _offset = offset;
        }

        public int Tile(TileDirections directions, int frame)
        {
            return _offset + frame;
        }

        public int Tile(TileDirections directions)
        {
            return Tile(directions, 0);
        }
    }
}
