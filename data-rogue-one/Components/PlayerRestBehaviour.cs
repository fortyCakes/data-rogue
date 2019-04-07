using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Behaviours
{
    public class PlayerRestBehaviour : BaseBehaviour
    {
        public bool Resting;

        private readonly IEventSystem EventSystem;
        private readonly IMessageSystem MessageSystem;

        public PlayerRestBehaviour(ISystemContainer systemContainer)
        {
            EventSystem = systemContainer.EventSystem;
            MessageSystem = systemContainer.MessageSystem;
        }

        public override ActionEventData ChooseAction(IEntity entity)
        {
            if (!Resting)
            {
                return null;
            }

            if (EventSystem.GetStat(entity, "Tension") > 0)
            {
                MessageSystem.Write("Your rest is interrupted!");
                Resting = false;
                return null;
            }

            var tilt = entity.Get<TiltFighter>();
            var aura = entity.Get<AuraFighter>();
            var currentAegis = EventSystem.GetStat(entity, "CurrentAegisLevel");
            var maxAegis = EventSystem.GetStat(entity, "Aegis");

            if (tilt.Tilt.Current == 0 && 
                aura.Aura.Current == aura.BaseAura && 
                tilt.BrokenTicks == 0 &&
                currentAegis == maxAegis)
            {
                MessageSystem.Write("You finish resting.");
                Resting = false;
                return null;
            }

            return new ActionEventData {Action = ActionType.Wait, Speed = 1000};
        }

    }
}