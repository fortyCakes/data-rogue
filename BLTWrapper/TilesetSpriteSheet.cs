using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{
    public class TilesetSpriteSheet : ISpriteSheet
    {
        public TilesetSpriteSheet(string name, int offset)
        {
            _offset = offset;
            Name = name;
        }

        public string Name { get; }

        public int Tile(TileDirections directions)
        {
            return _offset + mapping[directions];
        }

        private int _offset;

        private Dictionary<TileDirections, int> mapping = new Dictionary<TileDirections, int>
        {
            {TileDirections.None, 3},
            {TileDirections.Left, 1 },
            {TileDirections.Right, 1 },
            {TileDirections.Down, 6 },
            {TileDirections.Up, 7 },
            {TileDirections.Left | TileDirections.Up, 14 },
            {TileDirections.Right | TileDirections.Left, 1 },
            {TileDirections.Up | TileDirections.Right, 12 },
            {TileDirections.Up | TileDirections.Left | TileDirections.Right, 16 },
            {TileDirections.Left | TileDirections.Down, 2 },
            {TileDirections.Up | TileDirections.Down, 6 },
            {TileDirections.Down |TileDirections.Up|TileDirections.Left, 11 },
            {TileDirections.Down|TileDirections.Right, 0 },
            {TileDirections.Left|TileDirections.Right|TileDirections.Down, 4 },
            {TileDirections.Right|TileDirections.Down|TileDirections.Up, 9 },
            {TileDirections.Down|TileDirections.Up|TileDirections.Left|TileDirections.Right, 10 }
        };

    }

    public class BoxTilesetSpriteSheet : ISpriteSheet
    {
        public BoxTilesetSpriteSheet(string name, int offset)
        {
            _offset = offset;
            Name = name;
        }

        public string Name { get; }

        public int Tile(TileDirections directions)
        {
            return _offset + mapping[directions];
        }

        private int _offset;

        private Dictionary<TileDirections, int> mapping = new Dictionary<TileDirections, int>
        {
            {TileDirections.None, 0},
            {TileDirections.Left, 6},
            {TileDirections.Right, 6},
            {TileDirections.Down, 6},
            {TileDirections.Up, 6},
            {TileDirections.Left | TileDirections.Up, 11},
            {TileDirections.Right | TileDirections.Left, 6},
            {TileDirections.Up | TileDirections.Right, 9},
            {TileDirections.Up | TileDirections.Left | TileDirections.Right, 10},
            {TileDirections.Left | TileDirections.Down, 3},
            {TileDirections.Up | TileDirections.Down, 6},
            {TileDirections.Down |TileDirections.Up|TileDirections.Left, 7},
            {TileDirections.Down|TileDirections.Right, 1},
            {TileDirections.Left|TileDirections.Right|TileDirections.Down, 2},
            {TileDirections.Right|TileDirections.Down|TileDirections.Up, 5},
            {TileDirections.Down|TileDirections.Up|TileDirections.Left|TileDirections.Right, 6}
        };

    }
}
