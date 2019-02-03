using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.EventSystem.EventData
{
    public class DamageEventData
    {
        public int Damage { get; set; }

        public bool Overwhelming = false;
    }
}
