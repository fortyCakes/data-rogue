using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLTWrapper
{
    [Flags]
    public enum TileDirections : byte
    {
        None = 0,
        Left = 1,
        Up = 2,
        Right = 4,
        Down = 8,
        All = 15
    }
}
