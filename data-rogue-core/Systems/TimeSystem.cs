using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_core.Systems
{
    class TimeSystem : BaseSystem, ITimeSystem
    {
        private MapKey ActiveMapKey;

        public IBehaviourFactory BehaviourFactory { get; }
        public IEventSystem EventSystem { get; }

        public TimeSystem(IBehaviourFactory behaviourFactory, IEventSystem eventSystem)
        {
            BehaviourFactory = behaviourFactory;
            EventSystem = eventSystem;
        }

        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public bool TiltTick => CurrentTime % 1000 == 0;

        public bool AuraTick => CurrentTime % 100 == 0;

        public ulong CurrentTime { get; set; }

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Actor), typeof(Fighter) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public void Tick()
        {
            CurrentTime++;

            ActiveMapKey = Game.WorldState.Player.Get<Position>().MapCoordinate.Key;

            if (Entities != null)
            {
                var entitiesAtStartOfTick = new List<IEntity>(Entities);

                foreach (var entity in entitiesAtStartOfTick)
                {
                    if (entity.Has<Fighter>())
                    {
                        TickFighter(entity);
                    }

                    if (entity.Get<Actor>().NextTick <= CurrentTime)
                    {
                        Act(entity);
                    }
                }
            }
        }

        private void Act(IEntity entity)
        {
            if (entity.Has<Position>())
            {
                var mapKey = entity.Get<Position>().MapCoordinate.Key;

                if (mapKey != ActiveMapKey) return;
            }

            if (entity.Has<Actor>())
            {
                TickActor(entity);
            }
        }

        private void TickFighter(IEntity entity)
        {
            var fighter = entity.Get<Fighter>();

            UpdateTilt(fighter);

            if (AuraTick)
            {
                UpdateAura(entity, fighter);
            }
        }

        private void UpdateAura(IEntity entity, Fighter fighter)
        {
            if (entity.IsPlayer)
            {
                var tension = EventSystem.GetStat(entity, Stat.Tension);

                if (tension > 0)
                {
                    var auraAmount = Math.Ceiling(Math.Log((double) tension + 1));

                    fighter.Aura.Add((int) auraAmount);
                }
                else
                {
                    fighter.Aura.Subtract(1);
                }
            }
        }

        private void UpdateTilt(Fighter fighter)
        {
            if (fighter.BrokenTicks > 0)
            {
                fighter.BrokenTicks--;
                if (fighter.BrokenTicks == 0)
                {
                    fighter.Tilt.Current /= 2;
                }

                return;
            }

            if (fighter.Tilt.Current > 0 && TiltTick)
            {
                fighter.Tilt.Current--;
            }
        }

        private void TickActor(IEntity entity)
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

        public string TimeString  {
            get
            {
                var timeInAut = CurrentTime / 100;
                return timeInAut.ToString("F2");
            }            
        }
    }
}