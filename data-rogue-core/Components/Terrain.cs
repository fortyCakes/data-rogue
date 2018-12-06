using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Components
{
    public struct Terrain : IEntityComponent
    {
        public bool Passable;
        public bool Transparent;
    }
}
