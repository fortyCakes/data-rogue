using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Behaviours
{
    public class PlayerRestBehaviour : BaseBehaviour
    {
        public bool Resting;

        private readonly IEventSystem EventSystem;
        private readonly IMessageSystem MessageSystem;

        public PlayerRestBehaviour(IEventSystem eventSystem, IMessageSystem messageSystem)
        {
            EventSystem = eventSystem;
            MessageSystem = messageSystem;
        }

        public override BehaviourResult Act(IEntity entity)
        {
            if (!Resting)
            {
                return new BehaviourResult { Acted = false };
            }

            if (EventSystem.GetStat(entity, Stat.Tension) > 0)
            {
                MessageSystem.Write("Your rest is interrupted!");
                Resting = false;
                return new BehaviourResult { Acted = false };
            }

            var fighter = entity.Get<Fighter>();

            if (fighter.Tilt.Current == 0 && fighter.Aura.Current == fighter.BaseAura && fighter.BrokenTicks == 0)
            {
                MessageSystem.Write("You finish resting.");
                Resting = false;
                return new BehaviourResult { Acted = false };
            }

            SpendTimeEventData time = new SpendTimeEventData {Ticks = 1000};
            EventSystem.Try(EventType.SpendTime, entity, time);

            return new BehaviourResult();
        }

    }
}