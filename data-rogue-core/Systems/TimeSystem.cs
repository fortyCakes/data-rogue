using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    class TimeSystem : BaseSystem, ITimeSystem
    {
        public IBehaviourFactory BehaviourFactory { get; }

        public TimeSystem(IBehaviourFactory behaviourFactory)
        {
            BehaviourFactory = behaviourFactory;
        }

        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public ulong CurrentTime { get; set; }
        
        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Actor)};
        public override SystemComponents ForbiddenComponents => new SystemComponents{typeof(Prototype)};

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
            actor.HasActed = false;

            var behaviours = GetBehaviours(actor);

            foreach (IBehaviour behaviour in behaviours)
            {
                if (behaviour != null)
                {
                    var actionResult = behaviour.Act(entity);

                    if (actionResult.WaitForInput)
                    {
                        WaitingForInput = true;
                        break;
                    }
                    else if (actor.HasActed)
                    {
                        break;
                    }
                }
            }

            if (actor.NextTick <= CurrentTime)
            {
                actor.NextTick = CurrentTime + 1;
            }
        }

        private IEnumerable<IBehaviour> GetBehaviours(Actor actor)
        {
            var behaviours = actor.Behaviours?.Split(',') ?? new string[0];

            foreach (var behaviour in behaviours)
            {
                yield return BehaviourFactory.Get(behaviour);
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