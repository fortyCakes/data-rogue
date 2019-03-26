using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
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
        private readonly IPlayerSystem _playerSystem;
        private IEventSystem _eventSystem;
        private IStatSystem _statSystem;

        public IBehaviourFactory BehaviourFactory { get; }

        public TimeSystem(IBehaviourFactory behaviourFactory, IEventSystem eventSystem, IPlayerSystem playerSystem, IStatSystem statSystem)
        {
            BehaviourFactory = behaviourFactory;
            _eventSystem = eventSystem;
            _playerSystem = playerSystem;
            _statSystem = statSystem;
        }

        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public ulong CurrentTime { get; set; }

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Actor), typeof(TiltFighter) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public void Tick()
        {
            CurrentTime++;

            ActiveMapKey = _playerSystem.Player.Get<Position>().MapCoordinate.Key;

            if (Entities != null)
            {
                var entitiesAtStartOfTick = new List<IEntity>(Entities);

                foreach (var entity in entitiesAtStartOfTick)
                {
                    if (!entity.Removed)
                    {
                        RunTickUpdates(entity);

                        if (entity.Get<Actor>().NextTick <= CurrentTime)
                        {
                            Act(entity);
                        }
                    }
                }
            }
        }

        private void RunTickUpdates(IEntity entity)
        {
            var updatables = entity.Components.Where(c => c is ITickUpdate).ToList();

            foreach (var updatable in updatables)
            {
                ((ITickUpdate) updatable).Tick(_eventSystem, _playerSystem, _statSystem, entity, CurrentTime);
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

        private void UpdateAura(IEntity entity)
        {
            var fighter = entity.Get<AuraFighter>();

            if (entity.IsPlayer)
            {
                var tension = _eventSystem.GetStat(entity, "Tension");

                if (tension > 0)
                {
                    var auraAmount = Math.Ceiling(Math.Log((double) tension + 1));

                    fighter.Aura.Add((int) auraAmount);
                }
                else
                {
                    if (fighter.Aura.Current > fighter.BaseAura)
                    {
                        fighter.Aura.Subtract(1);
                    }
                    else if (fighter.Aura.Current < fighter.BaseAura)
                    {
                        fighter.Aura.Add(1);
                    }
                }
            }
        }

        private void TickActor(IEntity entity)
        {
            var actor = entity.Get<Actor>();

            var behaviours = GetBehaviours(entity);

            foreach (IBehaviour behaviour in behaviours.OrderByDescending(b => b.BehaviourPriority))
            {
                var chosenAction = behaviour.ChooseAction(entity);
                
                if (chosenAction != null)
                {
                    if (chosenAction.Action == ActionType.WaitForInput)
                    {
                        WaitingForInput = true;
                        break;
                    }

                    _eventSystem.Try(EventType.Action, entity, chosenAction);
                }

                if (actor.NextTick > CurrentTime)
                {
                    break;
                }
            }

            if (actor.NextTick <= CurrentTime)
            {
                actor.NextTick = CurrentTime + 1000;
            }
        }

        private IEnumerable<IBehaviour> GetBehaviours(IEntity entity)
        {
            return entity.Components.OfType<IBehaviour>();
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