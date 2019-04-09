using System.Collections.Generic;

namespace BLTWrapper
{
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
            {TileDirections.None, 5},
            {TileDirections.Left, 13},
            {TileDirections.Right, 11},
            {TileDirections.Down, 3},
            {TileDirections.Up, 17},
            {TileDirections.Left | TileDirections.Up, 16},
            {TileDirections.Right | TileDirections.Left, 12},
            {TileDirections.Up | TileDirections.Right, 14},
            {TileDirections.Up | TileDirections.Left | TileDirections.Right, 15},
            {TileDirections.Left | TileDirections.Down, 2},
            {TileDirections.Up | TileDirections.Down, 10},
            {TileDirections.Down |TileDirections.Up|TileDirections.Left, 9},
            {TileDirections.Down|TileDirections.Right, 0},
            {TileDirections.Left|TileDirections.Right|TileDirections.Down, 1},
            {TileDirections.Right|TileDirections.Down|TileDirections.Up, 7},
            {TileDirections.Down|TileDirections.Up|TileDirections.Left|TileDirections.Right, 8}
        };

    }
}