using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.EventSystem.EventData
{
    public enum Stat
    {
        Muscle,
        Agility
    }

    public class GetStatEventData
    {
        public Stat Stat;
        public decimal Value;
    }
}
