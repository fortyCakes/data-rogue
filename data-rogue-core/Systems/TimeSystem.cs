using System;
using data_rogue_core.Components;
using data_rogue_core.Components.Behaviours;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    class TimeSystem : BaseSystem, ITimeSystem
    {
        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public ulong CurrentTime { get; set; }
        
        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Actor)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public void Tick()
        {
            CurrentTime++;

            if (Entities != null)
            {
                foreach (var entity in Entities)
                {
                    if (entity.Get<Actor>().NextTick <= CurrentTime)
                    {
                        Act(entity);
                    }
                }
            }
        }

        private void Act(IEntity entity)
        {
            var actor = entity.Get<Actor>();

            foreach (IEntityComponent component in entity.Components)
            {
                var behaviour = component as IBehaviour;
                if (behaviour != null)
                {
                    var actionResult = behaviour.Act();

                    if (actionResult.WaitForInput)
                    {
                        WaitingForInput = true;
                        break;
                    }
                    else if (actionResult.Acted)
                    {
                        actor.NextTick = CurrentTime + (ulong)actionResult.Ticks;
                        break;
                    }
                }
            }

            if (actor.NextTick <= CurrentTime)
            {
                actor.NextTick = CurrentTime + 1;
            }
        }

        public void SpendTicks(IEntity entity, int ticks)
        {
            var actor = entity.Get<Actor>();
            actor.NextTick = CurrentTime + (ulong)ticks;
        }

        public bool WaitingForInput { get; set; }

    }
}