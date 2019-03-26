using System;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Components
{

    public class AuraFighter : IEntityComponent, ITickUpdate
    {
        public Counter Aura;
        public int BaseAura;
        public void Tick(IEventSystem eventSystem, IPlayerSystem playerSystem, IEntity entity, ulong currentTime)
        {
            if (currentTime % 100 == 0)
            {
                if (entity.IsPlayer)
                {
                    var tension = eventSystem.GetStat(entity, "Tension");

                    if (tension > 0)
                    {
                        var auraAmount = Math.Ceiling(Math.Log((double) tension + 1));

                        Aura.Add((int) auraAmount);
                    }
                    else
                    {
                        if (Aura.Current > BaseAura)
                        {
                            Aura.Subtract(1);
                        }
                        else if (Aura.Current < BaseAura)
                        {
                            Aura.Add(1);
                        }
                    }
                }
            }
        }
    }
}
