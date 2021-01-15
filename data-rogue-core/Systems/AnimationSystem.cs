using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class AnimationSystem: BaseSystem, IAnimationSystem
    {
        private IStopwatch _stopwatch;
        private readonly IRandom _random;
        private readonly IEntityEngine _entityEngine;
        private long _lastKnownTime;
        public const int TICK_LENGTH = 33;

        private bool _blockingAnimationPlaying = false;

        public AnimationSystem(IEntityEngine engine, IStopwatch stopwatch, IRandom random)
        {
            _entityEngine = engine;
            _stopwatch = stopwatch;
            _random = random;
            _lastKnownTime = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Start();
        }

        public override SystemComponents RequiredComponents => new SystemComponents{typeof(Animated)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();
        public void Tick()
        {
            var elapsed = _stopwatch.ElapsedMilliseconds - _lastKnownTime;
            int tickBy = (int)(elapsed / TICK_LENGTH);
            _lastKnownTime += tickBy * TICK_LENGTH;

            foreach (IEntity entity in Entities)
            {
                ResolveAnimationFrameUpdates(tickBy, entity);

                ResolveAnimatedMovement(elapsed, entity);
            }
        }

        private void ResolveAnimationFrameUpdates(int tickBy, IEntity entity)
        {
            var component = entity.Get<Animated>();
            if (component.CurrentAnimation == null)
            {
                component.CurrentAnimation = GetComponentAnimation(entity, component.DefaultAnimation);
            }

            var currentAnimation = component.CurrentAnimation;
            currentAnimation.CurrentTick += tickBy;

            while (currentAnimation.CurrentTick >= currentAnimation.FrameTicks)
            {
                currentAnimation.CurrentTick -= currentAnimation.FrameTicks;
                if (currentAnimation.RandomiseTicks)
                {
                    currentAnimation.CurrentTick -= _random.PickOneFrom(0, 1);
                }

                currentAnimation.CurrentFrame++;

                if (currentAnimation.CurrentFrame == currentAnimation.FrameCount)
                {
                    if (currentAnimation.Repeat)
                    {
                        currentAnimation.CurrentFrame = 0;
                    }
                    else
                    {
                        SetAnimation(entity, component.DefaultAnimation);
                    }
                }
            }
        }

        private void ResolveAnimatedMovement(long elapsedMs, IEntity entity)
        {
            var moving = entity.TryGet<Moving>();

            if (moving != null)
            {
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
        }

        private Animation GetComponentAnimation(IEntity entity, AnimationType animationType)
        {
            return entity.Components.OfType<Animation>().SingleOrDefault(a => a.AnimationType == animationType);
        }

        public AnimationFrame GetFrame(IEntity entity)
        {
            Animation currentAnimation = entity.Get<Animated>().CurrentAnimation;
            return currentAnimation.Frames[currentAnimation.CurrentFrame];
        }

        public void SetFrame(IEntity entity, int frame)
        {
            Animated component = entity.Get<Animated>();
            component.CurrentAnimation.CurrentFrame = frame;
            component.CurrentAnimation.CurrentTick = 0;
        }

        public void SetAnimation(IEntity entity, AnimationType animationType)
        {
            Animated component = entity.Get<Animated>();
            Animation animation = GetComponentAnimation(entity, animationType);

            if (animation != null)
            {
                animation.CurrentFrame = 0;
                animation.CurrentTick = 0;

                component.CurrentAnimation = animation;
            }
        }

        public void StartAnimatedMovement(IEntity entity, List<AnimationMovement> movements)
        {
            if (!entity.Has<Animated>())
            {
                throw new ApplicationException("Can't add animated movement to an entity that doesn't have an [Animated] Component.");
            }

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

        public bool IsBlockingAnimationPlaying()
        {
            return false;
        }
    }
}
