using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.EventSystem.EventData
{
    public class PickupItemEventData
    {
        public IEntity Item { get; internal set; }
    }

    public class PickupWealthEventData
    {
        public IEntity Item { get; internal set; }
    }
}
