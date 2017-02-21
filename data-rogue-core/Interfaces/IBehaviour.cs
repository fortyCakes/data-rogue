using data_rogue_core.Entities;
using data_rogue_core.System;

namespace data_rogue_core.Interfaces
{
    public interface IBehaviour
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}