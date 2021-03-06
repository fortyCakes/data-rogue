﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{
    public class TilesetSpriteSheet : ISpriteSheet
    {
        public int Frames => 1;

        public TilesetSpriteSheet(string name, int offset)
        {
            _offset = offset;
            Name = name;
        }

        public string Name { get; }

        public int Tile(TileDirections directions, int frame)
        {
            return _offset + mapping[directions];
        }
        public int Tile(TileDirections directions)
        {
            return Tile(directions, 0);
        }

        private int _offset;

        private Dictionary<TileDirections, int> mapping = new Dictionary<TileDirections, int>
        {
            {TileDirections.None, 3},

            {TileDirections.Left, 1 },
            {TileDirections.Right, 1 },
            {TileDirections.Down, 6 },
            {TileDirections.Up, 7 },

            {TileDirections.Right | TileDirections.Left, 1 },
            {TileDirections.Up | TileDirections.Down, 6 },

            {TileDirections.Left | TileDirections.Up, 14 },
            {TileDirections.Up | TileDirections.Right, 12 },
            {TileDirections.Down|TileDirections.Right, 0 },
            {TileDirections.Left | TileDirections.Down, 2 },

            {TileDirections.Up | TileDirections.Left | TileDirections.Right, 16 },
            {TileDirections.Down |TileDirections.Up|TileDirections.Left, 11 },
            {TileDirections.Left|TileDirections.Right|TileDirections.Down, 4 },
            {TileDirections.Right|TileDirections.Down|TileDirections.Up, 9 },

            {TileDirections.Down|TileDirections.Up|TileDirections.Left|TileDirections.Right, 10 }
        };

    }
}
