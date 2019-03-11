using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IPlayerSystem
    {
        IEntity Player { get; set; }

        bool IsPlayer(IEntity sender);
        void StartCharacterCreation();
    }
}
