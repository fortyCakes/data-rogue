using System;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Components
{
    public class AegisRecovery : ITickUpdate
    {
        public void Tick(IEventSystem eventSystem, IPlayerSystem playerSystem, IStatSystem statSystem, IEntity entity, ulong currentTime)
        {
            var maxAegis = eventSystem.GetStat(entity, "Aegis");

            var currentAegis = statSystem.GetEntityStat(entity, "CurrentAegisLevel");

            if (currentAegis < maxAegis && currentTime % 1000 == 0)
            {
                if (eventSystem.GetStat(entity, "Tension") == 0)
                {
                    statSystem.SetStat(entity, "CurrentAegisLevel", currentAegis + 1);
                }
            }
        }
    }
}