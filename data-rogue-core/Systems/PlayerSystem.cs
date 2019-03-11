using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class PlayerSystem : IPlayerSystem
    {
        private IActivitySystem _activitySystem;

        public PlayerSystem(IActivitySystem activitySystem)
        {
            _activitySystem = activitySystem;
        }

        public IEntity Player { get; set; }

        public bool IsPlayer(IEntity sender)
        {
            return sender == Player;
        }
    }
}