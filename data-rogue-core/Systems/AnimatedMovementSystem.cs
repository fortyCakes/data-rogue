using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class AnimatedMovementSystem: BaseSystem, IAnimatedMovementSystem
    {
        private IEntityEngine _entityEngine;
        private IStopwatch _stopwatch;
        private long _lastKnownTime;

        public const int TICK_LENGTH = 1;

        public AnimatedMovementSystem(IEntityEngine entityEngine, IStopwatch stopwatch)
        {
            _entityEngine = entityEngine;
            _stopwatch = stopwatch;
            _lastKnownTime = stopwatch.ElapsedMilliseconds;
            _stopwatch.Start();
        }

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(Moving) };
        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public void Tick()
        {
            var elapsed = _stopwatch.ElapsedMilliseconds - _lastKnownTime;
            int tickBy = (int)(elapsed / TICK_LENGTH);
            _lastKnownTime += tickBy * TICK_LENGTH;

            var movingEntities = new List<IEntity>(Entities);

            foreach (IEntity entity in movingEntities)
            {
                ResolveAnimatedMovement(tickBy, entity);
            }
        }

        private void ResolveAnimatedMovement(long elapsedMs, IEntity entity)
        {
            var moving = entity.Get<Moving>();
            while (elapsedMs > 0)
            {
                var currentMovement = moving.Movements.FirstOrDefault();

                if (currentMovement != null)
                {
                    var movementAmount = Math.Min(elapsedMs, currentMovement.TimeLeft);
                    double ratio = (double)movementAmount / (double)currentMovement.Duration;
                    moving.OffsetX += ratio * currentMovement.Vector.X;
                    moving.OffsetY += ratio * currentMovement.Vector.Y;
                    elapsedMs -= movementAmount;
                    currentMovement.TimeLeft -= (int)movementAmount;

                    if (currentMovement.TimeLeft <= 0)
                    {
                        moving.Movements.Remove(currentMovement);
                    }
                }
                else
                {
                    _entityEngine.RemoveComponent(entity, moving);
                    elapsedMs = 0;
                }
            }
        }
        public void StartAnimatedMovement(IEntity entity, List<AnimationMovement> movements)
        {
            Moving component = entity.TryGet<Moving>();

            if (component != null)
            {
                _entityEngine.RemoveComponent(entity, component);
            }

            var totalVectorX = movements.Sum(m => m.Vector.X);
            var totalVectorY = movements.Sum(m => m.Vector.Y);

            var moving = new Moving { Movements = movements, OffsetX = -totalVectorX, OffsetY = -totalVectorY };

            _entityEngine.AddComponent(entity, moving);
        }

        public void StartAnimatedMovement(IEntity entity, VectorDouble movement, int duration)
        {
            StartAnimatedMovement(entity, new List<AnimationMovement> { new AnimationMovement(movement, duration) });
        }

        public bool IsBlockingAnimationPlaying()
        {
            return false;
        }
    }
}
