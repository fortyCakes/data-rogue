using System;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Components
{
    public class AegisRecovery : ITickUpdate
    {
        public void Tick(ISystemContainer systemContainer, IEntity entity, ulong currentTime)
        {
            if (IsAegisTick(currentTime))
            {
                var maxAegis = systemContainer.EventSystem.GetStat(entity, "Aegis");

                var currentAegis = systemContainer.StatSystem.GetEntityStat(entity, "CurrentAegisLevel");

                if (currentAegis < maxAegis)
                {
                    if (!entity.IsPlayer || systemContainer.EventSystem.GetStat(entity, "Tension") == 0)
                    {
                        systemContainer.StatSystem.SetStat(entity, "CurrentAegisLevel", currentAegis + 1);
                    }
                }
            }
        }

        private static bool IsAegisTick(ulong currentTime)
        {
            return currentTime % 1000 == 0;
        }
    }
}