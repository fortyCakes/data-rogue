﻿using data_rogue_core.Activities;
using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems
{
    public class TargetingSystem : ITargetingSystem
    {
        public void GetTarget(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            if (sender.IsPlayer)
            {
                GetTargetForPlayer(data, callback);
            }
            else
            {
                GetTargetForNonPlayer(sender, data, callback);
            }
        }

        private void GetTargetForPlayer(TargetingData data, Action<MapCoordinate> callback)
        {
            var activity = new TargetingActivity(data, callback, Game.RendererFactory);

            Game.ActivityStack.Push(activity);
        }

        private void GetTargetForNonPlayer(IEntity sender, TargetingData data, Action<MapCoordinate> callback)
        {
            throw new NotImplementedException();
        }
    }
}
