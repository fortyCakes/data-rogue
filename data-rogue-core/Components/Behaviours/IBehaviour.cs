using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Components.Behaviours
{
    public interface IBehaviour
    {
        BehaviourResult Act();
    }
}
