using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    class Actor : IEntityComponent
    {
        public ulong NextTick;
        public bool HasActed;
    }
}
