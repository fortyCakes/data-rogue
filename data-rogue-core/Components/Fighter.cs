using System.Drawing;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Components
{
    public class Fighter : IEntityComponent
    {
        public StatCounter Health;

        public int Attack;
        public int Defence;
    }
}
