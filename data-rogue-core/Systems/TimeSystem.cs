﻿using System;
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
        private readonly ISystemContainer _systemContainer;
        private MapKey ActiveMapKey;
        private readonly IPlayerSystem _playerSystem;
        private IEventSystem _eventSystem;
        private IStatSystem _statSystem;
        

        public TimeSystem(ISystemContainer systemContainer)
        {
            _systemContainer = systemContainer;
            _eventSystem = systemContainer.EventSystem;
            _playerSystem = systemContainer.PlayerSystem;
            _statSystem = systemContainer.StatSystem;
        }

        public new void Initialise()
        {
            CurrentTime = 0;
            base.Initialise();
        }

        public ulong CurrentTime { get; set; }

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Actor) };
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
                ((ITickUpdate) updatable).Tick(_systemContainer, entity, CurrentTime);
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

        private void TickActor(IEntity entity)
        {
            var actor = entity.Get<Actor>();

            var behaviours = GetBehaviours(entity);

            foreach (IBehaviour behaviour in behaviours.OrderByDescending(b => b.BehaviourPriority))
            {
                if (_systemContainer.Random.PercentageChance(behaviour.BehaviourChance))
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
            }

            if (actor.NextTick <= CurrentTime)
            {
                actor.NextTick = CurrentTime + actor.Speed;
            }
        }

        private IEnumerable<IBehaviour> GetBehaviours(IEntity entity)
        {
            return entity.Components.OfType<IBehaviour>();
        }

        public void SpendTicks(IEntity entity, ulong ticks)
        {
            var actor = entity.Get<Actor>();
            actor.NextTick = CurrentTime + ticks;
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