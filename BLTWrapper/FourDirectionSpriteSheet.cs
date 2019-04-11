using System.Collections.Generic;

namespace BLTWrapper
{
    public class FourDirectionSpriteSheet : ISpriteSheet
    {
        public FourDirectionSpriteSheet(string name, int offset)
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
            {TileDirections.None, 2},
            {TileDirections.Left, 3},
            {TileDirections.Right, 1},
            {TileDirections.Down, 2},
            {TileDirections.Up, 0},
            {TileDirections.Left | TileDirections.Up, 2},
            {TileDirections.Right | TileDirections.Left, 2},
            {TileDirections.Up | TileDirections.Right, 2},
            {TileDirections.Up | TileDirections.Left | TileDirections.Right, 2},
            {TileDirections.Left | TileDirections.Down, 2},
            {TileDirections.Up | TileDirections.Down, 2},
            {TileDirections.Down |TileDirections.Up|TileDirections.Left, 2},
            {TileDirections.Down|TileDirections.Right, 2},
            {TileDirections.Left|TileDirections.Right|TileDirections.Down, 2},
            {TileDirections.Right|TileDirections.Down|TileDirections.Up, 2},
            {TileDirections.Down|TileDirections.Up|TileDirections.Left|TileDirections.Right, 2}
        };

    }
}